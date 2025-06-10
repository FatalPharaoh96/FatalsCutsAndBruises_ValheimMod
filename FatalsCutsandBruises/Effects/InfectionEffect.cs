using FatalsCutsAndBruises;
using Jotunn;
using System.Linq;
using UnityEngine;

namespace FatalsCutsAndBruises.Effects
{

    public class InfectionEffect : SE_Stats
    {
        private float damageTimer = 0f;

        public override void Setup(Character character)
        {
            base.Setup(character);
            m_ttl = CutsAndBruises.InfectionDuration.Value;
            Jotunn.Logger.LogInfo("InfectionEffect Setup() called");
        }

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);

            damageTimer += dt;
            if (damageTimer >= CutsAndBruises.InfectionTickInterval.Value)
            {
                damageTimer = 0f;
                Jotunn.Logger.LogInfo("InfectionEffect dealing damage");

                var hit = new HitData();
                hit.m_damage.m_damage = CutsAndBruises.InfectionDamagePerTick.Value;
                m_character.Damage(hit);
            }

        }
    }
}
