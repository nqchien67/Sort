using Data;
using IAP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.Shop
{
	public sealed class PackContent : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _rubyAmountText;
		[SerializeField] private GridLayoutGroup _layoutGroup;
		[SerializeField] private Transform _unlimitedEnergy;

		private Pack _pack;

		public void Init(Pack pack)
		{
			_pack = pack;
			SetRuby(pack);
			var groupTransform = _layoutGroup.transform;
			Transform itemPrefab = groupTransform.GetChild(0);
			SetItem(itemPrefab, 0);

			for (int i = 1; i < pack.RewardTypes.Length; i++)
			{
				Transform newItem = Instantiate(itemPrefab, groupTransform);
				SetItem(newItem, i);
			}

			SetGridSizeAndSpacing();

			// _unlimitedEnergy.GetComponentInChildren<TextMeshProUGUI>().text = pack.unlimitedEnergyTime + "m";
			if (pack.RewardTypes.Length % 2 != 0)
				_unlimitedEnergy.SetParent(groupTransform);

			_unlimitedEnergy.SetAsLastSibling();
		}

		private void SetRuby(Pack pack)
		{
			_rubyAmountText.text = pack.coinAmount.ToString();
		}

		private void SetItem(Transform itemPrefab, int index)
		{
			itemPrefab.GetComponent<Image>().sprite = RewardHelper.Instance.GetRewardSprite(_pack.RewardTypes[index]);
			itemPrefab.GetComponentInChildren<TextMeshProUGUI>().text = "x" + _pack.Quantities[index];
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
	}
}