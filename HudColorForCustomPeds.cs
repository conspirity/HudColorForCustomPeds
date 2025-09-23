using System;
using GTA;
using GTA.Native;
using GTA.UI;

/*
 *  116 	HUD_COLOUR_FREEMODE 	rgba(45, 110, 185, 255) <- custom hud color index, can be changed in ini file
 *  143     HUD_COLOUR_MICHAEL      rgba(101, 180, 212, 255) <- to be replaced
 *  144     HUD_COLOUR_FRANKLIN     rgba(171, 237, 171, 255) <- to be replaced
 *  145     HUD_COLOUR_TREVOR       rgba(255, 163, 87, 255) <- to be replaced
 *  
 *  Find color indexes here: https://docs.fivem.net/docs/game-references/hud-colors/
*/

namespace HudColorForCustomPeds
{
    public class HudColorForCustomPeds : Script
    {
        public string ModName = "Hud Color Override For Custom Peds";
        public string ModVersion = "v1.0";

        private ScriptSettings _config;
        private bool _isUsingCustomPed;
        private bool _isCustomHudColorSet;
        private readonly int _customColorId;

        public HudColorForCustomPeds()
        {
            Tick += OnTick;
            Aborted += OnAbort;

            LoadIniFile("scripts//HudColorForCustomPeds.ini");
            _customColorId = _config.GetValue("General", "color_index", 116);
        }

        private void LoadIniFile(string iniFile)
        {
            try
            {
                _config = ScriptSettings.Load(iniFile);
            }
            catch
            {
                Notification.PostTicker($"[{ModName} {ModVersion}] ~r~Error: ~w~Failed to load {iniFile}. Using default values instead.", false, false);
            }
        }

        private void UseCustomHudColor()
        {
            Function.Call(Hash.REPLACE_HUD_COLOUR, 143, _customColorId);
            Function.Call(Hash.REPLACE_HUD_COLOUR, 144, _customColorId);
            Function.Call(Hash.REPLACE_HUD_COLOUR, 145, _customColorId);

            _isCustomHudColorSet = true;
        }

        private void RestoreHudColor()
        {
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 143, 101, 180, 212, 255);
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 144, 171, 237, 171, 255);
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 145, 255, 163, 87, 255);

            _isCustomHudColorSet = false;
        }

        private void OnTick(object sender, EventArgs e)
        {
            Player player = Game.Player;
            Ped playerPed = player.Character;
            string playerPedHash = playerPed.Model.Hash.ToString();

            bool isMichael = playerPedHash == "225514697" || playerPedHash == "player_zero" || playerPedHash == "Michael";
            bool isFranklin = playerPedHash == "-1692214353" || playerPedHash == "player_one" || playerPedHash == "Franklin";
            bool isTrevor = playerPedHash == "-1686040670" || playerPedHash == "player_two" || playerPedHash == "Trevor";

            if (!isMichael && !isFranklin && !isTrevor) _isUsingCustomPed = true;
            else _isUsingCustomPed = false;

            if (_isUsingCustomPed && !_isCustomHudColorSet) UseCustomHudColor();
            if (!_isUsingCustomPed && _isCustomHudColorSet) RestoreHudColor();
        }

        private void OnAbort(object sender, EventArgs e)
        {
            RestoreHudColor();
        }
    }
}
