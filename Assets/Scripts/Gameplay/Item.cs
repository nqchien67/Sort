using System;
using System.Collections.Generic;
using Controllers;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
	public class Item : MonoBehaviour, IDraggable
	{
		public bool IsMoving;

		public Vector3 Position
		{
			get => transform.position;
			set => transform.position = value;
		}

		public Vector3 LocalPosition
		{
			get => transform.localPosition;
			set => transform.localPosition = value;
		}

		public string Type;
		public Sprite Sprite;

		[HideInInspector] public SpriteRenderer Renderer;
		private Collider2D _collider;

		protected List<Shelf> _touchingShelves = new List<Shelf>();
		private int _originIndex;
		protected ItemLayer _originLayer;

		public ItemLayer Layer
		{
			get => _layer;
			set
			{
				transform.parent = value.transform;
				_layer = value;
			}
		}

		private ItemLayer _layer;

		private LevelController LevelController => LevelController.Instance;

		private void Awake()
		{
			Renderer = GetComponent<SpriteRenderer>();
			_collider = GetComponent<Collider2D>();
			_collider.enabled = false;
		}

		public void Init(Sprite sprite)
		{
			SetSprite(sprite);
			gameObject.name = Type;
		}

		public void SetSprite(Sprite sprite)
		{
			Type = sprite.name;
			Sprite = sprite;
			Renderer.sprite = sprite;
		}

		public void Active(bool active)
		{
			_collider.enabled = active;
			Renderer.material = active ? LevelController.NormalMaterial : LevelController.DisabledMaterial;
		}

		public virtual void OnStartDrag()
		{
			IsMoving = true;
			_touchingShelves.Add(_layer.Shelf);
			_originLayer = _layer;
			_originIndex = Array.IndexOf(_layer.Items, this);
			_layer.RemoveItem(this);
			transform.parent = null;

			Renderer.sortingOrder = 1;
		}

		public virtual bool CanDrag()
		{
			return !_layer.Shelf.IsLocked && LevelController.CanDrag;
		}

		public virtual void OnEndDrag()
		{
			IsMoving = false;
			if (_touchingShelves == null || _touchingShelves.Count == 0)
			{
				MoveBack();
				return;
			}

			var closetShelf = GetClosetShelf();
			if (closetShelf.CanTakeItem())
			{
				closetShelf.TakeItem(this);
				_originLayer.CheckShouldDestroy();

				StartCoroutine(GameManager.WaitForFrames(1, () => LevelController.Instance.CheckFull()));
			}
			else
				MoveBack();

			_touchingShelves.Clear();
		}

		protected void MoveBack()
		{
			_originLayer.MoveItemToIndex(this, _originIndex);
		}

		public YieldInstruction LocalMove(Vector3 position, float duration)
		{
			return transform.DOLocalMove(position, duration)
				.OnComplete(() => Renderer.sortingOrder = 0)
				.WaitForCompletion();
		}
// anh chiến ăn cứt 
		protected Shelf GetClosetShelf()
		{
			Shelf nearestShelf = _touchingShelves[0];
			float nearestDistance = Vector2.Distance(Position, nearestShelf.Position);

			for (int i = 1; i < _touchingShelves.Count; i++)
			{
				var shelf = _touchingShelves[i];
				float distance = Vector2.Distance(Position, shelf.Position);
				if (distance < nearestDistance)
				{
					nearestShelf = shelf;
					nearestDistance = distance;
				}
			}

			return nearestShelf;
		}

		public bool Equals(Item item2)
		{
			return Type == item2.Type;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (IsMoving && other.TryGetComponent(out Shelf shelf))
			{
				_touchingShelves.Add(shelf);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.TryGetComponent(out Shelf shelf))
			{
				_touchingShelves.Remove(shelf);
			}
		}
	}
}