using BodyParts.Interfaces;
using UnityEngine;

namespace BodyParts {
    [RequireComponent(typeof(Animator))]
    public abstract class LegsComponent : MonoBehaviour, ILegs {
        [SerializeField] protected bool canMove = true;
        
        protected Animator _animator;
        
        // ILegs interface implementation
        public abstract bool DownHeld { get; set; }
        public abstract bool UpHeld { get; set; }
        public abstract MovementModes MovementMode { get; set; }
        public abstract float MaxSpeed { get; }
        public abstract float SneakSpeed { get; }
        public abstract float WalkSpeed { get; }
        public abstract float RunSpeed { get; }
        
        public virtual Vector2 Movement { get; set; }
        public virtual bool CanMove { 
            get => canMove; 
            set => canMove = value; 
        }
        
        public Animator Animator => _animator;
        
        protected virtual void Awake() {
            _animator = GetComponent<Animator>();
        }
        
        protected virtual void Start() {
            SetupAnimations();
        }
        
        // Helper method for derived classes to get movement mode color
        protected Color GetMoveColor() => GetMovementModeColor();
    }
}

