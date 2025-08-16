using System;
using JetBrains.Annotations;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.Events;
using Object = System.Object;

#nullable enable

//TODO: Actually like...apply the condition effects

namespace BodyParts {
	public class BuildupChangedEvent : UnityEvent<ConditionSO, float> { }
	public class EffectAppliedEvent : UnityEvent<ConditionSO> { }

	[Serializable] public class Organ : object {
		/// <summary> The system this organ belongs to </summary>
		[ShowInInspector, ReadOnly] protected Systems.System system;
		/// <summary> Once the condition builds up to 100%, the effect is applied. Should never be below 0 or above 100. Gradually decreases over time.</summary>
		public float Buildup{
			get => buildup;
			protected set {
				//throw an error if the value is out of bounds, then clamp it
				if (value is < 0 or > 100) {
					Debug.LogError($"Buildup value of {value} is out of bounds! Clamping to 0-100.");
					value = Mathf.Clamp(value, 0, 100);
				}

				buildup = value;
				OnBuildupChanged.Invoke(Condition, buildup);
			}
		}
		[ShowInInspector, ReadOnly] protected float buildup = 0.0f;
		/// <summary> The condition that is building up in this organ. Goes back to null when buildup goes down to 0. Should only ever be set from null because I'm not sure what to do if there's no "free" organ to apply condition buildup to yet. </summary>
		public ConditionSO? Condition{
			get => condition;
			protected set {
				//if the condition is not null and being set to a non-null value, throw a unity error
				if (condition != null && value != null) {
					Debug.LogError($"Condition of {condition} is being set to {value}! This is not allowed.");
					return;
				}

				condition = value;
			}
		}
		[ShowInInspector, ReadOnly] protected ConditionSO? condition;

		/// <summary> Is the effect active now? </summary>
		public bool EffectActive{
			get => effectActive;
			protected set {
				effectActive = value;
				//throw an error if the condition is null and the effect is being set to active
				if (condition == null && value) {
					Debug.LogError($"EffectActive is being set to true, but condition is null!");
					return;
				}

				if (value) { OnEffectApplied.Invoke(Condition); }
				else { OnEffectRemoved.Invoke(); }
			}
		}
		[ShowInInspector, ReadOnly] protected bool effectActive = false;

		/// <summary> Unity event containing the condition and buildup that is fired when the buildup amount changes. </summary>
		public BuildupChangedEvent OnBuildupChanged = new BuildupChangedEvent();
		public EffectAppliedEvent OnEffectApplied = new EffectAppliedEvent();
		public UnityEvent         OnEffectRemoved = new UnityEvent();
		public Organ(Systems.System system) { this.system = system; }

		/// <summary> Increase the amount of condition buildup. </summary>
		[Button] public void IncreaseBuildup(ConditionSO Condition, float Amount) {
			// if the amount is negative, throw a unity error
			if (Amount < 0) {
				Debug.LogError("Amount cannot be negative");
				return;
			}

			// if there's already a condition active, throw a unity error
			if (EffectActive) {
				Debug.LogError("Cannot apply condition to organ with active effect");
				return;
			}

			// set the condition
			if (this.Condition == null) this.Condition = Condition;
			// increase the buildup
			Buildup += Amount;

			// if the buildup is 100, activate the effect
			if (Buildup >= 100) {
				effectActive = true;
				//TODO: Apply the effect
			}
		}

		/// <summary> Reduce the amount of condition buildup. </summary>
		[Button] public void DecreaseBuildup(float Amount) {
			// if the amount is negative, throw a unity error
			if (Amount < 0) {
				Debug.LogError("Amount cannot be negative");
				return;
			}

			// if there's no condition active, throw a unity error
			if (!EffectActive) {
				Debug.LogError("Cannot remove condition from organ with no active effect");
				return;
			}

			// decrease the buildup
			Buildup -= Amount;

			//if buildup is below 0, throw an error
			if (Buildup < 0) {
				Debug.LogError($"Buildup is {Buildup}! Setting to 0.");
				Buildup = 0;
			}

			// if the buildup is 0, deactivate the effect
			if (Buildup <= 0) {
				EffectActive = false;
				Condition    = null;
			}
		}

		[Button] public void ClearBuildup() => DecreaseBuildup(Buildup);
		[Button] public void ApplyCondition(ConditionSO Condition) {
			ClearBuildup();
			IncreaseBuildup(Condition, 100);
		}

		public void Update() {
			// if the condition is null, there's no point
			if (Condition == null) { return; }

			// if the effect is active, decrease buildup according to RecoveryTime per second. Otherwise, decrease buildup according to ClearTime per second.
			float clearanceTime = EffectActive ? Condition.RecoveryTime : Condition.ClearTime;
			// if the clearance time is 0 that actually means it's infinite, so don't do anything
			if (clearanceTime == 0) { return; }
			float clearanceRate = 100 / clearanceTime;
			//decrease buildup but don't make it go below 0
			DecreaseBuildup(Mathf.Min(clearanceRate * Time.deltaTime, Buildup));
		}
	}
}