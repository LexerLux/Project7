using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;

namespace BodyParts.Systems {
	[RequireComponent(typeof(Entity)), HideMonoScript] public abstract class System : MonoBehaviour {
		private Entity            Owner => gameObject.GetComponent<Entity>();
		public  List<ConditionSO> Conditions;

		[ShowInInspector] public List<Organ> Organs = new List<Organ>();

		// ReSharper disable once UnusedMember.Local
		private                                                                                                                                                                                             string Suffix => $"{Level}/{maxLevel}";
		[ShowInInspector, SuffixLabel("$Suffix", Overlay = false), SerializeField, ProgressBar(0, "maxLevel", Segmented = true, ColorGetter = "Color"), OnValueChanged("updateLevel"), HideLabel] protected int    _Level = 6;
		public int Level{
			get => _Level;
			set {
				Debug.Log("test");
				_Level = Math.Clamp(value, 0, Owner.AttributeMax);
				//create and destroy organs as needed
				if (Level > Organs.Count) {
					for (int i = Organs.Count; i < Level; i++) { Organs.Add(new Organ(this)); }
				}
				else if (Level < Organs.Count) {
					for (int i = Organs.Count - 1; i >= Level; i--) { Organs.RemoveAt(i); }
				}

				eLevelChanged.Invoke();
			}
		}
		public abstract          Color      Color{ get; }
		[HideInInspector] public UnityEvent eLevelChanged;
		// ReSharper disable once MemberCanBePrivate.Global
		protected                                          int           maxLevel => Owner.AttributeMax;
		[HideLabel, FoldoutGroup("Debuff Manager")] public DebuffManager DebuffManager;

		protected void Update() {
			DebuffManager.Update();
			//update organs
			foreach (Organ organ in Organs) organ.Update();
		}

		protected System() {
			DebuffManager = new DebuffManager();
			eLevelChanged = new();
		}

		public virtual void Start() { }
		protected void updateLevel() {
			Debug.Log("test");
			_Level = Math.Clamp(_Level, 0, Owner.AttributeMax);
			//create and destroy organs as needed
			if (Level > Organs.Count) {
				for (int i = Organs.Count; i < Level; i++) { Organs.Add(new Organ(this)); }
			}
			else if (Level < Organs.Count) {
				for (int i = Organs.Count - 1; i >= Level; i--) { Organs.RemoveAt(i); }
			}

			eLevelChanged.Invoke();
		}
	}

	[Serializable] public class DebuffManager {
		#region Vulnerability
		[TabGroup("Vulnerability")] public List<ConditionSO> Conditions;
		#endregion

		#region Stacks
		[ShowInInspector, TabGroup("Stacks"), HideLabel] private List<ConditionSO> debuffStacks;
		internal                                                 void              onStackGained(ConditionSO type) => debuffStacks.Insert(0, type);
		// * if you don't understand why we add them to the beginning, think about how you want debuffs to look on hearts on the UI. it's hard to explain in text.
		public int StacksOf(ConditionSO Type) {
			int value = 0;
			foreach (ConditionSO stack in debuffStacks) {
				if (stack == Type) value++;
			}

			return value;
		}
		internal void onStackCleared(ConditionSO type) => debuffStacks.Remove(type);
		#endregion

		#region Buildup
		[ShowInInspector, TabGroup("Buildup"), PropertyOrder(1), ListDrawerSettings(DraggableItems = false, HideAddButton = true)] private List<debuffBuildupManager> debuffBuildupManagers;
		// * We're not using a dictionary because Odin won't let us visualize them very well.

		[Button, TabGroup("Buildup"), ShowIf("@testType != null")] public void        test() => GainOrLoseDebuffBuildup(testType, debuffAmount);
		[AssetSelector, TabGroup("Buildup"), HideLabel]            public ConditionSO testType;
		[TabGroup("Buildup"), HideLabel]                           public float       debuffAmount;

		private debuffBuildupManager buildupManagerOfType(ConditionSO type) {
			if (type == null) throw new Exception("Need a value.");
			foreach (debuffBuildupManager manager in debuffBuildupManagers) {
				if (manager.Type.name == type.name) return manager;
			}

			return null;
		}

		public void GainOrLoseDebuffBuildup(ConditionSO Type, float Amount) {
			if (Amount == 0) throw new Exception("Invalid value.");
			if (buildupManagerOfType(Type) == null)
				debuffBuildupManagers.Add(new debuffBuildupManager(Type, this));
			buildupManagerOfType(Type).Amount += Amount;
		}

		internal void clearBuildup(ConditionSO type) => debuffBuildupManagers.Remove(buildupManagerOfType(type));
		#endregion

		public void Update() {
			foreach (debuffBuildupManager VARIABLE in debuffBuildupManagers) { VARIABLE.Update(); }
		}

		internal DebuffManager() {
			debuffBuildupManagers = new List<debuffBuildupManager>();
			debuffStacks          = new List<ConditionSO>();
		}
	}

	[HideReferenceObjectPicker] internal class debuffBuildupManager {
		[HorizontalGroup("A", 50), VerticalGroup("A/A"), HideLabel, ShowInInspector, PreviewField(50, ObjectFieldAlignment.Left)] private Sprite sprite => Type.Sprite;

		[VerticalGroup("A/B"), HideLabel] public readonly ConditionSO   Type;
		private readonly                                  DebuffManager debuffManager;
		private                                           float         BuildupPerStack => Type.Capacity;
		private                                           float         RecoveryTime    => Type.RecoveryTime;

		[VerticalGroup("A/B"), HideLabel, ShowInInspector, ProgressBar(0, "BuildupPerStack", ColorGetter = "@Type.Color")] private float _amount;
		public float Amount{
			get => _amount;
			set {
				_amount = value;

				while (_amount > BuildupPerStack) {
					debuffManager.onStackGained(Type);
					_amount -= BuildupPerStack;
				}

				while (_amount < 0) {
					if (debuffManager.StacksOf(Type) > 0) {
						debuffManager.onStackCleared(Type);
						_amount += BuildupPerStack;
					}
					else {
						debuffManager.clearBuildup(Type);
						_amount = 0;
					}
				}
			}
		}

		internal debuffBuildupManager(ConditionSO Type, DebuffManager debuffManager) {
			this.debuffManager = debuffManager;
			this.Type          = Type;
		}

		internal void Update() => Amount -= RecoveryTime * Time.deltaTime;
	}
}