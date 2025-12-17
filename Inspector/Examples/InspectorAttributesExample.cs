namespace UniGame.ViewSystem.Inspector.Examples
{
    using UnityEngine;
    using Inspector;

    /// <summary>
    /// Example demonstrating all available inspector attributes
    /// </summary>
    public class InspectorAttributesExample : MonoBehaviour
    {
        // ==================== Basic Information ====================
        
        [Title("Basic Information", "This section shows basic player data")]
        [SerializeField] private string playerName = "Hero";
        
        [SerializeField] private int playerLevel = 1;

        // ==================== Combat Statistics ====================
        
        [BoxGroup("Combat Stats")]
        [SerializeField] private int maxHealth = 100;
        
        [BoxGroup("Combat Stats")]
        [SerializeField] private int currentHealth = 100;
        
        [BoxGroup("Combat Stats")]
        [MinMaxSlider(5, 50)]
        [SerializeField] private Vector2 damageRange = new Vector2(10, 30);

        // ==================== Advanced Settings ====================
        
        [PropertySpace(15)]
        [SerializeField] private bool useAdvancedSettings = false;

        [ShowIf("useAdvancedSettings")]
        [BoxGroup("Advanced")]
        [SerializeField] private float criticalStrikeChance = 0.15f;

        [ShowIf("useAdvancedSettings")]
        [BoxGroup("Advanced")]
        [SerializeField] private float evasionChance = 0.1f;

        [HideIf("useAdvancedSettings")]
        [SerializeField] private string basicModeMessage = "Advanced settings are disabled";

        // ==================== Read-Only Fields ====================
        
        [PropertySpace(10)]
        [ReadOnly]
        [SerializeField] private int experiencePoints = 0;
        
        [ReadOnly]
        [SerializeField] private float playtimeHours = 0f;

        // ==================== Layout Example ====================
        
        [PropertySpace(10)]
        [HorizontalGroup("Position")]
        [SerializeField] private float posX;
        
        [HorizontalGroup("Position")]
        [SerializeField] private float posY;
        
        [HorizontalGroup("Position")]
        [SerializeField] private float posZ;

        // ==================== Methods with Buttons ====================

        /// <summary>
        /// Reset player health to maximum
        /// </summary>
        [PropertySpace(15)]
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
