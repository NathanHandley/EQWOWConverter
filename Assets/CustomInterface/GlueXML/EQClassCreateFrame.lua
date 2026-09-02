--
-- EQ class info box for the character creation screen (EQWOW / mod-everquest)
--
-- Every WoW class maps onto a base ("primary") EQ class, and can additionally run one of a small set of
-- secondary EQ classes.  In game that lives on the EQ tab of the character pane (FrameXML/EQClassFrame.lua),
-- fed by the "EQCLASS" addon message.  The glue screen has no world connection and therefore no addon
-- message channel, so the mapping is instead baked into the patch MPQ as EQClassCreateData.lua by the
-- converter (EQWOWConverter/Files/WOWFiles/LUA/EQClassCreateLUA.cs).  Both come from the same
-- PlayerClassMapping.csv that builds mod_everquest_classmap, so this box always agrees with the server.
--
-- EQ_CREATE_CLASSMAP is keyed on the class file name reported by GetSelectedClass() (e.g. "SHAMAN"):
--   { base = "<primary EQ class>", default = "<secondary the character starts with>", seconds = { ... } }
--
-- EQClassCreateInfo_Update() is called from SetCharacterClass() in CharacterCreate.lua, which is the single
-- funnel for both the initial CharacterCreate_OnShow() pass and every subsequent class button click.
--

-- Matches the green used for the active class on the in-game EQ Class tab
local EQ_CLASS_COLOR = "|cff4CFF00";

-- Dashed like the race/class ability lists next door
local EQ_CLASS_NOTES = {
	"- Select your secondary class in-game via the 'EverQuest Classes' icon on the in-game Character Info pane",
	"- Secondary classes determine your level, so switching your secondary will change your level",
	"- You may learn any spells and abilities for both your Primary and Secondary EverQuest class",
};

-- Vertical gaps between the scroll child's font strings, matching the anchors in EQClassCreateFrame.xml,
-- plus a little padding past the last line
local GAP_PRIMARY = 4;
local GAP_SECONDARY_HEADER = 12;
local GAP_SECONDARY = 4;
local GAP_FOOTNOTE = 14;
local PAD_BOTTOM = 8;

-- The scroll child is declared at a placeholder height, so the scroll range only becomes correct once we
-- measure the (variable length, word wrapped) text and size it ourselves.  Setting the height fires
-- OnScrollRangeChanged, which hands the new range to the scroll bar.
local function EQClassCreateInfo_UpdateScrollHeight()
	local height = EQClassCreateInfoPrimaryHeader:GetStringHeight()
		+ GAP_PRIMARY + EQClassCreateInfoPrimaryText:GetStringHeight()
		+ GAP_SECONDARY_HEADER + EQClassCreateInfoSecondaryHeader:GetStringHeight()
		+ GAP_SECONDARY + EQClassCreateInfoSecondaryText:GetStringHeight()
		+ GAP_FOOTNOTE + EQClassCreateInfoFootnote:GetStringHeight()
		+ PAD_BOTTOM;
	EQClassCreateInfoScrollChild:SetHeight(height);
end

function EQClassCreateInfo_OnLoad(self)
	EQClassCreateInfoFootnote:SetText(table.concat(EQ_CLASS_NOTES, "\n"));
end

-- Remeasure once the box is actually on screen, in case the update ran before the font strings had
-- been laid out and reported no height
function EQClassCreateInfo_OnShow(self)
	EQClassCreateInfo_UpdateScrollHeight();
end

--------------------------------------------------------------------------------------------------
-- EverQuest Origins box (EQAlignmentInfo): start zone + alignment for the selected race/class
--------------------------------------------------------------------------------------------------

-- Per-alignment presentation: display name color, round icon (converter-shipped, see
-- AssetConverter.CopyCharacterCreateEQTextures), and the description shown under it
local EQ_ALIGNMENT_DISPLAY = {
	["GOOD"] = {
		name = "|cff4CFF00Good|r",
		icon = "Interface\\Glues\\CharacterCreate\\UI-EQAlign-Good",
		desc = "This character will be aligned Good and will be liked by other Good creatures, tolerated by Neutral, and despised by Evil. Good characters have far more travel ability in the world than Evil characters who must stay in the shadows.",
	},
	["NEUTRAL"] = {
		name = "|cffFFD100Neutral|r",
		icon = "Interface\\Glues\\CharacterCreate\\UI-EQAlign-Neutral",
		desc = "This character will be aligned Neutral and be well tolerated by all other alignments, and should have no issues traversing the world.",
	},
	["EVIL"] = {
		name = "|cffFF4C4CEvil|r",
		icon = "Interface\\Glues\\CharacterCreate\\UI-EQAlign-Evil",
		desc = "This character will be aligned Evil and only liked by other Evil creatures and only tolerated by many Neutral creatures.  Evil will have a hard life as many towns are hostile to them, and particularly port towns will need to be navigated very carefully.",
	},
};

-- Same gap scheme as the class box's scroll child, matching the anchors in EQClassCreateFrame.xml
local function EQAlignmentInfo_UpdateScrollHeight()
	local height = EQAlignmentInfoZoneHeader:GetStringHeight()
		+ GAP_PRIMARY + EQAlignmentInfoZoneText:GetStringHeight()
		+ GAP_SECONDARY_HEADER + EQAlignmentInfoAlignHeader:GetStringHeight()
		+ GAP_SECONDARY + EQAlignmentInfoAlignText:GetStringHeight()
		+ GAP_FOOTNOTE + EQAlignmentInfoAlignDesc:GetStringHeight()
		+ PAD_BOTTOM;
	EQAlignmentInfoScrollChild:SetHeight(height);
end

-- Reads the current race and class selections itself, since it must react to changes of either.
-- Called from both SetCharacterRace() and SetCharacterClass() in CharacterCreate.lua.
function EQAlignmentInfo_Update()
	if ( not EQ_CREATE_STARTINFO ) then
		EQAlignmentInfo:Hide();
		return;
	end

	local _, raceFileName = GetNameForRace();
	local _, classFileName = GetSelectedClass();
	if ( not raceFileName or not classFileName ) then
		EQAlignmentInfo:Hide();
		return;
	end

	local startInfo = EQ_CREATE_STARTINFO[strupper(raceFileName) .. "_" .. strupper(classFileName)];
	local alignmentDisplay = startInfo and EQ_ALIGNMENT_DISPLAY[startInfo.align];
	if ( not alignmentDisplay ) then
		EQAlignmentInfo:Hide();
		return;
	end

	EQAlignmentInfoZoneText:SetText(startInfo.zone);
	EQAlignmentInfoAlignText:SetText(alignmentDisplay.name);
	EQAlignmentInfoAlignDesc:SetText(alignmentDisplay.desc);
	EQAlignmentInfoIcon:SetTexture(alignmentDisplay.icon);

	EQAlignmentInfo_UpdateScrollHeight();
	EQAlignmentInfoScrollFrameScrollBar:SetValue(0);

	EQAlignmentInfo:Show();
end

function EQAlignmentInfo_OnShow(self)
	EQAlignmentInfo_UpdateScrollHeight();
end

-- classFileName is the upper case token from GetSelectedClass(), e.g. "WARRIOR" or "DEATHKNIGHT"
function EQClassCreateInfo_Update(classFileName)
	-- The data table is generated into the patch MPQ; degrade quietly rather than erroring the glue screen
	-- if a client is running without it.
	if ( not EQ_CREATE_CLASSMAP or not classFileName ) then
		EQClassCreateInfo:Hide();
		return;
	end

	local classInfo = EQ_CREATE_CLASSMAP[strupper(classFileName)];
	if ( not classInfo ) then
		EQClassCreateInfo:Hide();
		return;
	end

	EQClassCreateInfoPrimaryText:SetText(EQ_CLASS_COLOR .. classInfo.base .. "|r");

	local secondaryText = "";
	if ( classInfo.seconds ) then
		for _, secondaryName in ipairs(classInfo.seconds) do
			if ( secondaryText ~= "" ) then
				secondaryText = secondaryText .. "\n";
			end
			secondaryText = secondaryText .. EQ_CLASS_COLOR .. secondaryName .. "|r";
			-- Call out the one the character actually starts with
			if ( classInfo.default and secondaryName == classInfo.default ) then
				secondaryText = secondaryText .. "  (starting)";
			end
		end
	end
	if ( secondaryText == "" ) then
		secondaryText = "None";
	end
	EQClassCreateInfoSecondaryText:SetText(secondaryText);

	-- The list length varies by class, so remeasure and send the view back to the top
	EQClassCreateInfo_UpdateScrollHeight();
	EQClassCreateInfoScrollFrameScrollBar:SetValue(0);

	EQClassCreateInfo:Show();
end
