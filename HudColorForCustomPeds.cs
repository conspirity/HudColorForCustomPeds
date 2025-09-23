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
                Notification.Show($"[{ModName} {ModVersion}] ~r~Error: ~w~Failed to load {iniFile}. Using default values instead.");
            }
        }

        private void UseCustomHudColor()
        {
            Function.Call(Hash.REPLACE_HUD_COLOUR, 116, 143);
            Function.Call(Hash.REPLACE_HUD_COLOUR, 116, 144);
            Function.Call(Hash.REPLACE_HUD_COLOUR, 116, 145);

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
            var playerPed = Function.Call<Ped>(Hash.PLAYER_PED_ID);

            var michaelHash = Function.Call<Hash>(Hash.GET_HASH_KEY, "player_zero");
            var franklinHash = Function.Call<Hash>(Hash.GET_HASH_KEY, "player_one");
            var trevorHash = Function.Call<Hash>(Hash.GET_HASH_KEY, "player_two");

            bool isMichael = Function.Call<bool>(Hash.IS_PED_MODEL, playerPed, michaelHash);
            bool isFranklin = Function.Call<bool>(Hash.IS_PED_MODEL, playerPed,franklinHash);
            bool isTrevor = Function.Call<bool>(Hash.IS_PED_MODEL, playerPed, trevorHash);

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
