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
    class GhostPatch
    {
        private const string PLAYERS_COLLISION_LAYER_NAME = "Players";
        private const string DYNAMIC_OBJECT_COLLISION_LAYER_NAME = "Default";
        private const string STATIC_OBJECT_COLLISION_LAYER_NAME = "Statics";

        private static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("GhostPatch");
        private static bool GhostModeActivated = false;

        public static bool Update_CheckPrepState(PlayerView __instance, Rigidbody ___Rigidbody)
        {
            if (GameInfo.IsPreparationTime && !GhostModeActivated)
            {
                logger.LogInfo("Activating Ghost Mode");
                SetGhostMode(true);
                GhostModeActivated = true;
            } else if (!GameInfo.IsPreparationTime && GhostModeActivated)
            {
                logger.LogInfo("Deactivating Ghost Mode");
                SetGhostMode(false);
                GhostModeActivated = false;
            }
            
            return true;
        }

        public static void SetGhostMode(bool enable)
        {
            Physics.IgnoreLayerCollision(
                LayerMask.NameToLayer(PLAYERS_COLLISION_LAYER_NAME)
                , LayerMask.NameToLayer(DYNAMIC_OBJECT_COLLISION_LAYER_NAME)
                , enable);
            Physics.IgnoreLayerCollision(
                LayerMask.NameToLayer(PLAYERS_COLLISION_LAYER_NAME)
                , LayerMask.NameToLayer(STATIC_OBJECT_COLLISION_LAYER_NAME)
                , enable);
        }
    }
}
/***
 * 
 * 
 * 
 * 
 * 
 */
