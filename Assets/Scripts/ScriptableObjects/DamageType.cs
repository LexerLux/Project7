using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu] public class DamageType : ScriptableObject {
	public static List<DamageType> DamageTypes => Resources.LoadAll<DamageType>("Damage Types").ToList();

	public Sprite  Sprite;
	public Color   Color;
	public bool    Physical          = false;
	public byte StaggerMultiplier = 1;
}