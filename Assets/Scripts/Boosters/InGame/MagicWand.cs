using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Data;
using DG.Tweening;
using DigitalRuby.LightningBolt;
using InGame.Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boosters.InGame
{
	public class MagicWand : InGameBoosterButton
	{
		[SerializeField] private Sprite _transformedSprite;
		[SerializeField] private LightningBoltScript _lightningPrefab;
		[SerializeField] private GameObject _hitEffect;
		private bool _animCompleted;

		private int _countLayer;
		private int time;
		private List<Item> _effectedItems;
		private SkeletonAnimation _effectSkeletonAnimation;

		private HashSet<string> _typesInLock;

		public override void Use()
		{
			int maxLayerCount = 0;

			foreach (var shelf in LevelController.Shelves)
			{
				int layersCount = shelf.Layers.Count;
				if (layersCount > maxLayerCount)
					maxLayerCount = layersCount;
			}

			_typesInLock = GetAllTypeInLocks();

			List<Item> uniqueItems = new List<Item>();
			_effectedItems = new List<Item>();

			for (int i = 0; i < 3; i++)
			{
				if (uniqueItems.Count == 0)
				{
					List<Item> items = GetAllItemNotInLock(maxLayerCount);
					if (items.Count > 0)
						uniqueItems = RemoveDuplicate(items);
				}

				if (uniqueItems.Count == 0)
				{
					var items = GetAllItemsInLocks(maxLayerCount);
					if (items.Count > 0)
						uniqueItems = RemoveDuplicate(items);
				}

				if (uniqueItems.Count == 0)
					break;

				int randomIndex = Random.Range(0, uniqueItems.Count);
				Item randomItem = uniqueItems[randomIndex];
				uniqueItems.RemoveAt(randomIndex);

				_effectedItems.AddRange(FindSameItems(randomItem));
			}

			if (_effectedItems.Count == 0)
				return;
			
			base.Use();
			ReduceQuantity();
			StartCoroutine(TransformItems(_effectedItems));
			time++;
		}

		private List<Item> GetAllItemNotInLock(int maxLayerCount)
		{
			_countLayer = 0;
			List<Item> items;
			do
			{
				items = new List<Item>();

				foreach (var shelf in LevelController.Shelves)
				{
					if (shelf.IsLocked)
						continue;
					items.AddRange(GetItemsInShelf(shelf, _countLayer));
				}

				if (items.Count != 0)
					continue;

				_countLayer++;
				if (_countLayer >= maxLayerCount)
					break;
			} while (items.Count == 0);

			return items;
		}

		private List<Item> GetAllItemsInLocks(int maxLayerCount)
		{
			_countLayer = 0;
			List<Item> items;
			do
			{
				items = new List<Item>();

				foreach (var shelf in LevelController.LockedShelves)
					items.AddRange(GetItemsInShelf(shelf, _countLayer));

				if (items.Count != 0)
					continue;

				_countLayer++;
				if (_countLayer >= maxLayerCount)
					break;
			} while (items.Count == 0);

			return items;
		}

		private HashSet<string> GetAllTypeInLocks()
		{
			HashSet<string> result = new HashSet<string>();

			foreach (var lockShelf in LevelController.Instance.LockedShelves)
			{
				List<Item> items = lockShelf.GetAllItems();
				foreach (var i in items)
					result.Add(i.Type);
			}

			return result;
		}

		protected override Transform SpawnEffect()
		{
			Vector2 spawnPos = CameraController.Camera.ViewportToWorldPoint(new Vector2(0.5f, 0.2f));
			if (_spawnedEffect == null)
			{
				_spawnedEffect = Instantiate(_effectPrefab, spawnPos, Quaternion.identity);
				_effectSkeletonAnimation = _spawnedEffect.GetComponentInChildren<SkeletonAnimation>();

				_effectSkeletonAnimation.AnimationState.Complete += entry => _animCompleted = true;
				return _spawnedEffect;
			}

			_spawnedEffect.gameObject.SetActive(true);
			_effectSkeletonAnimation.AnimationState.SetAnimation(0, "animation", false);
			_spawnedEffect.transform.position = spawnPos;
			return _spawnedEffect;
		}

		protected override bool CanUse()
		{
			return LevelController.CanDrag && !LevelController.AnyItemMoving;
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

		private IEnumerable<Item> GetItemsInShelf(Shelf shelf, int x)
		{
			var layers = shelf.Layers;
			int layerIndex = layers.Count - 1 - x;
			layerIndex = Mathf.Max(0, layerIndex);

			var foundItems = layers[layerIndex].GetAllItems()
				.Where(item => item.Type != "_CuLac" && !_effectedItems.Contains(item));
			return foundItems;
		}

		private IEnumerator TransformItems(List<Item> items)
		{
			LevelController.CanDrag = false;

			SpawnEffect();
			yield return new WaitForSeconds(0.3f);

			for (int i = 0; i < items.Count; i++)
			{
				Item item = items[i];
				item.SetSprite(_transformedSprite);
				item.gameObject.name = "CULAC " + time;
			}

			yield return CastLightnings(items, _spawnedEffect.gameObject);
			yield return new WaitUntil(() => _animCompleted);

			Vector2 movePos = new Vector2(CameraController.BottomLeft.x - 3, _spawnedEffect.position.y);
			yield return _spawnedEffect.DOMove(movePos, 0.5f).WaitForCompletion();

			_effectSkeletonAnimation.AnimationState.ClearTracks();
			_spawnedEffect.gameObject.SetActive(false);


			LevelController.CanDrag = true;
		}

		private YieldInstruction CastLightnings(List<Item> items, GameObject startObject)
		{
			const float duration = 0.45f;
			for (int i = 0; i < items.Count; i++)
			{
				LightningBoltScript lightning = Instantiate(_lightningPrefab);
				lightning.StartObject = startObject.gameObject;
				lightning.EndObject = items[i].gameObject;

				Instantiate(_hitEffect, items[i].transform.position, Quaternion.identity);

				Destroy(lightning.gameObject, duration);
			}

			return new WaitForSeconds(duration);
		}

		protected override bool IsBoosterUnlocked()
		{
			int currentLevel = LevelController.Instance.LevelIndex;
			return currentLevel > 6;
		}
	}
}