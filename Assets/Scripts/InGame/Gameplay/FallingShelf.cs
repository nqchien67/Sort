using System;
using Controllers;
using DG.Tweening;
using UnityEngine;
using Utilities;

namespace InGame.Gameplay
{
	public class FallingShelf : Shelf
	{
		public Vector2Int _cell;
		private Tween _moveTween;

		private FallingLevelController _fallingLevelController;

		private Sprite[] _sprites;

		public void Init(FallingLevelController fallingLevelController, Vector2Int cell)
		{
			gameObject.name = cell.ToString();
			_fallingLevelController = fallingLevelController;
			_cell = cell;

			_sprites = cell.x switch
			{
				0 => _leftSprites,
				1 => _centerSprites,
				_ => _rightSprites
			};

			if (cell.y == fallingLevelController._gridRow - 1)
				Renderer.sprite = _sprites[0];
			else if (cell.y <= 3)
				Renderer.sprite = _sprites[4 - cell.y];
			else
				Renderer.sprite = _sprites[1];
		}

		public YieldInstruction SetCell(Vector2Int newCell, Action onComplete = null)
		{
			_cell = newCell;

			_moveTween.Kill();
			_moveTween = transform.DOMove(_fallingLevelController.ConvertToWorldPosition(newCell),
					_fallingLevelController.ActionTime)
				.SetEase(Ease.OutBounce)
				.OnComplete(() => onComplete?.Invoke());

			return _moveTween.WaitForCompletion();
		}

		public override void RemoveFrontLayer()
		{
			if (_cell.y + 1 < _fallingLevelController._gridRow &&
			    _fallingLevelController.Grid[_cell.x, _cell.y + 1] == null && _cell.y - 1 >= 0)
			{
				var shelfBellow = _fallingLevelController.Grid[_cell.x, _cell.y - 1];
				if (shelfBellow != null)
					shelfBellow.Renderer.sprite = _sprites[0];
			}

			_fallingLevelController.StartFall(_cell);

			_fallingLevelController.RemoveShelf(this);
			Destroy(gameObject);
		}
	}
}