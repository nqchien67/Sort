using System;
using Data;
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

		private Sprite _unselectedBox;
		private Sprite _unselectedAmountBox;
		public ConsumableType consumableType;

		private void Awake()
		{
			_image = GetComponent<Image>();
			Button = GetComponent<Button>();
			transform.Find("Icon").GetComponent<Image>().sprite = Data.Sprite;
		}

		private void Start()
		{
			_unselectedBox = _image.sprite;
			_unselectedAmountBox = _amountBoxImage.sprite;
			Button.onClick.AddListener(Select);

			consumableType = _booster switch
			{
				StartBooster.HugeHammer => ConsumableType.HugeHammer,
				StartBooster.Time => ConsumableType.Time,
				StartBooster.DoublePoint => ConsumableType.DoublePoint,
				_ => throw new ArgumentOutOfRangeException()
			};

			RefreshAmountText();
		}

		public void Select()
		{
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

		public void RefreshAmountText()
		{
			_amountText.text = DataController.Instance.GetItemQuantity(consumableType).ToString();
		}
	}
}