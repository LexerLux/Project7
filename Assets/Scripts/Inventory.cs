using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable] public sealed class MaterialIntDictionary : UnitySerializedDictionary<MaterialSO, uint> { }

public sealed class Inventory : MonoBehaviour {
	[ShowInInspector] private MaterialIntDictionary _items = new MaterialIntDictionary();
	public void Add(MaterialSO material, uint amount) {
		if (_items.ContainsKey(material)) { _items[material] += amount; }
		else { _items.Add(material, amount); }
	}
	public void Remove(MaterialSO material, uint amount) {
		if (!_items.ContainsKey(material) || _items[material] < amount) {
			Debug.LogError("Not enough items to remove");
			return;
		}

		_items[material] -= amount;
		if (_items[material] == 0) { _items.Remove(material); }
	}
	public bool Has(MaterialSO material, uint amount) { return _items.ContainsKey(material) && _items[material] >= amount; }
}