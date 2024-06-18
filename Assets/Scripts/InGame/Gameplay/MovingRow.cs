using System;
using System.Collections.Generic;
using Controllers;
using InGame.Gameplay;
using UnityEngine;

public class MovingRow : MonoBehaviour
{
	[Serializable]
	public enum MoveDirection
	{
		LeftToRight = 1,
		RightToLeft = -1
	}

	[SerializeField] private float _moveSpeed = 5;
	[SerializeField] public MoveDirection _moveDirection;
	[SerializeField] private LinkedList<Shelf> _shelves;

	private Vector2 _moveVector;
	private float _shelfDistance;
	private Shelf _endOfLine;

	private void Start()
	{
		Shelf[] array = GetComponentsInChildren<Shelf>();
		_shelfDistance = array[1].Position.x - array[0].Position.x;

		_shelves = new LinkedList<Shelf>();
		foreach (var shelf in array)
			_shelves.AddLast(shelf);

		_moveVector = new Vector2((int)_moveDirection, 0);
	}
// anh chiến ăn cứt 
	private void Update()
	{
		transform.Translate(_moveSpeed * Time.deltaTime * _moveVector);

		float maxX = CameraController.TopRight.x;

		switch (_moveDirection)
		{
			case MoveDirection.LeftToRight:
				var last = _shelves.Last;
				if (last.Value.Position.x > maxX + 3)
				{
					last.Value.Position = _shelves.First.Value.Position + _shelfDistance * Vector3.left;

					_shelves.RemoveLast();
					_shelves.AddFirst(last);
				}

				break;
			case MoveDirection.RightToLeft:
				var first = _shelves.First;
				if (first.Value.Position.x < -maxX - 3)
				{
					first.Value.Position = _shelves.Last.Value.Position + _shelfDistance * Vector3.right;

					_shelves.RemoveFirst();
					_shelves.AddLast(first);
				}

				break;
		}
	}
}