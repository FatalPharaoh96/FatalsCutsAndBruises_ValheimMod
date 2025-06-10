using FatalsCutsAndBruises;
using Jotunn;
using System.Linq;
using UnityEngine;

namespace FatalsCutsAndBruises.Effects
{

    public class BruiseEffect : SE_Stats
    {
        private float damageTimer = 0f;

        public override void Setup(Character character)
        {
            base.Setup(character);
            m_ttl = CutsAndBruises.BruiseDuration.Value;
            Jotunn.Logger.LogInfo("BruiseEffect Setup() called");
        }

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);

            damageTimer += dt;
            if (damageTimer >= CutsAndBruises.BruiseTickInterval.Value)
            {
                damageTimer = 0f;
                Jotunn.Logger.LogInfo("BruiseEffect reducing stamina");

                if (m_character is Player player)
                {
                    float staminaDrain = CutsAndBruises.BruiseStaminaReduction.Value;
                    player.UseStamina(staminaDrain);
                }
            }
        }

    }
}