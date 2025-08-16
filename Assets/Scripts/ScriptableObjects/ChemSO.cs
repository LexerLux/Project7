using System;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable] public class ChemEffectsDictionary : UnitySerializedDictionary<ChemEffectSO, byte> { }

// Data for drugs/chems goes here
[CreateAssetMenu(fileName = "FILENAME", menuName = "Chem", order = 0)] public class ChemSO : ScriptableObject {
	[HideLabel]
	[PreviewField(50, ObjectFieldAlignment.Left)]
	[HorizontalGroup("row2", 50), VerticalGroup("row2/left")]
	public Sprite Sprite;
	[VerticalGroup("row2/right"), LabelWidth(-54)] public string Name, ShortName;
	
	public ushort                DosageSize         = 100;
	public ushort                OralDosageDuration = 600;
	public bool                  IV, PO             = false;
	public ChemEffectsDictionary Effects            = new ChemEffectsDictionary();
	[MultiLineProperty(5)] public string                Notes;

}