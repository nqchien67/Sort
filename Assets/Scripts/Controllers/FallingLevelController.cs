using System;
using System.Collections;
using UnityEngine;

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

		protected override void Start()
		{
			_gridColumn = 3;
			_gridRow = LevelData.TotalShelves / 3;
			CreateGrid();

			base.Start();
			LockedShelves.Sort((s1, s2) => s1.Position.y.CompareTo(s2.Position.y));
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
			yield return null;
			int x = emptyCell.x;
			for (int y = emptyCell.y; y < _gridRow; y++)
			{
				if (!IsOutsideBound(new Vector2Int(x, y + 1)) && Grid[x, y + 1] != null)
				{
					MoveEntityToCell(Grid[x, y + 1], new Vector2Int(x, y));
				}
			}
		}

		private void MoveEntityToCell(FallingShelf entity, Vector2Int endCell)
		{
			Grid[entity._cell.x, entity._cell.y] = null;
			Grid[endCell.x, endCell.y] = entity;
			entity.SetCell(endCell);
		}

		public Vector3 ConvertWorldPosition(Vector2Int cellPos)
		{
			var position = new Vector3(cellPos.x * _cellSizeX, cellPos.y * _cellSizeY, cellPos.y * -0.005f);
			position += ShelvesParent.position + 0.5f * new Vector3(_cellSizeX, _cellSizeY);
			return position;
		}

		public bool IsOutsideBound(Vector2Int cell)
		{
			return cell.x < 0 || cell.x >= _gridColumn || cell.y < 0 || cell.y >= _gridRow;
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
		
				Gizmos.DrawSphere(ConvertWorldPosition(new Vector2Int(x, y)), 0.5f);
			}
		}
	}
}