using HarmonyLib;
using Kitchen;
using KitchenMods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlateupPrepGhost
{
    public class PlateupPrepGhost : GenericSystemBase, IModSystem
    {
        public static readonly string VERSION = "1.15";
        protected override void Initialise()
        {
            if (GameObject.FindObjectOfType<PrepGhostPatcher>() != null)
                return;
            GameObject prepGhostMod = new GameObject("PlateupPrepGhost");
            prepGhostMod.AddComponent<PrepGhostPatcher>();
            GameObject.DontDestroyOnLoad(prepGhostMod);
        }

        protected override void OnUpdate() {}
    }

    public class PrepGhostPatcher : MonoBehaviour
    {
        private readonly HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("happening.plateup.plateupprepghost");

        private void Awake()
        {
            harmony.PatchAll();
        }
    }
}
