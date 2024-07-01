using Boosters;
using Controllers;
using Data;
using MainMenu;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InGame.UI
{
	public class BuyBoosterPanel : Popup
	{
		private BoosterData Data;

		[SerializeField] private Image _boosterIcon;
		[SerializeField] private TextMeshProUGUI _description;
		[SerializeField] private TextMeshProUGUI _coinPriceText;
		public UnityAction OnBuy;

		private int _buyPrice;

		private int BoughtTimes
		{
			get => PlayerPrefs.GetInt("bought_" + Data.name, 0);
			set => PlayerPrefs.SetInt("bought_" + Data.name, value);
		}

		public void Show(BoosterData data, UnityAction onBuy)
		{
			Data = data;
			_boosterIcon.sprite = data.Sprite;
			_description.text = data.Description;

			_buyPrice = 500 + BoughtTimes * 50;
			_buyPrice = Mathf.Min(_buyPrice, 1550);
			_coinPriceText.text = _buyPrice.ToString();
			OnBuy += onBuy;

			base.Show();
			Time.timeScale = 0;
		}

		public void OnClickBuy()
		{
			int coinHave = DataController.Instance.Coin;
			if (coinHave < _buyPrice)
			{
				LevelUIController.Instance.ShowNotEnoughCoin();
				LevelUIController.Instance.ShowCoinPackPanel(_buyPrice - coinHave);
				return;
			}

			DataController.Instance.Coin -= _buyPrice;

			IncreaseBooster();
			Close();
		}

		public void OnClickAds()
		{
			Debug.Log("Show video ads reward");
			IncreaseBooster();
			Close();
		}

		private void IncreaseBooster()
		{
			DataController.Instance.AddBooster(Data.Type, 1);
			DataController.Instance.SaveData();

			BoughtTimes++;
			OnBuy?.Invoke();
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			Time.timeScale = 1;
			Destroy(gameObject);
		}
	}
}