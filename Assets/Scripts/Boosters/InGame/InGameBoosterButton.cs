using System.Collections.Generic;
using System.Linq;
using Controllers;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Boosters.InGame
{
	public abstract class InGameBoosterButton : MonoBehaviour
	{
		public BoosterData Data;

		[SerializeField] private Image _icon;
		[HideInInspector] public Button Button;

		protected LevelController LevelController => LevelController.Instance;

		protected virtual void Awake()
		{
			_icon.sprite = Data.Sprite;
			Button = GetComponent<Button>();
			Button.onClick.AddListener(Active);
		}

		public abstract void Active();

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