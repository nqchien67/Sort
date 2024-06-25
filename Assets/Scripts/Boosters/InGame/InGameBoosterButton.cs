using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Controllers;
using Data;
using InGame.Gameplay;
using InGame.UI;
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
		[SerializeField] private BuyBoosterPanel _boosterPanelPrefab;

		protected LevelController LevelController => LevelController.Instance;
		protected int _quantity;

		protected virtual void Awake()
		{
			_icon.sprite = Data.Sprite;
			Button = GetComponent<Button>();
			_quantityText = GetComponentInChildren<TextMeshProUGUI>();
			Button.onClick.AddListener(OnClick);
		}

		private void Start()
		{
			// if (!IsBoosterUnlocked())
			// {
			// 	_lock.SetActive(true);
			// 	_quantityText.gameObject.SetActive(false);
			// 	_buyIcon.gameObject.SetActive(false);
			// 	Button.interactable = false;
			// 	return;
			// }

			RefreshQuantity();
		}

		public void OnClick()
		{
			if (_quantity > 0)
			{
				if (CanUse())
					Use();
			}
			else
			{
				var buyBoosterPanel = LevelUIController.Instance.SpawnBuyBoosterPanel();
				buyBoosterPanel.Show(Data, RefreshQuantity);
			}
		}

		public virtual void Use()
		{
			AudioController.Instance.PlaySfx(LevelController.Instance.UseBoosterSfx);
		}

		public List<Item> GetAllFrontItems()
		{
			List<Item> items = new List<Item>();

			foreach (var s in LevelController.Shelves)
				items.AddRange(s.FrontLayer.GetAllItems());
			return items;
		}

		public List<Item> FindSameItems(Item baseItem)
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
		protected abstract bool CanUse();

		protected void ReduceQuantity()
		{
			_quantity--;
			DataController.Instance.AddBooster(Data.Type, -1);
			_quantityText.text = _quantity.ToString();
			_buyIcon.gameObject.SetActive(_quantity <= 0);
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