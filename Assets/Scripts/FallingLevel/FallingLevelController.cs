using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using DG.Tweening;
using InGame.Gameplay;
using UnityEngine;
using Utilities;

namespace FallingLevel
{
	public class FallingLevelController : LevelController
	{
		public FallingShelf[,] Grid;

		[Space] public int _gridColumn;
		public int _gridRow;

		[SerializeField] private float _cellSizeX;
		[SerializeField] private float _cellSizeY;
		public Transform ShelvesParent;

		public float ActionTime;

		private bool _falling;
		private List<FallingShelf> _fallingShelves = new List<FallingShelf>();

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

		public void StartFall()
		{
			_refillThisFrame = true;
		}

		private bool _refillThisFrame;

		private void LateUpdate()
		{
			if (_refillThisFrame)
			{
				_refillThisFrame = false;
				StartCoroutine(Refill());
			}
		}

		private IEnumerator Refill()
		{
			_falling = true;
			for (int i = 0; i < _gridRow; i++)
			for (int y = _gridRow - 1; y >= 0; y--)
			for (int x = 0; x < _gridColumn; x++)
			{
				if (Grid[x, y] == null && !IsOutsideBound(new Vector2Int(x, y + 1)) && Grid[x, y + 1] != null)
				{
					MoveShelfToCell(Grid[x, y + 1], new Vector2Int(x, y));
					yield return null;
				}
			}

			yield return new WaitUntil(() => _fallingShelves.Count == 0);
			_falling = false;
		}

		private YieldInstruction MoveShelfToCell(FallingShelf shelf, Vector2Int endCell)
		{
			_fallingShelves.Add(shelf);
			Grid[shelf._cell.x, shelf._cell.y] = null;
			Grid[endCell.x, endCell.y] = shelf;
			return shelf.SetCell(endCell, () => _fallingShelves.Remove(shelf));
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
			StartCoroutine(CheckFullRoutine());
		}

		private IEnumerator CheckFullRoutine()
		{
			yield return null;
			while (_falling)
				yield return new WaitForEndOfFrame();

			var shelvesOnScreen = GetShelvesOnScreen();
			if (shelvesOnScreen.Count <= 1)
				yield break;

			foreach (var shelf in shelvesOnScreen)
			{
				if (shelf.FrontLayer.ItemsCount < 3)
					yield break;
			}

			if (shelvesOnScreen.Count > 0)
			{
				foreach (var s in shelvesOnScreen)
				{
					Debug.Log(s.gameObject.name);
				}
			}

			Lose();
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
			if (!Application.isPlaying)
				return;

			for (int x = 0; x < _gridColumn; x++)
			for (int y = 0; y < _gridRow; y++)
			{
				if (Grid[x, y] == null)
					continue;

				Gizmos.DrawSphere(ConvertToWorldPosition(new Vector2Int(x, y)), 0.5f);
			}
		}
	}
}