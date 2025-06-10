// CutsAndBruises.cs
using BepInEx;
using BepInEx.Configuration;
using FatalsCutsAndBruises.Effects;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using StatusEffectManager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FatalsCutsAndBruises
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class CutsAndBruises : BaseUnityPlugin
    {
        public const string PluginGUID = "jotunn.fatal.cutsandbruises";
        public const string PluginName = "Fatals Cuts and Bruises";
        public const string PluginVersion = "1.0.0";

        public static ConfigEntry<float> CutChance;
        public static ConfigEntry<float> CutDuration;
        public static ConfigEntry<float> BruiseChance;
        public static ConfigEntry<float> BruiseDuration;
        public static ConfigEntry<float> BruiseTickInterval;
        public static ConfigEntry<float> BruiseStaminaReduction;
        public static ConfigEntry<float> InfectionChance;
        public static ConfigEntry<float> InfectionDamagePerTick;
        public static ConfigEntry<float> InfectionTickInterval;
        public static ConfigEntry<float> InfectionDuration;

        public static Sprite CutSprite;
        public static Sprite InfectedSprite;
        public static Sprite BruiseSprite;
        public static Sprite AntibioticSprite;

        public static StatusEffect CutEffectPrefab;
        public static StatusEffect BruiseEffectPrefab;
        public static StatusEffect InfectionEffectPrefab;

        public static int CutEffectHash;
        public static int BruiseEffectHash;
        public static int InfectionEffectHash;

        private Harmony harmony;

        private void Awake()
        {
            Jotunn.Logger.LogInfo("CutsAndBruises mod initializing...");

            harmony = new Harmony("com.fatal.cutsandbruises");
            harmony.PatchAll();

            CreateConfig();
            LoadIcons();

            PrefabManager.OnVanillaPrefabsAvailable += RegisterStatusEffects;
            PrefabManager.OnVanillaPrefabsAvailable += CreateAntibioticItem;

            AddLocalizations();
        }

        private void CreateConfig()
        {
            CutChance = Config.Bind("General", "Cut Chance", 0.1f);
            CutDuration = Config.Bind("General", "Cut Duration", 30.0f);
            BruiseChance = Config.Bind("General", "Bruise Chance", 0.2f);
            BruiseDuration = Config.Bind("General", "Bruise Duration", 45.0f);
            BruiseTickInterval = Config.Bind("General", "Bruise Tick Interval", 2f);
            BruiseStaminaReduction = Config.Bind("General", "Bruise Stamina Reduction", 10f);
            InfectionChance = Config.Bind("General", "Infection Chance", 0.01f);
            InfectionDamagePerTick = Config.Bind("General", "Infection Damage", 2f);
            InfectionTickInterval = Config.Bind("General", "Infection Tick Interval", 4f);
            InfectionDuration = Config.Bind("General", "Infection Duration", 2000.0f);
        }

        private void AddLocalizations()
        {
            var localization = LocalizationManager.Instance.GetLocalization();
            localization.AddTranslation("English", new Dictionary<string, string>
            {
                { "status_cut", "Cut" },
                { "status_infected", "Infected" },
                { "status_bruise", "Bruise" },
                { "item_antibiotic", "Antibiotic" },
                { "item_antibiotic_description", "A dose of antibiotics. Removes infection." }
            });

            LocalizationManager.Instance.AddLocalization(localization);
        }

        private void RegisterStatusEffects()
        {
            // Unsubscribe to prevent duplicate registration
            PrefabManager.OnVanillaPrefabsAvailable -= RegisterStatusEffects;

            var cutEffect = ScriptableObject.CreateInstance<CutEffect>();
            cutEffect.name = "Cut";
            cutEffect.m_name = "$status_cut";
            cutEffect.m_icon = CutSprite;
            cutEffect.m_tooltip = "You're bleeding from a wound.";
            cutEffect.m_ttl = CutDuration.Value;
            //cutEffect.m_flashIcon = true;

            CutsAndBruises.CutEffectPrefab = cutEffect;
            CutsAndBruises.CutEffectHash = cutEffect.name.GetStableHashCode();

            var bruiseEffect = ScriptableObject.CreateInstance<BruiseEffect>();
            bruiseEffect.name = "Bruise";
            bruiseEffect.m_name = "$status_bruise";
            bruiseEffect.m_icon = BruiseSprite;
            bruiseEffect.m_tooltip = "You have a bruise that reduces your stamina.";
            bruiseEffect.m_ttl = BruiseDuration.Value;

            CutsAndBruises.BruiseEffectPrefab = bruiseEffect;
            CutsAndBruises.BruiseEffectHash = bruiseEffect.name.GetStableHashCode();

            var infectionEffect = ScriptableObject.CreateInstance<InfectionEffect>();
            infectionEffect.name = "Infected";
            infectionEffect.m_name = "$status_infected";
            infectionEffect.m_icon = InfectedSprite;
            infectionEffect.m_tooltip = "The wound is infected and draining your strength.";
            infectionEffect.m_ttl = CutsAndBruises.InfectionDuration.Value;
            //infectionEffect.m_flashIcon = true;

            CutsAndBruises.InfectionEffectPrefab = infectionEffect;
            CutsAndBruises.InfectionEffectHash = infectionEffect.name.GetStableHashCode();

            // Debug: Log icon status
            Jotunn.Logger.LogInfo($"CutEffect m_icon is null: {cutEffect.m_icon == null}");
            Jotunn.Logger.LogInfo($"InfectionEffect m_icon is null: {infectionEffect.m_icon == null}");

            // Register
            ObjectDB.instance.m_StatusEffects.Add(CutEffectPrefab);
            ObjectDB.instance.m_StatusEffects.Add(InfectionEffectPrefab);

            Jotunn.Logger.LogInfo("Custom status effects registered using SE_Stats clone.");
        }

        private void LoadIcons()
        {
            Jotunn.Logger.LogInfo("LoadIcons() called");

            var cutBundle = AssetUtils.LoadAssetBundleFromResources("cut", typeof(CutsAndBruises).Assembly);
            if (cutBundle == null)
            {
                Jotunn.Logger.LogError("Failed to load Cut asset bundle");
                return;
            }

            CutSprite = cutBundle.LoadAsset<Sprite>("cut");
            Jotunn.Logger.LogInfo($"CutSprite loaded: {CutSprite != null}");

            var infectedBundle = AssetUtils.LoadAssetBundleFromResources("infected", typeof(CutsAndBruises).Assembly);
            if (infectedBundle == null)
            {
                Jotunn.Logger.LogError("Failed to load Infected asset bundle");
                return;
            }
            InfectedSprite = infectedBundle.LoadAsset<Sprite>("infected");
            Jotunn.Logger.LogInfo($"InfectedSprite loaded: {InfectedSprite != null}");

            var antibioticBundle = AssetUtils.LoadAssetBundleFromResources("antibiotic", typeof(CutsAndBruises).Assembly);
            if (antibioticBundle == null)
            {
                Jotunn.Logger.LogError("Failed to load Antibiotic asset bundle");
                return;
            }
            AntibioticSprite = antibioticBundle.LoadAsset<Sprite>("antibiotic");
            Jotunn.Logger.LogInfo($"AntibioticSprite loaded: {AntibioticSprite != null}");

            var bruiseBundle = AssetUtils.LoadAssetBundleFromResources("bruise", typeof(CutsAndBruises).Assembly);
            if (bruiseBundle == null)
            {
                Jotunn.Logger.LogError("Failed to load Bruise asset bundle");
                return;
            }
            BruiseSprite = bruiseBundle.LoadAsset<Sprite>("bruise");
            Jotunn.Logger.LogInfo($"BruiseSprite loaded: {BruiseSprite != null}");
        }

        private void CreateAntibioticItem()
        {
            ItemConfig antibioticConfig = new ItemConfig
            {
                Name = "$item_antibiotic",
                Description = "$item_antibiotic_description",
                CraftingStation = "Workbench",
                Icon = AntibioticSprite,
                Requirements = new RequirementConfig[]
                {
            new RequirementConfig("Mushroom", 1),
            new RequirementConfig("Dandelion", 2)
                }
            };

            CustomItem antibioticItem = new CustomItem("antibioticPrefab", "Mushroom", antibioticConfig);

            antibioticItem.ItemDrop.m_itemData.m_shared.m_consumeStatusEffect = ScriptableObject.CreateInstance<RemoveInfectionEffect>();

            ItemManager.Instance.AddItem(antibioticItem);

            PrefabManager.OnVanillaPrefabsAvailable -= CreateAntibioticItem;
        }



        // Custom status effect to remove infection
        public class RemoveInfectionEffect : StatusEffect
        {
            public override void Setup(Character character)
            {
                base.Setup(character);
                if (m_character != null)
                {
                    // Remove the infection effect if present
                    m_character.m_seman.RemoveStatusEffect(CutsAndBruises.InfectionEffectHash, true);
                }
            }
        }
    }
}
