using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using BepInEx;
using BepInEx.Configuration;

namespace DrillDamage
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Log files shouldn't be readonly")]
        private static string logFile = Path.GetFullPath(".\\BepInEx\\plugins\\DrillDamage\\DrillDamage.log");
        public void FileEnsurer()
        {
            if (!File.Exists(logFile))
            {
                File.Create(logFile);
            }
        }

        public static void Log(string message, int level)
        {
            if (ConfigDebugLevel.Value >= level)
            {
                File.AppendAllText(logFile, string.Concat(DateTime.Now, "\r\n", message, "\r\n"));
            }
        }
        #region[Declarations]
        private const string
            MODNAME = "DrillDamage",
            AUTHOR = "russleeiv",
            GUID = "russleeiv.subnautica.drilldamage",
            VERSION = "1.2.1.0";
        #endregion

        public void Awake()
        {
            Console.WriteLine("DrillDamage - Started patching v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            InitializeConfig();
            var harmony = new Harmony(GUID);
            Patch_Drillable(harmony);
            Patch_TakeDamage(harmony);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Console.WriteLine("DrillDamage - Finished patching");
        }

        public static ConfigEntry<int> ConfigAdditionalDamage;
        public static ConfigEntry<bool> ConfigAffectCreatures;
        public static ConfigEntry<int> ConfigDebugLevel;
        public static ConfigEntry<bool> ConfigAffectSeamothArms;
        public static ConfigEntry<int> ConfigSeamothAdditionalDamage;
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
            ConfigAdditionalDamage = base.Config.Bind(new ConfigDefinition("General", "Additional Damage"), 5, new ConfigDescription("This number will be added to drill damage calculation, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigAffectCreatures = base.Config.Bind(new ConfigDefinition("General", "Affects Creatures"), false, new ConfigDescription("Should the moddified damage calculation be applied to creatures as well?"));
            ConfigDebugLevel = base.Config.Bind(new ConfigDefinition("General", "Debug Level"), 0, new ConfigDescription("The greater the number, the more information is outputed to DrillDamage.log inside this mod's folder. By default nothing is outputed there. Increasing the value may negativley affect performance.", new AcceptableValueRange<int>(0, 3)));
            ConfigAffectSeamothArms = base.Config.Bind(new ConfigDefinition("General", "Affects Seamoth Arms"), true);
            ConfigSeamothAdditionalDamage = base.Config.Bind(new ConfigDefinition("General", "Additional Damage - Seamoth Arms"), ConfigAdditionalDamage.Value, new ConfigDescription("Will be the same as Additional Damage by default.", new AcceptableValueRange<int>(0, 999)));
            ConfigVariableModeEnabled = base.Config.Bind(new ConfigDefinition("Variable Mode", "Variable Mode Enabled?"), false, new ConfigDescription("Enabling this option will make the extra damage applied differ between the kinds of drillable resources. Disabled by default."));
            ConfigDrillableSaltDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Salt"), 30, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableQuartzDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Quartz"), 20, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableCopperDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Copper"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableTitaniumDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Titanium"), 5, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableLeadDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Lead"), 10, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableSilverDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Silver"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableDiamondDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Diamond"), 3, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableGoldDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Gold"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableMagnetiteDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Magnetite"), 10, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableLithiumDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Lithium"), 4, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableMercuryDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Mercury(cut content)"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableUraniumDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Uranium"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableAluminiumOxideDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Ruby aka Aluminium Oxide"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableNickelDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Nickiel"), 4, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableSulphurDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Sulphur"), 15, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
            ConfigDrillableKyaniteDamage = base.Config.Bind(new ConfigDefinition("Variable Mode", "Additional Damage - Kyanite"), 2, new ConfigDescription("This number will be added to drill damage calculation of this mineable resource, making it faster.", new AcceptableValueRange<int>(0, 999)));
        }
        private static void Patch_Drillable(Harmony harmony)
        {
            MethodInfo method = typeof(Drillable_OnDrill_Patch).GetMethod("OnDrill");
            Patch_OnDrill(harmony, typeof(Drillable), method);
            if (ConfigAffectSeamothArms.Value == true)
            {
                Patch_OnDrill(harmony, GetType("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Subnautica\\BepInEx\\plugins\\ModdedArmsHelper\\ModdedArmsHelper.dll", "ModdedArmsHelper.API.SeamothDrillable"), method);
            }
        }

        private static void Patch_OnDrill(Harmony harmony, Type type, MethodInfo prefix)
        {
            if (!(type == null))
            {
                MethodInfo method = type.GetMethod("OnDrill");
                Log("RALIV.Subnautica.DrillDamage Patch Method " + method.DeclaringType.FullName + "." + method.Name, 1);
                harmony.Patch(method, new HarmonyMethod(prefix), null);
            }
        }

        private static Type GetType(string path, string typeName)
        {
            try
            {
                Type type = Assembly.LoadFrom(path).GetType(typeName);
                Log("RALIV.Subnautica.DrillDamage Found Type " + type.FullName, 2);
                return type;
            }
            catch (Exception)
            {
            }
            return null;
        }

        private static void Patch_TakeDamage(Harmony harmony)
        {
            Log("RALIV.Subnautica.DrillDamage Patch Method LiveMixin.TakeDamage", 1);
            harmony.Patch(typeof(LiveMixin).GetMethod("TakeDamage"), new HarmonyMethod(typeof(LiveMixin_TakeDamage_Patch).GetMethod("TakeDamage")), null);
        }
    }
}
