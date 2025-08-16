using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Hurtable : MonoBehaviour {
	public                                          UnityEvent<Attack, int, int> eOnHit  = new();
	public                                          int                          Balance = 0;
	public                                          int                          Armor   = 0;

	public                                          void   ProcessDamage(Attack attack) { eOnHit.Invoke(attack, Armor, Balance); }
	[SerializeField, ShowInInspector, InlineButton("testarino")] private Attack testAttack;
	public                                          void   testarino() => ProcessDamage(testAttack);
}