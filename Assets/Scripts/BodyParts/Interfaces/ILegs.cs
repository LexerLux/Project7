using System.Collections.Generic;
using UnityEngine;

namespace BodyParts.Interfaces {
    public enum MovementModes { 
        Crouch, 
        Walk, 
        Run, 
        Airborne 
    }

    public interface ILegs {
        // Core properties
        bool DownHeld { get; set; }
        bool UpHeld { get; set; }
        MovementModes MovementMode { get; set; }
        
        // Speed properties
        float MaxSpeed { get; }
        float SneakSpeed { get; }
        float WalkSpeed { get; }
        float RunSpeed { get; }
        
        // Movement control
        Vector2 Movement { get; set; }
        bool CanMove { get; set; }
        
        // Animation control
        Animator Animator { get; }
        
        // Default implementations
        static readonly Dictionary<MovementModes, Color> MovementModeColors = new Dictionary<MovementModes, Color>() {
            {MovementModes.Crouch, Color.blue}, 
            {MovementModes.Walk, new Color(0f, 0.4f, 1f)}, 
            {MovementModes.Run, new Color(0f, 0.85f, 1f)}, 
            {MovementModes.Airborne, new Color(0f, 1f, 0.69f)}
        };
        
        // Default animation setup
        void SetupAnimations() {
            Animator.SetFloat("Sneak Playback Speed", 1);
            Animator.SetFloat("Walk Playback Speed", 1);
            Animator.SetFloat("Run Playback Speed", 1);
        }
        
        // Default movement mode color getter
        Color GetMovementModeColor() => MovementModeColors[MovementMode];
    }
}

