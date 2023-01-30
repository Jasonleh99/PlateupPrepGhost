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
    [HarmonyPatch(typeof(PlayerView), "Update")]
    class GhostPatch
    {
        // This is the value set by the game, is defined in the PlayerView class
        private static readonly float PLAYER_SPEED = 3000f;

        public static bool GhostModeActivated = false;
        public static bool GhostModeSetByMenu = false;
        // Speed to set players at while ghost mode is activated. Default of a 50%
        // speed boost, 1.5f
        public static float GhostSpeed = 1.5f;
        public static bool GhostSpeedSet = false;

        [HarmonyPrefix]
        public static void Update_CheckPrepState(PlayerView __instance, bool ___IsMyPlayer, Rigidbody ___Rigidbody)
        {
            // Do nothing if this view is not owned by the current player
            if (!___IsMyPlayer)
            {
                return;
            }

            HandleSpeed(__instance, ___Rigidbody, PLAYER_SPEED * GhostSpeed);
            HandleNoClip(___Rigidbody);
        }

        public static void SetGhostMode(bool enable, Rigidbody rigidbody)
        {
            Debug.Log(typeof(GhostPatch).Name + ": Ghost Mode set to: " + enable);
            rigidbody.detectCollisions = !enable;
            GhostSpeedSet = false;
        }

        private static bool GhostEnabledForBody(Rigidbody rigidbody)
        {
            return !rigidbody.detectCollisions;
        }

        private static void HandleSpeed(PlayerView playerView, Rigidbody rigidbody, float setSpeed)
        {
            // Don't modify speed if its already been set
            if (GhostSpeedSet)
            {
                return;
            }

            if (GhostModeActivated)
            {
                Debug.Log(typeof(GhostPatch).Name + ": Setting player speed to " + (PLAYER_SPEED * GhostSpeed));
                playerView.Speed = PLAYER_SPEED * GhostSpeed;
                GhostSpeedSet = true;
            } else
            {
                playerView.Speed = PLAYER_SPEED;
                GhostSpeedSet = true;
            }
        }

        private static void HandleNoClip(Rigidbody rigidbody)
        {
            // Disable ghost mode during non prep time no matter what if:
            // 1. ghost mode for rigid body is enabled
            // 2. It isn't prep time or practice time
            // 3. and it isn't in the kitchen
            if (GhostEnabledForBody(rigidbody)
                && !GameInfo.IsPreparationTime
                && GameInfo.CurrentScene == SceneType.Kitchen)
            {
                SetGhostMode(false, rigidbody);
                GhostModeSetByMenu = false;
                GhostModeActivated = false;
                GhostSpeedSet = false;
                return;
            }

            // Ghost mode menu setting takes precedent
            if (GhostModeSetByMenu
                && GhostEnabledForBody(rigidbody) != GhostModeActivated)
            {
                SetGhostMode(GhostModeActivated, rigidbody);
                return;
            }

            // Otherwise activate ghost mode during practice
            if (GameInfo.IsPreparationTime
                && !GhostModeSetByMenu
                && !GhostEnabledForBody(rigidbody))
            {
                GhostModeActivated = true;
                SetGhostMode(true, rigidbody);
                return;
            }
        }
    }
}
