using System;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MainMenu.LuckySpin
{
	public class LuckySpinItem : MonoBehaviour
	{
		public float probability;
		public float rotationZ;
		[SerializeField] private List<RewardType> _consumables;
		[SerializeField] private List<int> _consumableQuantities;
		private int multiple;
		readonly int time = 0;

		private List<int> _initialConsumableQuantities;

		private void OnValidate()
		{
			var amountText = GetComponentInChildren<TextMeshProUGUI>(true);

			if (_consumableQuantities.Count > 0)
				amountText.text = "x" + _consumableQuantities[0];
		}

		private void Awake()
		{
			_initialConsumableQuantities = new List<int>(_consumableQuantities);
		}

		public void SetAdsRewardValue()
		{
			var amountText = GetComponentInChildren<TextMeshProUGUI>(true);

			if (_consumableQuantities.Count > 0)
			{
				for (int i = 0; i < _consumableQuantities.Count; i++)
				{
					if (_consumables[i] == RewardType.Coin)
						_consumableQuantities[i] = _initialConsumableQuantities[i] + 20;
					else
						_consumableQuantities[i] = _initialConsumableQuantities[i] * 2;
				}

				if (_consumableQuantities.Count == 1)
					amountText.text = "x" + _consumableQuantities[0];
			}

			amountText.transform.DOScale(new Vector3(1.15f, 1.15f, 1), 0.3f).SetLoops(-1, LoopType.Yoyo)
				.SetEase(Ease.InOutSine);
		}

		public void SetFreeRewardValue()
		{
			var amountText = GetComponentInChildren<TextMeshProUGUI>(true);

			 if (_consumableQuantities.Count > 0)
			{
				for (int i = 0; i < _consumableQuantities.Count; i++)
					_consumableQuantities[i] = _initialConsumableQuantities[i];

				if (_consumableQuantities.Count == 1)
					amountText.text = "x" + _consumableQuantities[0];
			}

			amountText.transform.DOKill();
			amountText.transform.localScale = Vector3.one;
		}

		public void OnOpenRewardPanel()
		{
			List<RewardType> rewardTypes;
			List<int> quantities;

			if (_consumables.Count > 1) //Chi lay 1 phan thuong thoi
				(rewardTypes, quantities) = ChooseOneReward();
			else
			{
				rewardTypes = _consumables;
				quantities = _consumableQuantities;
			}

			FindObjectOfType<RewardPanelController>().Init(rewardTypes.ToArray(), quantities.ToArray(), false);
		}

		private (List<RewardType>, List<int>) ChooseOneReward()
		{
			int randomIndex = Random.Range(0, _consumables.Count);
			List<RewardType> rewardTypes = new List<RewardType>
			{
				_consumables[randomIndex]
			};

			List<int> quantities = new List<int>
			{
				_consumableQuantities[randomIndex]
			};

			return (rewardTypes, quantities);
		}
	}
}