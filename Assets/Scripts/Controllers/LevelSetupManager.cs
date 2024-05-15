using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Gameplay;
using UnityEngine;
using Item = Gameplay.Item;
using Random = UnityEngine.Random;

namespace Controllers
{
	public class LevelSetupManager : MonoBehaviour
	{
		private SkinCollection _skinCollection;

		private LevelData LevelData => LevelController.Instance.LevelData;
		protected Shelf[] Shelves => LevelController.Instance.Shelves;

		private List<Shelf> LockedShelves
		{
			get => LevelController.Instance.LockedShelves;
			set => LevelController.Instance.LockedShelves = value;
		}

		private const int ItemNumbEachLayer = 3;
		public List<ShelfIndexPair> shelfIndexPairs;


		public virtual void SetUpLevel()
		{
			_skinCollection = SkinCollection.Instance;
			_skinCollection.InitUnplacedItems(LevelData.ItemTypes, LevelData.id);
			LevelController.Instance.RemainItemTypes = _skinCollection._unplacedItems;

			SetUpLocks();

			SetUpItems();
			PlaceLastTwoType();
			if (LockedShelves != null && LockedShelves.Count > 0)
				FillLockedShelves();

			foreach (var shelf in Shelves)
				shelf.RenderLayers();

			var itemCount = new List<Item>();
			foreach (var s in Shelves)
			{
				itemCount.AddRange(s.GetAllItems());
			}

			Debug.Log(itemCount.Count); 
		}

		private void SetUpItems()
		{
			shelfIndexPairs = new List<ShelfIndexPair>();

			foreach (var shelf in Shelves)
			{
				shelf.AddNewLayer();

				for (int i = 0; i < ItemNumbEachLayer; i++)
					shelfIndexPairs.Add(new ShelfIndexPair(shelf, i));
			}

			// StartCoroutine(PlaceItems(shelfIndexPairs));
			PlaceItems(shelfIndexPairs);
		}

		public List<Item> fixedItems;

		private void PlaceLastTwoType()
		{
			fixedItems = new List<Item>();
			for (int i = 0; i < 2; i++)
			{
				Sprite itemSprite = _skinCollection.GetNextUnplacedItem();

				for (int j = 0; j < 3; j++)
				{
					ShelfIndexPair shelfIndexPair =
						GetValidShelfIndexPair2(itemSprite, cloneShelfIndexPairs, currentLayer);

					var layer = shelfIndexPair.Shelf.Layers[currentLayer];

					var item = Instantiate(LevelController.Instance.ItemPrefab, layer.transform);
					item.Init(itemSprite);
					layer.PlaceItemAtIndex(item, shelfIndexPair.Index);

					fixedItems.Add(item);
				}
			}
		}

		private List<ShelfIndexPair> cloneShelfIndexPairs;
		private int _maxItemPerLayer;
		private int currentLayer;

		private void PlaceItems(List<ShelfIndexPair> shelfIndexPairs)
		{
			CalculateMaxItemPerLayer();

			currentLayer = 0;
			int count = 0;
			cloneShelfIndexPairs = ShuffleList(shelfIndexPairs);

			int unknownItems = 0;
			if (LevelData.UnknownItem)
				unknownItems = Random.Range(6, 16);
			float probabilityIsUnknown = (float)unknownItems / LevelData.ItemTypes;

			while (_skinCollection._unplacedItems.Count > 2)
			{
				Sprite itemSprite = _skinCollection.GetNextUnplacedItem();
				Item item = null;

				for (int i = 0; i < 3; i++)
				{
					ShelfIndexPair shelfIndexPair =
						GetValidShelfIndexPair(itemSprite, cloneShelfIndexPairs, currentLayer);

					var layer = shelfIndexPair.Shelf.Layers[currentLayer];

					item = Instantiate(LevelController.Instance.ItemPrefab, layer.transform);
					item.Init(itemSprite);
					layer.PlaceItemAtIndex(item, shelfIndexPair.Index);
					count++;

					if (count < _maxItemPerLayer)
						continue;
					Fill(currentLayer);
					count = 0;
					currentLayer++;
					cloneShelfIndexPairs = ShuffleList(shelfIndexPairs);
				}

// anh chiến ăn cứt 
				if (unknownItems <= 0 || Random.value >= probabilityIsUnknown || item == null)
					continue;
				item.Renderer.material = LevelController.Instance.UnknownMaterial;
				unknownItems--;
			}

			// yield return null;
		}

		private void CalculateMaxItemPerLayer()
		{
			_maxItemPerLayer = 3 * Shelves.Length;
			int emptySpace = _maxItemPerLayer -
			                 Mathf.CeilToInt((float)LevelData.TotalItems / LevelData.LayerPerShelf);

			_maxItemPerLayer -= emptySpace;
		}

		public void ShuffleItems(List<Item> refreshItems)
		{
			List<Item> savingItems = new List<Item>();
			for (int i = 0; i < 2; i++)
			{
				Item item = refreshItems[0];
				savingItems.Add(item);
				refreshItems.RemoveAt(0);

				int count = 1;
				for (int j = refreshItems.Count - 1; j >= 0; j--)
				{
					if (refreshItems[j].Type != item.Type)
						continue;

					savingItems.Add(refreshItems[j]);
					refreshItems.RemoveAt(j);
					count++;
					if (count >= 3)
						break;
				}
			}

			PlaceItemsShuffle(shelfIndexPairs, refreshItems);
			PlaceLastTwoTypeShuffle(savingItems);
			if (LockedShelves != null && LockedShelves.Count > 0)
				FillLockedShelves();

			foreach (Shelf shelf in Shelves)
			{
				for (int i = shelf.Layers.Count - 1; i >= 0; i--)
				{
					var layer = shelf.Layers[i];
					layer.CheckShouldDestroy(false);
				}
			}

			foreach (var shelf in Shelves)
				shelf.RenderLayers();
		}

		private void PlaceItemsShuffle(List<ShelfIndexPair> shelfIndexPairs, List<Item> refreshItems)
		{
			currentLayer = 0;
			int count = 0;
			cloneShelfIndexPairs = ShuffleList(shelfIndexPairs);
			while (refreshItems.Count > 0)
			{
				for (int i = 0; i < 3; i++)
				{
					Item item = refreshItems[0];
					refreshItems.RemoveAt(0);

					ShelfIndexPair shelfIndexPair =
						GetValidShelfIndexPair(item.Sprite, cloneShelfIndexPairs, currentLayer);

					var layer = shelfIndexPair.Shelf.Layers[currentLayer];
					layer.PlaceItemAtIndex(item, shelfIndexPair.Index);
					count++;

					if (count < _maxItemPerLayer)
						continue;
					Fill(currentLayer);
					count = 0;
					currentLayer++;
					cloneShelfIndexPairs = ShuffleList(shelfIndexPairs);
				}
			}
		}

		private void PlaceLastTwoTypeShuffle(List<Item> remainItems)
		{
			fixedItems = new List<Item>();
			while (remainItems.Count > 0)
			{
				var item = remainItems[0];
				remainItems.RemoveAt(0);

				ShelfIndexPair shelfIndexPair =
					GetValidShelfIndexPair2(item.Sprite, cloneShelfIndexPairs, currentLayer);

				var layer = shelfIndexPair.Shelf.Layers[currentLayer];
				layer.PlaceItemAtIndex(item, shelfIndexPair.Index);

				fixedItems.Add(item);
			}
		}


		private ShelfIndexPair GetValidShelfIndexPair(Sprite itemSprite, List<ShelfIndexPair> shelfIndexPairs,
			int currentLayer)
		{
			ShelfIndexPair result = shelfIndexPairs[0];
			for (int i = 0; i < shelfIndexPairs.Count; i++)
			{
				var pair = shelfIndexPairs[i];
				Shelf shelf = pair.Shelf;
				ItemLayer layer = shelf.GetLayerAtIndex(currentLayer);
				if (layer.IsAlreadyHaveTwoOfThisType(itemSprite))
					continue;

				result = pair;
				shelfIndexPairs.RemoveAt(i);
				break;
			}

			return result;
		}

		private ShelfIndexPair GetValidShelfIndexPair2(Sprite itemSprite, List<ShelfIndexPair> shelfIndexPairs,
			int currentLayer)
		{
			ShelfIndexPair result = shelfIndexPairs[0];
			for (int i = 0; i < shelfIndexPairs.Count; i++)
			{
				var pair = shelfIndexPairs[i];
				Shelf shelf = pair.Shelf;
				if (shelf.IsLocked)
					continue;

				ItemLayer layer = shelf.GetLayerAtIndex(currentLayer);
				if (layer.IsAlreadyHaveTwoOfThisType(itemSprite))
					continue;

				result = pair;
				shelfIndexPairs.RemoveAt(i);
				break;
			}

			return result;
		}

		private void SetUpLocks()
		{
			var shelvesClone = new List<Shelf>(Shelves);
			LockedShelves = new List<Shelf>();

			for (int i = 0; i < LevelData.LockShelves; i++)
			{
				Shelf randomShelf = shelvesClone[Random.Range(0, shelvesClone.Count)];
				randomShelf.SetUpLock(LevelData.LockNumber);

				shelvesClone.Remove(randomShelf);
				LockedShelves.Add(randomShelf);
			}
		}

		private void Fill(int layerIndex)
		{
			foreach (var shelf in Shelves)
			{
				if (shelf.GetLayerAtIndex(layerIndex).ItemsCount > 0)
					continue;

				var item = GetItemFromLayer(layerIndex);
				shelf.Layers[layerIndex].PlaceItemAtIndex(item, Random.Range(0, 3));
			}
		}

		private Item GetItemFromLayer(int layerIndex)
		{
			foreach (var shelf in Shelves)
			{
				if (shelf.Layers.Count <= layerIndex || shelf.Layers[layerIndex].ItemsCount < 2)
					continue;

				var layer = shelf.Layers[layerIndex];
				for (int i = 0; i < ItemNumbEachLayer; i++)
				{
					if (layer.Items[i] == null)
						continue;
					var item = layer.Items[i];
					layer.RemoveItem(item);

					return item;
				}
			}

			return null;
		}

		private void FillLockedShelves()
		{
			List<Item> items = new List<Item>();
			foreach (var shelf in Shelves)
			{
				if (LockedShelves.Contains(shelf) || shelf.Layers.Count == 0 || shelf.FrontLayer.ItemsCount < 2)
					continue;

				foreach (var item in shelf.FrontLayer.GetAllItems())
				{
					if (fixedItems.Contains(item))
						continue;
					items.Add(item);
					break;
				}
			}

			foreach (var lockedShelf in LockedShelves)
			{
				ItemLayer frontLayer = lockedShelf.FrontLayer;

				for (int i = 0; i < ItemNumbEachLayer; i++)
				{
					if (items.Count == 0)
						return;

					if (frontLayer.Items[i] != null)
						continue;

					Item item = items[0];
					items.RemoveAt(0);
					ItemLayer prevLayer = item.Layer;

					prevLayer.RemoveItem(item);
					prevLayer.CheckShouldDestroy(false);

					frontLayer.PlaceItemAtIndex(item, i);
				}
			}
		}

		private static List<T> ShuffleList<T>(List<T> list)
		{
			return list.OrderBy(_ => Random.value).ToList();
		}


		[Serializable]
		public class ShelfIndexPair
		{
			public Shelf Shelf;
			public int Index;

			public ShelfIndexPair(Shelf shelf, int index)
			{
				Shelf = shelf;
				Index = index;
			}
		}
	}
}