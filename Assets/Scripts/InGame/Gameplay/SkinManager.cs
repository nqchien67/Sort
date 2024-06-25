using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace InGame.Gameplay
{
	public class SkinManager : SingletonCore<SkinManager>
	{
		[SerializeField] private GameObject[] _sortEffects;

		[HideInInspector] public List<Sprite> _unplacedItems;

		private List<Sprite> _notPriorityItems;
		private List<Sprite> _priorityItems;

		private int _totalItemTypes;
		private SpritesCollection SpritesCollection => SpritesCollection.Instance;

		private const string SkinFolder = "BackgroundSkins/";
		private Skin _backgroundSkin;

		protected override void Awake()
		{
			base.Awake();
			_backgroundSkin = SpritesCollection.BackgroundSkinInUse;
		}

		public void InitUnplacedItems(int totalItemTypes, int level)
		{
			_totalItemTypes = totalItemTypes;
			_notPriorityItems = SpritesCollection.SimpleItemSprites.ToList();
			if (level >= 20)
				_notPriorityItems.AddRange(SpritesCollection.DetailItemSprites);

			_unplacedItems = new List<Sprite>();
			AddPriorityItems();

			foreach (var itemSkin in SpritesCollection.Instance.GetUnlockedItems())
			{
				var sprite = SpritesCollection.Instance.GetItemSprite(itemSkin);
				if (!_unplacedItems.Contains(sprite))
					_notPriorityItems.Add(sprite);
			}

			while (_unplacedItems.Count < totalItemTypes)
				_unplacedItems.Add(GetRandomItem());
		}

		public void SetShelfAndBackgroundSkin(Shelf[] shelves)
		{
			if (_backgroundSkin.Id <= 1)
				return;

			Sprite bgSprite = Resources.Load<Sprite>(SkinFolder + (_backgroundSkin.Id + 1));
			GameObject.Find("BackgroundWall").GetComponent<Image>().sprite = bgSprite;

			foreach (var shelf in shelves)
			{
				Sprite sprite = GetShelfSkin(shelf.Renderer.sprite);
				shelf.Renderer.sprite = sprite;
			}
		}

		public Sprite GetShelfSkin(Sprite defaultSkin)
		{
			if (_backgroundSkin.Id <= 1)
				return defaultSkin;
			
			return Resources.Load<Sprite>($"{SkinFolder}{_backgroundSkin.Id + 1}/{defaultSkin.name}");
		}

		public GameObject GetSortEffect()
		{
			Skin effect = SpritesCollection.EffectInUse;
			return _sortEffects[effect.Id];
		}

		private void AddPriorityItems()
		{
			List<ItemSkin> priorityItems = SpritesCollection.PriorityItems;

			while (priorityItems.Count > 0)
			{
				var item = priorityItems[0];
				if (item is UnlockableItemSkin _)
				{
					_unplacedItems.Add(SpritesCollection.GetUnlockableItemSprite(item.Id));
				}
				else
				{
					if (item.InUse)
						_unplacedItems.Add(SpritesCollection.GetPurchasableItemSprite(item.Id));
				}

				priorityItems.RemoveAt(0);

				if (_unplacedItems.Count >= _totalItemTypes)
					break;
			}
		}

		public Sprite GetNextUnplacedItem()
		{
			if (_unplacedItems.Count == 0)
				return null;

			Sprite sprite = _unplacedItems[0];
			_unplacedItems.RemoveAt(0);
			return sprite;
		}

		private Sprite GetRandomItem()
		{
			List<Sprite> pool = _notPriorityItems;
			int randomIndex = Random.Range(0, pool.Count);
			var result = pool[randomIndex];
			pool.RemoveAt(randomIndex);

			return result;
		}
	}
}