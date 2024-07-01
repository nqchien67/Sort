using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Controllers;
using DG.Tweening;
using UnityEngine;
using Utilities;

namespace InGame.Gameplay
{
	public class Item : MonoBehaviour, IDraggable
	{
		public bool IsMoving;

		public string Type;
		public Sprite Sprite;

		[HideInInspector] public SpriteRenderer Renderer;

		protected readonly List<Shelf> _touchingShelves = new List<Shelf>();
		private Collider2D _collider;

		private ItemLayer _layer;
		private int _originIndex;
		protected ItemLayer _originLayer;

		private Tween _moveTween;

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

		public ItemLayer Layer
		{
			get => _layer;
			set
			{
				if (value == null)
				{
					Debug.Log("oi doi oi");
					return;
				}

				transform.parent = value.transform;
				_layer = value;
			}
		}

		private LevelController LevelController => LevelController.Instance;

		private void Awake()
		{
			Renderer = GetComponent<SpriteRenderer>();
			_collider = GetComponent<Collider2D>();
			_collider.enabled = false;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (IsMoving && other.TryGetComponent(out Shelf shelf)) _touchingShelves.Add(shelf);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.TryGetComponent(out Shelf shelf)) _touchingShelves.Remove(shelf);
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
			AudioController.Instance.PlaySfx(LevelController.Instance.PickupItemSfx);
			LevelController.MovingItemsCount++;
			_moveTween.Kill();
		}

		public virtual bool CanDrag()
		{
			return !_layer.Shelf.IsLocked && LevelController.CanDrag && !IsMoving;
		}

		public Vector2 ClampDragZone(Vector2 position)
		{
			var levelUIController = LevelUIController.Instance;
			position.y = Mathf.Clamp(position.y, levelUIController.MinY, levelUIController.MaxY);

			return position;
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
				closetShelf.ReceiveItem(this);
				_originLayer.CheckShouldDestroy();

				LevelController.Instance.CheckFull();
			}
			else
			{
				MoveBack();
			}

			_touchingShelves.Clear();
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

		protected void MoveBack()
		{
			YieldInstruction yieldInstruction = _originLayer.MoveItemToIndex(this, _originIndex);

			StartCoroutine(CommonIEnumerator.Wait(yieldInstruction,
				() =>
				{
					AudioController.Instance.PlaySfx(LevelController.Instance.PutDownItemSfx);
					LevelController.MovingItemsCount--;
				}));
		}

		public YieldInstruction LocalMove(Vector3 position, float duration)
		{
			_moveTween.Kill();
			_moveTween = transform.DOLocalMove(position, duration)
				.OnComplete(() => Renderer.sortingOrder = 0);

			return _moveTween.WaitForCompletion();
		}

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

		public IEnumerator Disappear()
		{
			_collider.enabled = false;

			Vector3 newScale = new Vector3(1.3f, 1.3f, 0);
			transform.DOScale(newScale, 0.3f).SetEase(Ease.InBack);
			yield return new WaitForSeconds(0.05f);
			Renderer.DOFade(0, 0.2f).SetEase(Ease.InCubic);
			yield return new WaitForSeconds(0.05f);
			var effect = Instantiate(LevelController.Instance.SortEffect, Position, Quaternion.identity);
			SpawnProps();

			// bool animCompleted = false;
			// effect.AnimationState.Complete += entry => animCompleted = true;
			// yield return new WaitUntil(() => animCompleted);
			yield return new WaitForSeconds(0.5f);
			// Destroy(effect.gameObject);
			Destroy(gameObject);
		}

		private void SpawnProps()
		{
			Prop starProp = Instantiate(LevelController.Instance.StarProp, Position, Quaternion.identity);
			starProp.Init(LevelUIController.Instance.StarIcon.position,
				() => LevelUIController.Instance.BlinkStarIcon());
		}
	}
}