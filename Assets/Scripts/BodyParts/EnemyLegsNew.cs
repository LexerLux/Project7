using System.Collections.Generic;
using BodyParts.Interfaces;
using Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BodyParts.Mob {
    public class EnemyLegsNew : MonoBehaviour, ILegs {
        [SerializeField] private bool canMove = true;
        private Animator _animator;
        private Entity _owner;
        
        // ILegs interface implementation
        public bool DownHeld {
            set {
                if (value == false) MovementMode = MovementModes.Walk;
                else MovementMode = MovementModes.Crouch;
            }
        }
        
        public bool UpHeld {
            set {
                if (value == false) MovementMode = MovementModes.Walk;
                else MovementMode = MovementModes.Run;
            }
        }
        
        public MovementModes MovementMode { get; set; } = MovementModes.Walk;
        
        public Vector2 Movement { get; set; }
        public bool CanMove { 
            get => canMove; 
            set => canMove = value; 
        }
        
        public Animator Animator => _animator;
        
        // Speed properties
        [ShowInInspector, ProgressBar(0, "MaxSpeed", ColorGetter = "moveColor")]
        public float Velocity => _owner.RigidBody.linearVelocity.magnitude;
        
        [ShowInInspector, ReadOnly]
        public float MaxSpeed => MoveModeVelocitiesDictionary[MovementMode];
        
        private int nominalMaxSpeed = 50;
        [SerializeField, ProgressBar(0, "nominalMaxSpeed"), SuffixLabel("m/s")]
        private float _SneakSpeed = 3;
        public float SneakSpeed => _SneakSpeed;
        
        [SerializeField, ProgressBar(0, "nominalMaxSpeed"), SuffixLabel("m/s")]
        public float _WalkSpeed = 6;
        public float WalkSpeed => _WalkSpeed;
        
        [SerializeField, ProgressBar(0, "nominalMaxSpeed"), SuffixLabel("m/s")]
        public float _RunSpeed = 9;
        public float RunSpeed => _RunSpeed;
        
        private readonly Dictionary<MovementModes, float> MoveModeVelocitiesDictionary;
        
        private Color moveColor() => ILegs.MovementModeColors[MovementMode];
        
        private void Awake() {
            _animator = GetComponent<Animator>();
            _owner = GetComponent<Entity>();
        }
        
        public void Start() {
            SetupAnimations();
        }
        
        public EnemyLegsNew() {
            MoveModeVelocitiesDictionary = new Dictionary<MovementModes, float>() {
                {MovementModes.Crouch, SneakSpeed},
                {MovementModes.Walk, WalkSpeed},
                {MovementModes.Run, RunSpeed}
            };
        }
    }
}

