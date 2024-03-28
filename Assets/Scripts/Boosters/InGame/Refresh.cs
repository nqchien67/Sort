using System.Collections.Generic;
using UnityEngine;

namespace Boosters
{
	public class Refresh : InGameBooster
	{
		protected override void Active()
		{
			if(!LevelController.CanDrag)
				return;
			
			LevelController.CanDrag = false;
			var items = GetAllItems();
			foreach (var i in items)
			{
				i.Layer.RemoveItem(i);
			}

			LevelController.ShuffleItems(items);
			LevelController.CanDrag = true;
		}

		private List<Item> GetAllItems()
		{
			List<Item> items = new List<Item>();
			foreach (var s in LevelController.Shelves)
			{
				items.AddRange(s.GetAllItems());
			}

			return items;
		}
	}
}