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
using Utilities;

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

		[SerializeField] private AudioClip _sfx;
		[SerializeField] private float _delaySfx;

		protected virtual void Awake()
		{
			_icon.sprite = Data.Sprite;
			Button = GetComponent<Button>();
			_quantityText = GetComponentInChildren<TextMeshProUGUI>();
			Button.onClick.AddListener(OnClick);
		}

		private void Start()
		{
			RefreshQuantity();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Q) && this is LittleHammer)
				OnClick();

			if (Input.GetKeyDown(KeyCode.W) && this is MagicWand)
				OnClick();

			if (Input.GetKeyDown(KeyCode.E) && this is Freeze)
				OnClick();

			if (Input.GetKeyDown(KeyCode.R) && this is Refresh)
				OnClick();
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
			StartCoroutine(CommonIEnumerator.WaiForSeconds(_delaySfx, () => AudioController.Instance.PlaySfx(_sfx)));
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
			DataController.Instance.SaveData();

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

		private void Lock()
		{
			_lock.SetActive(true);
			_quantityText.gameObject.SetActive(false);
			_buyIcon.gameObject.SetActive(false);
			Button.interactable = false;
		}

		public void Unlock()
		{
			_lock.SetActive(false);
			_quantityText.gameObject.SetActive(true);
			_buyIcon.gameObject.SetActive(false);
			Button.interactable = true;
			RefreshQuantity();
		}
	}
}