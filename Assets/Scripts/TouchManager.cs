using System;
using System.Collections;
using Controllers;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
	private PlayerInput _playerInput;
	private Camera _camera => CameraController.Camera;

	private InputAction _touchPositionAction;
	private InputAction _touchPressAction;

	[SerializeField] private LayerMask _draggableMask;
	[SerializeField] private float _dragSmoothTime;

	public Vector2 TouchPosition => _camera.ScreenToWorldPoint(_touchPositionAction.ReadValue<Vector2>());

	private Vector2 _currentVelocity = Vector2.zero;

	private void Awake()
	{
		_playerInput = GetComponent<PlayerInput>();
		_touchPressAction = _playerInput.actions["TouchPress"];
		_touchPositionAction = _playerInput.actions["TouchPosition"];
	}

	private void OnEnable()
	{
		_touchPressAction.performed += TouchPressed;
	}

	private void OnDisable()
	{
		_touchPressAction.performed -= TouchPressed;
	}

	private void TouchPressed(InputAction.CallbackContext context)
	{
		// _isPressed = context.ReadValue<float>();
		Collider2D hit = Physics2D.OverlapPoint(TouchPosition, _draggableMask);
		if (hit && hit.TryGetComponent(out IDraggable draggable) && draggable.CanDrag())
		{
			draggable.OnStartDrag();
			 StartCoroutine(DragUpdate(hit.transform));
		}
	}

	private IEnumerator DragUpdate(Transform clickedTransform)
	{
		var waitForEndOfFrame = new WaitForEndOfFrame();
		while (_touchPressAction.ReadValue<float>() != 0)
		{
			yield return waitForEndOfFrame;
			clickedTransform.position = Vector2.SmoothDamp(clickedTransform.position, TouchPosition,
				ref _currentVelocity, _dragSmoothTime);
			// yield return null;
		}

		if (clickedTransform.TryGetComponent(out IDraggable draggable))
			draggable.OnEndDrag();
	}
}