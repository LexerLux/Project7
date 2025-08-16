using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using static MyCamera.CameraModes;

namespace BodyParts {
	public class PlayerEyes : MonoBehaviour {
		private                                             PlayerInput playerInput => GetComponent<PlayerInput>();
		[SerializeField, ShowInInspector, Required] private MyCamera    myCamera;
		[SerializeField, ShowInInspector, Required] private GameObject  cameraFollowTransform;
		[SerializeField, ShowInInspector, Required] private GameObject  cursorTransform;
		private                                             Vector2     crosshairScreenPosition => new Vector2(Screen.width / 2, Screen.height / 2);
		public                                              float       MouseTurnSpeed            = 1;
		public                                              float       GamepadTurnSpeed          = 5;
		public                                              float       TopDownCameraSpeedMouse   = 0.5f;
		public                                              float       TopDownCameraSpeedGamepad = 0.5f;
		private                                             Vector2     lookInputValue;
		public                                              void        OnLook(InputValue value) => lookInputValue = value.Get<Vector2>();

		public void Start() {
			myCamera.eCameraModeChanged.AddListener(cameraModeChanged);
			updateLookSpeed();
		}

		public void cameraModeChanged() {
			//reset cursor location
			cursorTransform.transform.position = gameObject.transform.position;

			updateLookSpeed();
			switch (MyCamera.CameraMode) {
				case MyCamera.CameraModes.FirstPerson or MyCamera.CameraModes.ThirdPerson:
					cameraFollowTransform.transform.rotation = Quaternion.Euler(0, 0, 0);
					break;
				case MyCamera.CameraModes.TopDown:
					cursorTransform.transform.position = new Vector3(gameObject.transform.position.x, 0f, gameObject.transform.position.z);
					break;
			}
		}

		[ShowInInspector, ReadOnly] private float lookSpeed;
		public                              void  OnControlsChanged(PlayerInput playerInput) { updateLookSpeed(); }

		private void updateLookSpeed() {
			switch (MyCamera.CameraMode) {
				case MyCamera.CameraModes.TopDown when playerInput.currentControlScheme == "Keyboard&Mouse":
					lookSpeed = TopDownCameraSpeedMouse;
					break;
				case MyCamera.CameraModes.TopDown when playerInput.currentControlScheme == "Gamepad":
					lookSpeed = TopDownCameraSpeedGamepad;
					break;
				case MyCamera.CameraModes.ThirdPerson or MyCamera.CameraModes.FirstPerson when playerInput.currentControlScheme == "Keyboard&Mouse":
					lookSpeed = MouseTurnSpeed;
					break;
				case MyCamera.CameraModes.ThirdPerson or MyCamera.CameraModes.FirstPerson when playerInput.currentControlScheme == "Gamepad":
					lookSpeed = GamepadTurnSpeed;
					break;
				default: throw new ArgumentOutOfRangeException($"$Either camera mode or input mode is invalid. Input mode is: {playerInput.currentControlScheme}, Camera mode is {MyCamera.CameraMode}");
			}
		}

		public void Update() {
			updateLookValue();

			void updateLookValue() {
				switch (MyCamera.CameraMode) {
					case MyCamera.CameraModes.TopDown:
						/*Vector3 newCursorTransform = cursorTransform.transform.position;
					newCursorTransform.x               += lookInputValue.x * lookSpeed;
					newCursorTransform.z               += lookInputValue.y * lookSpeed;
					cursorTransform.transform.position =  newCursorTransform;*/
						Rigidbody camFollow = cursorTransform.GetComponent<Rigidbody>();
						camFollow.linearVelocity = new Vector3(lookInputValue.x * lookSpeed, 0, lookInputValue.y * lookSpeed);
						break;
					case MyCamera.CameraModes.ThirdPerson or MyCamera.CameraModes.FirstPerson:
						cursorTransform.transform.position = myCamera.Camera.ScreenToWorldPoint(new Vector3(crosshairScreenPosition.x, crosshairScreenPosition.y, 25f));
						gameObject.transform.Rotate(Vector3.forward, lookInputValue.x * lookSpeed * -1);
						cameraFollowTransform.transform.Rotate(Vector3.right, lookInputValue.y * lookSpeed * -1);
						break;
				}
			}
		}

		public float facingDirection{
			get {
				Vector3 cameraPosition = cursorTransform.transform.position;
				//cameraPosition.y = transform.position.y;
				Vector3 relativePosition = transform.position - cameraPosition;
				return Vector3.SignedAngle(relativePosition, transform.forward, Vector3.up);
			}
		}

		public enum FacingDirections { Up, Down, Left, Right }
		public FacingDirections facingDirection4{
			get {
				float angle = facingDirection;
				if (angle > 45 && angle < 135) return FacingDirections.Up;
				if (angle > 135 && angle < 225) return FacingDirections.Left;
				if (angle > 225 && angle < 315) return FacingDirections.Down;
				return FacingDirections.Right;
			}
		}
		void turnTowardsCursor() {
			// this will be used to change the player's sprite to face the cursor.
			//TODO: Implement smooth rotation using addtorque so it looks better and makes the pigtail physics get used.
			// or maybe add a bit of rotation on the 3d axis as the player turns so it looks like they're leaning into the turn.
		}
	}
}