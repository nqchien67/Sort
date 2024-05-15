using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using MainMenu;
using Menu;
using UnityEngine;
using TMPro;


public class LuckySpinItem : MonoBehaviour
{
	public float probability;
	public float rotationZ;
	public int coinAmount, unlimitEnergyAmount;
	[SerializeField] private List<ConsumableType> _consumables;
	[SerializeField] private List<int> _consumableQuantities;
	private int multiple;
	readonly int time = 0;

	private void OnValidate()
	{
		var amountText = GetComponentInChildren<TextMeshProUGUI>(true);

		if (coinAmount > 0)
			amountText.text = "x" + coinAmount;
		else if (_consumableQuantities.Count > 0)
			amountText.text = "x" + _consumableQuantities[0];
	}

	public void IncreaseRewardValue()
	{
		var amountText = GetComponentInChildren<TextMeshProUGUI>(true);

		if (coinAmount > 0)
		{
			coinAmount += 20;
			amountText.text = "x" + coinAmount;
		}
		else if (_consumableQuantities.Count > 0)
		{
			_consumableQuantities[0] *= 2;
			amountText.text = "x" + _consumableQuantities[0];
		}

		amountText.transform.DOScale(new Vector3(1.15f, 1.15f, 1), 0.3f).SetLoops(-1, LoopType.Yoyo)
			.SetEase(Ease.InOutSine);
	}

	public void OnCollect()
	{
		if (coinAmount > 0)
		{
			//DWHLog.Log.ResourceLog(DataController.Instance.GetMaxPassedLevelToInt(), FlowType.Source, "lucky_spin", "coin", "coin", coinAmount);
			// APIController.Instance.LogEventEarnGold(coinAmount, "lucky_spin");
			DataController.Instance.Coin += coinAmount;
		}

		if (unlimitEnergyAmount > 0)
		{
			//DWHLog.Log.ResourceLog(DataController.Instance.GetMaxPassedLevelToInt(), FlowType.Source, "lucky_spin", "unlimitEnergy", "unlimitEnergy", unlimitEnergyAmount);
			// DataController.Instance.AddUnlimitedEnergy(unlimitEnergyAmount * _multiple);
		}

		if (_consumables.Count > 0)
		{
			// for (int i = 0; i < itemsId.Count; i++)
			// {
			//     if (itemsAmount[i] != 0)
			//     {
			//         if (itemsId[i] == 121212)
			//         {
			//             PlayerPrefs.SetInt("MAX_ENERGY", 8);
			//             DataController.Instance.Energy = 8;
			//             FindObjectOfType<EnergyController>().SetTextEnergyCount();
			//             DataController.Instance.SaveData();
			//         }
			//         else if (itemsId[i] == 310000 || itemsId[i] == 320000 || itemsId[i] == 330000)
			//         {
			//             DataController.Instance.AddCustomerSkin(itemsId[i]);
			//             PlayerPrefs.SetInt("showoff_skin", 1);
			//         }
			//         else
			//         {
			//             DataController.Instance.AddItem(itemsId[i], itemsAmount[i] * _multiple);
			//         }
			//     }
			// }
		}
	}

	public void OnOpenRewardPanel()
	{
		FindObjectOfType<RewardPanelController>()
			.Init(coinAmount, time, _consumables.ToArray(), _consumableQuantities.ToArray(), false, false);
		DataController.Instance.SaveData();
	}
}