using System.Collections;
using System.Linq;
using Controllers;
using Data;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.ShopSkin
{
	public class ShopSkinPanel : Popup
	{
		[SerializeField] private Scrollbar _scrollbar;
		[SerializeField] private TabButton[] _tabButtons;
		[SerializeField] private SkinGroup[] _skinGroups;
		[SerializeField] private float _scrollTime;
		[SerializeField] private int _buyPrice;

		[Header("Component")] [SerializeField] private GameObject _buyButtonsGroup;
		[SerializeField] private TextMeshProUGUI _priceText;

		public bool Switching { get; set; }
		private int _currentTabIndex;

		public SkinButton SelectingButton
		{
			get => _selectingButton;
			set
			{
				if (_selectingButton != null)
					_selectingButton.DeSelect();

				if (_selectingButton == null && value != null)
					ActiveBuyButtons(true);
				else if (_selectingButton != null && value == null)
					ActiveBuyButtons(false);

				_selectingButton = value;
			}
		}

		private SkinButton _selectingButton;

		private void Start()
		{
			InitTabButtons();
			InitSkinGroups();
			_scrollbar.value = 0;

			_buyButtonLayoutGroup = _buyButtonsGroup.GetComponent<HorizontalLayoutGroup>();
			_initialSpacing = _buyButtonLayoutGroup.spacing;
			_buyButtonLayoutGroup.spacing = Screen.width * 2;
			_priceText.text = _buyPrice.ToString();
		}

		private void InitTabButtons()
		{
			for (int i = 0; i < _tabButtons.Length; i++)
			{
				int tabIndex = i;
				_tabButtons[i].OnPressTab += () => OnPressTab(tabIndex);
			}
		}

		private void InitSkinGroups()
		{
			_skinGroups[0].InitSkinButtons(SpritesCollection.Instance.PurchasableItemSkins.ToList());
			_skinGroups[1].InitSkinButtons(SpritesCollection.Instance.BackgroundSkins.ToList());
			_skinGroups[2].InitSkinButtons(SpritesCollection.Instance.Effects.ToList());
		}

		private Tween _tween;

		private void OnPressTab(int tabIndex)
		{
			// if (Switching)
			// 	return;

			Switching = true;
			Scroll(tabIndex);

			for (int i = 0; i < _tabButtons.Length; i++)
			{
				if (i == tabIndex)
				{
					_tabButtons[i].Select();
					_skinGroups[i].Active(true);
				}
				else
				{
					_tabButtons[i].DeSelect();
					_skinGroups[i].Active(false);
				}
			}

			if (_currentTabIndex != tabIndex)
				SelectingButton = null;
		}

		public void OnClickBuy()
		{
			int coinHave = DataController.Instance.Coin;
			if (coinHave < _buyPrice)
			{
				MainMenuUIController.Instance.ShowNotEnoughCoin();
				MainMenuUIController.Instance.OpenShop();
				return;
			}

			SelectingButton.Unlock();

			DataController.Instance.Coin -= _buyPrice;
			MainMenuUIController.Instance.Coin.UpdateValue();

			DataController.Instance.SaveData();
		}

		public void OnLickAds()
		{
			Debug.Log("Show video ads reward");

			SelectingButton.Unlock();
			DataController.Instance.SaveData();
		}

		private void Scroll(int tabIndex)
		{
			float startValue = _scrollbar.value;
			float endValue = tabIndex / 2f;

			_tween?.Kill();
			float duration = Mathf.Abs(startValue - endValue) / _scrollTime;
			_tween = DOVirtual.Float(startValue, endValue, duration, value => _scrollbar.value = value)
				.SetEase(Ease.InOutQuad)
				.OnComplete(() => Switching = false);
		}

		private Coroutine _activeBuyButtonsTween;
		private HorizontalLayoutGroup _buyButtonLayoutGroup;
		private float _initialSpacing;

		private void ActiveBuyButtons(bool isActive)
		{
			if (_activeBuyButtonsTween != null)
				StopCoroutine(_activeBuyButtonsTween);

			StartCoroutine(isActive ? LerpBuyButtonsSpacing(_initialSpacing) : LerpBuyButtonsSpacing(Screen.width * 2));
		}

		private IEnumerator LerpBuyButtonsSpacing(float newSpacing)
		{
			_buyButtonsGroup.GetComponent<CanvasGroup>().interactable = false;

			float lerpDuration = 0.04f;
			float elapsedTime = 0;

			float startValue = _buyButtonLayoutGroup.spacing;
			while (elapsedTime < lerpDuration)
			{
				float spacing = Mathf.Lerp(startValue, newSpacing, elapsedTime / lerpDuration);
				_buyButtonLayoutGroup.spacing = spacing;

				elapsedTime += Time.deltaTime;
				yield return null;
			}

			_buyButtonLayoutGroup.spacing = newSpacing;
			_buyButtonsGroup.GetComponent<CanvasGroup>().interactable = true;
		}

		public override void Show()
		{
			gameObject.SetActive(true);
			_animator.Play("Appear");
		}

		public override void Close()
		{
			_animator.Play("Disappear");
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			gameObject.SetActive(false);
		}
	}
}