using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects {
	[CreateAssetMenu(fileName = "FILENAME", menuName = "Condition", order = 0)] public class ConditionSO : ScriptableObject {
		[Tooltip("Obsolete. One stack is always 100%."), ReadOnly]                                                                                                                                                      public ushort Capacity = 100;
		[SuffixLabel("/s"), Range(0, 360), Tooltip("How long it takes, in seconds, to clear one full stack. A value of 0 means no recovery over time.")]                                                                public ushort RecoveryTime;
		[SuffixLabel("/s"), Range(0, 180), Tooltip("How long it takes, in seconds, to clear one NON-FULL stack -- as if it were 99.999% of the way full but not quite there. A value of 0 means no natural recovery.")] public ushort ClearTime;
		public                                                                                                                                                                                                                 Sprite Sprite;
		public                                                                                                                                                                                                                 Color  Color;
		[MultiLineProperty(10), HideLabel] public                                                                                                                                                                              string Description;
	}
}