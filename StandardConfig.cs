using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace DrillDamage
{
    [Menu("DrillDamage Options")]
    public class Config : ConfigFile
    {
        [Slider("Additional Damage", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "This number will be added to drill damage calculation, making it faster.")]
        public int additionaldamage = 10;

        [Toggle("Affect Creatures")]
        public bool affectcreatures = false;

        [Slider("Additional Damage - Creatures", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "Will be the same as Additional Damage by default.")]
        public int additionaldamagecreatures = 10;

        [Toggle("Affects Seamoth Arms", Tooltip = "Only works if Seamoth Arms mod is enabled.")]
        public bool affectsseamotharms = true;

        [Slider("Additional Damage - Seamoth Arms", Min = 1, Max = 100, DefaultValue = 10, Step = 1, Tooltip = "Will be the same as Additional Damage by default.")]
        public int additionaldamageseamotharms = 10;

        [Toggle("Debug mode")]
        public bool debugmode = false;

        [Toggle("Variable Mode", Tooltip = "Enabling this option will make the extra damage applied differ between the kinds of drillable resources. Disabled by default. Enabling this will ignore the Additional Damage and Additional Damage - Seamoth Arms.")]
        public bool variablemode;

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
        [Slider("additional Damage - Ion Cube", Min = 1, Max = 100, DefaultValue = 3, Step = 1, Tooltip = "This number will be added to drill damage calculation of this mineable resource, making it faster.")]
        public int additionaldamageion = 3;
    }
}
