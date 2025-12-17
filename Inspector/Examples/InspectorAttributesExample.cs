namespace UniGame.ViewSystem.Inspector.Examples
{
    using UnityEngine;
    using Inspector;

    /// <summary>
    /// Example demonstrating all available inspector attributes
    /// Comprehensive showcase of UI Toolkit based custom inspector attributes
    /// </summary>
    public class InspectorAttributesExample : MonoBehaviour
    {
        // ==================== Basic Information ====================
        
        [Title("Basic Information", "This section shows basic player data")]
        [SerializeField] private string playerName = "Hero";
        
        [InspectorTooltip("The player's current level")]
        [SerializeField] private int playerLevel = 1;

        // ==================== Combat Statistics ====================
        
        [BoxGroup("Combat Stats")]
        [SerializeField] private int maxHealth = 100;
        
        [BoxGroup("Combat Stats")]
        [SerializeField] private int currentHealth = 100;
        
        [BoxGroup("Combat Stats")]
        [MinMaxSlider(5, 50)]
        [SerializeField] private Vector2 damageRange = new Vector2(10, 30);

        // ==================== Range and Slider ====================
        
        [PropertySpace(15)]
        [Title("Stat Modifiers", "Values with range constraints")]
        [InspectorRange(0f, 1f)]
        [SerializeField] private float attackSpeed = 0.5f;
        
        [InspectorRange(1, 100)]
        [SerializeField] private int mana = 50;

        // ==================== Advanced Settings ====================
        
        [PropertySpace(15)]
        [Title("Conditional Visibility")]
        [SerializeField] private bool useAdvancedSettings = false;

        [ShowIf("useAdvancedSettings")]
        [BoxGroup("Advanced")]
        [SerializeField] private float criticalStrikeChance = 0.15f;

        [ShowIf("useAdvancedSettings")]
        [BoxGroup("Advanced")]
        [SerializeField] private float evasionChance = 0.1f;

        [HideIf("useAdvancedSettings")]
        [SerializeField] private string basicModeMessage = "Advanced settings are disabled";

        // ==================== Enable/Disable If ====================
        
        [PropertySpace(15)]
        [SerializeField] private bool enableSpecialAbility = true;

        [EnableIf("enableSpecialAbility")]
        [SerializeField] private float abilityPower = 1.0f;
        
        [DisableIf("enableSpecialAbility")]
        [SerializeField] private string disabledReason = "Ability is enabled";

        // ==================== Read-Only Fields ====================
        
        [PropertySpace(10)]
        [Title("Statistics", "Display-only fields")]
        [ReadOnly]
        [SerializeField] private int experiencePoints = 0;
        
        [ReadOnly]
        [SerializeField] private float playtimeHours = 0f;

        // ==================== Color Settings ====================
        
        [PropertySpace(10)]
        [Title("Visual Settings")]
        [InspectorColorUsage(true, false)]
        [SerializeField] private Color primaryColor = Color.white;
        
        [InspectorColorUsage(true, true)]
        [SerializeField] private Color hdrEmissionColor = Color.white;

        // ==================== Foldout Groups ====================
        
        [PropertySpace(10)]
        [FoldoutGroup("Character Appearance", true)]
        [SerializeField] private string characterModel = "Human";
        
        [FoldoutGroup("Character Appearance")]
        [SerializeField] private int skinToneIndex = 0;
        
        [FoldoutGroup("Character Appearance")]
        [SerializeField] private int hairStyleIndex = 0;

        // ==================== Tab Groups ====================
        
        [PropertySpace(10)]
        [TabGroup("EquipmentSettings", "Armor")]
        [SerializeField] private int armorValue = 10;
        
        [TabGroup("EquipmentSettings", "Armor")]
        [SerializeField] private float armorRegeneration = 0.5f;
        
        [TabGroup("EquipmentSettings", "Weapons")]
        [SerializeField] private int weaponDamage = 15;
        
        [TabGroup("EquipmentSettings", "Weapons")]
        [SerializeField] private float weaponAttackSpeed = 1.2f;

        // ==================== Horizontal Layout ====================
        
        [PropertySpace(10)]
        [Title("Position Vector")]
        [HorizontalGroup("Position")]
        [LabelWidth(50)]
        [SerializeField] private float posX;
        
        [HorizontalGroup("Position")]
        [LabelWidth(50)]
        [SerializeField] private float posY;
        
        [HorizontalGroup("Position")]
        [LabelWidth(50)]
        [SerializeField] private float posZ;

        // ==================== Preview Attribute ====================
        
        [PropertySpace(10)]
        [Title("Asset References")]
        [Preview(80)]
        [SerializeField] private Sprite characterIcon;
        
        [Preview(100, 100)]
        [SerializeField] private Texture2D characterPortrait;

        // ==================== Methods with Buttons ====================

        [PropertySpace(15)]
        [Title("Actions", "Click buttons to trigger methods")]
        
        /// <summary>
        /// Reset player health to maximum
        /// </summary>
        [Button("Reset Health")]
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            Debug.Log($"Health reset to {maxHealth}");
        }

        /// <summary>
        /// Add experience points
        /// </summary>
        [Button("Add 100 XP")]
        public void AddExperience()
        {
            experiencePoints += 100;
            Debug.Log($"Total XP: {experiencePoints}");
        }

        /// <summary>
        /// Level up the character
        /// </summary>
        [Button("Level Up")]
        public void LevelUp()
        {
            playerLevel++;
            maxHealth += 10;
            currentHealth = maxHealth;
            Debug.Log($"Leveled up to {playerLevel}");
        }

        /// <summary>
        /// Toggle advanced settings
        /// </summary>
        [Button("Toggle Advanced")]
        public void ToggleAdvanced()
        {
            useAdvancedSettings = !useAdvancedSettings;
            Debug.Log($"Advanced settings: {(useAdvancedSettings ? "Enabled" : "Disabled")}");
        }

        /// <summary>
        /// Reset all character data
        /// </summary>
        [Button("Reset Character")]
        public void ResetCharacter()
        {
            playerLevel = 1;
            maxHealth = 100;
            currentHealth = 100;
            experiencePoints = 0;
            playtimeHours = 0f;
            Debug.Log("Character reset!");
        }

        /// <summary>
        /// Example of conditional method visibility
        /// </summary>
        private bool IsHealthLow => currentHealth < maxHealth * 0.3f;
    }
}
