using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using static MyCamera;

#nullable enable

namespace BodyParts.Player {
	[RequireComponent(typeof(PlayerInput)), DisallowMultipleComponent] public sealed class Brain : MonoBehaviour {
		private BodyParts.Legs legs        => GetComponent<BodyParts.Legs>();
		private BodyParts.Arms arms        => GetComponent<BodyParts.Arms>();
		private PlayerInput    playerInput => GetComponent<PlayerInput>();

		private Vector2 moveInputValue;

		public void OnDown(InputValue value) {
			legs.DownHeld = value.isPressed;
			Debug.Log("down is down");
		}
		public void OnUp(InputValue value)   => legs.UpHeld = value.isPressed;
		public void OnMove(InputValue value) => moveInputValue = value.Get<Vector2>();

		public void OnLeftHand(InputValue value) {
			Debug.Log("Left Hand");
			arms.LeftHandPressed = value.isPressed;
		}
		public void OnLeftArm(InputValue value)   => arms.LeftArmPressed = value.isPressed;
		public void OnRightHand(InputValue value) => arms.RightHandPressed = value.isPressed;
		public void OnRightArm(InputValue value)  => arms.RightArmPressed = value.isPressed;
	}
}