--
-- EQ Item Tooltips (EQWOW / mod-everquest)
--
-- Visually shows whether an item is usable by your current EQ class, mirroring how
-- mod-everquest::EverQuestMod::IsItemEQClassAllowedForPlayer enforces usability (your
-- PRIMARY EQ class OR your currently-active SECONDARY EQ class).
--
-- Two data sources, both already present in the running client -- no server or converter
-- changes are needed:
--   * The item's allowed classes are read from the item's own tooltip text.  The converter
--     stamps every restricted item's description with "|cFFFFFFFFEQ Classes: WAR CLR ...|r"
--     (ItemTemplate.GetDescriptionStringWithAddedAllowedClasses).
--   * The player's live primary + active secondary EQ class come from the hidden "EQCLASS"
--     addon message the server pushes (EverQuestMod::SendClassInfoAddonMessageToPlayer), the
--     same feed the EQ Class character pane (EQClassFrame.lua) uses.  Payload after the prefix:
--       H|<baseId>|<baseName>|<currentSecondId>|<nextSecondId>|<expCur>|<expMax>~R|...
--
-- Where it colors:
--   * Item tooltips (bags, equipped, vendor hover, chat links): the "EQ Classes:" line turns
--     green (usable) or red (not usable).
--   * Merchant window: the item name turns red when the item is not usable by your class.
--   * Auction house (browse/bids/auctions tabs): the item icon gets the default UI's red
--     "can't use" tint when the item is not usable by your class.  (The "Usable Items"
--     search filter is handled server-side by mod-everquest, which strips EQ-class-unusable
--     items out of the search result packet.)
--

local EQCLASS_PREFIX = "EQCLASS";

-- EQ class id (as sent in the EQCLASS payload, 1-based to match the server enum) -> abbreviation
-- used in the stamped "EQ Classes:" line.  Order matches EverQuestMod::GetEQClassStringFromID.
local CLASS_ABBR = {
	[1]  = "WAR",
	[2]  = "CLR",
	[3]  = "PAL",
	[4]  = "RNG",
	[5]  = "SHD",
	[6]  = "DRU",
	[7]  = "MNK",
	[8]  = "BRD",
	[9]  = "ROG",
	[10] = "SHM",
	[11] = "NEC",
	[12] = "WIZ",
	[13] = "MAG",
	[14] = "ENC",
};

local COLOR_USABLE   = "|cff20ff20"; -- green
local COLOR_UNUSABLE = "|cffff2020"; -- red

-- Set of class abbreviations the player can currently use (primary + active secondary).
local playerAbbrs = {};
-- Stays false until the first EQCLASS message arrives; while false we leave tooltips untouched
-- so items are never mis-colored before we know the player's class.
local haveClass = false;

-- Parses the EQCLASS payload and rebuilds the usable-class set from the H segment
-- (field 2 = primary id, field 4 = active secondary id; secondary id 0 means "None").
local function EQItemTooltips_UpdatePlayerClass(payload)
	if ( not payload ) then
		return;
	end

	local segments = { strsplit("~", payload) };
	for _, segment in ipairs(segments) do
		local kind, baseId, _, secondId = strsplit("|", segment);
		if ( kind == "H" ) then
			wipe(playerAbbrs);
			baseId = tonumber(baseId);
			secondId = tonumber(secondId);
			if ( baseId and CLASS_ABBR[baseId] ) then
				playerAbbrs[CLASS_ABBR[baseId]] = true;
			end
			if ( secondId and CLASS_ABBR[secondId] ) then
				playerAbbrs[CLASS_ABBR[secondId]] = true;
			end
			haveClass = true;

			-- Repaint an open merchant or auction window so a class switch reflects immediately
			if ( MerchantFrame and MerchantFrame:IsShown() ) then
				EQItemTooltips_UpdateMerchant();
			end
			if ( AuctionFrame and AuctionFrame:IsShown() ) then
				EQItemTooltips_UpdateAuctionHouse();
			end
			return;
		end
	end
end

-- Given a tooltip line's raw text, returns whether the player can use the item:
--   true  = usable, false = not usable, nil = the line is not an "EQ Classes:" line
local function EQItemTooltips_EvaluateClassLine(text)
	-- Strip color codes, then isolate the "|n"-separated segment that carries the class list
	-- (learn-on-use items put the teaching description first, other items put the class line
	-- first) so surrounding description text does not get parsed as class abbreviations.
	local plain = text:gsub("|c%x%x%x%x%x%x%x%x", ""):gsub("|r", "");

	local list;
	for segment in (plain .. "|n"):gmatch("(.-)|n") do
		list = segment:match("EQ Classes:%s*(.+)");
		if ( list ) then
			break;
		end
	end
	if ( not list ) then
		return nil;
	end
	if ( list:find("ALL") ) then
		return true;
	end
	if ( list:find("NONE") ) then
		return false;
	end
	for abbr in list:gmatch("%u%u%u") do
		if ( playerAbbrs[abbr] ) then
			return true;
		end
	end
	return false;
end

-- Recolors the "EQ Classes:" line on a populated item tooltip and fixes the quote placement.
--
-- The client wraps the whole description field in orange double quotes, so they land in front of
-- the "EQ Classes:" line.  We strip them off that line; if a real description follows (after the
-- converter's "|n"), the quotes are moved to wrap just that description instead.
--
-- Keying off the raw stamped white prefix ("|cFFFFFFFFEQ Classes:") also makes this idempotent:
-- once rewritten the prefix is gone, so re-fires of OnTooltipSetItem on the same line are skipped.
local function EQItemTooltips_ColorTooltip(tooltip)
	if ( not haveClass ) then
		return;
	end
	local name = tooltip:GetName();
	if ( not name ) then
		return;
	end

	for i = 1, tooltip:NumLines() do
		local fontString = _G[name .. "TextLeft" .. i];
		local text = fontString and fontString:GetText();
		if ( text and text:find("|cFFFFFFFFEQ Classes:", 1, true) ) then
			local usable = EQItemTooltips_EvaluateClassLine(text);
			if ( usable ~= nil ) then
				local color = usable and COLOR_USABLE or COLOR_UNUSABLE;

				-- Learn-on-use items place the class line AFTER the teaching text on the green
				-- "Use:" line, where the client adds no quotes: just recolor the stamp in place.
				if ( not (text:find("^|cFFFFFFFFEQ Classes:") or text:find("^\"|cFFFFFFFFEQ Classes:")) ) then
					fontString:SetText((text:gsub("|cFFFFFFFFEQ Classes:", color .. "EQ Classes:")));
					return;
				end

				-- Peel the client-added surrounding quotes off the description block
				local inner = text;
				local hadQuotes = false;
				if ( inner:sub(1, 1) == "\"" ) then
					inner = inner:sub(2);
					hadQuotes = true;
				end
				if ( inner:sub(-1) == "\"" ) then
					inner = inner:sub(1, -2);
				end

				-- Split the classes line from any real description that follows the first "|n"
				local classesPart, restPart = inner:match("^(.-)|n(.*)$");
				if ( not classesPart ) then
					classesPart = inner;
				end

				-- Color the classes line green (usable) / red (not usable)
				classesPart = classesPart:gsub("|cFFFFFFFFEQ Classes:", color .. "EQ Classes:");

				local newText;
				if ( restPart and restPart ~= "" ) then
					-- Description present: quotes wrap only the description (only re-add them if the
					-- client had put quotes there in the first place)
					if ( hadQuotes ) then
						newText = classesPart .. "|n\"" .. restPart .. "\"";
					else
						newText = classesPart .. "|n" .. restPart;
					end
				else
					-- Classes line only: no quotes at all
					newText = classesPart;
				end
				fontString:SetText(newText);
			end
			return;
		end
	end
end

-- Hidden tooltip used to read items' class line without showing anything.
local scanner = CreateFrame("GameTooltip", "EQItemTooltipsScanner", UIParent, "GameTooltipTemplate");

-- Returns usability (true/false) for an item link, or nil if the item has no class line.
local function EQItemTooltips_LinkUsable(link)
	if ( not link ) then
		return nil;
	end
	scanner:SetOwner(UIParent, "ANCHOR_NONE");
	scanner:ClearLines();
	scanner:SetHyperlink(link);
	for i = 1, scanner:NumLines() do
		local fontString = _G["EQItemTooltipsScannerTextLeft" .. i];
		local text = fontString and fontString:GetText();
		if ( text and text:find("EQ Classes:") ) then
			return EQItemTooltips_EvaluateClassLine(text);
		end
	end
	return nil;
end

-- Returns usability (true/false) for a merchant slot, or nil if the item has no class line.
-- Scans the item link (reliable) rather than SetMerchantItem, which does not populate the
-- description on a hidden tooltip.
local function EQItemTooltips_MerchantItemUsable(merchantIndex)
	return EQItemTooltips_LinkUsable(GetMerchantItemLink(merchantIndex));
end

-- Runs after the default merchant refresh.  For items that declare EQ class usability, EQ class is
-- the authoritative signal, so we redraw the slot using Blizzard's own usable/unusable treatment
-- (red slot background when unusable, normal otherwise) but keyed on EQ class instead of WoW class.
-- This is the exact branch logic from MerchantFrame_UpdateMerchantInfo with isUsable swapped for the
-- EQ result, so the item name keeps its stock yellow text.  Items with no "EQ Classes:" line are
-- left exactly as the default UI drew them (no visual change).
function EQItemTooltips_UpdateMerchant()
	if ( not haveClass ) then
		return;
	end
	local numItems = GetMerchantNumItems();
	for i = 1, MERCHANT_ITEMS_PER_PAGE do
		local index = (MerchantFrame.page - 1) * MERCHANT_ITEMS_PER_PAGE + i;
		if ( index <= numItems ) then
			local usable = EQItemTooltips_MerchantItemUsable(index);
			if ( usable ~= nil ) then
				local merchantButton = _G["MerchantItem" .. i];
				local itemButton = _G["MerchantItem" .. i .. "ItemButton"];
				local _, _, _, _, numAvailable = GetMerchantItemInfo(index);
				if ( numAvailable == 0 ) then
					-- Out of stock: dim, deeper red when also unusable
					if ( not usable ) then
						SetItemButtonNameFrameVertexColor(merchantButton, 0.5, 0, 0);
						SetItemButtonSlotVertexColor(merchantButton, 0.5, 0, 0);
						SetItemButtonTextureVertexColor(itemButton, 0.5, 0, 0);
						SetItemButtonNormalTextureVertexColor(itemButton, 0.5, 0, 0);
					else
						SetItemButtonNameFrameVertexColor(merchantButton, 0.5, 0.5, 0.5);
						SetItemButtonSlotVertexColor(merchantButton, 0.5, 0.5, 0.5);
						SetItemButtonTextureVertexColor(itemButton, 0.5, 0.5, 0.5);
						SetItemButtonNormalTextureVertexColor(itemButton, 0.5, 0.5, 0.5);
					end
				elseif ( not usable ) then
					-- In stock but not usable by this EQ class: red slot background
					SetItemButtonNameFrameVertexColor(merchantButton, 1.0, 0, 0);
					SetItemButtonSlotVertexColor(merchantButton, 1.0, 0, 0);
					SetItemButtonTextureVertexColor(itemButton, 0.9, 0, 0);
					SetItemButtonNormalTextureVertexColor(itemButton, 0.9, 0, 0);
				else
					-- Usable: normal appearance
					SetItemButtonNameFrameVertexColor(merchantButton, 0.5, 0.5, 0.5);
					SetItemButtonSlotVertexColor(merchantButton, 1.0, 1.0, 1.0);
					SetItemButtonTextureVertexColor(itemButton, 1.0, 1.0, 1.0);
					SetItemButtonNormalTextureVertexColor(itemButton, 1.0, 1.0, 1.0);
				end
			end
		end
	end
end

-- Applies the default UI's "can't use" red icon tint (the same vertex color the AuctionFrame*_Update
-- functions use for items failing WOW usability) to auction rows holding items the player's EQ class can
-- not use.  Only ever paints toward red: rows the default UI already drew red (level requirement, etc)
-- stay red, and EQ-usable rows are left exactly as the default UI drew them.
local function EQItemTooltips_PaintAuctionRows(listType, buttonPrefix, numToDisplay, scrollFrame)
	if ( not haveClass or not scrollFrame ) then
		return;
	end
	local offset = FauxScrollFrame_GetOffset(scrollFrame);
	for i = 1, numToDisplay do
		local button = _G[buttonPrefix .. i];
		if ( button and button:IsShown() ) then
			local usable = EQItemTooltips_LinkUsable(GetAuctionItemLink(listType, offset + i));
			if ( usable == false ) then
				local iconTexture = _G[buttonPrefix .. i .. "ItemIconTexture"];
				if ( iconTexture ) then
					iconTexture:SetVertexColor(1.0, 0.1, 0.1);
				end
			end
		end
	end
end

-- Repaints all three auction house tabs (safe no-ops for tabs that are not populated / shown).
function EQItemTooltips_UpdateAuctionHouse()
	EQItemTooltips_PaintAuctionRows("list", "BrowseButton", NUM_BROWSE_TO_DISPLAY, BrowseScrollFrame);
	EQItemTooltips_PaintAuctionRows("bidder", "BidButton", NUM_BIDS_TO_DISPLAY, BidScrollFrame);
	EQItemTooltips_PaintAuctionRows("owner", "AuctionsButton", NUM_AUCTIONS_TO_DISPLAY, AuctionsScrollFrame);
end

local function EQItemTooltips_UpdateAuctionBrowse()
	EQItemTooltips_PaintAuctionRows("list", "BrowseButton", NUM_BROWSE_TO_DISPLAY, BrowseScrollFrame);
end

local function EQItemTooltips_UpdateAuctionBids()
	EQItemTooltips_PaintAuctionRows("bidder", "BidButton", NUM_BIDS_TO_DISPLAY, BidScrollFrame);
end

local function EQItemTooltips_UpdateAuctionOwned()
	EQItemTooltips_PaintAuctionRows("owner", "AuctionsButton", NUM_AUCTIONS_TO_DISPLAY, AuctionsScrollFrame);
end

-- The auction UI is load-on-demand, so these hooks install when Blizzard_AuctionUI loads.
local auctionHooksInstalled = false;
local function EQItemTooltips_TryInstallAuctionHooks()
	if ( auctionHooksInstalled or not AuctionFrameBrowse_Update ) then
		return;
	end
	auctionHooksInstalled = true;
	hooksecurefunc("AuctionFrameBrowse_Update", EQItemTooltips_UpdateAuctionBrowse);
	hooksecurefunc("AuctionFrameBid_Update", EQItemTooltips_UpdateAuctionBids);
	hooksecurefunc("AuctionFrameAuctions_Update", EQItemTooltips_UpdateAuctionOwned);
end

-- Tooltip hooks
GameTooltip:HookScript("OnTooltipSetItem", EQItemTooltips_ColorTooltip);
ItemRefTooltip:HookScript("OnTooltipSetItem", EQItemTooltips_ColorTooltip);

-- Merchant window hook
hooksecurefunc("MerchantFrame_UpdateMerchantInfo", EQItemTooltips_UpdateMerchant);

-- Auction UI hooks (in case Blizzard_AuctionUI is somehow already in, otherwise ADDON_LOADED installs them)
EQItemTooltips_TryInstallAuctionHooks();

-- Class-state feed.  The server broadcasts EQCLASS as a hidden addon message; we also ask for it
-- on entering the world (silently handled by ".class uiinfo") so the addon is self-sufficient and
-- not dependent on the EQ Class pane having been opened.
local eventFrame = CreateFrame("Frame");
eventFrame:RegisterEvent("CHAT_MSG_ADDON");
eventFrame:RegisterEvent("PLAYER_ENTERING_WORLD");
eventFrame:RegisterEvent("ADDON_LOADED");
eventFrame:SetScript("OnEvent", function(self, event, arg1, arg2)
	if ( event == "ADDON_LOADED" ) then
		if ( arg1 == "Blizzard_AuctionUI" ) then
			EQItemTooltips_TryInstallAuctionHooks();
		end
		return;
	end
	if ( event == "PLAYER_ENTERING_WORLD" ) then
		SendChatMessage(".class uiinfo", "SAY");
		return;
	end

	-- CHAT_MSG_ADDON: 3.3.5 usually delivers (prefix, message); fall back to a tab-split if joined.
	if ( arg1 == EQCLASS_PREFIX ) then
		EQItemTooltips_UpdatePlayerClass(arg2);
	elseif ( arg1 and string.find(arg1, "^" .. EQCLASS_PREFIX .. "\t") ) then
		EQItemTooltips_UpdatePlayerClass(string.gsub(arg1, "^" .. EQCLASS_PREFIX .. "\t", ""));
	end
end);
