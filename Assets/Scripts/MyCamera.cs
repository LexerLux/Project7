using System;
using Cinemachine;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

#nullable enable

[RequireComponent(typeof(Camera)), DisallowMultipleComponent, HideMonoScript] public sealed class MyCamera : MonoBehaviour {
	[ShowInInspector, SerializeField, Required] private CinemachineVirtualCamera topDownCamera;
	[ShowInInspector, SerializeField, Required] private CinemachineVirtualCamera thirdPersonCamera;
	[ShowInInspector, SerializeField, Required] private CinemachineVirtualCamera firstPersonCamera;
	public static                                       MyCamera?                Instance;
	[ShowInInspector, SerializeField, Required] private GameObject               topDownCameraUpOverride;
	private                                             CinemachineBrain         cinemachineBrain => GetComponent<CinemachineBrain>();
	public                                              Camera                   Camera           => GetComponent<Camera>();
	[HideInInspector] public                            UnityEvent               eCameraModeChanged;

	[SerializeField] private float cameraDistance;
	[ShowInInspector, PropertyRange(0.1, 25)]
	public float CameraDistance{
		get => cameraDistance;
		set {
			topDownCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_MaximumOrthoSize = value;
			topDownCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_MinimumOrthoSize = value;
			cameraDistance                                                                           = value;
		}
	}

	public enum CameraModes { TopDown, ThirdPerson, FirstPerson }
	private CameraModes cameraMode = CameraModes.TopDown;
	[ShowInInspector, EnumToggleButtons]
	public static CameraModes CameraMode{
		get => Instance.cameraMode;
		set {
			if (value == Instance.cameraMode) return;
			switch (value) {
				case CameraModes.FirstPerson:
					Instance.firstPersonCamera.MoveToTopOfPrioritySubqueue();
					Instance.cinemachineBrain.m_WorldUpOverride = null;
					Instance.Camera.orthographic                = false;
					break;
				case CameraModes.ThirdPerson:
					Instance.thirdPersonCamera.MoveToTopOfPrioritySubqueue();
					Instance.cinemachineBrain.m_WorldUpOverride = null;
					Instance.Camera.orthographic                = false;
					break;
				case CameraModes.TopDown:
					Instance.topDownCamera.MoveToTopOfPrioritySubqueue();
					Instance.cinemachineBrain.m_WorldUpOverride = Instance.topDownCameraUpOverride.transform;
					Instance.Camera.orthographic                = true;
					break;
				default: throw new Exception("Not a valid camera type.");
			}

			Instance.cameraMode = value;
			print("invoking event");
			Instance.eCameraModeChanged.Invoke();
		}
	}

	private bool requirementsSet => topDownCamera != null && thirdPersonCamera != null && firstPersonCamera != null && topDownCameraUpOverride != null;

	public MyCamera() => eCameraModeChanged = new();
	private void Start() {
		CameraDistance = CameraDistance;
		if (Instance != null) throw new Exception("MyCamera is meant to be a singleton.");
		if (!requirementsSet) throw new Exception("Required value not set.");
		Instance = this;

		CameraMode = CameraMode;
	}
}