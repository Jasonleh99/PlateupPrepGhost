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
            if (GameInfo.IsPreparationTime 
                && !GhostModeActivated 
                && !GhostModeSetByMenu)
            {
                SetGhostMode(true, ___Rigidbody);
            } else if (!GameInfo.IsPreparationTime 
                && GhostModeActivated 
                && GameInfo.CurrentScene == SceneType.Kitchen)
            {
                SetGhostMode(false, ___Rigidbody);
                GhostModeSetByMenu = false;
            }

            return true;
        }

        public static void SetGhostMode(bool enable, Rigidbody rigidbody)
        {
            logger.LogInfo("Ghost Mode set to: " + enable);
            rigidbody.detectCollisions = !enable;
            GhostModeActivated = enable;
        }

        public static void SetGhostModeForAllPlayers(bool value)
        {
            PlayerView[] players = UnityEngine.Object.FindObjectsOfType<PlayerView>();
            List<Rigidbody> rigidbodies = new List<Rigidbody>();
            players.ToList().ForEach(player => rigidbodies.Add(player.GameObject.GetComponent<Rigidbody>()));
            rigidbodies.ForEach(player => GhostPatch.SetGhostMode(value, player));
        }
    }
}
