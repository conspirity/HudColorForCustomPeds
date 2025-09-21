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
        private ScriptSettings _config;
        private bool _isUsingCustomPed;
        private bool _isCustomHudColorSet;
        private int _customColorId;

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
                Notification.Show($"~r~Error~w~: Failed to {iniFile}");
            }
        }

        private void SetCustomHudColor()
        {
            Function.Call(Hash.REPLACE_HUD_COLOUR, _customColorId, 143);
            Function.Call(Hash.REPLACE_HUD_COLOUR, _customColorId, 144);
            Function.Call(Hash.REPLACE_HUD_COLOUR, _customColorId, 145);
        }

        private void RestoreHudColor()
        {
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 143, 101, 180, 212, 255);
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 144, 171, 237, 171, 255);
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 145, 255, 163, 87, 255);
        }

        private void OnTick(object sender, EventArgs e)
        {
            var playerPed = Function.Call<Ped>(Hash.GET_PLAYER_PED);

            var michaelHash = Function.Call<Hash>(Hash.GET_HASH_KEY, "player_zero");
            var franklinHash = Function.Call<Hash>(Hash.GET_HASH_KEY, "player_one");
            var trevorHash = Function.Call<Hash>(Hash.GET_HASH_KEY, "player_two");

            bool isMichael = Function.Call<bool>(Hash.IS_PED_MODEL, playerPed, michaelHash);
            bool isFranklin = Function.Call<bool>(Hash.IS_PED_MODEL, playerPed,franklinHash);
            bool isTrevor = Function.Call<bool>(Hash.IS_PED_MODEL, playerPed, trevorHash);

            if (!isMichael && !isFranklin && !isTrevor)
            {
                _isUsingCustomPed = true;
            }
            else
            { 
                _isUsingCustomPed = false;
            }

            if (_isUsingCustomPed && !_isCustomHudColorSet)
            {
                SetCustomHudColor();
                _isCustomHudColorSet = true;
            }
            else if (!_isUsingCustomPed && _isCustomHudColorSet)
            {
                RestoreHudColor();
                _isCustomHudColorSet = false;
            }
        }

        private void OnAbort(object sender, EventArgs e)
        {
            RestoreHudColor();
        }
    }
}
