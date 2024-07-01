using System;
using System.Collections;
using Controllers;
using UnityEngine;
using UnityEngine.EventSystems;
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
		Collider2D hit = Physics2D.OverlapPoint(TouchPosition, _draggableMask);
		if (hit && hit.TryGetComponent(out IDraggable draggable) && draggable.CanDrag())
		{
			draggable.OnStartDrag();
			_currentVelocity = Vector2.zero;
			StartCoroutine(DragUpdate(hit.transform, draggable));
		}
	}

	private IEnumerator DragUpdate(Transform clickedTransform, IDraggable draggable)
	{
		var waitForEndOfFrame = new WaitForEndOfFrame();
		while (_touchPressAction.ReadValue<float>() != 0)
		{
			yield return waitForEndOfFrame;
			Vector2 newPosition = Vector2.SmoothDamp(clickedTransform.position, TouchPosition, ref _currentVelocity,
				_dragSmoothTime);

			clickedTransform.position = draggable.ClampDragZone(newPosition);
		}

		draggable.OnEndDrag();
	}
}