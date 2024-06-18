using System;
using Controllers;
using InGame.Gameplay;
using UnityEngine;
using Utilities;

namespace Gameplay.Tutorial.Level1
{
	public class TutItem : Item
	{
		[SerializeField] private Shelf _targetShelf;

		private void Start()
		{
			SetSprite(Renderer.sprite);
			Layer = GetComponentInParent<ItemLayer>();
			Active(true);
		}

		public override bool CanDrag()
		{
			return _targetShelf != null && base.CanDrag();
		}

		public override void OnEndDrag()
		{
			IsMoving = false;
			if (_touchingShelves == null || _touchingShelves.Count == 0)
			{
				MoveBack();
				return;
			}

			var closetShelf = GetClosetShelf();
			if (closetShelf == _targetShelf && closetShelf.CanTakeItem())
			{
				closetShelf.TakeItem(this);
				_originLayer.CheckShouldDestroy();

				// StartCoroutine(CommonIEnumerator.WaitForFrames(1, () => LevelController.Instance.CheckFull()));
			}
			else
				MoveBack();

			_touchingShelves.Clear();
		}
	}
}