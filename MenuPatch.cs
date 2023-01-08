using HarmonyLib;
using Kitchen;
using Kitchen.Modules;
using KitchenData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlateupPrepGhost
{
    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Setup))]
    class MenuPatch
    {
        [HarmonyPrefix]
        public static void Setup_AddPrepGhostMenu(MainMenu __instance)
        {
            MethodInfo m_addButtonMenu = GetMethod(__instance.GetType(), "AddSubmenuButton");
            m_addButtonMenu.Invoke(__instance, new object[3] { "PrepGhost", typeof(PrepGhostOptionsMenu), false });
        }

        public static MethodInfo GetMethod(Type _typeOfOriginal, string _name, Type _genericT = null)
        {
            MethodInfo retVal = _typeOfOriginal.GetMethod(_name, BindingFlags.NonPublic | BindingFlags.Instance);
            if (_genericT != null)
            {
                retVal = retVal.MakeGenericMethod(_genericT);
            }
            return retVal;
        }
    }

    [HarmonyPatch(typeof(PlayerPauseView), "SetupMenus")]
    class PausePatch
    {
        [HarmonyPrefix]
        public static void SetupMenus_AddPrepGhostMenu(PlayerPauseView __instance)
        {
            ModuleList moduleList = (ModuleList)__instance.GetType().GetField("ModuleList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            MethodInfo mInfo = MenuPatch.GetMethod(__instance.GetType(), "AddMenu");

            mInfo.Invoke(__instance, new object[2] { typeof(PrepGhostOptionsMenu), new PrepGhostOptionsMenu(__instance.ButtonContainer, moduleList) });
        }
    }

    class PrepGhostOptionsMenu : Menu<PauseMenuAction>
    {
        public Option<bool> EnableOption;
        // Default player set while ghost mode is activated
        public Option<float> SpeedOption;

        public PrepGhostOptionsMenu(Transform container, ModuleList module_list) : base(container, module_list) {}

        public override void Setup(int player_id)
        {
            EnableOption = GetEnableOption();
            this.AddLabel("Ghost Mode");
            Add(EnableOption);

            SpeedOption = GetSpeedOption();
            this.AddLabel("Ghost Speed");
            Add(SpeedOption);

            AddButton(Localisation["MENU_BACK_SETTINGS"], (Action<int>)(i => RequestPreviousMenu()));
        }

        private Option<bool> GetEnableOption()
        {
            List<bool> enableOptions = new List<bool>
            {
                false, true
            };
            bool current = GhostPatch.GhostModeActivated;
            List<string> localizationOptions = new List<string>
            {
                this.Localisation["SETTING_DISABLED"],
                this.Localisation["SETTING_ENABLED"]
            };

            Option<bool> enableOption = new Option<bool>(enableOptions, current, localizationOptions, null);
            enableOption.OnChanged += delegate (object _, bool value)
            {
                GhostPatch.GhostModeActivated = value;
                GhostPatch.GhostModeSetByMenu = true;
            };

            return enableOption;
        }

        private Option<float> GetSpeedOption()
        {
            List<float> speedOptions = new List<float>()
            {
                1f
            };
            List<string> localization = new List<string>
            {
                this.Localisation["SETTING_DISABLED"]
            };

            for (float i = 1.5f; i <= 10f; i += 0.5f)
            {
                speedOptions.Add(i);
                localization.Add(i + "");
            }
            float current = GhostPatch.GhostSpeed;

            Option<float> speedOption = new Option<float>(speedOptions, current, localization, null);
            speedOption.OnChanged += delegate (object _, float value)
            {
                GhostPatch.GhostSpeed = value;
            };

            return speedOption;
        }
    }
}
