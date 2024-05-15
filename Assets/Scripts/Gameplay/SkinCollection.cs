using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
	public class SkinCollection : SingletonCore<SkinCollection>
	{
		private List<Sprite> _notPriorityItems;

		public List<Sprite> _unplacedItems;
		private List<Sprite> _priorityItems;
		private SkinDataController SkinDataController => SkinDataController.Instance;

		private int _totalItemTypes;

		public void InitUnplacedItems(int totalItemTypes, int level)
		{
			_totalItemTypes = totalItemTypes;
			_notPriorityItems = SkinDataController.SimpleItemSprites.ToList();
			if (level >= 20)
				_notPriorityItems.AddRange(SkinDataController.DetailItemSprites);

			_unplacedItems = new List<Sprite>();
			AddPriorityItems();

			foreach (var itemSkin in SkinDataController.Instance.GetUnlockedItems())
			{
				var sprite = SkinDataController.Instance.GetItemSprite(itemSkin);
				if (!_unplacedItems.Contains(sprite))
					_notPriorityItems.Add(sprite);
			}

			for (int i = 0; i < totalItemTypes; i++)
				_unplacedItems.Add(GetRandomItem());
		}

		private void AddPriorityItems()
		{
			List<ItemSkin> priorityItems = SkinDataController.PriorityItems;

			while (priorityItems.Count > 0)
			{
				var item = priorityItems[0];
				if (item is UnlockableItemSkin _)
					_unplacedItems.Add(SkinDataController.GetUnlockableItemSprite(item.Id));
				else
				{
					if (item.InUse)
						_unplacedItems.Add(SkinDataController.GetPurchasableItemSprite(item.Id));
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