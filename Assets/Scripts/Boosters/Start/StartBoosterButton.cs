using System;
using Controllers;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StartBooster = Boosters.StartBoosterController.StartBooster;

namespace Boosters.Start
{
	[RequireComponent(typeof(Button))]
	public class StartBoosterButton : MonoBehaviour
	{
		private bool _selected;
		private Image _image;
		[HideInInspector] public Button Button;

		[SerializeField] private StartBooster _booster;
		public BoosterData Data;
		[SerializeField] private Image _amountBoxImage;
		[SerializeField] private TextMeshProUGUI _amountText;
		[SerializeField] private Sprite _selectedBox;
		[SerializeField] private Sprite _selectedAmountBox;
		[SerializeField] private GameObject _lock;

		private Sprite _unselectedBox;
		private Sprite _unselectedAmountBox;
		public BoosterType boosterType;

		private int _quantity;

		private void Awake()
		{
			_image = GetComponent<Image>();
			Button = GetComponent<Button>();
			transform.Find("Icon").GetComponent<Image>().sprite = Data.Sprite;
		}

		private void Start()
		{
			if (!IsBoosterUnlocked())
			{
				_lock.SetActive(true);
				_amountBoxImage.gameObject.SetActive(false);
				return;
			}

			_unselectedBox = _image.sprite;
			_unselectedAmountBox = _amountBoxImage.sprite;
			Button.onClick.AddListener(Select);

			boosterType = _booster switch
			{
				StartBooster.HugeHammer => BoosterType.HugeHammer,
				StartBooster.Time => BoosterType.Time,
				StartBooster.DoublePoint => BoosterType.DoublePoint,
				_ => throw new ArgumentOutOfRangeException()
			};

			RefreshQuantityText();
		}

		public void Select()
		{
			if (_quantity <= 0)
				return;

			_selected = !_selected;

			if (_selected)
			{
				StartBoosterController.Instance.SelectBooster(_booster);
				_image.sprite = _selectedBox;
				_amountBoxImage.sprite = _selectedAmountBox;
			}
			else
			{
				StartBoosterController.Instance.DeselectBooster(_booster);
				_image.sprite = _unselectedBox;
				_amountBoxImage.sprite = _unselectedAmountBox;
			}

			_amountText.gameObject.SetActive(!_selected);
		}

		public void RefreshQuantityText()
		{
			_quantity = DataController.Instance.GetBoosterQuantity(boosterType);
			_amountText.text = _quantity.ToString();
			
			transform.DOScale(1.1f, 0.098f).SetLoops(2, LoopType.Yoyo);
		}

		private bool IsBoosterUnlocked()
		{
			int highestPassedLevel = PlayerPrefs.GetInt("level", 0);
			switch (_booster)
			{
				case StartBooster.HugeHammer:
					return highestPassedLevel >= 2;
				case StartBooster.Time:
					return highestPassedLevel >= 4;
				case StartBooster.DoublePoint:
					return highestPassedLevel >= 6;
			}

			return false;
		}
	}
}