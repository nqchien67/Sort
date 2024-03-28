using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using DG.Tweening;
using DigitalRuby.LightningBolt;
using UnityEngine;

namespace Boosters
{
	public class MagicWand : InGameBooster
	{
		[SerializeField] private Sprite _transformedSprite;
		[SerializeField] private LightningBoltScript _lightningPrefab;
		[SerializeField] private Transform _magicWandPrefab;

		private int _x;
		private int time;
		private List<Item> _effectedItems;

		protected override void Active()
		{
			if (!LevelController.CanDrag)
				return;
			
			_x = 0;

			int maxLayerCount = 0;

			foreach (var shelf in LevelController.Shelves)
			{
				int layersCount = shelf.Layers.Count;
				if (layersCount > maxLayerCount)
					maxLayerCount = layersCount;
			}

			List<Item> uniqueItems = new List<Item>();
			_effectedItems = new List<Item>();

			for (int i = 0; i < 3; i++)
			{
				if (uniqueItems.Count == 0)
				{
					while (true)
					{
						List<Item> items = GetAllNotCuLacItems();
						if (items.Count == 0)
						{
							_x++;
							if (_x >= maxLayerCount)
								break;
							continue;
						}

						if (items.Count > 0)
							uniqueItems = RemoveDuplicate(items);
						break;
					}
				}

				if (uniqueItems.Count == 0)
					return;
				int randomIndex = Random.Range(0, uniqueItems.Count);
				Item randomItem = uniqueItems[randomIndex];
				uniqueItems.RemoveAt(randomIndex);

				_effectedItems.AddRange(FindSameItems(randomItem));
			}

			StartCoroutine(TransformItems(_effectedItems));
			time++;
		}

		private List<Item> RemoveDuplicate(List<Item> items)
		{
			Dictionary<string, Item> uniqueItems = new Dictionary<string, Item>();

			foreach (Item item in items)
			{
				if (!uniqueItems.ContainsKey(item.Type))
					uniqueItems.Add(item.Type, item);
			}

			return new List<Item>(uniqueItems.Values);
		}

		private List<Item> GetAllNotCuLacItems()
		{
			List<Item> items = new List<Item>();

			foreach (var shelf in LevelController.Shelves)
			{
				var layers = shelf.Layers;
				int layerIndex = layers.Count - 1 - _x;
				layerIndex = Mathf.Max(0, layerIndex);

				items.AddRange(layers[layerIndex].GetAllItems()
					.Where(item => item.Type != "_CuLac" && !_effectedItems.Contains(item)));
			}

			return items;
		}

		private IEnumerator TransformItems(List<Item> items)
		{
			LevelController.CanDrag = false;
			
			Transform magicWand = SpawnMagicWand();
			yield return magicWand.DOMoveX(0, 0.3f).WaitForCompletion();
			for (int i = 0; i < items.Count; i++)
			{
				LightningBoltScript lightning = Instantiate(_lightningPrefab);
				lightning.StartObject = magicWand.gameObject;
				lightning.EndObject = items[i].gameObject;

				Destroy(lightning.gameObject, 0.5f);
			}

			yield return new WaitForSeconds(0.5f);
			Destroy(magicWand.gameObject);

			for (int i = 0; i < items.Count; i++)
			{
				Item item = items[i];
				item.SetSprite(_transformedSprite);
				item.gameObject.name = "CULAC " + time;
			}

			LevelController.CanDrag = true;
		}

		private Transform SpawnMagicWand()
		{
			Vector2 spawnPos = CameraController.Camera.ViewportToWorldPoint(new Vector2(1.2f, 0.2f));
			return Instantiate(_magicWandPrefab, spawnPos, Quaternion.identity);
		}
	}
}