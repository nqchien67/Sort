using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using InGame.Gameplay;
using UI;
using UnityEngine;
using Utilities;

namespace Controllers
{
	public class FallingLevelController : LevelController
	{
		public FallingShelf[,] Grid;

		public int _gridColumn;
		public int _gridRow;

		[SerializeField] private float _cellSizeX;
		[SerializeField] private float _cellSizeY;
		public Transform ShelvesParent;

		public float ActionTime;

		private bool _falling;

		protected override IEnumerator Start()
		{
			StartCoroutine(CommonIEnumerator.WaitForFrames(1, RePositionShelfParent));

			_gridColumn = 3;
			_gridRow = LevelData.TotalShelves / 3;
			CreateGrid();

			yield return base.Start();
			if (!CanDrag)
				CanDrag = true;

			LockedShelves.Sort((a, b) => a.Position.y.CompareTo(b.Position.y));
		}

		private void RePositionShelfParent()
		{
			float maxY = LevelUIController.Instance.TopBar.MinY();
			float minY = LevelUIController.Instance.BottomBar.MaxY() + 0.2f;

			int rowsOnScreen = Mathf.FloorToInt((maxY - minY) / _cellSizeY);

			float shelvesParentY = maxY - rowsOnScreen * _cellSizeY + 0.221f; //0.221 :))))
			ShelvesParent.DOMove(new Vector3(ShelvesParent.position.x, shelvesParentY), 0.3f);
		}

		private void CreateGrid()
		{
			Grid = new FallingShelf[_gridColumn, _gridRow];
		}

		public void StartFall(Vector2Int emptyCell)
		{
			StartCoroutine(Fall(emptyCell));
		}

		private IEnumerator Fall(Vector2Int emptyCell)
		{
			_falling = true;
			yield return null;
			int x = emptyCell.x;

			YieldInstruction wait = MoveEntityToCell(Grid[x, emptyCell.y + 1], new Vector2Int(x, emptyCell.y));
			//TODO: kho hieu vai
			for (int y = emptyCell.y + 1; y < _gridRow; y++)
			{
				if (!IsOutsideBound(new Vector2Int(x, y + 1)) && Grid[x, y + 1] != null)
				{
					MoveEntityToCell(Grid[x, y + 1], new Vector2Int(x, y));
				}
			}

			yield return wait;
			_falling = false;
		}

		private YieldInstruction MoveEntityToCell(FallingShelf entity, Vector2Int endCell)
		{
			Grid[entity._cell.x, entity._cell.y] = null;
			Grid[endCell.x, endCell.y] = entity;
			return entity.SetCell(endCell);
		}

		public Vector3 ConvertToWorldPosition(Vector2Int cellPos)
		{
			var position = new Vector3(cellPos.x * _cellSizeX, cellPos.y * _cellSizeY, cellPos.y * -0.005f);
			position += ShelvesParent.position + 0.5f * new Vector3(_cellSizeX, _cellSizeY, 0);
			return position;
		}

		private bool IsOutsideBound(Vector2Int cell)
		{
			return cell.x < 0 || cell.x >= _gridColumn || cell.y < 0 || cell.y >= _gridRow;
		}

		public override void CheckFull()
		{
			StartCoroutine(CommonIEnumerator.WaitUntil(() => !_falling, () =>
			{
				foreach (var shelf in GetShelvesOnScreen())
				{
					if (shelf.FrontLayer.ItemsCount < 3)
						return;
				}

				Lose();
			}));
		}

		private List<Shelf> GetShelvesOnScreen()
		{
			float maxY = LevelUIController.Instance.TopBar.MinY();
			return Shelves.Where(shelf => shelf.Position.y < maxY).ToList();
		}

		public void RemoveShelf(Shelf shelf)
		{
			List<Shelf> temp = Shelves.ToList();
			temp.Remove(shelf);
			Shelves = temp.ToArray();
		}

		private void OnDrawGizmos()
		{
			// 	if (!Application.isPlaying)
			// 		return;
			//
			// 	for (int x = 0; x < _gridColumn; x++)
			// 	for (int y = 0; y < _gridRow; y++)
			// 	{
			// 		if (Grid[x, y] == null)
			// 			continue;
			//
			// 		Gizmos.DrawSphere(ConvertWorldPosition(new Vector2Int(x, y)), 0.5f);
			// 	}
		}
	}
}