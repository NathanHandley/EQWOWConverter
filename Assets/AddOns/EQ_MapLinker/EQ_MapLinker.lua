local EQ_MapLinker = CreateFrame("Frame")

local MAP_W, MAP_H = 1002, 668
EQ_MapLinker.LINKS = EQ_MapLinker.LINKS or {}

-- Floating zone text frame
local ZoneText = CreateFrame("Frame", "EQ_MapLinkerZoneText", WorldMapFrame)
ZoneText:SetFrameStrata("TOOLTIP")
ZoneText:SetFrameLevel(1000)
ZoneText:SetSize(600, 60)
ZoneText:SetPoint("TOP", WorldMapDetailFrame, "TOP", 0, -15)
ZoneText:Hide()

local ZoneTextString = ZoneText:CreateFontString(nil, "OVERLAY")
ZoneTextString:SetPoint("CENTER", 1, -1)
ZoneTextString:SetFont("Fonts\\FRIZQT__.TTF", 28, "OUTLINE, THICK")
ZoneTextString:SetTextColor(1, 1, 1)
ZoneTextString:SetShadowOffset(0, 0)
ZoneTextString:SetShadowColor(0, 0, 0, 0)

EQ_MapLinker.ZoneText = ZoneText
EQ_MapLinker.ZoneTextString = ZoneTextString

EQ_MapLinkerDB = EQ_MapLinkerDB or { showLinks = true }
EQ_MapLinker.db = EQ_MapLinkerDB

EQ_MapLinker:RegisterEvent("ADDON_LOADED")
EQ_MapLinker:RegisterEvent("PLAYER_LOGIN")

EQ_MapLinker:SetScript("OnEvent", function(self, event, arg1)
    if event == "ADDON_LOADED" and arg1 == "EQ_MapLinker" then
        self:Init()
    elseif event == "WORLD_MAP_UPDATE" then
        self:UpdateButtons()
    elseif event == "PLAYER_LOGIN" then
        self:HookZoomOutButton()
    end
end)

function EQ_MapLinker:Init()
    if EQ_MapLinks then self.LINKS = EQ_MapLinks.LINKS or {} end
    
	-- Add 1 to all of the source map IDs since it's 0 vs 1 based index
	local newLinks = {}
    for sourceMapID, links in pairs(self.LINKS) do
        local newSourceID = sourceMapID + 1
        newLinks[newSourceID] = links
    end
	self.LINKS = newLinks
	self:BuildTargetIndex()
	
	self:CreateToggleButton()
	
	local count = 0 for _ in pairs(self.LINKS) do count = count + 1 end
	
    self.buttons = {}
    self:BuildTargetIndex()
    self:HookMap()
end

function EQ_MapLinker:HookZoomOutButton()
    WorldMapZoomOutButton:HookScript("OnClick", function()
		local currentMapID = GetCurrentMapAreaID()
		local mapEntry = nil
        if EQ_MapLinker.LINKS[currentMapID] then
            mapEntry = EQ_MapLinker.LINKS[currentMapID]
        else
            local originalID = currentMapID - 1
            if EQ_MapLinks and EQ_MapLinks.LINKS and EQ_MapLinks.LINKS[originalID] then
                mapEntry = EQ_MapLinks.LINKS[originalID]
            end
        end
		if mapEntry then
			local customZoomOut = mapEntry and mapEntry.zoomOutMapID
			SetMapByID(customZoomOut)
		end
    end)
end

function EQ_MapLinker:CreateToggleButton()
    local btn = CreateFrame("CheckButton", "EQ_MapLinkerShowLinks", WorldMapFrame, "OptionsCheckButtonTemplate")
    btn:SetSize(24, 24)
    btn:SetPoint("BOTTOMLEFT", WorldMapDetailFrame, "BOTTOMLEFT", 0, -27)
    btn:SetFrameStrata("TOOLTIP")        -- Must be TOOLTIP
    btn:SetFrameLevel(5000)              -- Very high level

    -- Text
    local text = btn:CreateFontString(nil, "OVERLAY", "GameFontNormal")
    text:SetPoint("LEFT", btn, "RIGHT", 4, 0)
    text:SetText("Always Show Linked EQ Zones")

    -- Load saved state
    btn:SetChecked(EQ_MapLinkerDB.showLinks)

    btn:SetScript("OnClick", function(self)
        EQ_MapLinkerDB.showLinks = self:GetChecked()
        EQ_MapLinker:UpdateButtons()
    end)

    btn:SetScript("OnEnter", function(self)
        GameTooltip:SetOwner(self, "ANCHOR_TOPLEFT")
        GameTooltip:SetText("Show/hide map link zones\n(unchecked = invisible until hover)", 1, 1, 1)
        GameTooltip:Show()
    end)
    btn:SetScript("OnLeave", GameTooltip_Hide)

    self.toggleBtn = btn
end

function EQ_MapLinker:BuildTargetIndex()
    self.targetButtons = {}
    for sourceMapID, links in pairs(self.LINKS) do
        for _, link in ipairs(links) do
            local targetID = link.mapID
            if not self.targetButtons[targetID] then
                self.targetButtons[targetID] = {}
            end
            table.insert(self.targetButtons[targetID], {sourceMapID = sourceMapID, link = link})
        end
    end
end

function EQ_MapLinker:HookMap()
    if WorldMapFrame then
        WorldMapFrame:HookScript("OnShow", function() self:UpdateButtons() end)
        hooksecurefunc("SetMapZoom", function() self:DelayUpdate() end)
        hooksecurefunc("SetMapByID", function() self:DelayUpdate() end)
    end
end

function EQ_MapLinker:DelayUpdate()
    if self.timer then return end
    self.timer = true
    local elapsed = 0
    self:SetScript("OnUpdate", function(_, dt)
        elapsed = elapsed + dt
        if elapsed > 0.2 then
            self:UpdateButtons()
            self:SetScript("OnUpdate", nil)
            self.timer = nil
        end
    end)
end

function EQ_MapLinker:GetCurrentMapID()
    return GetCurrentMapAreaID() or 0
end

function EQ_MapLinker:UpdateButtons()
    if not WorldMapFrame or not WorldMapFrame:IsShown() then return end

    local mapID = self:GetCurrentMapID()
    local list = self.LINKS[mapID]

    -- Hide all
    for _, b in ipairs(self.buttons or {}) do b:Hide() end
    self.buttons = {}
    if self.ZoneText then self.ZoneText:Hide() end

    if not list then return end

    local parent = WorldMapDetailFrame
    local sx = parent:GetWidth() / MAP_W
    local sy = parent:GetHeight() / MAP_H

    for i, link in ipairs(list) do
	
        local btn = CreateFrame("Button", "ML_Btn"..i, parent, "SecureActionButtonTemplate")
        btn:SetSize(link.w * sx, link.h * sy)
        btn:SetPoint("TOPLEFT", parent, "TOPLEFT", link.x * sx, -link.y * sy)
        btn:SetFrameStrata("HIGH")
        btn:SetFrameLevel(500)

        -- Grey background
        local bg = btn:CreateTexture(nil, "BACKGROUND")
        bg:SetAllPoints()
        bg:SetTexture("Interface\\Buttons\\WHITE8X8")
        bg:SetVertexColor(1, 1, 1)
		bg:SetAlpha(EQ_MapLinkerDB.showLinks and 0.15 or 0)

        -- Glow
        local glow = btn:CreateTexture(nil, "OVERLAY")
        glow:SetAllPoints()
		glow:SetTexture("Interface\\Buttons\\WHITE8X8")
        glow:SetBlendMode("ADD")
        glow:SetVertexColor(.2, .2, .2, 0.7)
        glow:SetAlpha(0)

        -- Store data
        btn.glow = glow
        btn.zoneName = link.name
        btn.targetMapID = link.mapID

        -- Hover In: Glow THIS + ALL buttons to same target
        btn:SetScript("OnEnter", function(self)
            -- Glow current button
            UIFrameFadeIn(self.glow, 0.2, 0, 1)
            
            -- Glow ALL buttons to same target mapID
            local targetButtons = EQ_MapLinker.targetButtons[self.targetMapID] or {}
            for _, targetInfo in ipairs(targetButtons) do
                -- Find current map's buttons only
                for _, currentBtn in ipairs(EQ_MapLinker.buttons) do
                    if currentBtn.targetMapID == self.targetMapID then
                        UIFrameFadeIn(currentBtn.glow, 0.2, 0, 1)
                    end
                end
            end
            
            -- Show floating text
            EQ_MapLinker.ZoneTextString:SetText(self.zoneName)
            UIFrameFadeIn(EQ_MapLinker.ZoneText, 0.2, 0, 1)
        end)

        -- Hover Out: Stop glow on linked buttons
        btn:SetScript("OnLeave", function(self)
            -- Stop glow on current button
            UIFrameFadeOut(self.glow, 0.4, 1, 0)
            
            -- Stop glow on ALL linked buttons
            local targetButtons = EQ_MapLinker.targetButtons[self.targetMapID] or {}
            for _, targetInfo in ipairs(targetButtons) do
                for _, currentBtn in ipairs(EQ_MapLinker.buttons) do
                    if currentBtn.targetMapID == self.targetMapID then
                        UIFrameFadeOut(currentBtn.glow, 0.4, 1, 0)
                    end
                end
            end

            -- Hide floating text if no other hover
            local anyHover = false
            for _, other in ipairs(EQ_MapLinker.buttons) do
                if other:IsMouseOver() then anyHover = true; break end
            end
            if not anyHover then
                UIFrameFadeOut(EQ_MapLinker.ZoneText, 0.4, 1, 0)
            end
        end)

        -- Click
        btn:SetAttribute("type", "macro")
        btn:SetAttribute("macrotext", "/run SetMapByID("..link.mapID..")")

        btn:Show()
        table.insert(self.buttons, btn)
    end
end

EQ_MapLinker:RegisterEvent("WORLD_MAP_UPDATE")
EQ_MapLinker:RegisterEvent("PLAYER_ENTERING_WORLD")