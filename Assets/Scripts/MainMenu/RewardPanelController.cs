using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using Data;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace MainMenu
{
	public class RewardPanelController : Popup
	{
		public string adsName = "";
		[SerializeField] private GameObject rewardPanel;
		[SerializeField] private Transform shopIcon, topRewardLayer, bottomRewardLayer;
		[SerializeField] private Button claimBtn, claim1Btn, videoRewardBtn;
		[SerializeField] private RewardItemController itemReward;
		[SerializeField] private GameObject itemEffectPrefabs;
		[SerializeField] private AudioClip popUpClip;
		private UnityAction onCloseCallback;
		private bool canClaimReward;
		private bool logEvent;
		private int _multiple = 1;
		private List<GameObject> goList;
		private List<ItemQuantityPair> _rewardQuantityPairs;
		private bool isWatchedAds;

		public void Init(int coinAmount,
			int unlimitEnergyTime,
			RewardType[] rewardTypes,
			int[] itemAmounts,
			bool logEvent = true,
			bool claimReward = true,
			bool canDouble = false,
			string adsName = null,
			int multiple = 1,
			UnityAction _closeCallBack = null) //maximun display 3 rewards
		{
			this.logEvent = logEvent;
			transform.SetAsLastSibling();
			_multiple = multiple;
			_rewardQuantityPairs = new List<ItemQuantityPair>();
			goList = new List<GameObject>();
			canClaimReward = true;
			// FindObjectOfType<MainMenuController>().setIndexTab(5);
			// AudioController.Instance.PlaySfx(popUpClip);
			onCloseCallback = _closeCallBack;
			if (coinAmount > 0)
			{
				_rewardQuantityPairs.Add(new ItemQuantityPair(RewardType.Coin, coinAmount));
				if (claimReward) IncreaseCoin(coinAmount, _multiple);
			}

			// if (_unlimitEnergyTime > 0)
			// {
			// 	tmpItemIds.Add(new ItemQuantityPair(100600, _unlimitEnergyTime));
			// 	if (_claimReward) IncreaseEnergy(_unlimitEnergyTime, multiple);
			// }

			if (rewardTypes.Length > 0)
				for (int i = 0; i < rewardTypes.Length; i++)
					if (itemAmounts[i] != 0)
					{
						_rewardQuantityPairs.Add(new ItemQuantityPair(rewardTypes[i], itemAmounts[i]));
						if (claimReward)
							// if (_itemIds[i] == 121212)
							// {
							// 	PlayerPrefs.SetInt("MAX_ENERGY", 8);
							// 	DataController.Instance.Energy = 8;
							// 	FindObjectOfType<EnergyController>().SetTextEnergyCount();
							// 	DataController.Instance.SaveData();
							// }
							// else
							// if (_itemIds[i] == 310000 || _itemIds[i] == 320000 || _itemIds[i] == 330000)
							// {
							// 	DataController.Instance.AddCustomerSkin(_itemIds[i]);
							// 	PlayerPrefs.SetInt("showoff_skin", 1);
							// }
							// else
							// {
						{
							// if (EnumConverter.TryConvertToBoosterType(rewardTypes[i], out var boosterType))
							// 	InCreaseBooster(boosterType, itemAmounts[i], _multiple);
						}
						// }
					}

			if (goList.Count > 0)
			{
				for (int i = 0; i < goList.Count; i++) Destroy(goList[i]);

				goList.Clear();
			}

			if (_rewardQuantityPairs.Count > 3)
			{
				bottomRewardLayer.gameObject.SetActive(true);
				int idHaflLeftList = _rewardQuantityPairs.Count / 2;
				for (int i = 0; i < idHaflLeftList; i++)
				{
					RewardItemController rewardItemController = Instantiate(itemReward, topRewardLayer);
					rewardItemController.Init(_rewardQuantityPairs[i].RewardType,
						_rewardQuantityPairs[i].Quantity * multiple);
					goList.Add(rewardItemController.gameObject);
				}

				for (int i = idHaflLeftList; i < _rewardQuantityPairs.Count; i++)
				{
					var go = Instantiate(itemReward, bottomRewardLayer);
					go.Init(_rewardQuantityPairs[i].RewardType, _rewardQuantityPairs[i].Quantity * multiple);
					goList.Add(go.gameObject);
				}
			}
			else
			{
				bottomRewardLayer.gameObject.SetActive(false);
				for (int i = 0; i < _rewardQuantityPairs.Count; i++)
				{
					var go = Instantiate(itemReward, topRewardLayer);
					go.Init(_rewardQuantityPairs[i].RewardType, _rewardQuantityPairs[i].Quantity * multiple);
					goList.Add(go.gameObject);
				}
			}

			this.adsName = adsName;
			// bool canShowVideo = _canDouble && AdsController.Instance.IsRewardVideoAdsReady();
			bool canShowVideo = true;
			claimBtn.gameObject.SetActive(!canShowVideo);
			claim1Btn.gameObject.SetActive(canShowVideo);
			videoRewardBtn.gameObject.SetActive(canShowVideo);
			videoRewardBtn.interactable = canShowVideo;
			rewardPanel.SetActive(true);
			rewardPanel.GetComponent<Animator>().Play("Appear");
			// if (canShowVideo)
			// {
			// 	APIController.Instance.LogEventShowAds(adsName);
			// }
			//
			// UIController.Instance.PushUitoStack(this);
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
			OnClickClaim(_multiple);
		}

		public void OnClickClaim(int multiple)
		{
			if (canClaimReward)
			{
				canClaimReward = false;
				for (int i = 0; i < _rewardQuantityPairs.Count; i++)
				{
					RewardType rewardType = _rewardQuantityPairs[i].RewardType;
					int quantity = _rewardQuantityPairs[i].Quantity;

					if (rewardType == RewardType.Coin)
					{
						if (multiple > 0)
							IncreaseCoin(quantity, multiple);
						MainMenuController.Instance.PlayClaimCoinEffect(transform.position);
					}

					// else if (tmpItemIds[i].Item == 100600)
					// {
					// FindObjectOfType<EnergyController>().StopAllCoroutines();
					// FindObjectOfType<EnergyController>().PlayUnlimitedEnergyEffect();
					// if (_multiple > 0) IncreaseEnergy(tmpItemIds[i].itemQuantity, _multiple);
					// }
					// else if (tmpItemIds[i].Item == 121212)
					// {
					// PlayerPrefs.SetInt("MAX_ENERGY", 8);
					// DataController.Instance.Energy = 8;
					// FindObjectOfType<EnergyController>().SetTextEnergyCount();
					// DataController.Instance.SaveData();
					// }
					// else if (tmpItemIds[i].Item == 310000)
					// {
					// DataController.Instance.AddCustomerSkin(310000);
					// PlayerPrefs.SetInt("showoff_skin", 1);
					// }
					else if (multiple > 0 &&
					         RewardHelper.TryConvertEnum(rewardType, out BoosterType boosterType))
					{
						ClaimBooster(multiple, rewardType, quantity);
					}
				}

				DataController.Instance.SaveData();
				Claim();
			}
		}

		private void ClaimBooster(int multiple, RewardType rewardType, int quantity)
		{
			for (int i = 0; i < quantity; i++)
			{
				BoosterType boosterType = (BoosterType)Enum.Parse(typeof(BoosterType),
					rewardType.ToString());

				InCreaseBooster(boosterType, quantity, multiple);
				MainMenuController.Instance.PlayClaimRewardEffect(rewardType, transform.position);
			}
		}

		public void IncreaseCoin(int coin, int _multiple)
		{
			coin *= _multiple;
			DataController.Instance.Coin += coin;
			// if (logEvent)
			// {
			// APIController.Instance.LogEventEarnGold(coin, "reward");
			//DWHLog.Log.ResourceLog(DataController.Instance.GetMaxPassedLevelToInt(), FlowType.Source, "reward_panel", "gold", "gold", _gold);
			// }
		}

		public void IncreaseEnergy(int _time, int _multiple)
		{
			// _time *= _multiple;
			// //FindObjectOfType<EnergyController>().StopAllCoroutines();
			// DataController.Instance.AddUnlimitedEnergy(_time);
			// // FindObjectOfType<EnergyController>().PlayUnlimitedEnergyEffect();
			// if (logEvent)
			// {
			// 	//DWHLog.Log.ResourceLog(DataController.Instance.GetMaxPassedLevelToInt(), FlowType.Source, "reward_panel", "unlimit energy", "unlimit energy", _time);
			// }
		}

		public void InCreaseBooster(BoosterType boosterType, int quantity, int multiple)
		{
			quantity *= multiple;
			DataController.Instance.AddBooster(boosterType, quantity);
			// if (logEvent)
			// {
			// 	//DWHLog.Log.ResourceLog(DataController.Instance.GetMaxPassedLevelToInt(), FlowType.Source, "reward_panel", "" + _itemId.ToString(), DataController.Instance.GetItemName(_itemId), _quantity);
			// }
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
			// UIController.Instance.PopUiOutStack();
			rewardPanel.GetComponent<Animator>().Play("Disappear");
			yield return new WaitForSeconds(0.2f);
			rewardPanel.SetActive(false);
			MainMenuController.Instance.DisplayMenuPanel();
		}

		private void HidePopup()
		{
			StartCoroutine(DelayHide());
		}

		public void OnHide()
		{
			// UIController.Instance.PopUiOutStack();
			if (canClaimReward)
				OnClickClaim(0);
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