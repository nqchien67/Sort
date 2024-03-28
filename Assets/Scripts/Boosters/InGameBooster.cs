using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Boosters
{
	public abstract class InGameBooster : MonoBehaviour
	{
		private Button _button;

		protected LevelController LevelController => LevelController.Instance;

		protected virtual void Awake()
		{
			_button = GetComponent<Button>();
			_button.onClick.AddListener(Active);
		}

		protected abstract void Active();
		
		protected List<Item> GetAllFrontItems()
		{
			List<Item> items = new List<Item>();

			foreach (var s in LevelController.Shelves)
				items.AddRange(s.FrontLayer.GetAllItems());
			return items;
		}
		
		protected List<Item> FindSameItems(Item baseItem)
		{
			List<Item> items = new List<Item>();

			foreach (var s in LevelController.Shelves)
			{
				foreach (var item in s.GetAllItems().Where(item => item.Type == baseItem.Type))
				{
					items.Add(item);
					if (items.Count >= 3)
						return items;
				}
			}

			return items;
		}
	}
}