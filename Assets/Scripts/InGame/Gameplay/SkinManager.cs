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
		private List<Sprite> _notPriorityItems;

		public List<Sprite> _unplacedItems;
		private List<Sprite> _priorityItems;
		private SpritesCollection SpritesCollection => SpritesCollection.Instance;

		private int _totalItemTypes;

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

			for (int i = 0; i < totalItemTypes; i++)
				_unplacedItems.Add(GetRandomItem());
		}

		public void SetShelfAndBackgroundSkin(Shelf[] shelves)
		{
			Skin backgroundSkin = SpritesCollection.BackgroundSkinInUse;
			if (backgroundSkin == null || backgroundSkin.Id == 0)
				return;

			const string skinFolder = "BackgroundSkins/";
			Sprite bgSprite = Resources.Load<Sprite>(skinFolder + (backgroundSkin.Id + 1));
			GameObject.Find("Background").GetComponent<Image>().sprite = bgSprite;

			foreach (var shelf in shelves)
			{
				string spriteName = shelf.Renderer.sprite.name;
				Sprite sprite = Resources.Load<Sprite>($"{skinFolder}{backgroundSkin.Id + 1}/{spriteName}");

				shelf.Renderer.sprite = sprite;
			}
		}

		private void AddPriorityItems()
		{
			List<ItemSkin> priorityItems = SpritesCollection.PriorityItems;

			while (priorityItems.Count > 0)
			{
				var item = priorityItems[0];
				if (item is UnlockableItemSkin _)
					_unplacedItems.Add(SpritesCollection.GetUnlockableItemSprite(item.Id));
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