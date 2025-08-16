using BodyParts.Interfaces;

namespace BodyParts.Behaviors {
    /// <summary>
    /// Interface for speed modifiers that can be applied to any ILegs implementation.
    /// This demonstrates how composition allows for flexible behavior mixing.
    /// </summary>
    public interface ISpeedModifier {
        float GetSpeedModifier(ILegs legs);
        bool CanApplyModifier(ILegs legs);
    }
    
    /// <summary>
    /// Example speed modifier that reduces speed when legs are damaged
    /// </summary>
    public class DamageSpeedModifier : ISpeedModifier {
        private readonly float damageMultiplier;
        
        public DamageSpeedModifier(float damageMultiplier = 0.5f) {
            this.damageMultiplier = damageMultiplier;
        }
        
        public float GetSpeedModifier(ILegs legs) {
            // This could check for damage status, health, etc.
            // For now, just return the damage multiplier
            return damageMultiplier;
        }
        
        public bool CanApplyModifier(ILegs legs) {
            // Check if legs are damaged
            return true; // Simplified for example
        }
    }
    
    /// <summary>
    /// Example speed modifier for different terrain types
    /// </summary>
    public class TerrainSpeedModifier : ISpeedModifier {
        private readonly float terrainMultiplier;
        
        public TerrainSpeedModifier(float terrainMultiplier = 0.8f) {
            this.terrainMultiplier = terrainMultiplier;
        }
        
        public float GetSpeedModifier(ILegs legs) {
            return terrainMultiplier;
        }
        
        public bool CanApplyModifier(ILegs legs) {
            // Check current terrain type
            return true; // Simplified for example
        }
    }
}

