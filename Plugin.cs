using System;
using System.Reflection;
using HarmonyLib;
using BepInEx;
using BepInEx.Configuration;
using Nautilus.Handlers;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

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
            VERSION = "1.3.5.0";
        #endregion

        public void Awake()
        {
            Logger.LogInfo("DrillDamage - Started patching v" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            DrillDamage.Config.Register();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo("DrillDamage - Finished patching");
        }

        void Start()
        {
            // Wait for the main scene to load; discovery will run then.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            // Run discovery once when the main scene is loaded.
            if (scene.name == "Main")
            {
                // initial discovery (existing)
                DrillDamage.Config.RebuildDrillableListDeferred();
                // start retry coroutine to catch late-loading mods
                StartCoroutine(RetryDiscovery());
                // Unsubscribe here to avoid repeated runs from scene changes
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        private IEnumerator RetryDiscovery()
        {
            // small set of retries with exponential backoff
            int[] delays = new[] { 1, 2, 4, 8 }; // seconds
            int lastCount = DrillDamage.Config.DrillableOreList?.Count ?? 0;

            for (int i = 0; i < delays.Length; i++)
            {
                yield return new WaitForSeconds(delays[i]);
                try
                {
                    DrillDamage.Config.RebuildDrillableListDeferred(force: true);
                    int newCount = DrillDamage.Config.DrillableOreList?.Count ?? 0;
                    Logger.LogInfo($"DrillDamage - Plugin: RetryDiscovery attempt {i + 1}, discovered {newCount} entries (previous {lastCount}).");
                    if (newCount > lastCount)
                    {
                        // if we found new entries, update lastCount and continue a little longer to see if more appear
                        lastCount = newCount;
                        continue;
                    }
                    else
                    {
                        // no change — stop early
                        Logger.LogInfo("DrillDamage - Plugin: RetryDiscovery stopping early (no new entries).");
                        yield break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"DrillDamage - Plugin: RetryDiscovery exception: {ex}");
                }
            }

            Logger.LogInfo("DrillDamage - Plugin: RetryDiscovery finished.");
        }
    }
}
