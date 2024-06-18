using System.Collections.Generic;
using InGame.Gameplay;
using UnityEngine;

namespace Controllers
{
	public class FallingLevelSetupManager : LevelSetupManager
	{
		[SerializeField] private FallingShelf _fallingShelfPrefab;
		private FallingLevelController _fallingLevelController;

		public override void SetUpLevel()
		{
			_fallingLevelController = GetComponent<FallingLevelController>();
			SpawnShelves();
			base.SetUpLevel();
		}

		private void SpawnShelves()
		{
			List<Shelf> shelves = new List<Shelf>();

			for (int x = 0; x < _fallingLevelController._gridColumn; x++)
			for (int y = 0; y < _fallingLevelController._gridRow; y++)
			{
				Vector3 spawnPos =
					_fallingLevelController.ConvertToWorldPosition(new Vector2Int(x, y));
				FallingShelf shelf = Instantiate(_fallingShelfPrefab, spawnPos, Quaternion.identity,
					_fallingLevelController.ShelvesParent);

				shelves.Add(shelf);
				_fallingLevelController.Grid[x, y] = shelf;

				shelf.Init(_fallingLevelController, new Vector2Int(x, y));
			}

			for (int i = shelves.Count - 1; i >= 0; i--)
			{
				Shelf shelf = shelves[i];
				foreach (var layer in shelf.Layers) 
					layer.CheckShouldDestroy();
			}

			_fallingLevelController.Shelves = shelves.ToArray();
			SkinManager.Instance.SetShelfAndBackgroundSkin(shelves.ToArray());
		}
	}
}