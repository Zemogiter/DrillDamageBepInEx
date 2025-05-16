using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using Nautilus.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DrillDamage
{
    [Menu("DrillDamage Options - General", LoadOn = (MenuAttribute.LoadEvents.MenuRegistered | MenuAttribute.LoadEvents.MenuOpened), SaveOn = (MenuAttribute.SaveEvents.ChangeValue | MenuAttribute.SaveEvents.SaveGame | MenuAttribute.SaveEvents.QuitGame))]
    public class Config : ConfigFile
    {
        [Slider("*Additional Damage - Main*", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "This number will be added to drill damage calculation, making it faster.")]
        public int additionaldamage = 10;

        [Toggle("Affect Creatures")]
        public bool affectcreatures = false;

        private int _additionaldamagecreatures;
        [Slider("Additional Damage - Creatures", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "Will be the same as Additional Damage by default.")]
        public int additionaldamagecreatures
        {
            get => _additionaldamagecreatures == 0 ? additionaldamage : _additionaldamagecreatures;
            set => _additionaldamagecreatures = value;
        }

        [Toggle("Affects Seamoth Arms", Tooltip = "Only works if Seamoth Arms mod is enabled.")]
        public bool affectsseamotharms = true;

        private int _additionaldamageseamotharms;
        [Slider("Additional Damage - Seamoth Arms", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "Will be the same as Additional Damage by default.")]
        public int additionaldamageseamotharms
        {
            get => _additionaldamageseamotharms == 0 ? additionaldamage : _additionaldamageseamotharms;
            set => _additionaldamageseamotharms = value;
        }

        [Toggle("Affects Vehicle Framework drills", Tooltip = "Only works if Vehicle Framework mod is enabled.")]
        public bool affectsvehicleframeworkdrills = true;

        private int _additionaldamagevehicleframeworkdrills; 
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

        private class VariableModeOptionsMenu : ModOptions
        {
            public VariableModeOptionsMenu()
                : base("Drill Damage - Variable Mode")
            {
                Instance.drillableOreList = DrillableOreList.OrderBy((KeyValuePair<string, int> pair) => pair.Key).ToDictionary((KeyValuePair<string, int> pair) => pair.Key, (KeyValuePair<string, int> pair) => pair.Value);
                Instance.Save();
                foreach (KeyValuePair<string, int> drillableOre in DrillableOreList)
                {
                    ModSliderOption option = ModSliderOption.Create(drillableOre.Key, drillableOre.Key, 0f, 100f, drillableOre.Value, drillableOre.Value);
                    option.OnChanged += delegate (object sender, SliderChangedEventArgs args)
                    {
                        DrillableOreList[drillableOre.Key] = Mathf.CeilToInt(args.Value);
                        Instance.Save();
                    };
                    AddItem(option);
                }
            }

            public override void BuildModOptions(uGUI_TabbedControlsPanel panel, int modsTabIndex, IReadOnlyCollection<OptionItem> options)
            {
                base.BuildModOptions(panel, modsTabIndex, (IReadOnlyCollection<OptionItem>)(object)options.OrderBy((OptionItem o) => o.Label).ToArray());
            }
        }
        public Dictionary<string, int> drillableOreList = [];
        public static Dictionary<string, int> DrillableOreList => Instance.drillableOreList;
        public static Config Instance { get; private set; }
        public static ModOptions OptionsMenu { get; private set; }
        internal static void Register()
        {
            if (Instance == null)
            {
                Instance = OptionsPanelHandler.RegisterModOptions<Config>();
                Instance.Load();
                OptionsMenu = new VariableModeOptionsMenu();
                if (DrillableOreList.Count > 0)
                {
                    OptionsPanelHandler.RegisterModOptions(OptionsMenu);
                }
                SaveUtils.RegisterOnSaveEvent(Instance.Save);
            }
        }
    }
    /*
    [Menu("DrillDamage Options - Variable Mode")]
    public class ConfigVM : ConfigFile
    {
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
    }
    */
}
