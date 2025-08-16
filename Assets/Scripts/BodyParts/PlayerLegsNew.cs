using System;
using BodyParts.Interfaces;
using BodyParts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

#nullable enable

namespace BodyParts.Player {
    [RequireComponent(typeof(PlayerCharacter), typeof(Animator), typeof(Rigidbody))]
    [RequireComponent(typeof(CirculatorySystem), typeof(NervousSystem), typeof(IntegumentarySystem))]
    public sealed class PlayerLegsNew : MonoBehaviour, ILegs {
        [SerializeField] private bool canMove = true;
        private Animator _animator;
        private Rigidbody _rigidBody;
        private Vector3 _movement = Vector3.zero;
        
        // System references
        private CirculatorySystem _circulatorySystem;
        private NervousSystem _nervousSystem;
        private IntegumentarySystem _integumentarySystem;
        
        // ILegs interface implementation
        public bool DownHeld { 
            set => _animator.SetBool("Down Held", value); 
        }
        
        public bool UpHeld { 
            set => _animator.SetBool("Up Held", value); 
        }
        
        public MovementModes MovementMode {
            get {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Crouch")) { return MovementModes.Crouch; }
                else if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Run")) { return MovementModes.Run; }
                else if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Walk")) { return MovementModes.Walk; }
                else if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Airborne")) { return MovementModes.Airborne; }
                else throw new Exception();
            }
            set { /* Movement mode is determined by animator state */ }
        }
        
        public Vector2 Movement {
            set {
                _movement = new Vector3(value.x, 0, value.y);
                _animator.SetFloat("Movingness", value.magnitude);
            }
        }
        
        public bool CanMove { 
            get => canMove; 
            set => canMove = value; 
        }
        
        public Animator Animator => _animator;
        
        // Speed properties
        [TitleGroup("Velocity")]
        [ShowInInspector, HideInEditorMode, ProgressBar(0, "MaxSpeed", ColorGetter = "moveColor")]
        public float Velocity => _rigidBody.linearVelocity.magnitude;
        
        [SerializeField, HorizontalGroup("Velocity/Sneak", 0.4f), LabelText("Sneak ="), Range(0, 10), LabelWidth(50)]
        private float baseSneakSpeed = 0.5f;
        [SerializeField, HorizontalGroup("Velocity/Sneak", 0.4f), LabelText("+ INT x"), LabelWidth(50), Range(0, 1)]
        private float sneakSpeedPerINT = 0.1f;
        [ShowInInspector, ReadOnly, HorizontalGroup("Velocity/Sneak", 0.2f), LabelText("="), SuffixLabel("m/s"), LabelWidth(25)]
        public float SneakSpeed => _integumentarySystem.Level * sneakSpeedPerINT + baseSneakSpeed;
        
        [SerializeField, HorizontalGroup("Velocity/Walk", 0.4f), LabelText("Walk ="), Range(0, 10), LabelWidth(50)]
        private float baseWalkSpeed = 1.0f;
        [SerializeField, HorizontalGroup("Velocity/Walk", 0.4f), LabelText("+ NRV x"), LabelWidth(50), Range(0, 1)]
        private float walkSpeedPerNRV = 0.1f;
        [ShowInInspector, ReadOnly, HorizontalGroup("Velocity/Walk", 0.2f), LabelText("="), SuffixLabel("m/s"), LabelWidth(25)]
        public float WalkSpeed => _nervousSystem.Level * walkSpeedPerNRV + baseWalkSpeed;
        
        [SerializeField, HorizontalGroup("Velocity/Run", 0.4f), LabelText("Run ="), Range(0, 10), LabelWidth(50)]
        private float baseRunSpeed = 3f;
        [SerializeField, HorizontalGroup("Velocity/Run", 0.4f), LabelText("+ CRC x"), LabelWidth(50), Range(0, 1)]
        private float runSpeedPerCRC = 0.5f;
        [ShowInInspector, ReadOnly, HorizontalGroup("Velocity/Run", 0.2f), LabelText("="), SuffixLabel("m/s"), LabelWidth(25)]
        public float RunSpeed => _circulatorySystem.Level * runSpeedPerCRC + baseRunSpeed;
        
        [SerializeField, TitleGroup("Velocity"), Range(0, 100), SuffixLabel("m/s")]
        private float AirborneMaxSpeed = 10.0f;
        
        public float MaxSpeed {
            get {
                return MovementMode switch {
                    MovementModes.Crouch => SneakSpeed,
                    MovementModes.Walk => WalkSpeed,
                    MovementModes.Run => RunSpeed,
                    MovementModes.Airborne => AirborneMaxSpeed,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        
        // Momentum properties
        [TitleGroup("Momentum")]
        [ShowInInspector, HorizontalGroup("Momentum/Sneak", 0.25f), LabelText("Sneak ="), SuffixLabel("N・s"), LabelWidth(50), PropertyOrder(0)]
        private float sneakMomentum => adjustedMomentum * sneakMomentumMultiplier;
        [ShowInInspector, HorizontalGroup("Momentum/Walk", 0.25f), LabelText("Walk ="), SuffixLabel("N・s"), LabelWidth(50), PropertyOrder(0)]
        private float walkMomentum => adjustedMomentum;
        [ShowInInspector, HorizontalGroup("Momentum/Run", 0.25f), LabelText("Run ="), SuffixLabel("N・s"), LabelWidth(50), PropertyOrder(0)]
        private float runMomentum => adjustedMomentum * runMomentumMultiplier;
        
        [SerializeField, LabelText("="), LabelWidth(20), HorizontalGroup("Momentum/Sneak", 0.25f), HorizontalGroup("Momentum/Walk", 0.25f), HorizontalGroup("Momentum/Run", 0.25f), PropertyOrder(1), Range(0, 100)]
        private float baseMomentum = 50f;
        [SerializeField, ShowInInspector, LabelText("+ INT x"), LabelWidth(35), HorizontalGroup("Momentum/Sneak", 0.25f), HorizontalGroup("Momentum/Walk", 0.25f), HorizontalGroup("Momentum/Run", 0.25f), PropertyOrder(2), Range(0, 100)]
        private float momentumPerINT = 10f;
        [SerializeField, ShowInInspector, HorizontalGroup("Momentum/Sneak", 0.25f), LabelText("x"), LabelWidth(15), PropertyOrder(3), Range(0, 1)]
        private float sneakMomentumMultiplier = 0.5f;
        [SerializeField, ShowInInspector, HorizontalGroup("Momentum/Run", 0.25f), LabelText("x"), LabelWidth(15), PropertyOrder(3), Range(1, 3)]
        private float runMomentumMultiplier = 1.5f;
        
        private float adjustedMomentum => baseMomentum + _integumentarySystem.Level * momentumPerINT;
        
        [ShowInInspector]
        public float momentum {
            get {
                return MovementMode switch {
                    MovementModes.Crouch => sneakMomentum,
                    MovementModes.Walk => walkMomentum,
                    MovementModes.Run => runMomentum,
                    MovementModes.Airborne => 0,
                    _ => throw new Exception("That's not a movement mode that exists (yet).")
                };
            }
        }
        
        private Color moveColor() => ILegs.MovementModeColors[MovementMode];
        
        private void Awake() {
            _animator = GetComponent<Animator>();
            _rigidBody = GetComponent<Rigidbody>();
            _circulatorySystem = GetComponent<CirculatorySystem>();
            _nervousSystem = GetComponent<NervousSystem>();
            _integumentarySystem = GetComponent<IntegumentarySystem>();
        }
        
        public void Start() {
            SetupAnimations();
            _integumentarySystem.eLevelChanged.AddListener(SetSneakPlaybackSpeed);
            _nervousSystem.eLevelChanged.AddListener(SetWalkPlaybackSpeed);
            _circulatorySystem.eLevelChanged.AddListener(SetRunPlaybackSpeed);
        }
        
        private void FixedUpdate() {
            if (!CanMove) return;
            
            _animator.SetFloat("Move Magnitude X", _movement.x);
            _animator.SetFloat("Move Magnitude Y", _movement.y);
            
            // If you've only pressed the stick 0.5 of the way, then you should only move 50% of max speed.
            float desiredSpeed = _movement.magnitude * MaxSpeed;
            // How much momentum is being applied this physics tick?
            Vector3 movementImpulse = _movement * (momentum * Time.deltaTime);
            
            _animator.SetBool("Inputting Movement", _movement.magnitude > 0);
            
            float currentSpeed = _rigidBody.linearVelocity.magnitude;
            
            if (currentSpeed < desiredSpeed) {
                _rigidBody.AddForce(movementImpulse, ForceMode.Impulse);
                // In case we overshot it 
                if (currentSpeed > desiredSpeed) _rigidBody.linearVelocity = Vector3.ClampMagnitude(_rigidBody.linearVelocity, MaxSpeed);
            }
            else if (currentSpeed > desiredSpeed) {
                Decelerate();
                if (currentSpeed < desiredSpeed) _rigidBody.linearVelocity = _rigidBody.linearVelocity.normalized * desiredSpeed;
            }
            
            _rigidBody.linearVelocity = Vector3.ClampMagnitude(_rigidBody.linearVelocity, MaxSpeed);
            _animator.SetFloat("Velocity", _rigidBody.linearVelocity.magnitude);
        }
        
        private void Decelerate() {
            // This took me ages to get right and I'm still not sure how it works so I'll just leave it here, untouched.
            float frictionMomentum = momentum / _rigidBody.mass;
            float currentMomentum = _rigidBody.linearVelocity.magnitude * _rigidBody.mass;
            if (frictionMomentum < currentMomentum) {
                Vector3 friction = -_rigidBody.linearVelocity.normalized * frictionMomentum;
                _rigidBody.AddForce(friction, ForceMode.Impulse);
            }
        }
        
        private static float playbackSpeedModifier = 0.15f;
        private void SetSneakPlaybackSpeed() => _animator.SetFloat("Sneak Playback Speed", 1 + ((_integumentarySystem.Level - 6) * playbackSpeedModifier));
        private void SetWalkPlaybackSpeed() => _animator.SetFloat("Walk Playback Speed", 1 + ((_nervousSystem.Level - 6) * playbackSpeedModifier));
        private void SetRunPlaybackSpeed() => _animator.SetFloat("Run Playback Speed", 1 + ((_circulatorySystem.Level - 6) * playbackSpeedModifier));
    }
}

