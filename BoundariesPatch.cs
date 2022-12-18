using HarmonyLib;
using Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlateupPrepGhost
{
    [HarmonyPatch(typeof(EnforcePlayerBounds), "OnUpdate")]
    class BoundariesPatch
    {
        [HarmonyPrefix]
        public static bool OnUpdate_disableBounds(EnforcePlayerBounds __instance)
        {
            return false;
        }
    }
}