using System.Collections;
using System.Collections.Generic;
using Controllers;
using InGame.Gameplay;
using UnityEngine;
using Utilities;

namespace FallingLevel
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
			DestroyUnusedLayer();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				StartCoroutine(Test());
			}
		}

		private IEnumerator Test()
		{
			foreach (var s in _fallingLevelController.Shelves)
			{
				Destroy(s.gameObject);
			}

			yield return null;
			SpawnShelves();
			base.SetUpLevel();
			KhongMotTuNaoBiBoLaiPhiaSau();

			yield return null;
			foreach (var s in _fallingLevelController.Shelves)
			{
				if (s.GetAllItems().Count == 0)
				{
					Debug.Log("alsjdlasjdaslkj");
					Debug.Break();
				}
			}
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

		public override void ShuffleItems(List<Item> refreshItems)
		{
			foreach (var shelf in Shelves)
			{
				ItemLayer layer = shelf.Layers[0];
				shelf.RemoveLayer(layer);
				Destroy(layer.gameObject);
			}

			CreateShelfIndexPairs();

			base.ShuffleItems(refreshItems);

			KhongMotTuNaoBiBoLaiPhiaSau();
			LevelController.Instance.Shelves = ToolHelper.RemoveNulls(Shelves);
		}

		protected override void DestroyUnusedLayer()
		{
			//Do tủ loại level này chỉ có 1 layer, nên cố gắng gắn 1 item vào nếu layer trống để ngăn tủ bị Destroy
			KhongMotTuNaoBiBoLaiPhiaSau();
			base.DestroyUnusedLayer();
			_fallingLevelController.StartFall();
		}

		private void KhongMotTuNaoBiBoLaiPhiaSau()
		{
			_itemsSafeToTake ??= FindSafeItemsFromFrontLayers();

			foreach (var shelf in Shelves)
			{
				if (shelf.IsLocked || shelf.FrontLayer.ItemsCount > 0)
					continue;

				if (_itemsSafeToTake.Count == 0)
					break;
				Item item = _itemsSafeToTake[0];
				_itemsSafeToTake.RemoveAt(0);
				ItemLayer prevLayer = item.Layer;
				prevLayer.RemoveItem(item);

				shelf.FrontLayer.PlaceItemAnywhere(item);
			}
		}
	}
}