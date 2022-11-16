using BepInEx;
using HarmonyLib;
using Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateupPrepGhost
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class PlateupPrepGhost : BaseUnityPlugin
    {
        public const string pluginGuid = "happening.plateup.plateupprepghost";
        public const string pluginName = "Plateup Prep Ghost";
        public const string pluginVersion = "1.0";

        public void Awake()
        {
            Harmony harmony = new Harmony(pluginGuid);

            harmony.Patch(AccessTools.Method(typeof(PlayerView), "Update"), 
                prefix:
                new HarmonyMethod(
                    AccessTools.Method(typeof(GhostPatch), "Update_CheckPrepState")));
        }

        
    }
}
