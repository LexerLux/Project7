using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects {
	[CreateAssetMenu(fileName = "FILENAME", menuName = "Chem Effect", order = 0)] public class ChemEffectSO : ScriptableObject {
		public                                    string Name;
		[MultiLineProperty(10), HideLabel] public string Description;
	}
}