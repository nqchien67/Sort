using System.Collections;
using System.Collections.Generic;
using Audio;
using Controllers;
using Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainMenu
{
	public class RewardPanelController : Popup
	{
		public string adsName = "";
		[SerializeField] private GameObject rewardPanel;
		[SerializeField] private Transform shopIcon, topRewardLayer, middleRewardLayer, bottomRewardLayer;
		[SerializeField] private Button claimBtn, claim1Btn, videoRewardBtn;
		[SerializeField] private RewardItemController itemReward;
		[SerializeField] private GameObject itemEffectPrefabs;
		private UnityAction onCloseCallback;
		private bool canClaimReward;
		private bool logEvent;
		private int _multiple = 1;
		private List<GameObject> goList;
		private List<ItemQuantityPair> _rewardQuantityPairs;
		private bool isWatchedAds;

		public void Init(
			RewardType[] rewardTypes,
			int[] rewardQuantities,
			bool logEvent = true,
			bool canDouble = false,
			string adsName = null,
			int multiple = 1,
			UnityAction _closeCallBack = null) //maximun display 3 rewards
		{
			this.logEvent = logEvent;
			transform.SetAsLastSibling();
			_multiple = multiple;
			goList = new List<GameObject>();
			canClaimReward = true;
			onCloseCallback = _closeCallBack;

			_rewardQuantityPairs = new List<ItemQuantityPair>();
			if (rewardTypes.Length > 0)
				for (int i = 0; i < rewardTypes.Length; i++)
					_rewardQuantityPairs.Add(new ItemQuantityPair(rewardTypes[i], rewardQuantities[i]));

			if (goList.Count > 0)
			{
				for (int i = 0; i < goList.Count; i++) Destroy(goList[i]);

				goList.Clear();
			}

			if (_rewardQuantityPairs.Count > 3)
			{
				bottomRewardLayer.gameObject.SetActive(true);
				int topRewardCount = _rewardQuantityPairs.Count / 3;
				int middleRewardCount = _rewardQuantityPairs.Count * (2 / 3);
				for (int i = 0; i < topRewardCount; i++) 
					SpawnRewardItem(i, multiple, topRewardLayer);

				for (int i = topRewardCount; i < middleRewardCount; i++)
					SpawnRewardItem(i, multiple, middleRewardLayer);

				for (int i = topRewardCount; i < _rewardQuantityPairs.Count; i++)
					SpawnRewardItem(i, multiple, bottomRewardLayer);
			}
			else
			{
				bottomRewardLayer.gameObject.SetActive(false);
				for (int i = 0; i < _rewardQuantityPairs.Count; i++)
				{
					var go = Instantiate(itemReward, middleRewardLayer);
					go.Init(_rewardQuantityPairs[i].RewardType, _rewardQuantityPairs[i].Quantity * multiple);
					goList.Add(go.gameObject);
				}
			}

			this.adsName = adsName;
			// bool canShowVideo = canDouble && AdsController.Instance.IsRewardVideoAdsReady();
			bool canShowVideo = canDouble;
			claimBtn.gameObject.SetActive(!canShowVideo);
			claim1Btn.gameObject.SetActive(canShowVideo);
			videoRewardBtn.gameObject.SetActive(canShowVideo);
			videoRewardBtn.interactable = canShowVideo;
			rewardPanel.SetActive(true);
			rewardPanel.GetComponent<Animator>().Play("Appear");
			AudioController.Instance.PlaySfx(_showSfx);
		}

		private void SpawnRewardItem(int index, int multiple, Transform parent)
		{
			RewardItemController rewardItemController = Instantiate(itemReward, parent);
			rewardItemController.Init(_rewardQuantityPairs[index].RewardType,
				_rewardQuantityPairs[index].Quantity * multiple);
			goList.Add(rewardItemController.gameObject);
		}

		public void OnClickWatchAds()
		{
			// AdsController.Instance.ShowVideoReward(OnEarnReward, OnCloseAds, "reward_panel");
			// APIController.Instance.LogEventAdsClick("Unlock_Res");
		}

		public void OnCloseAds()
		{
			if (isWatchedAds) isWatchedAds = false;
		}

		public void OnEarnReward()
		{
			isWatchedAds = true;
			StartCoroutine(DelayRewardPlayer());
			// GSMController.Instance.AdsLogReward(DataController.Instance.GetMaxPassedLevelToInt(), "reward_panel");
		}

		private IEnumerator DelayRewardPlayer()
		{
			yield return new WaitForSeconds(0.1f);
			// multiple * 2;
			// AdsController.Instance.OnWatchAdsCompleted(adsName);
			OnClickClaim();
		}

		public void OnClickClaim()
		{
			if (canClaimReward)
			{
				canClaimReward = false;
				for (int i = 0; i < _rewardQuantityPairs.Count; i++)
				{
					RewardType rewardType = _rewardQuantityPairs[i].RewardType;
					int quantity = _rewardQuantityPairs[i].Quantity;

					if (rewardType == RewardType.Coin)
						ClaimCoin(quantity);
					else if (rewardType == RewardType.Energy)
						ClaimEnergy(quantity);
					else if (RewardHelper.TryConvertRewardToBooster(rewardType, out BoosterType boosterType))
						ClaimBooster(quantity, boosterType, rewardType);
				}

				DataController.Instance.SaveData();
				Claim();
			}
		}

		private void ClaimBooster(int quantity, BoosterType boosterType, RewardType rewardType)
		{
			for (int j = 0; j < quantity; j++)
			{
				DataController.Instance.AddBooster(boosterType, quantity);
				if (MainMenuController.Instance != null)
					MainMenuController.Instance.PlayClaimRewardEffect(rewardType, transform.position);
			}
		}

		private void ClaimCoin(int coin)
		{
			coin *= _multiple;
			DataController.Instance.Coin += coin;
			if (MainMenuController.Instance != null)
				MainMenuController.Instance.PlayClaimCoinEffect(transform.position);
		}

		private void ClaimEnergy(int minutes)
		{
			minutes *= _multiple;
			DataController.Instance.AddUnlimitedEnergy(minutes);
			if (EnergyController.Instance != null)
				EnergyController.Instance.PlayUnlimitedEnergyEffect();
		}


		private void Claim()
		{
			for (int i = 0; i < goList.Count; i++) Destroy(goList[i]);

			goList.Clear();
			_multiple = 1;
			HidePopup();
			onCloseCallback?.Invoke();
		}

		private IEnumerator DelayHide()
		{
			rewardPanel.GetComponent<Animator>().Play("Disappear");
			yield return new WaitForSeconds(0.2f);
			rewardPanel.SetActive(false);

			if (MainMenuController.Instance != null)
				MainMenuController.Instance.DisplayMenuPanel();
		}

		private void HidePopup()
		{
			StartCoroutine(DelayHide());
		}

		public void OnHide()
		{
			if (canClaimReward)
				OnClickClaim();
			else
				HidePopup();
		}
	}

	public struct ItemQuantityPair
	{
		public RewardType RewardType;
		public int Quantity;

		public ItemQuantityPair(RewardType rewardType, int quantity)
		{
			Quantity = quantity;
			RewardType = rewardType;
		}
	}
}