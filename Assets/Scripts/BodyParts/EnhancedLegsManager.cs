using System.Collections.Generic;
using BodyParts.Behaviors;
using BodyParts.Interfaces;
using UnityEngine;

namespace BodyParts {
    /// <summary>
    /// Enhanced legs manager that demonstrates the power of composition.
    /// Can work with any ILegs implementation and apply various modifiers.
    /// </summary>
    public class EnhancedLegsManager : MonoBehaviour {
        [SerializeField] private ILegs legs;
        [SerializeField] private List<ISpeedModifier> speedModifiers = new List<ISpeedModifier>();
        
        // Events for when legs change
        public System.Action<ILegs> OnLegsChanged;
        public System.Action<float> OnSpeedModified;
        
        public void SetLegs(ILegs newLegs) {
            legs = newLegs;
            OnLegsChanged?.Invoke(legs);
        }
        
        public void AddSpeedModifier(ISpeedModifier modifier) {
            if (!speedModifiers.Contains(modifier)) {
                speedModifiers.Add(modifier);
            }
        }
        
        public void RemoveSpeedModifier(ISpeedModifier modifier) {
            speedModifiers.Remove(modifier);
        }
        
        public float GetModifiedSpeed() {
            if (legs == null) return 0f;
            
            float baseSpeed = legs.MaxSpeed;
            float totalModifier = 1f;
            
            foreach (var modifier in speedModifiers) {
                if (modifier.CanApplyModifier(legs)) {
                    totalModifier *= modifier.GetSpeedModifier(legs);
                }
            }
            
            float modifiedSpeed = baseSpeed * totalModifier;
            OnSpeedModified?.Invoke(modifiedSpeed);
            return modifiedSpeed;
        }
        
        // Example usage methods
        public void ApplyDamageModifier() {
            AddSpeedModifier(new DamageSpeedModifier(0.5f));
        }
        
        public void ApplyTerrainModifier(float terrainMultiplier) {
            AddSpeedModifier(new TerrainSpeedModifier(terrainMultiplier));
        }
        
        public void ClearAllModifiers() {
            speedModifiers.Clear();
        }
        
        // Standard legs operations
        public void SetMovement(Vector2 movement) {
            if (legs != null) {
                legs.Movement = movement;
            }
        }
        
        public void SetDownHeld(bool held) {
            if (legs != null) {
                legs.DownHeld = held;
            }
        }
        
        public void SetUpHeld(bool held) {
            if (legs != null) {
                legs.UpHeld = held;
            }
        }
        
        public MovementModes GetCurrentMovementMode() {
            return legs?.MovementMode ?? MovementModes.Walk;
        }
        
        public bool CanMove() {
            return legs?.CanMove ?? false;
        }
        
        public void SetCanMove(bool canMove) {
            if (legs != null) {
                legs.CanMove = canMove;
            }
        }
    }
}

