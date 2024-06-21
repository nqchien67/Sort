using System.Collections.Generic;
using System.Linq;
using Data;
using IAP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.Shop
{
	public sealed class PackContent : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _coinAmountText;

		[SerializeField] private GridLayoutGroup _layoutGroup;

		// [SerializeField] private Transform _unlimitedEnergy;
		[SerializeField] private Transform _gridElementPrefab;

		private Pack _pack;

		public void Init(Pack pack)
		{
			_pack = pack;

			int coinAmount = pack.GetCoinAmount();
			if (coinAmount > 0)
				SetCoin(coinAmount);

			Transform groupTransform = _layoutGroup.transform;
			for (int i = 0; i < pack.RewardTypes.Length; i++)
			{
				if (pack.RewardTypes[i] == RewardType.Coin)
					continue;

				Transform newItem = Instantiate(_gridElementPrefab, groupTransform);
				SetReward(newItem, i);
			}

			// SetGridSizeAndSpacing();

			// _unlimitedEnergy.GetComponentInChildren<TextMeshProUGUI>().text = pack.unlimitedEnergyTime + "m";
			// if (pack.RewardTypes.Length % 2 != 0)
			// 	_unlimitedEnergy.SetParent(groupTransform);

			// _unlimitedEnergy.SetAsLastSibling();
		}

		private void SetCoin(int coinAmount)
		{
			_coinAmountText.text = coinAmount.ToString();
		}

		private void SetReward(Transform itemPrefab, int index)
		{
			RewardType reward = _pack.RewardTypes[index];
			int rewardQuantity = _pack.Quantities[index];

			itemPrefab.GetComponentInChildren<Image>().sprite =
				RewardHelper.Instance.GetRewardSprite(reward);

			if (reward == RewardType.Energy)
			{
				int hours = rewardQuantity / 60;
				itemPrefab.GetComponentInChildren<TextMeshProUGUI>().text = hours + "h";
			}
			else
			{
				itemPrefab.GetComponentInChildren<TextMeshProUGUI>().text = "x" + rewardQuantity;
			}
		}

		private void SetGridSizeAndSpacing()
		{
			const int maxRow = 4;

			int rowCount = Mathf.CeilToInt(_pack.RewardTypes.Length / 2f);
			int x = maxRow - rowCount;

			Vector2 spacing = _layoutGroup.spacing;
			spacing.y = -3 + Mathf.Pow(3, x);
			_layoutGroup.spacing = spacing;

			_layoutGroup.cellSize = 5 * x * Vector2.one + _layoutGroup.cellSize;
		}

		private bool TryFindCoinIndex(out int coinIndex)
		{
			for (int i = 0; i < _pack.RewardTypes.Length; i++)
			{
				if (_pack.RewardTypes[i] == RewardType.Coin)
				{
					coinIndex = i;
					return true;
				}
			}

			coinIndex = -1;
			return false;
		}
	}
}