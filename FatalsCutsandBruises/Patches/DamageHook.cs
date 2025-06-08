using HarmonyLib;
using UnityEngine;

namespace FatalsCutsAndBruises.Patches
{
    [HarmonyPatch(typeof(Character), nameof(Character.Damage))]
    public static class DamageHook
    {
        [HarmonyPrefix]
        private static void Prefix(Character __instance, HitData hit)
        {
            if (__instance == null || !__instance.IsPlayer() || !__instance.IsOwner()) return;

            var player = __instance as Player;
            if (player == null) return;

            var seMan = player.GetSEMan();
            if (!seMan.HaveStatusEffect(CutsAndBruises.CutEffectHash) && UnityEngine.Random.value < CutsAndBruises.CutChance.Value)
            {
                seMan.AddStatusEffect(CutsAndBruises.CutEffectPrefab);
                Jotunn.Logger.LogInfo("Cut status effect applied");
            }
        }
    }
}
