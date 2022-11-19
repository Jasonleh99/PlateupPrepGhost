using BepInEx.Logging;
using Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlateupPrepGhost
{
    class BoundariesPatch
    {

        private static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("BoundariesPatch");

        public static bool OnUpdate_disableBounds(EnforcePlayerBounds __instance)
        {
            return false;
        }
    }
}