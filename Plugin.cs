using System;
using System.Reflection;
using HarmonyLib;
using BepInEx;
using BepInEx.Configuration;

namespace DrillDamage
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        internal const string
            MODNAME = "DrillDamage",
            AUTHOR = "russleeiv",
            GUID = "russleeiv.subnautica.drilldamage",
            VERSION = "1.3.2.0";
        #endregion

        public void Awake()
        {
            Logger.LogInfo("DrillDamage - Started patching v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            InitializeConfig();
            var harmony = new Harmony(GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo("DrillDamage - Finished patching");
        }

        public static ConfigEntry<int> ConfigAdditionalDamage;
        public static ConfigEntry<bool> ConfigAffectCreatures;
        public static ConfigEntry<bool> ConfigAffectSeamothArms;
        public static ConfigEntry<int> ConfigSeamothAdditionalDamage;
        public static ConfigEntry<bool> ConfigDebugMode;
        public static ConfigEntry<bool> ConfigVariableModeEnabled;
        public static ConfigEntry<int> ConfigDrillableSaltDamage;
        public static ConfigEntry<int> ConfigDrillableQuartzDamage;
        public static ConfigEntry<int> ConfigDrillableCopperDamage;
        public static ConfigEntry<int> ConfigDrillableTitaniumDamage;
        public static ConfigEntry<int> ConfigDrillableLeadDamage;
        public static ConfigEntry<int> ConfigDrillableSilverDamage;
        public static ConfigEntry<int> ConfigDrillableDiamondDamage;
        public static ConfigEntry<int> ConfigDrillableGoldDamage;
        public static ConfigEntry<int> ConfigDrillableMagnetiteDamage;
        public static ConfigEntry<int> ConfigDrillableLithiumDamage;
        public static ConfigEntry<int> ConfigDrillableMercuryDamage;
        public static ConfigEntry<int> ConfigDrillableUraniumDamage;
        public static ConfigEntry<int> ConfigDrillableAluminiumOxideDamage;
        public static ConfigEntry<int> ConfigDrillableNickelDamage;
        public static ConfigEntry<int> ConfigDrillableSulphurDamage;
        public static ConfigEntry<int> ConfigDrillableKyaniteDamage;

        private void InitializeConfig()
        {
            ConfigAdditionalDamage = Config.Bind(new ConfigDefinition("General", "Additional Damage"), 5, new ConfigDescription("This number will be added to drill damage calculation, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigAffectCreatures = Config.Bind(new ConfigDefinition("General", "Affects Creatures"), false, new ConfigDescription("Should the moddified damage calculation be applied to creatures as well?"));
            ConfigAffectSeamothArms = Config.Bind(new ConfigDefinition("General", "Affects Seamoth Arms"), true);
            ConfigSeamothAdditionalDamage = Config.Bind(new ConfigDefinition("General", "Additional Damage - Seamoth Arms"), ConfigAdditionalDamage.Value, new ConfigDescription("Will be the same as Additional Damage by default.", new AcceptableValueRange<int>(0, 999)));
            ConfigDebugMode = Config.Bind(new ConfigDefinition("General", "Debug Mode Enabled?"), true);
            ConfigVariableModeEnabled = Config.Bind(new ConfigDefinition("Variable Mode", "*Variable Mode Enabled?"), false, new ConfigDescription("Enabling this option will make the extra damage applied differ between the kinds of drillable resources. Disabled by default."));
            ConfigDrillableSaltDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Salt"), 30, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableQuartzDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Quartz"), 20, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableCopperDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Copper"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableTitaniumDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Titanium"), 5, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableLeadDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Lead"), 10, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableSilverDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Silver"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableDiamondDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Diamond"), 3, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableGoldDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Gold"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableMagnetiteDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Magnetite"), 10, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableLithiumDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Lithium"), 4, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableMercuryDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Mercury(cut content)"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableUraniumDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Uranium"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableAluminiumOxideDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Ruby/Aluminium Oxide"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableNickelDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Nickiel"), 4, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableSulphurDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Sulphur"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableKyaniteDamage = Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Kyanite"), 2, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
        }
    }
}
