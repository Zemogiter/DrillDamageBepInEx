using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using Nautilus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using BepInEx.Logging;

namespace DrillDamage
{
    [Menu("DrillDamage Options - General", LoadOn = (MenuAttribute.LoadEvents.MenuRegistered | MenuAttribute.LoadEvents.MenuOpened), SaveOn = (MenuAttribute.SaveEvents.ChangeValue | MenuAttribute.SaveEvents.SaveGame | MenuAttribute.SaveEvents.QuitGame))]
    public class Config : ConfigFile
    {
        [Slider("Additional Damage - Main", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "This number will be added to drill damage calculation, making it faster.")]
        public int additionaldamage = 10;

        [Toggle("Affect Creatures")]
        public bool affectcreatures = false;

        public int _additionaldamagecreatures;
        [Slider("Additional Damage - Creatures", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "Will be the same as Additional Damage by default.")]
        public int additionaldamagecreatures
        {
            get => _additionaldamagecreatures == 0 ? additionaldamage : _additionaldamagecreatures;
            set => _additionaldamagecreatures = value;
        }

        [Toggle("Affects Seamoth Arms", Tooltip = "Only works if Seamoth Arms mod is enabled.")]
        public bool affectsseamotharms = true;

        public int _additionaldamageseamotharms;
        [Slider("Additional Damage - Seamoth Arms", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "Will be the same as Additional Damage by default.")]
        public int additionaldamageseamotharms
        {
            get => _additionaldamageseamotharms == 0 ? additionaldamage : _additionaldamageseamotharms;
            set => _additionaldamageseamotharms = value;
        }

        [Toggle("Affects Vehicle Framework drills", Tooltip = "Only works if Vehicle Framework mod is enabled.")]
        public bool affectsvehicleframeworkdrills = true;

        public int _additionaldamagevehicleframeworkdrills;
        [Slider("Additional Damage - Vehicle Framework drills", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "Will be the same as Additional Damage by default.")]
        public int additionaldamagevehicleframeworkdrills
        {
            get => _additionaldamagevehicleframeworkdrills == 0 ? additionaldamage : _additionaldamagevehicleframeworkdrills;
            set => _additionaldamagevehicleframeworkdrills = value;
        }

        [Toggle("Debug mode")]
        public bool debugmode = false;

        [Toggle("Variable Mode", Tooltip = "Enabling this option will make the extra damage applied differ between the kinds of drillable resources. Disabled by default. Enabling this will ignore every Additional Damage setting, except for the creatures one.")]
        public bool variablemode = false;

        // runtime-populated dictionary of techName -> extraDamage
        public Dictionary<string, int> drillableOreList = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public static Dictionary<string, int> DrillableOreList => Instance.drillableOreList;
        public static Config Instance { get; private set; }
        public static ModOptions OptionsMenu { get; internal set; }
        // new flag to know if OptionsPanelHandler.RegisterModOptions was called
        private static bool optionsRegistered = false;

        // tracks whether deferred discovery has completed
        private static bool discoveryCompleted = false;

        internal static void Register()
        {
            if (Instance == null)
            {
                Instance = OptionsPanelHandler.RegisterModOptions<Config>();
                Instance.Load();

                // Load existing saved list, but defer discovery until the main scene is loaded.
                if (Instance.drillableOreList == null)
                {
                    Instance.drillableOreList = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                }

                // Register the dynamic menu now if discovery already performed previously (saved list present).
                ConfigVM.RegisterDynamicOptions();

                SaveUtils.RegisterOnSaveEvent(Instance.Save);
            }
        }

        // Called from Plugin when Main scene finishes loading.
        public static void RebuildDrillableListDeferred(bool force = false)
        {
            if (discoveryCompleted && !force) return;
            if (Instance == null)
            {
                Debug.LogWarning("DrillDamage: RebuildDrillableListDeferred called before Config.Instance created.");
                return;
            }

            var logSource = new ManualLogSource("DrillDamage - Config");
            BepInEx.Logging.Logger.Sources.Add(logSource);

            Dictionary<string, int> discovered = null;
            try
            {
                discovered = BuildDrillableOreList();
                logSource.LogInfo($"RebuildDrillableListDeferred: discovered {discovered?.Count ?? 0} distinct tech types.");
            }
            catch (Exception ex)
            {
                logSource.LogError($"RebuildDrillableListDeferred: discovery failed: {ex}");
                discovered = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            }

            try
            {
                Instance.drillableOreList = MergeAndSanitizeDrillableLists(Instance.drillableOreList, discovered, logSource);
                Instance.Save();
                discoveryCompleted = true;
                logSource.LogInfo($"RebuildDrillableListDeferred: merged list now contains {Instance.drillableOreList.Count} entries.");
            }
            catch (Exception ex)
            {
                logSource.LogError($"RebuildDrillableListDeferred: merge/save failed: {ex}");
            }

            // Re-create/register dynamic options so UI reflects discovered items.
            try
            {
                ConfigVM.RegisterDynamicOptions();
            }
            catch (Exception ex)
            {
                logSource.LogError($"RebuildDrillableListDeferred: RegisterDynamicOptions failed: {ex}");
            }
        }

        // Merge saved and discovered lists. Rules:
        // - Keep existing saved positives (>0).
        // - If saved value is 0 or missing, use discovered default.
        // - Add discovered-only keys.
        // - Log actions.
        private static Dictionary<string, int> MergeAndSanitizeDrillableLists(Dictionary<string, int> saved, Dictionary<string, int> discovered, ManualLogSource logSource)
        {
            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (saved == null) saved = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (discovered == null) discovered = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // Start with saved values (preserve user choices > 0)
            foreach (var kv in saved)
            {
                result[kv.Key] = kv.Value;
            }

            // Merge discovered keys; fill missing or zero entries with discovered default
            foreach (var kv in discovered)
            {
                string key = kv.Key;
                int discoveredValue = kv.Value;

                if (!result.ContainsKey(key))
                {
                    // new key discovered
                    result[key] = discoveredValue;
                    if (Instance != null && Instance.debugmode)
                    {
                        logSource.LogInfo($"Merge: added missing key '{key}' => {discoveredValue}");
                    }
                }
                else
                {
                    int savedValue = result[key];
                    if (savedValue <= 0)
                    {
                        // sanitize invalid saved value (0 or negative) — restore discovered default
                        result[key] = discoveredValue;
                        if (Instance != null && Instance.debugmode)
                        {
                            logSource.LogInfo($"Merge: sanitized key '{key}' (saved={savedValue}) => {discoveredValue}");
                        }
                    }
                    else
                    {
                        // valid saved user value; preserve it
                        if (Instance != null && Instance.debugmode)
                        {
                            logSource.LogInfo($"Merge: preserved key '{key}' => {savedValue}");
                        }
                    }
                }
            }

            // Log any saved-only keys with zero value (they might be accidental)
            foreach (var kv in saved)
            {
                if (!discovered.ContainsKey(kv.Key) && kv.Value <= 0)
                {
                    if (Instance != null && Instance.debugmode)
                    {
                        logSource.LogWarning($"Merge: saved key '{kv.Key}' not found among discovered Drillables and has non-positive value {kv.Value} — consider removing or fixing it.");
                    }
                }
            }

            if (Instance != null && Instance.debugmode)
            {
                logSource.LogInfo($"Merge: final drillable list contains {result.Count} entries. Keys: {string.Join(", ", result.Keys)}");
            }

            return result;
        }

        // Scans loaded prefabs/objects for Drillable components and also inspects all components for
        // members that expose a dominant resource. This improves discovery of modded drillables.
        private static Dictionary<string, int> BuildDrillableOreList()
        {
            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            var logSource = new ManualLogSource("DrillDamage - Config");
            BepInEx.Logging.Logger.Sources.Add(logSource);

            try
            {
                // 1) Direct Drillable components (vanilla)
                Drillable[] drillables = Resources.FindObjectsOfTypeAll<Drillable>();
                // unconditional log so you always know discovery ran
                logSource.LogInfo($"BuildDrillableOreList: found {drillables?.Length ?? 0} Drillable components.");

                foreach (Drillable d in drillables)
                {
                    if (d == null) continue;
                    TechType tt = TechType.None;
                    try
                    {
                        tt = d.GetDominantResourceType();
                    }
                    catch
                    {
                        // ignore
                    }

                    if (tt == TechType.None) continue;

                    string key = tt.AsString();
                    if (string.IsNullOrEmpty(key)) continue;

                    if (!result.ContainsKey(key))
                    {
                        int defaultValue = GetDefaultValueForTechType(tt);
                        result.Add(key, defaultValue);

                        if (Instance != null && Instance.debugmode)
                        {
                            logSource.LogInfo($"BuildDrillableOreList: added '{key}' => {defaultValue} (from Drillable)");
                        }
                    }
                }

                // 2) General scan: iterate all loaded GameObjects and inspect components by reflection
                GameObject[] gos = Resources.FindObjectsOfTypeAll<GameObject>();
                logSource.LogInfo($"BuildDrillableOreList: scanning {gos?.Length ?? 0} GameObjects for drillable-like components.");

                foreach (GameObject go in gos)
                {
                    if (go == null) continue;
                    Component[] comps;
                    try
                    {
                        comps = go.GetComponents<Component>();
                    }
                    catch
                    {
                        continue;
                    }

                    foreach (Component comp in comps)
                    {
                        if (comp == null) continue;
                        Type compType = comp.GetType();

                        try
                        {
                            // Prefer explicit method GetDominantResourceType()
                            MethodInfo getDom = compType.GetMethod("GetDominantResourceType", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                            if (getDom != null)
                            {
                                object ret = null;
                                try
                                {
                                    ret = getDom.Invoke(comp, null);
                                }
                                catch { /* ignore invocation errors */ }

                                ProcessPotentialTechType(ret, result, logSource, compType.Name, "GetDominantResourceType");
                                continue; // we found and processed this component
                            }

                            // Check common property/field names which mods might use
                            string[] candidateNames = new[] { "dominantResource", "dominantResourceType", "resourceType", "resource", "dominantTechType" };
                            bool found = false;

                            foreach (string name in candidateNames)
                            {
                                PropertyInfo pi = compType.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                if (pi != null)
                                {
                                    object ret = null;
                                    try
                                    {
                                        ret = pi.GetValue(comp);
                                    }
                                    catch { }

                                    ProcessPotentialTechType(ret, result, logSource, compType.Name, name);
                                    found = true;
                                    break;
                                }

                                FieldInfo fi = compType.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                if (fi != null)
                                {
                                    object ret = null;
                                    try
                                    {
                                        ret = fi.GetValue(comp);
                                    }
                                    catch { }

                                    ProcessPotentialTechType(ret, result, logSource, compType.Name, name);
                                    found = true;
                                    break;
                                }
                            }

                            if (found) continue;

                            // Heuristic: component type name contains "Drill" or "Drillable" or "Ore"
                            string tname = compType.Name.ToLowerInvariant();
                            if (tname.Contains("drill") || tname.Contains("drillable") || tname.Contains("ore") || tname.Contains("mine"))
                            {
                                // Try to find any property/field returning TechType or string on this component
                                var members = compType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                foreach (var m in members)
                                {
                                    object ret = null;
                                    try
                                    {
                                        if (m is PropertyInfo p && (p.PropertyType == typeof(TechType) || p.PropertyType == typeof(string)))
                                        {
                                            ret = p.GetValue(comp);
                                        }
                                        else if (m is FieldInfo f && (f.FieldType == typeof(TechType) || f.FieldType == typeof(string)))
                                        {
                                            ret = f.GetValue(comp);
                                        }
                                    }
                                    catch { }

                                    if (ret != null)
                                    {
                                        ProcessPotentialTechType(ret, result, logSource, compType.Name, m.Name);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // per-component reflection errors ignored to avoid breaking discovery
                        }
                    }
                }

                // Ensure certain well-known vanilla resources exist even if discovery missed them
                string[] ensureNames = new[] { "Kyanite", "Sulphur", "PrecursorIonCrystal", "IonCube", "IonCrystal" };
                foreach (string name in ensureNames)
                {
                    try
                    {
                        // FIX: Enum.TryParse<TechType>(string value, bool ignoreCase, out TechType result)
                        if (Enum.TryParse<TechType>(name, true, out TechType ptt) && ptt != TechType.None)
                        {
                            string key = ptt.AsString();
                            if (!result.ContainsKey(key))
                            {
                                int defaultValue = GetDefaultValueForTechType(ptt);
                                result[key] = defaultValue;
                                logSource.LogInfo($"BuildDrillableOreList: ensured vanilla key '{key}' => {defaultValue} (fallback)");
                            }
                        }
                    }
                    catch { /* non-critical */ }
                }

                // unconditional summary so you always see discovery results
                logSource.LogInfo($"BuildDrillableOreList: total distinct tech types: {result.Count}. Keys: {string.Join(", ", result.Keys)}");
            }
            catch (Exception ex)
            {
                logSource.LogError($"BuildDrillableOreList: exception while scanning Drillables: {ex}");
            }

            return result;
        }

        // Diagnostic routine: logs candidate components and assembly types for troubleshooting
        public static void LogDiscoveryDiagnostics(int maxEntries = 300)
        {
            var log = new ManualLogSource("DrillDamage - Diagnostic");
            BepInEx.Logging.Logger.Sources.Add(log);

            try
            {
                log.LogInfo("LogDiscoveryDiagnostics: START");

                int entries = 0;
                GameObject[] gos = Resources.FindObjectsOfTypeAll<GameObject>();
                log.LogInfo($"LogDiscoveryDiagnostics: scanning {gos?.Length ?? 0} GameObjects for candidate components.");

                foreach (var go in gos)
                {
                    if (go == null) continue;
                    Component[] comps;
                    try
                    {
                        comps = go.GetComponents<Component>();
                    }
                    catch { continue; }

                    foreach (var comp in comps)
                    {
                        if (comp == null) continue;
                        var ct = comp.GetType();
                        string tname = ct.Name.ToLowerInvariant();

                        // candidate heuristics
                        bool candidateName = tname.Contains("drill") || tname.Contains("drillable") || tname.Contains("ore") || tname.Contains("mine") || tname.Contains("resource");
                        bool nonUnityAssembly = !ct.Assembly.FullName.StartsWith("UnityEngine") && !ct.Assembly.FullName.StartsWith("Unity.") && !ct.Assembly.FullName.Contains("mscorlib") && !ct.Assembly.FullName.Contains("System");

                        if (candidateName || nonUnityAssembly)
                        {
                            // Collect info about common members that may expose resource types
                            var props = ct.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .Where(p => p.CanRead)
                                .Select(p => p.Name + ":" + p.PropertyType.Name).Take(6);
                            var fields = ct.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .Select(f => f.Name + ":" + f.FieldType.Name).Take(6);

                            log.LogInfo($"Diagnostic: GO='{go.name}' comp='{ct.FullName}' assembly='{ct.Assembly.GetName().Name}' props=[{string.Join(", ", props)}] fields=[{string.Join(", ", fields)}]");

                            entries++;
                            if (entries >= maxEntries)
                            {
                                log.LogInfo($"LogDiscoveryDiagnostics: reached maxEntries={maxEntries}, stopping.");
                                goto AssemblyScan;
                            }
                        }
                    }
                }

AssemblyScan:
                // Also scan loaded assemblies for types that look like drillable components (static members)
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var asm in assemblies)
                {
                    try
                    {
                        var types = asm.GetTypes().Where(t => t.IsClass && (t.Name.ToLowerInvariant().Contains("drill") || t.Name.ToLowerInvariant().Contains("drillable") || t.Name.ToLowerInvariant().Contains("ore") || t.Name.ToLowerInvariant().Contains("mine"))).Take(60);
                        foreach (var t in types)
                        {
                            try
                            {
                                var staticFields = t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                    .Where(f => f.FieldType == typeof(TechType) || f.FieldType == typeof(string) || f.FieldType == typeof(int))
                                    .Select(f => f.Name + ":" + f.FieldType.Name);
                                var staticProps = t.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                    .Where(p => p.PropertyType == typeof(TechType) || p.PropertyType == typeof(string) || p.PropertyType == typeof(int))
                                    .Select(p => p.Name + ":" + p.PropertyType.Name);

                                if (staticFields.Any() || staticProps.Any())
                                {
                                    log.LogInfo($"Diagnostic(Type): {t.FullName} in assembly {asm.GetName().Name} staticMembers=[{string.Join(", ", staticFields.Concat(staticProps))}]");
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                log.LogInfo("LogDiscoveryDiagnostics: END");
            }
            catch (Exception ex)
            {
                log.LogError($"LogDiscoveryDiagnostics: exception {ex}");
            }
        }

        // Normalize and add potential results returned by reflection
        private static void ProcessPotentialTechType(object ret, Dictionary<string, int> result, ManualLogSource logSource, string compName, string memberName)
        {
            if (ret == null) return;

            try
            {
                TechType tt = TechType.None;
                if (ret is TechType t)
                {
                    tt = t;
                }
                else if (ret is string s && !string.IsNullOrEmpty(s))
                {
                    // Some components may return the tech name as string
                    // Try to parse using TechType extensions if available, else use the string directly
                    try
                    {
                        // TechType extensions often provide FromString/Parse, but to be safe try to convert name -> TechType via reflection
                        var parseMethod = typeof(TechType).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);
                        if (parseMethod != null)
                        {
                            var parsed = parseMethod.Invoke(null, new object[] { s });
                            if (parsed is TechType ptt) tt = ptt;
                        }
                    }
                    catch
                    {
                        // Fallback: try to match by name in known enum values
                        foreach (TechType candidate in Enum.GetValues(typeof(TechType)))
                        {
                            try
                            {
                                if (candidate.AsString().Equals(s, StringComparison.OrdinalIgnoreCase) || candidate.ToString().Equals(s, StringComparison.OrdinalIgnoreCase))
                                {
                                    tt = candidate;
                                    break;
                                }
                            }
                            catch { }
                        }
                    }
                }
                else if (ret is int ival)
                {
                    // maybe numeric TechType id
                    try
                    {
                        tt = (TechType)ival;
                    }
                    catch { }
                }

                if (tt != TechType.None)
                {
                    string key = tt.AsString();
                    if (!string.IsNullOrEmpty(key) && !result.ContainsKey(key))
                    {
                        int defaultValue = GetDefaultValueForTechType(tt);
                        result.Add(key, defaultValue);

                        if (Instance != null && Instance.debugmode)
                        {
                            logSource.LogInfo($"BuildDrillableOreList: added '{key}' => {defaultValue} (from {compName}.{memberName})");
                        }
                    }
                }
            }
            catch
            {
                // ignore parsing errors
            }
        }

        // Default values mirror the sliders in ConfigVM; modded/unknown ores fallback to additionaldamage
        private static int GetDefaultValueForTechType(TechType tt)
        {
            switch (tt)
            {
                case TechType.Salt:
                    return 30; // additionaldamagesalt
                case TechType.Quartz:
                    return 20; // additionaldamagequartz
                case TechType.Copper:
                    return 30; // additionaldamagescopper
                case TechType.Titanium:
                    return 10; // additionaldamagetitanium
                case TechType.Lead:
                    return 10; // additionaldamagelead
                case TechType.Silver:
                    return 15; // additionaldamagesilver
                case TechType.Diamond:
                    return 5; // additionaldamagediamond
                case TechType.Gold:
                    return 15; // additionaldamagegold
                case TechType.Magnetite:
                    return 10; // additionaldamagemagnetite
                case TechType.Lithium:
                    return 5; // additionaldamagelithium
                case TechType.MercuryOre:
                    return 15; // additionaldamagemercury
                case TechType.Uranium:
                    return 15; // additionaldamageuranium
                case TechType.AluminumOxide:
                    return 15; // additionaldamagealuminiumoxide
                case TechType.Nickel:
                    return 5; // additionaldamagenickiel
                case TechType.Sulphur:
                    return 15; // additionaldamagesulphur
                case TechType.Kyanite:
                    return 5; // additionaldamagekyanite
                case TechType.PrecursorIonCrystal:
                    return 3; // additionaldamageion
                default:
                    return Instance != null ? Instance.additionaldamage : 10;
            }
        }
    }

    public class ConfigVM : ConfigFile
    {
        private static bool optionsRegistered;

        // Legacy explicit sliders kept as defaults for manual editing if desired.
        [Slider("Additional Damage - Salt", Min = 1, Max = 100, DefaultValue = 30, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagesalt = 30;

        [Slider("Additional Damage - Quartz", Min = 1, Max = 100, DefaultValue = 20, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagequartz = 20;

        [Slider("Additional Damage - Copper", Min = 1, Max = 100, DefaultValue = 15, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagecopper = 30;

        [Slider("Additional Damage - Titanium", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagetitanium = 10;

        [Slider("Additional Damage - Lead", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagelead = 10;

        [Slider("Additional Damage - Silver", Min = 1, Max = 100, DefaultValue = 15, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagesilver = 15;

        [Slider("Additional Damage - Diamond", Min = 1, Max = 100, DefaultValue = 5, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagediamond = 5;

        [Slider("Additional Damage - Gold", Min = 1, Max = 100, DefaultValue = 15, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagegold = 15;

        [Slider("Additional Damage - Magnetite", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagemagnetite = 10;

        [Slider("Additional Damage - Lithium", Min = 1, Max = 100, DefaultValue = 5, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagelithium = 5;

        [Slider("Additional Damage - Mercury(cut content)", Min = 1, Max = 100, DefaultValue = 15, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagemercury = 15;

        [Slider("Additional Damage - Uraniumn", Min = 1, Max = 100, DefaultValue = 15, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamageuranium = 15;

        [Slider("Additional Damage - Ruby/Aluminium Oxide", Min = 1, Max = 100, DefaultValue = 15, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagealuminiumoxide = 15;

        [Slider("Additional Damage - Nickiel", Min = 1, Max = 100, DefaultValue = 5, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagenickiel = 5;

        [Slider("Additional Damage - Sulphur", Min = 1, Max = 100, DefaultValue = 15, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagesulphur = 15;

        [Slider("Additional Damage - Kyanite", Min = 1, Max = 100, DefaultValue = 5, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamagekyanite = 5;

        [Slider("Additional Damage - Ion Cube", Min = 1, Max = 100, DefaultValue = 3, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamageion = 3;

        // Dynamic menu registration
        public static void RegisterDynamicOptions()
        {
            // Avoid registering twice if already registered
            if (optionsRegistered)
            {
                return;
            }

            var logSource = new ManualLogSource("DrillDamage - ConfigVM");
            BepInEx.Logging.Logger.Sources.Add(logSource);

            try
            {
                // If no drillables discovered yet, do not create/assign the OptionsMenu — allow later registration
                if (Config.DrillableOreList == null || Config.DrillableOreList.Count == 0)
                {
                    logSource.LogInfo("RegisterDynamicOptions: no drillable entries to register (will retry after discovery).");
                    return;
                }

                // Create and register menu only when we have entries. Ensure title matches general menu prefix so order is correct.
                var menu = new DynamicVariableModeMenu();
                Config.OptionsMenu = menu;
                OptionsPanelHandler.RegisterModOptions(menu);

                optionsRegistered = true;
                logSource.LogInfo($"RegisterDynamicOptions: registered menu with {Config.DrillableOreList.Count} entries.");
            }
            catch (Exception ex)
            {
                logSource.LogError($"RegisterDynamicOptions: exception while creating menu: {ex}");
            }
        }

        // ModOptions subclass that builds sliders from Config.DrillableOreList
        private class DynamicVariableModeMenu : ModOptions
        {
            public DynamicVariableModeMenu()
                : base("DrillDamage Options - Variable Mode")
            {
                var logSource = new ManualLogSource("DrillDamage - ConfigVM");
                BepInEx.Logging.Logger.Sources.Add(logSource);

                if (Config.DrillableOreList == null || Config.DrillableOreList.Count == 0)
                {
                    logSource.LogInfo("DynamicVariableModeMenu: drillable list is empty; no options created.");
                    return;
                }

                // Order keys for consistent UI
                foreach (KeyValuePair<string, int> pair in Config.DrillableOreList.OrderBy(k => k.Key))
                {
                    string key = pair.Key;
                    int value = pair.Value;

                    try
                    {
                        // Ensure unique id, clamp initial/current value and set sensible step
                        const float min = 0f;
                        const float max = 100f;
                        float cur = Mathf.Clamp((float)value, min, max);
                        string optionId = $"drilldamage.variable.{key}";

                        ModSliderOption option = ModSliderOption.Create(optionId, key, min, max, cur, cur, null, 1f, "This number will be added to drill damage calculation of this mineable resource, making it faster.");
                        option.OnChanged += (sender, args) =>
                        {
                            int newVal = Mathf.Clamp(Mathf.RoundToInt(args.Value), (int)min, (int)max);
                            Config.DrillableOreList[key] = newVal;
                            Config.Instance?.Save();
                            if (Config.Instance != null && Config.Instance.debugmode)
                            {
                                logSource.LogInfo($"DynamicVariableModeMenu: '{key}' changed => {newVal}");
                            }
                        };

                        AddItem(option);

                        if (Config.Instance != null && Config.Instance.debugmode)
                        {
                            logSource.LogInfo($"DynamicVariableModeMenu: created option '{optionId}' label='{key}' initial={cur}");
                        }
                    }
                    catch (Exception ex)
                    {
                        logSource.LogError($"DynamicVariableModeMenu: failed to create option for '{key}': {ex}");
                    }
                }

                if (Config.Instance != null && Config.Instance.debugmode)
                {
                    logSource.LogInfo($"DynamicVariableModeMenu: created {Config.DrillableOreList.Count} slider options.");
                }
            }
        }
    }
}
