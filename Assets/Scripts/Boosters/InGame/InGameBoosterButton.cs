using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Data;
using Gameplay;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Boosters.InGame
{
	public abstract class InGameBoosterButton : MonoBehaviour
	{
		public BoosterData Data;

		[SerializeField] private Image _icon;
		[HideInInspector] public Button Button;
		private TextMeshProUGUI _quantityText;
		[SerializeField] protected Transform _effectPrefab;

		[SerializeField] private Image _buyIcon;
		[SerializeField] private GameObject _lock;
		protected Transform _spawnedEffect;

		protected LevelController LevelController => LevelController.Instance;
		protected int _quantity;

		protected virtual void Awake()
		{
			_icon.sprite = Data.Sprite;
			Button = GetComponent<Button>();
			_quantityText = GetComponentInChildren<TextMeshProUGUI>();
			Button.onClick.AddListener(Active);
		}

		private void Start()
		{
			if (!IsBoosterUnlocked())
			{
				_lock.SetActive(true);
				_quantityText.gameObject.SetActive(false);
				_buyIcon.gameObject.SetActive(false);
				Button.interactable = false;
				return;
			}

			RefreshQuantity();
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

		protected abstract Transform SpawnEffect();

		protected void ReduceQuantity()
		{
			_quantity--;
			DataController.Instance.AddBooster(Data.Type, -1);
			_quantityText.text = _quantity.ToString();
		}

		public void RefreshQuantity()
		{
			_quantity = DataController.Instance.GetBoosterQuantity(Data.Type);
			_quantityText.text = _quantity.ToString();

			_buyIcon.gameObject.SetActive(_quantity <= 0);
		}

		protected abstract bool IsBoosterUnlocked();
	}
}