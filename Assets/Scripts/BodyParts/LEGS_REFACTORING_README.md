# Legs System Refactoring: Inheritance to Composition

## Overview

This refactoring demonstrates how to move from an inheritance-based approach to a composition-based approach using interfaces and default interface implementations in C#.

## Before: Inheritance-Based Approach

```csharp
// Abstract base class
public abstract class Legs : MonoBehaviour {
    // Common functionality
    protected Animator animator;
    public enum MovementModes { Crouch, Walk, Run, Airborne };
    
    // Abstract methods that must be implemented
    public abstract bool DownHeld { set; }
    public abstract bool UpHeld { set; }
}

// Player implementation
public sealed class PlayerLegs : Legs {
    // Must implement all abstract methods
    public override bool DownHeld { set => animator.SetBool("Down Held", value); }
    public override bool UpHeld { set => animator.SetBool("Up Held", value); }
}

// Enemy implementation  
public class EnemyLegs : Legs {
    // Must implement all abstract methods
    public override bool DownHeld { set { /* implementation */ } }
    public override bool UpHeld { set { /* implementation */ } }
}
```

## After: Composition-Based Approach

```csharp
// Interface with default implementations
public interface ILegs {
    bool DownHeld { get; set; }
    bool UpHeld { get; set; }
    MovementModes MovementMode { get; set; }
    
    // Default implementations
    static readonly Dictionary<MovementModes, Color> MovementModeColors = new Dictionary<MovementModes, Color>() { /* ... */ };
    
    void SetupAnimations() {
        Animator.SetFloat("Sneak Playback Speed", 1);
        Animator.SetFloat("Walk Playback Speed", 1);
        Animator.SetFloat("Run Playback Speed", 1);
    }
}

// Player implementation
public sealed class PlayerLegsNew : MonoBehaviour, ILegs {
    // Implements interface directly
    public bool DownHeld { set => _animator.SetBool("Down Held", value); }
    public bool UpHeld { set => _animator.SetBool("Up Held", value); }
}

// Enemy implementation
public class EnemyLegsNew : MonoBehaviour, ILegs {
    // Implements interface directly
    public bool DownHeld { set { /* implementation */ } }
    public bool UpHeld { set { /* implementation */ } }
}
```

## Benefits of Composition Over Inheritance

### 1. **Flexibility**
- You can easily swap different leg implementations without changing the interface
- No need to inherit from a specific base class
- Can implement multiple interfaces if needed

### 2. **Testability**
- Easier to mock interfaces for unit testing
- Can test components in isolation
- No dependency on base class implementation

### 3. **Extensibility**
- Can add new behaviors through composition (like speed modifiers)
- Don't need to modify base classes to add new functionality
- Can mix and match different behaviors

### 4. **Default Interface Implementations**
- Share common functionality without inheritance
- Can provide sensible defaults while allowing overrides
- Reduces code duplication

## Example: Speed Modifiers

With composition, you can easily add new behaviors:

```csharp
public interface ISpeedModifier {
    float GetSpeedModifier(ILegs legs);
    bool CanApplyModifier(ILegs legs);
}

public class DamageSpeedModifier : ISpeedModifier {
    public float GetSpeedModifier(ILegs legs) => 0.5f; // 50% speed when damaged
}

public class TerrainSpeedModifier : ISpeedModifier {
    public float GetSpeedModifier(ILegs legs) => 0.8f; // 80% speed on rough terrain
}

// Usage
var manager = new EnhancedLegsManager();
manager.SetLegs(playerLegs);
manager.AddSpeedModifier(new DamageSpeedModifier());
manager.AddSpeedModifier(new TerrainSpeedModifier());
float finalSpeed = manager.GetModifiedSpeed();
```

## Migration Strategy

1. **Create the interface** (`ILegs`) with default implementations
2. **Create new implementations** (`PlayerLegsNew`, `EnemyLegsNew`) that implement the interface
3. **Update managers** to work with the interface instead of base class
4. **Test thoroughly** to ensure functionality is preserved
5. **Gradually migrate** existing code to use the new implementations
6. **Remove old inheritance-based classes** once migration is complete

## Files Created

- `BodyParts/Interfaces/ILegs.cs` - Main interface with default implementations
- `BodyParts/LegsComponent.cs` - Optional base component (if you want some shared functionality)
- `BodyParts/PlayerLegsNew.cs` - New player legs implementation
- `BodyParts/EnemyLegsNew.cs` - New enemy legs implementation
- `BodyParts/LegsManager.cs` - Basic manager that works with any ILegs
- `BodyParts/EnhancedLegsManager.cs` - Advanced manager with speed modifiers
- `BodyParts/Behaviors/ISpeedModifier.cs` - Example of composable behaviors

## Next Steps

1. Test the new implementations thoroughly
2. Update any code that references the old `Legs` base class
3. Consider creating additional interfaces for other body parts (arms, eyes, etc.)
4. Explore other composable behaviors you could add (stamina, equipment effects, etc.)

This refactoring makes your code more modular, testable, and flexible while maintaining all existing functionality.

