using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#nullable enable

[ShowInInspector, Serializable] public struct  Attack {
	public DamageType DamageType;
	public bool       Physical;
	public byte       Damage;
	public byte       Stagger;
	public byte       ArmorPiercing;
	public Vector3?   Origin;
	public Attack(DamageType damageType, byte damage, Vector3 origin, byte armorPiercing = 0) {
		DamageType         = damageType;
		Damage             = damage;
		Origin             = origin;
		ArmorPiercing = armorPiercing;
		Physical           = damageType.Physical;
		Damage             = damage;
		Stagger            = (byte) (Damage * DamageType.StaggerMultiplier);
	}
}
