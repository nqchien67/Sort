using System;
using Controllers;
using Data;
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
		public UnityAction OnBuyCompleted;

		private void Start()
		{
			// transform.SetSiblingIndex(MainMenuUIController.Instance.InitialTopBarSiblingIndex + 1);
			// bool isReadyAds = AdsController.Instance.IsRewardVideoAdsReady() &&
			//                   AdsController.Instance.CanShowAds(adsName);

			bool isReadyAds = true;
			if (isReadyAds)
			{
				// APIController.Instance.LogEventShowAds(adsName);
			}
			else
				refillBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -215f);

			videoRewardBtn.interactable = isReadyAds;
			videoRewardBtn.gameObject.SetActive(isReadyAds);
			refillBtn.interactable = true;
			AudioController.Instance.PlaySfx(popUpClip);
		}

		private void Update()
		{
			_energyCountText.text = EnergyController.Instance.energyCount.text;
			_timeCooldownText.text = EnergyController.Instance.timeCountDown.text;
		}

		public void Init(UnityAction onBuyCallback)
		{
			// Firebase.Analytics.FirebaseAnalytics.LogEvent("energy", "click", 1);
			OnBuyCompleted = onBuyCallback;
			Show();
		}

		public void OnClickBuyEnergy()
		{
			if (DataController.Instance.Coin >= _coinPrice)
			{
				// Firebase.Analytics.FirebaseAnalytics.LogEvent("energy", "fill", PlayerPrefs.GetInt("MAX_ENERGY", 5));
				DataController.Instance.Coin -= _coinPrice;
				MainMenuUIController.Instance.Coin.UpdateValue();

				// GSMController.Instance.LogResourceSpend(DataController.Instance.GetMaxPassedLevelToInt(), "ruby",
				// 	"energy_panel", 40);
				// APIController.Instance.LogEventSpentRuby(40, "buy_energy");

				DataController.Instance.IncreaseOneEnergy();
				DataController.Instance.SaveData();
				OnBuyCompleted?.Invoke();
				Close();
			}
			else
			{
				if (OnBuyCompleted != null)
					MainMenuUIController.Instance.OpenShop();
				else
					Instantiate(_inGameBuyCoinPrefab, transform.parent);
			}
		}


		public void OnClickShowVideo()
		{
			// GameController gameController = FindObjectOfType<GameController>();
			// if (gameController != null)
			// 	gameController.IsShowAds = true;
			// AdsController.Instance.ShowVideoReward(OnEarnReward, OnCloseAds, "energy_panel");
			// APIController.Instance.LogEventAdsClick("Energy");

			OnEarnReward();

			videoRewardBtn.interactable = false;
			refillBtn.interactable = false;
		}

		public void OnCloseAds()
		{
		}

		public void OnEarnReward()
		{
			// GSMController.Instance.AdsLogReward(DataController.Instance.GetMaxPassedLevelToInt(), "energy_panel");
			DelayRewardPlayer();
		}

		private void DelayRewardPlayer()
		{
			// AdsController.Instance.OnWatchAdsCompleted(adsName);
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