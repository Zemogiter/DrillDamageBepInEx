using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using BepInEx;
using BepInEx.Configuration;

namespace DrillDamage;

[BepInPlugin(GUID, MODNAME, VERSION)]
internal class Plugin : BaseUnityPlugin
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Log files shouldn't be readonly")]
    private static string logFile = Path.GetFullPath(".\\BepInEx\\plugins\\DrillDamage\\DrillDamage.log");

    public static void Log(string message, int level)
    {
        if ((int)ConfigDebugLevel.BoxedValue >= level)
        {
            File.AppendAllText(logFile, string.Concat(DateTime.Now, "\r\n", message, "\r\n"));
        }
    }
    #region[Declarations]
    private const string
        MODNAME = "DrillDamage",
        AUTHOR = "russleeiv",
        GUID = "russleeiv.subnautica.drilldamage",
        VERSION = "1.1.1.0";
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
    private void InitializeConfig()
    {
        AcceptableValueBase acceptable = new AcceptableValueRange<int>(0,3);
        ConfigAdditionalDamage = base.Config.Bind(new ConfigDefinition("General", "Additional Damage"), 5, new ConfigDescription("This number will be added to drill damage calculation, making it faster.", new AcceptableValueRange<int>(0, 9999)));
        ConfigAffectCreatures = base.Config.Bind(new ConfigDefinition("General", "Affects Creatures"), defaultValue: false, new ConfigDescription("Should the moddified damage calculation be applied to creatures as well?", null));
        ConfigDebugLevel = base.Config.Bind(new ConfigDefinition("General", "Debug Level"), defaultValue: 0, new ConfigDescription("The greater the number, the more information is outputed to DrillDamage.log. By default nothing is outputed there. Increasing the value may negativley affect performance.", acceptable));
        
    }
    private static void Patch_Drillable(Harmony harmony)
    {
        MethodInfo method = typeof(Drillable_OnDrill_Patch).GetMethod("OnDrill");
        Patch_OnDrill(harmony, typeof(Drillable), method);
        Patch_OnDrill(harmony, GetType(".\\BepInEx\\plugins\\SeamothDrillArm\\SeamothDrillArm.dll", "SeamothDrillArm.MonoBehaviours.BetterDrillable"), method);
        Patch_OnDrill(harmony, GetType(".\\BepInEx\\plugins\\SeamothArms\\SeamothArms.dll", "SeamothArms.SeamothDrillable"), method);
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
