using System;
using GunParts.MuzzleDevices;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Weapons.Ranged;

#nullable enable

namespace GunParts {
	public sealed class Barrel : GunPart {
		[OdinSerialize, ShowInInspector, Range(1, 100), TabGroup("1", "Stats")] private int     baseVelocity = 25;
		[ShowInInspector, ProgressBar(0, 10), TabGroup("1", "Stats")]          public  int     velocity => (int) (baseVelocity + velocityUpgrade.StatModifier);
		//[TabGroup("1", "Stats")]                                               public  bool    IntegralSuppressor; // TODO: This.
		[ShowInInspector, OdinSerialize, TabGroup("1", "Velocity Upgrade")]    private Upgrade velocityUpgrade;

		[OdinSerialize, TabGroup("1", "Attachment")]                                    private GameObject?   Threading;
		[ShowInInspector, TabGroup("1", "Attachment"), InlineEditor(InlineEditorObjectFieldModes.Boxed)] public  MuzzleDevice? MuzzleDevice => _muzzleDevice;
		[OdinSerialize, HideInInspector]                                                                                  private MuzzleDevice? _muzzleDevice;

		public Barrel() => velocityUpgrade = new(this);

		[Button, TabGroup("1", "Debug")] public void PassBullet() {
			if (_muzzleDevice != null) _muzzleDevice.PassBullet();
		}

		[Button, TabGroup("1", "Attachment")] public void AttachMuzzleDevice(MuzzleDevice Device) {
			if (MuzzleDevice != null) {
				Debug.LogWarning("Though having two suppresssors attached at once would probably be twice as quiet, you can't attach a muzzle device to a barrel that already has one.");
				return;
			}

			if (Threading == null) {
				Exception up = new Exception("No threading to attach to!");
				Debug.LogException(up);
				throw up;
			}

			Device.DisablePhysics();
			Device.transform.SetParent(transform);
			Device.transform.position = Threading.transform.position;
			_muzzleDevice             = Device;
		}
	}
}