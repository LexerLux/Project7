using BodyParts.Interfaces;
using UnityEngine;

namespace BodyParts {
    /// <summary>
    /// A manager that can work with any ILegs implementation.
    /// This demonstrates the power of composition over inheritance.
    /// </summary>
    public class LegsManager : MonoBehaviour {
        [SerializeField] private ILegs legs;
        
        // You can easily swap different legs implementations
        public void SetLegs(ILegs newLegs) {
            legs = newLegs;
        }
        
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
        
        public float GetCurrentSpeed() {
            return legs?.MaxSpeed ?? 0f;
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
        
        // Example of how you can easily add new functionality
        public void ApplySpeedModifier(float modifier) {
            // This could work with any ILegs implementation
            // You could even create a SpeedModifier component that works with any legs
        }
        
        // Example of how you can create different movement behaviors
        public void SetMovementMode(MovementModes mode) {
            if (legs != null) {
                legs.MovementMode = mode;
            }
        }
    }
}

