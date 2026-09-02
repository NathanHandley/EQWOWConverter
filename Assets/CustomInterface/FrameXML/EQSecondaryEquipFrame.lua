--
-- Secondary Class Equipment window (EQWOW / mod-everquest)
--
-- Opened by the third right-side tab on the CharacterFrame, this window mirrors the paper doll layout for a
-- NON-ACTIVE secondary EQ class: a class dropdown where the title dropdown normally sits, the 19 equipment
-- slots, and a dress-up model previewing the stored gear on the player.  Items move in real time:
--   * Drag (or pick up) an item from a bag and click/drop it on a slot -> ".class equipset <classId> <bag> <slot> <equipSlot> <itemEntry>"
--   * Drag an item from the character's own equipped slots onto a storage slot -> ".class equipswap" (two-way:
--     the equipped item is stored and any stored occupant gets equipped in its place)
--   * Left-click a stored item -> emulated cursor pickup (see heldStoredItem); then a click on an empty bag
--     slot runs ".class equipremove <classId> <equipSlot> <bag> <slot>", a click on another equipment
--     slot runs ".class equipmove <classId> <fromSlot> <toSlot>" (move or swap within storage), and a click
--     on a live paper doll slot runs ".class equipswap <classId> <storageSlot> <liveSlot>" (equips the stored
--     item on the character, displaced equipped item goes into the vacated storage slot)
--   * Right-click a stored item -> ".class equipremove <classId> <equipSlot>" (straight to wherever it fits)
--
-- The server answers every request/change with a hidden addon message (prefix "EQCLASSEQUIP", built by
-- EverQuestMod::SendClassEquipmentAddonMessageToPlayer).  Payload format (after the prefix):
--   H|<classId>|<className>
--   ~S|<slot>|<itemEntry>|<randomPropertyId>|<permEnchant>   (one per stored equipment slot, slots are 0-18 server ids)
--
-- The class list for the dropdown is parsed from the same "EQCLASS" message the EQ Class tab uses; every class
-- is listed, with the currently active secondary greyed out ("(current)") and unselectable.  Cursor-held items are tracked via a PickupContainerItem
-- hook because the cursor APIs expose the item but not the bag position it came from.
--

local EQCLASS_PREFIX = "EQCLASS";
local EQCLASSEQUIP_PREFIX = "EQCLASSEQUIP";

-- Paper doll layout: { slotName (for GetInventorySlotInfo / tooltip text), server equipment slot id }
local SLOT_COLUMN_LEFT = {
	{ "HeadSlot", 0 }, { "NeckSlot", 1 }, { "ShoulderSlot", 2 }, { "BackSlot", 14 },
	{ "ChestSlot", 4 }, { "ShirtSlot", 3 }, { "TabardSlot", 18 }, { "WristSlot", 8 },
};
local SLOT_COLUMN_RIGHT = {
	{ "HandsSlot", 9 }, { "WaistSlot", 5 }, { "LegsSlot", 6 }, { "FeetSlot", 7 },
	{ "Finger0Slot", 10 }, { "Finger1Slot", 11 }, { "Trinket0Slot", 12 }, { "Trinket1Slot", 13 },
};
local SLOT_ROW_BOTTOM = {
	{ "MainHandSlot", 15 }, { "SecondaryHandSlot", 16 }, { "RangedSlot", 17 },
};

-- Client-side mirror of the server's slot rules, keyed by GetItemInfo's itemEquipLoc, so obvious misdrops fail
-- fast with a local error (the server revalidates everything)
local EQUIPLOC_ALLOWED_SLOTS = {
	INVTYPE_HEAD = { [0] = true }, INVTYPE_NECK = { [1] = true }, INVTYPE_SHOULDER = { [2] = true },
	INVTYPE_BODY = { [3] = true }, INVTYPE_CHEST = { [4] = true }, INVTYPE_ROBE = { [4] = true },
	INVTYPE_WAIST = { [5] = true }, INVTYPE_LEGS = { [6] = true }, INVTYPE_FEET = { [7] = true },
	INVTYPE_WRIST = { [8] = true }, INVTYPE_HAND = { [9] = true },
	INVTYPE_FINGER = { [10] = true, [11] = true }, INVTYPE_TRINKET = { [12] = true, [13] = true },
	INVTYPE_CLOAK = { [14] = true },
	INVTYPE_WEAPON = { [15] = true, [16] = true }, INVTYPE_2HWEAPON = { [15] = true },
	INVTYPE_WEAPONMAINHAND = { [15] = true }, INVTYPE_WEAPONOFFHAND = { [16] = true },
	INVTYPE_SHIELD = { [16] = true }, INVTYPE_HOLDABLE = { [16] = true },
	INVTYPE_RANGED = { [17] = true }, INVTYPE_RANGEDRIGHT = { [17] = true },
	INVTYPE_THROWN = { [17] = true }, INVTYPE_RELIC = { [17] = true },
	INVTYPE_TABARD = { [18] = true },
};

-- Class rows parsed from the EQCLASS message: array of { id, name, level }, plus the active secondary id
local classRows = {};
local currentClassId = nil;
local selectedClassId = nil;

-- Stored equipment of the selected class: server slot id -> { itemEntry, randomPropertyId, permEnchant }
local storedBySlot = {};

local slotButtons = {};

-- Where the item currently on the cursor was picked up from ({ bag, slot, itemID }), maintained by the
-- PickupContainerItem hook below.  nil whenever the cursor item did not come straight from a bag slot.
local cursorItemOrigin = nil;

-- Same idea for an item picked up from the character's own equipped slots ({ invSlot (1-19), itemID }),
-- maintained by the PickupInventoryItem hook.  Dropping such an item on a storage slot runs the two-way
-- ".class equipswap" instead of ".class equipset".
local cursorEquippedItemOrigin = nil;

-- Emulated cursor pickup of a stored item ({ serverSlot, itemEntry, icon }).  A stored item cannot go on the
-- real cursor (the client does not possess it), so the pickup is faked: the cursor shows the item icon (kept
-- applied every frame in OnUpdate since mouseovers reset it), the source slot is desaturated, and the next
-- click on an empty bag slot / another equipment slot places it.  Right click or any real cursor action cancels.
local heldStoredItem = nil;

-- Re-render countdown for icons that were not yet in the local item cache when first drawn
local iconRetriesRemaining = 0;
local iconRetryElapsed = 0;

local function EQSecondaryEquipFrame_GetItemLink(slotData)
	return "item:" .. slotData.itemEntry .. ":" .. (slotData.permEnchant or 0) .. ":0:0:0:0:" .. (slotData.randomPropertyId or 0) .. ":0";
end

local function EQSecondaryEquipFrame_RequestClassList()
	SendChatMessage(".class uiinfo", "SAY");
end

local function EQSecondaryEquipFrame_RequestEquipment()
	-- Only pull equipment data while the window is actually open (the class list arrives at every login)
	if ( selectedClassId and EQSecondaryEquipFrame:IsShown() ) then
		SendChatMessage(".class equipinfo " .. selectedClassId, "SAY");
	end
end

-- Updates the slot icons and the dress-up model from storedBySlot.  Icons of items not yet in the local cache
-- come back nil from GetItemIcon; those get a question mark, a hidden tooltip request to force the server
-- query, and a short retry loop (see OnUpdate)
local function EQSecondaryEquipFrame_Render()
	local anyIconMissing = false;
	for _, button in pairs(slotButtons) do
		local slotData = storedBySlot[button.serverSlot];
		if ( slotData ) then
			local icon = GetItemIcon(slotData.itemEntry);
			if ( not icon ) then
				icon = "Interface\\Icons\\INV_Misc_QuestionMark";
				anyIconMissing = true;
				EQSecondaryEquipFrameCacheTooltip:SetOwner(UIParent, "ANCHOR_NONE");
				EQSecondaryEquipFrameCacheTooltip:SetHyperlink(EQSecondaryEquipFrame_GetItemLink(slotData));
				EQSecondaryEquipFrameCacheTooltip:Hide();
			end
			SetItemButtonTexture(button, icon);
		else
			SetItemButtonTexture(button, button.backgroundTextureName);
		end
		-- The slot whose item is on the emulated cursor shows greyed out, like a real cursor pickup
		SetItemButtonDesaturated(button, (heldStoredItem and heldStoredItem.serverSlot == button.serverSlot) and 1 or nil);
	end

	local model = EQSecondaryEquipFrameModel;
	model:SetUnit("player");
	model:Undress();
	for _, slotData in pairs(storedBySlot) do
		model:TryOn("item:" .. slotData.itemEntry);
	end

	if ( anyIconMissing and iconRetriesRemaining == 0 ) then
		iconRetriesRemaining = 5;
		iconRetryElapsed = 0;
	elseif ( not anyIconMissing ) then
		iconRetriesRemaining = 0;
	end
end

-- Drops the emulated cursor pickup and restores the normal cursor and slot coloring
local function EQSecondaryEquipFrame_CancelHeldItem()
	if ( heldStoredItem ) then
		heldStoredItem = nil;
		ResetCursor();
		EQSecondaryEquipFrame_Render();
	end
end

local function EQSecondaryEquipFrame_UpdateDropDownText()
	local text = "No Class Selected";
	for _, row in ipairs(classRows) do
		if ( row.id == selectedClassId ) then
			text = row.name;
		end
	end
	UIDropDownMenu_SetText(EQSecondaryEquipFrameClassDropDown, text);
end

local function EQSecondaryEquipFrame_SelectClass(classId)
	if ( selectedClassId == classId ) then
		return;
	end
	EQSecondaryEquipFrame_CancelHeldItem();
	selectedClassId = classId;
	storedBySlot = {};
	EQSecondaryEquipFrame_UpdateDropDownText();
	EQSecondaryEquipFrame_Render();
	EQSecondaryEquipFrame_RequestEquipment();
end

local function EQSecondaryEquipFrame_ClassDropDown_OnClick(self)
	UIDropDownMenu_SetSelectedValue(EQSecondaryEquipFrameClassDropDown, self.value);
	EQSecondaryEquipFrame_SelectClass(self.value);
end

local function EQSecondaryEquipFrame_ClassDropDown_Initialize()
	for _, row in ipairs(classRows) do
		-- Every eligible secondary (including None, a valid combination with its own stored gear).  The class
		-- the character is right now is listed too, but greyed out and unselectable -- this window only edits
		-- the STORED gear of inactive classes
		local info = UIDropDownMenu_CreateInfo();
		info.text = row.name;
		info.value = row.id;
		if ( row.id == currentClassId ) then
			info.text = info.text .. "  (current)";
			info.disabled = 1;
		else
			info.func = EQSecondaryEquipFrame_ClassDropDown_OnClick;
			info.checked = (row.id == selectedClassId);
		end
		UIDropDownMenu_AddButton(info);
	end
end

-- Keeps the selection valid against a fresh class list: drop it if it became the active class or vanished,
-- then default to the first selectable real class (falling back to None when nothing else is selectable)
local function EQSecondaryEquipFrame_ValidateSelection()
	local selectionValid = false;
	local firstSelectable = nil;
	local firstRealClass = nil;
	for _, row in ipairs(classRows) do
		if ( row.id ~= currentClassId ) then
			if ( not firstSelectable ) then
				firstSelectable = row.id;
			end
			if ( not firstRealClass and row.id ~= 0 ) then
				firstRealClass = row.id;
			end
			if ( row.id == selectedClassId ) then
				selectionValid = true;
			end
		end
	end
	if ( not selectionValid ) then
		selectedClassId = nil;
		local defaultClassId = firstRealClass or firstSelectable;
		if ( defaultClassId ) then
			EQSecondaryEquipFrame_SelectClass(defaultClassId);
		else
			storedBySlot = {};
			EQSecondaryEquipFrame_UpdateDropDownText();
			EQSecondaryEquipFrame_Render();
		end
	else
		EQSecondaryEquipFrame_UpdateDropDownText();
	end
end

-- Parses the shared EQCLASS message (see EQClassFrame.lua for the format) just for the class rows and active id
local function EQSecondaryEquipFrame_HandleClassInfo(payload)
	if ( not payload ) then
		return;
	end
	classRows = {};
	currentClassId = nil;
	local segments = { strsplit("~", payload) };
	for _, segment in ipairs(segments) do
		local kind, f2, f3, f4, f5 = strsplit("|", segment);
		if ( kind == "H" ) then
			currentClassId = tonumber(f4);
		elseif ( kind == "R" ) then
			tinsert(classRows, { id = tonumber(f2), name = (f3 or ""), level = tonumber(f4) or 1 });
		end
	end
	EQSecondaryEquipFrame_ValidateSelection();
end

local function EQSecondaryEquipFrame_HandleEquipment(payload)
	if ( not payload ) then
		return;
	end
	local payloadClassId = nil;
	local newStored = {};
	local segments = { strsplit("~", payload) };
	for _, segment in ipairs(segments) do
		local kind, f2, f3, f4, f5 = strsplit("|", segment);
		if ( kind == "H" ) then
			payloadClassId = tonumber(f2);
		elseif ( kind == "S" ) then
			local slot = tonumber(f2);
			if ( slot ) then
				newStored[slot] = { itemEntry = tonumber(f3) or 0, randomPropertyId = tonumber(f4) or 0, permEnchant = tonumber(f5) or 0 };
			end
		end
	end
	-- Only the selected class is tracked; stale pushes for other classes are ignored
	if ( payloadClassId == selectedClassId ) then
		storedBySlot = newStored;
		-- Drop the emulated pickup if its item is no longer where it was picked up from
		if ( heldStoredItem ) then
			local heldData = storedBySlot[heldStoredItem.serverSlot];
			if ( not heldData or heldData.itemEntry ~= heldStoredItem.itemEntry ) then
				heldStoredItem = nil;
				ResetCursor();
			end
		end
		EQSecondaryEquipFrame_Render();
	end
end

local function EQSecondaryEquipFrame_SlotButton_OnClick(self, mouseButton)
	if ( not selectedClassId ) then
		return;
	end

	-- A real item on the cursor always means a store request, from either a bag slot or an equipped slot
	if ( CursorHasItem() ) then
		local cursorType, cursorItemID = GetCursorInfo();
		local fromBag = (cursorType == "item" and cursorItemOrigin and cursorItemOrigin.itemID == cursorItemID);
		local fromEquipped = (cursorType == "item" and cursorEquippedItemOrigin and cursorEquippedItemOrigin.itemID == cursorItemID);
		if ( not fromBag and not fromEquipped ) then
			UIErrorsFrame:AddMessage("Only items picked up from your bags or equipped slots can be stored.", 1.0, 0.1, 0.1, 1.0);
			return;
		end
		-- Block only on known-bad data; the server revalidates everything
		local _, _, _, _, _, _, _, _, itemEquipLoc = GetItemInfo(cursorItemID);
		if ( itemEquipLoc ) then
			local allowedSlots = EQUIPLOC_ALLOWED_SLOTS[itemEquipLoc];
			if ( not allowedSlots or not allowedSlots[self.serverSlot] ) then
				UIErrorsFrame:AddMessage("That item cannot go in that slot.", 1.0, 0.1, 0.1, 1.0);
				return;
			end
		end
		if ( fromBag ) then
			SendChatMessage(".class equipset " .. selectedClassId .. " " .. cursorItemOrigin.bag .. " " .. cursorItemOrigin.slot .. " " .. self.serverSlot .. " " .. cursorItemID, "SAY");
		else
			-- Two-way exchange: the equipped item goes into this storage slot, and any stored item here gets
			-- equipped in its place (which the server validates as a real equip).  Pre-check the occupant's
			-- slot fit when its data is cached so obvious mismatches fail fast
			local liveServerSlot = cursorEquippedItemOrigin.invSlot - 1;
			local occupant = storedBySlot[self.serverSlot];
			if ( occupant ) then
				local _, _, _, _, _, _, _, _, occupantEquipLoc = GetItemInfo(occupant.itemEntry);
				if ( occupantEquipLoc ) then
					local occupantAllowedSlots = EQUIPLOC_ALLOWED_SLOTS[occupantEquipLoc];
					if ( not occupantAllowedSlots or not occupantAllowedSlots[liveServerSlot] ) then
						UIErrorsFrame:AddMessage("Those items cannot swap slots.", 1.0, 0.1, 0.1, 1.0);
						return;
					end
				end
			end
			SendChatMessage(".class equipswap " .. selectedClassId .. " " .. self.serverSlot .. " " .. liveServerSlot, "SAY");
		end
		ClearCursor();
		cursorItemOrigin = nil;
		cursorEquippedItemOrigin = nil;
		return;
	end

	-- Right click cancels a held item, or sends a stored item straight to the bags
	if ( mouseButton == "RightButton" ) then
		if ( heldStoredItem ) then
			EQSecondaryEquipFrame_CancelHeldItem();
		elseif ( storedBySlot[self.serverSlot] ) then
			SendChatMessage(".class equipremove " .. selectedClassId .. " " .. self.serverSlot, "SAY");
		end
		return;
	end

	-- Holding a stored item: place it here (move or swap within storage).  Client checks only block on KNOWN
	-- incompatible data; an item missing from the local cache (GetItemInfo nil) goes to the server, which
	-- revalidates everything and pushes back the real state
	if ( heldStoredItem ) then
		if ( heldStoredItem.serverSlot == self.serverSlot ) then
			EQSecondaryEquipFrame_CancelHeldItem();
			return;
		end
		local _, _, _, _, _, _, _, _, heldEquipLoc = GetItemInfo(heldStoredItem.itemEntry);
		if ( heldEquipLoc ) then
			local heldAllowedSlots = EQUIPLOC_ALLOWED_SLOTS[heldEquipLoc];
			if ( not heldAllowedSlots or not heldAllowedSlots[self.serverSlot] ) then
				UIErrorsFrame:AddMessage("That item cannot go in that slot.", 1.0, 0.1, 0.1, 1.0);
				return;
			end
		end
		local occupant = storedBySlot[self.serverSlot];
		if ( occupant ) then
			local _, _, _, _, _, _, _, _, occupantEquipLoc = GetItemInfo(occupant.itemEntry);
			if ( occupantEquipLoc ) then
				local occupantAllowedSlots = EQUIPLOC_ALLOWED_SLOTS[occupantEquipLoc];
				if ( not occupantAllowedSlots or not occupantAllowedSlots[heldStoredItem.serverSlot] ) then
					UIErrorsFrame:AddMessage("Those items cannot swap slots.", 1.0, 0.1, 0.1, 1.0);
					return;
				end
			end
		end
		SendChatMessage(".class equipmove " .. selectedClassId .. " " .. heldStoredItem.serverSlot .. " " .. self.serverSlot, "SAY");
		heldStoredItem = nil;
		ResetCursor();
		return;
	end

	-- Empty cursor on a stored item: pick it up (emulated cursor)
	local slotData = storedBySlot[self.serverSlot];
	if ( slotData ) then
		heldStoredItem = {
			serverSlot = self.serverSlot,
			itemEntry = slotData.itemEntry,
			icon = GetItemIcon(slotData.itemEntry) or "Interface\\Icons\\INV_Misc_QuestionMark",
		};
		PlaySound("igMainMenuOptionCheckBoxOn");
		SetCursor(heldStoredItem.icon);
		EQSecondaryEquipFrame_Render();
	end
end

local function EQSecondaryEquipFrame_SlotButton_OnEnter(self)
	GameTooltip:SetOwner(self, "ANCHOR_RIGHT");
	local slotData = storedBySlot[self.serverSlot];
	if ( slotData ) then
		GameTooltip:SetHyperlink(EQSecondaryEquipFrame_GetItemLink(slotData));
		GameTooltip:AddLine("Left-click to pick up, right-click to send to your bags", 0.1, 1.0, 0.1);
	else
		GameTooltip:SetText(_G[self.slotTextKey] or "");
	end
	GameTooltip:Show();
end

local function EQSecondaryEquipFrame_SlotButton_OnLeave(self)
	GameTooltip:Hide();
end

local function EQSecondaryEquipFrame_CreateSlotButton(slotName, serverSlot)
	local button = CreateFrame("Button", "EQSecondaryEquipFrame" .. slotName, EQSecondaryEquipFrame, "ItemButtonTemplate");
	button.serverSlot = serverSlot;
	button.slotName = slotName;
	button.slotTextKey = strupper(slotName);
	local _, backgroundTextureName = GetInventorySlotInfo(slotName);
	button.backgroundTextureName = backgroundTextureName;
	SetItemButtonTexture(button, backgroundTextureName);
	button:RegisterForClicks("LeftButtonUp", "RightButtonUp");
	button:SetScript("OnClick", EQSecondaryEquipFrame_SlotButton_OnClick);
	button:SetScript("OnReceiveDrag", EQSecondaryEquipFrame_SlotButton_OnClick);
	button:SetScript("OnEnter", EQSecondaryEquipFrame_SlotButton_OnEnter);
	button:SetScript("OnLeave", EQSecondaryEquipFrame_SlotButton_OnLeave);
	slotButtons[serverSlot] = button;
	return button;
end

local function EQSecondaryEquipFrame_CreateSlotColumn(slots, xOffset)
	local previous = nil;
	for _, slotInfo in ipairs(slots) do
		local button = EQSecondaryEquipFrame_CreateSlotButton(slotInfo[1], slotInfo[2]);
		if ( previous ) then
			button:SetPoint("TOPLEFT", previous, "BOTTOMLEFT", 0, -4);
		else
			button:SetPoint("TOPLEFT", EQSecondaryEquipFrame, "TOPLEFT", xOffset, -74);
		end
		previous = button;
	end
end

function EQSecondaryEquipFrame_OnPickupContainerItem(bag, slot)
	cursorEquippedItemOrigin = nil;
	if ( CursorHasItem() ) then
		local cursorType, cursorItemID = GetCursorInfo();
		if ( cursorType == "item" ) then
			cursorItemOrigin = { bag = bag, slot = slot, itemID = cursorItemID };
		else
			cursorItemOrigin = nil;
		end
	else
		cursorItemOrigin = nil;
	end
end

function EQSecondaryEquipFrame_OnPickupInventoryItem(invSlot)
	cursorItemOrigin = nil;
	if ( CursorHasItem() ) then
		local cursorType, cursorItemID = GetCursorInfo();
		if ( cursorType == "item" and invSlot and invSlot >= 1 and invSlot <= 19 ) then
			cursorEquippedItemOrigin = { invSlot = invSlot, itemID = cursorItemID };
			return;
		end
	end
	cursorEquippedItemOrigin = nil;
end

function EQSecondaryEquipFrame_ClearCursorItemOrigin()
	cursorItemOrigin = nil;
	cursorEquippedItemOrigin = nil;
end

-- Runs after the stock paper doll slot click.  While a stored item is held, clicking a live equipment slot
-- swaps: the stored item is equipped on the character and the displaced equipped item goes back into the
-- vacated storage slot.  The stock click may have picked the equipped item onto the real cursor first, so
-- that pickup is undone before the swap is requested.
function EQSecondaryEquipFrame_OnPaperDollItemClick(self, mouseButton)
	if ( not heldStoredItem or not selectedClassId ) then
		return;
	end
	if ( mouseButton and mouseButton ~= "LeftButton" ) then
		return;
	end
	local liveServerSlot = self:GetID() - 1;
	if ( liveServerSlot < 0 or liveServerSlot > 18 ) then
		return;
	end
	if ( CursorHasItem() ) then
		ClearCursor();
		EQSecondaryEquipFrame_ClearCursorItemOrigin();
	end
	-- Block only on known-bad data; the server fully validates the live equip (proficiency, level, etc.)
	local _, _, _, _, _, _, _, _, heldEquipLoc = GetItemInfo(heldStoredItem.itemEntry);
	if ( heldEquipLoc ) then
		local heldAllowedSlots = EQUIPLOC_ALLOWED_SLOTS[heldEquipLoc];
		if ( not heldAllowedSlots or not heldAllowedSlots[liveServerSlot] ) then
			UIErrorsFrame:AddMessage("That item cannot go in that slot.", 1.0, 0.1, 0.1, 1.0);
			return;
		end
	end
	SendChatMessage(".class equipswap " .. selectedClassId .. " " .. heldStoredItem.serverSlot .. " " .. liveServerSlot, "SAY");
	heldStoredItem = nil;
	ResetCursor();
end

-- Runs after the stock bag button click.  While a stored item is held, a click on an empty bag slot deposits
-- it exactly there; a click on an occupied slot would have picked that item up onto the real cursor, so the
-- pickup is undone and the hold kept
function EQSecondaryEquipFrame_OnContainerItemClick(self, mouseButton)
	if ( not heldStoredItem or not selectedClassId ) then
		return;
	end
	if ( mouseButton and mouseButton ~= "LeftButton" ) then
		return;
	end
	local bag = self:GetParent():GetID();
	local slot = self:GetID();
	if ( bag < 0 or bag > 4 ) then
		UIErrorsFrame:AddMessage("Stored items can only go into your backpack and bags.", 1.0, 0.1, 0.1, 1.0);
		return;
	end
	if ( GetContainerItemLink(bag, slot) ) then
		ClearCursor();
		EQSecondaryEquipFrame_ClearCursorItemOrigin();
		UIErrorsFrame:AddMessage("That bag slot is occupied.", 1.0, 0.1, 0.1, 1.0);
	else
		SendChatMessage(".class equipremove " .. selectedClassId .. " " .. heldStoredItem.serverSlot .. " " .. bag .. " " .. slot, "SAY");
		heldStoredItem = nil;
		ResetCursor();
	end
end

-- Maintains the emulated cursor (mouseovers constantly reset the real one) and runs the icon retry loop
local function EQSecondaryEquipFrame_OnUpdate(self, elapsed)
	if ( heldStoredItem ) then
		if ( GetCursorInfo() ) then
			-- Any real cursor action (item, spell, money...) takes precedence over the emulated hold
			EQSecondaryEquipFrame_CancelHeldItem();
		else
			SetCursor(heldStoredItem.icon);
		end
	end
	if ( iconRetriesRemaining > 0 ) then
		iconRetryElapsed = iconRetryElapsed + elapsed;
		if ( iconRetryElapsed >= 1.0 ) then
			iconRetryElapsed = 0;
			iconRetriesRemaining = iconRetriesRemaining - 1;
			EQSecondaryEquipFrame_Render();
		end
	end
end

function EQSecondaryEquipFrame_OnLoad(self)
	self:SetFrameLevel(self:GetParent():GetFrameLevel() + 5);
	self:RegisterEvent("CHAT_MSG_ADDON");
	self:SetScript("OnUpdate", EQSecondaryEquipFrame_OnUpdate);

	-- Hidden tooltip used purely to force-cache item data for icons
	CreateFrame("GameTooltip", "EQSecondaryEquipFrameCacheTooltip", nil, "GameTooltipTemplate");

	-- Equipment slots in the stock paper doll arrangement
	EQSecondaryEquipFrame_CreateSlotColumn(SLOT_COLUMN_LEFT, 21);
	EQSecondaryEquipFrame_CreateSlotColumn(SLOT_COLUMN_RIGHT, 305);
	local previous = nil;
	for _, slotInfo in ipairs(SLOT_ROW_BOTTOM) do
		local button = EQSecondaryEquipFrame_CreateSlotButton(slotInfo[1], slotInfo[2]);
		if ( previous ) then
			button:SetPoint("TOPLEFT", previous, "TOPRIGHT", 5, 0);
		else
			button:SetPoint("TOPLEFT", EQSecondaryEquipFrame, "BOTTOMLEFT", 122, 127);
		end
		previous = button;
	end

	-- Gear preview model, same footprint as the paper doll's CharacterModelFrame
	local model = CreateFrame("DressUpModel", "EQSecondaryEquipFrameModel", self);
	model:SetPoint("TOPLEFT", self, "TOPLEFT", 65, -78);
	model:SetSize(233, 215);

	UIDropDownMenu_SetWidth(EQSecondaryEquipFrameClassDropDown, 200);
	UIDropDownMenu_Initialize(EQSecondaryEquipFrameClassDropDown, EQSecondaryEquipFrame_ClassDropDown_Initialize);
	EQSecondaryEquipFrame_UpdateDropDownText();

	-- The cursor APIs report the held item but not its source, so remember the bag position at pickup time.
	-- A pickup call that empties the cursor was a drop; a split or an equipped-item pickup poisons the origin.
	-- (These are C API functions, so they exist at OnLoad time.)
	hooksecurefunc("PickupContainerItem", EQSecondaryEquipFrame_OnPickupContainerItem);
	hooksecurefunc("SplitContainerItem", EQSecondaryEquipFrame_ClearCursorItemOrigin);
	hooksecurefunc("PickupInventoryItem", EQSecondaryEquipFrame_OnPickupInventoryItem);

	-- ContainerFrameItemButton_OnClick is FrameXML that loads AFTER this file (ContainerFrame.xml is later in
	-- FrameXML.toc), so hooking it here would error and kill the deposit-to-bag path.  Hook it at PLAYER_LOGIN,
	-- by which point every FrameXML file is loaded.
	self:RegisterEvent("PLAYER_LOGIN");
end

function EQSecondaryEquipFrame_OnShow(self)
	PlaySound("igCharacterInfoOpen");
	SetPortraitTexture(EQSecondaryEquipFramePortrait, "player");
	EQSecondaryEquipFrame_Render();
	EQSecondaryEquipFrame_RequestClassList();
	EQSecondaryEquipFrame_RequestEquipment();
	if ( CharacterFrame_UpdateSideTabs ) then
		CharacterFrame_UpdateSideTabs();
	end
end

function EQSecondaryEquipFrame_OnHide(self)
	EQSecondaryEquipFrame_CancelHeldItem();
	PlaySound("igCharacterInfoClose");
	if ( CharacterFrame_UpdateSideTabs ) then
		CharacterFrame_UpdateSideTabs();
	end
end

function EQSecondaryEquipFrame_OnEvent(self, event, arg1, arg2)
	if ( event == "PLAYER_LOGIN" ) then
		-- All FrameXML is loaded now, so the bag and paper doll button handlers exist and can be hooked (see OnLoad)
		if ( type(ContainerFrameItemButton_OnClick) == "function" ) then
			hooksecurefunc("ContainerFrameItemButton_OnClick", EQSecondaryEquipFrame_OnContainerItemClick);
		end
		if ( type(PaperDollItemSlotButton_OnClick) == "function" ) then
			hooksecurefunc("PaperDollItemSlotButton_OnClick", EQSecondaryEquipFrame_OnPaperDollItemClick);
		end
		return;
	end

	if ( event ~= "CHAT_MSG_ADDON" ) then
		return;
	end

	-- 3.3.5 normally delivers (prefix, message); fall back to a tab-split if the client passes them joined
	if ( arg1 == EQCLASSEQUIP_PREFIX ) then
		EQSecondaryEquipFrame_HandleEquipment(arg2);
	elseif ( arg1 and string.find(arg1, "^" .. EQCLASSEQUIP_PREFIX .. "\t") ) then
		EQSecondaryEquipFrame_HandleEquipment(string.gsub(arg1, "^" .. EQCLASSEQUIP_PREFIX .. "\t", ""));
	elseif ( arg1 == EQCLASS_PREFIX ) then
		EQSecondaryEquipFrame_HandleClassInfo(arg2);
	elseif ( arg1 and string.find(arg1, "^" .. EQCLASS_PREFIX .. "\t") ) then
		EQSecondaryEquipFrame_HandleClassInfo(string.gsub(arg1, "^" .. EQCLASS_PREFIX .. "\t", ""));
	end
end
