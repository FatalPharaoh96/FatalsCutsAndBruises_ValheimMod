using FatalsCutsAndBruises;
using Jotunn;
using System.Linq;
using UnityEngine;

namespace FatalsCutsAndBruises.Effects
{
    public class CutEffect : SE_Stats
    {
        private float infectionCheckTimer = 0f;

        public override void Setup(Character character)
        {
            base.Setup(character);
            Jotunn.Logger.LogInfo("CutEffect Setup() called");
        }

        private bool PlayerHasAntibioticInFoodBar(Player player)
        {
            return player.m_foods.Any(fd =>
                fd.m_item != null &&
                fd.m_item.m_shared != null &&
                fd.m_item.m_shared.m_name == "$item_antibiotic");
        }

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            infectionCheckTimer += dt;

            if (infectionCheckTimer >= 1f)
            {
                infectionCheckTimer = 0f;

                var player = m_character as Player;
                if (player == null)
                    return;

                Jotunn.Logger.LogInfo("CutEffect UpdateStatusEffect() tick");

                var seMan = m_character.GetSEMan();

                bool hasAntibiotic = PlayerHasAntibioticInFoodBar(player);

                if (!hasAntibiotic &&
                    !seMan.HaveStatusEffect(CutsAndBruises.InfectionEffectHash) &&
                    Random.value < CutsAndBruises.InfectionChance.Value)
                {
                    Jotunn.Logger.LogInfo("Applying InfectionEffect from CutEffect");
                    seMan.AddStatusEffect(CutsAndBruises.InfectionEffectPrefab);
                }
                else if (hasAntibiotic)
                {
                    Jotunn.Logger.LogInfo("Player has antibiotic in food bar, skipping infection");
                }
            }
        }
    }
}
