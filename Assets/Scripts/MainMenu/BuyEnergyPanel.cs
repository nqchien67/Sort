using System;
using Audio;
using Controllers;
using Data;
using InGame.UI;
using MainMenu.TopBar;
using TMPro;
using UI;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainMenu
{
	public class BuyEnergyPanel : Popup
	{
		public string adsName = "";
		[SerializeField] private AudioClip popUpClip;
		[SerializeField] private Button videoRewardBtn, refillBtn;
		[SerializeField] private GameObject _inGameBuyCoinPrefab;
		[SerializeField] private TextMeshProUGUI _energyCountText;
		[SerializeField] private TextMeshProUGUI _timeCooldownText;
		[SerializeField] private int _coinPrice = 500;
		[SerializeField] private Coin _coinHave;
		
		private UnityAction OnBuyCompleted;

		private void Start()
		{
			bool isReadyAds = true;

			videoRewardBtn.interactable = isReadyAds;
			videoRewardBtn.gameObject.SetActive(isReadyAds);
			refillBtn.interactable = true;

			if (EnergyController.Instance == null)
			{
				_timeCooldownText.gameObject.SetActive(false);
				_coinHave.gameObject.SetActive(true);
			}

			AudioController.Instance.PlaySfx(popUpClip);
		}

		private void Update()
		{
			if (EnergyController.Instance != null)
				_timeCooldownText.text = EnergyController.Instance.timeCountDown.text;

			_energyCountText.text = DataController.Instance.Energy.ToString();
		}

		public void Init(UnityAction onBuyCallback)
		{
			OnBuyCompleted = onBuyCallback;
			Show();
		}

		public void OnClickBuyEnergy()
		{
			if (DataController.Instance.Coin >= _coinPrice)
			{
				DataController.Instance.Coin -= _coinPrice;

				if (MainMenuUIController.Instance != null)
					MainMenuUIController.Instance.Coin.UpdateValue();

				DataController.Instance.IncreaseOneEnergy();
				DataController.Instance.SaveData();
				OnBuyCompleted?.Invoke();
				Close();
			}
			else
			{
				if (OnBuyCompleted != null) //OnBuyCompleted != null khi panel nay mo trong main menu :v
				{
					MainMenuUIController.Instance.OpenShop();
					MainMenuUIController.Instance.ShowNotEnoughCoin();
				}
				else if (LevelUIController.Instance != null)
				{
					LevelUIController.Instance.ShowNotEnoughCoin();
					LevelUIController.Instance.ShowCoinPackPanel(_coinPrice);
				}
			}
		}

		public void OnClickShowVideo()
		{
			Debug.Log("Show video reward");
			OnEarnReward();

			videoRewardBtn.interactable = false;
			refillBtn.interactable = false;
		}

		public void OnCloseAds()
		{
		}

		public void OnEarnReward()
		{
			DelayRewardPlayer();
		}

		private void DelayRewardPlayer()
		{
			DataController.Instance.IncreaseOneEnergy();
			DataController.Instance.SaveData();

			if (OnBuyCompleted != null)
			{
				OnBuyCompleted.Invoke();
			}

			Close();
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			Destroy(gameObject);
		}
	}
}