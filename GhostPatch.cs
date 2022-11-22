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
        private static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("GhostPatch");
        public static bool GhostModeActivated = false;
        public static bool GhostModeSetByMenu = false;

        public static bool Update_CheckPrepState(Rigidbody ___Rigidbody)
        {
            // Disable ghost mode during non prep time no matter what if:
            // 1. ghost mode for rigid body is enabled
            // 2. It isn't prep time or practice time
            // 3. and it isn't in the kitchen
            if (GhostEnabledForBody(___Rigidbody)
                && !GameInfo.IsPreparationTime
                && GameInfo.CurrentScene == SceneType.Kitchen)
            {
                SetGhostMode(false, ___Rigidbody);
                GhostModeSetByMenu = false;
                GhostModeActivated = false;
                return true;
            }

            // Ghost mode menu setting takes precedent
            if (GhostModeSetByMenu && GhostEnabledForBody(___Rigidbody) != GhostModeActivated)
            {
                SetGhostMode(GhostModeActivated, ___Rigidbody);
                return true;
            }

            // Otherwise activate ghost mode during practice
            if (GameInfo.IsPreparationTime 
                && !GhostModeSetByMenu
                && !GhostEnabledForBody(___Rigidbody))
            {
                GhostModeActivated = true;
                SetGhostMode(true, ___Rigidbody);
            }

            return true;
        }

        public static void SetGhostMode(bool enable, Rigidbody rigidbody)
        {
            logger.LogInfo("Ghost Mode set to: " + enable);
            rigidbody.detectCollisions = !enable;
        }

        public static void SetGhostModeForAllPlayers(bool value)
        {
            GhostModeActivated = value;
            PlayerView[] players = UnityEngine.Object.FindObjectsOfType<PlayerView>();
            List<Rigidbody> rigidbodies = new List<Rigidbody>();
            players.ToList().ForEach(player => rigidbodies.Add(player.GameObject.GetComponent<Rigidbody>()));
            rigidbodies.ForEach(player => GhostPatch.SetGhostMode(value, player));
        }

        private static bool GhostEnabledForBody(Rigidbody rigidbody)
        {
            return !rigidbody.detectCollisions;
        }
    }
}
