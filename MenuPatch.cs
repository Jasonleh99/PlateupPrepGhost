using BepInEx.Logging;
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
    class MenuPatch
    {
        private static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("PlateupPrepGhost_MenuPatch");

        public static void Setup_AddPrepGhostMenu(MainMenu __instance)
        {
            MethodInfo m_addButtonMenu = GetMethod(__instance.GetType(), "AddSubmenuButton");
            m_addButtonMenu.Invoke(__instance, new object[3] { "PrepGhost", typeof(PrepGhostOptionsMenu), false });
        }

        public static void SetupMenus_AddPrepGhostMenu(PlayerPauseView __instance)
        {
            ModuleList moduleList = (ModuleList)__instance.GetType().GetField("ModuleList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            MethodInfo mInfo = GetMethod(__instance.GetType(), "AddMenu");

            mInfo.Invoke(__instance, new object[2] { typeof(PrepGhostOptionsMenu), new PrepGhostOptionsMenu(__instance.ButtonContainer, moduleList) });
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

    class PrepGhostOptionsMenu : Menu<PauseMenuAction>
    {
        private static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("PlateupPrepGhost_PrepGhostOptionsMenu");

        public PrepGhostOptionsMenu(Transform container, ModuleList module_list) : base(container, module_list) {}

        public override void Setup(int player_id)
        {
            Add(GetEnableOption())
                .OnChanged += delegate (object _, bool value)
                {
                    GhostPatch.SetGhostModeForAllPlayers(value);
                    GhostPatch.GhostModeSetByMenu = true;
                };
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

            return new Option<bool>(enableOptions, current, localizationOptions, null);
        }
    }
}
