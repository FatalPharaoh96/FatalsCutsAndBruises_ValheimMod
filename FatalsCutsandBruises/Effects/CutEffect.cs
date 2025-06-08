using UnityEngine;
using Jotunn;
using FatalsCutsAndBruises;

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

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);

            infectionCheckTimer += dt;
            if (infectionCheckTimer >= 1f)
            {
                // infectionCheckTimer = 0f;

                Jotunn.Logger.LogInfo("CutEffect UpdateStatusEffect() tick");

                var seMan = m_character.GetSEMan();
                if (!seMan.HaveStatusEffect(CutsAndBruises.InfectionEffectHash) &&
                    Random.value < CutsAndBruises.InfectionChance.Value)
                {
                    Jotunn.Logger.LogInfo("Applying InfectionEffect from CutEffect");
                    seMan.AddStatusEffect(CutsAndBruises.InfectionEffectPrefab);
                }
            }
        }
    }
}
