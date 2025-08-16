using System;
using Entities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace BodyParts.Systems {
	public abstract class SystemWithStat : BodyParts.Systems.System, UI.IBarTrackable {
		[HorizontalGroup("Colors"), HideLabel] public Color ValueColor;
		[HorizontalGroup("Colors"), HideLabel] public Color CapColor;
		[HorizontalGroup("Colors"), HideLabel] public Color MaxValueColor;

		[SerializeField] private int value;
		/// <summary> Shouldn't edit this directly! </summary>
		[ShowInInspector, ProgressBar(0, "Cap", Segmented = true, ColorGetter = "ValueColor", BackgroundColorGetter = "CapColor"), HideLabel]
		public int Value{
			get => value;
			set {
				this.value = Math.Clamp(value, int.MinValue, Cap);
				eValueChanged.Invoke();
			}
		}

		[SerializeField] private int cap;
		/// <summary> Shouldn't edit this directly! </summary>
		[ShowInInspector, ProgressBar(0, "MaxValue", Segmented = true, ColorGetter = "CapColor", BackgroundColorGetter = "MaxValueColor"), HideLabel]
		public int Cap{
			get => cap;
			set {
				cap = Math.Clamp(value, int.MinValue, MaxValue);
				eCapChanged.Invoke();
				if (Value > Cap) Value = Cap;
			}
		}

		public abstract          int                         MaxValue{ get; }
		[HideInInspector] public UnityEvent                  eValueChanged, eCapChanged, eMaxValueChanged = new();
		public                   UnityEvent<DamageType, int> eValueDamaged, eCapDamaged                   = new();

		[OnValueChanged("ResetVulnerabilitiesIfNeeded"), DictionaryDrawerSettings(IsReadOnly = true)] public DamageVulnerabilityDictionary Vulnerability;
		// ReSharper disable once UnusedMember.Local
		private void ResetVulnerabilitiesIfNeeded() {
			if (Vulnerability.Count == DamageType.DamageTypes.Count) return;
			print("Resetting damage vulnerabilites.");
			Vulnerability = blankDamageVulnerabilityDictionary;
		}
		private DamageVulnerabilityDictionary blankDamageVulnerabilityDictionary{
			get {
				DamageVulnerabilityDictionary r = new();
				foreach (DamageType damageType in DamageType.DamageTypes) { r.Add(damageType, new DamageVulnerabilityInfo {Vulnerability = DamageVulnerabilityInfo.VulnerabilityValues.Immune, HarmType = DamageVulnerabilityInfo.HarmTypes.Value}); }

				return r;
			}
		}

		public SystemWithStat() { eLevelChanged.AddListener(onMaxValueChanged); }

		public void TakeDamage(Attack attack, int armor, int balance) {
			DamageVulnerabilityInfo vulnerabilityInfo = Vulnerability[attack.DamageType];
			int                     damage            = attack.Damage;
			int                     balanceLoss       = attack.Stagger;
			// If GMST.ArmorBeforeResistance is true, armor is applied before resistance. Otherwise, it's applied after.
			if (GMST.ArmorBeforeResistance) {
				applyArmor();
				applyResistances();
			}
			else {
				applyResistances();
				applyArmor();
			}

			GetComponent<Entity>().StaggerTime += balanceLoss;
			//if HarmType is Value, then damage the value. If it's Cap, then damage the cap.
			switch (vulnerabilityInfo.HarmType) {
				case DamageVulnerabilityInfo.HarmTypes.Value:
					Value -= damage;
					eValueDamaged.Invoke(attack.DamageType, damage);
					break;
				case DamageVulnerabilityInfo.HarmTypes.Cap:
					Cap -= damage;
					eCapDamaged.Invoke(attack.DamageType, damage);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			void applyResistances() {
				// Multiply the damage and stun by an amount depending on vulnerabilityInfo.VulnerabilityTypes
				switch (vulnerabilityInfo.Vulnerability) {
					case DamageVulnerabilityInfo.VulnerabilityValues.Normal:
						break;
					case DamageVulnerabilityInfo.VulnerabilityValues.Resistant:
						damage      = (int) Math.Round(GMST.DamageResistanceModifier * damage, MidpointRounding.AwayFromZero);
						balanceLoss = (int) Math.Round(GMST.DamageResistanceModifier * balanceLoss, MidpointRounding.AwayFromZero);
						break;
					case DamageVulnerabilityInfo.VulnerabilityValues.Weak:
						damage      = (int) Math.Round(GMST.DamageVulnerabilityModifier * damage, MidpointRounding.AwayFromZero);
						balanceLoss = (int) Math.Round(GMST.DamageVulnerabilityModifier * balanceLoss, MidpointRounding.AwayFromZero);
						break;
					case DamageVulnerabilityInfo.VulnerabilityValues.Absorbs:
						throw new NotImplementedException("Absorbs damage type not implemented yet.");
					case DamageVulnerabilityInfo.VulnerabilityValues.Immune:
						// TODO: Add some gag. Like they go "hey!" "ouch!" "cut it out!" when taking 0 DMG (in all cases, not just immune),,
						damage      = 0;
						balanceLoss = 0;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			void applyArmor() {
				//Subtract armor and balance from damage and balanceloss respectively, but ensure they don't go below 0.
				damage      = Math.Max(damage - armor, 0);
				balanceLoss = Math.Max(balanceLoss - balance, 0);
			}
		}

		public override void Start() {
			base.Start();
			/*eValueChanged    = new();
			eMaxValueChanged = new();
			eCapChanged      = new();
			eValueDamaged    = new();
			eCapDamaged        = new();*/
		}
		private void onMaxValueChanged() {
			if (Value > MaxValue) Value = MaxValue;
		}
	}

	[Serializable] public class DamageVulnerabilityInfo {
		public enum VulnerabilityValues { Weak, Normal, Resistant, Absorbs, Immune }
		public enum HarmTypes { Value, Cap }
		public VulnerabilityValues Vulnerability;
		public HarmTypes           HarmType;
	}

	[Serializable] public class DamageVulnerabilityDictionary : UnitySerializedDictionary<DamageType, DamageVulnerabilityInfo> { }
}