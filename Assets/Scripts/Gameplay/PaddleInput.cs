using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Pong
{
	public class PaddleInput : MonoBehaviour
	{
		[Header("Input")] [SerializeField] private InputAction _movementAxisAction;
		[SerializeField] private InputAction _powerupButtonAction;
		[FormerlySerializedAs("joystickID")] [SerializeField] private int ignoreFromDeviceID;
		private PaddleMovement _paddleMovement;
		private Paddle _paddle;

		private InputDevice _device;
		private void Awake()
		{
			_paddleMovement = GetComponent<PaddleMovement>();
			_paddle = GetComponent<Paddle>();
			foreach (var device in InputSystem.devices)
			{
				if (device.deviceId == ignoreFromDeviceID)
				{

					_device = device;
					break;
				}
			}
		}

		private void OnEnable()
		{
			_movementAxisAction.Enable();
			_powerupButtonAction.Enable();
		}

		private void Update()
		{
			// _device.allControls[1]..
			// InputSystem.devices
			
			if (_movementAxisAction.activeControl != null)
			{
				if(_movementAxisAction.activeControl.device.deviceId != ignoreFromDeviceID)
				{
					_paddleMovement.Move(_movementAxisAction.ReadValue<float>());
				}
			}
			else
			{
				//pass zeros
				_paddleMovement.Move(_movementAxisAction.ReadValue<float>());
			}

			if (_powerupButtonAction.WasPerformedThisFrame())
			{
				_paddle.Player.ActivatePowerup();
			}
		}
	}
}