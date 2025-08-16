using System;
using UnityEngine;

public class Billboarding : MonoBehaviour {
	private Camera mainCamera;
	private void   Start() { mainCamera = Camera.main; }
	private void LateUpdate() {
		switch (MyCamera.CameraMode) {
			//if MyCamera is in Top down mode, billboard on one axis. In first or third person, billboard on all axes.
			case MyCamera.CameraModes.TopDown:
				transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
				break;
			case MyCamera.CameraModes.FirstPerson:
			case MyCamera.CameraModes.ThirdPerson:
				transform.LookAt(mainCamera.transform);
				break;
			default: throw new ArgumentOutOfRangeException();
		}
	}
}