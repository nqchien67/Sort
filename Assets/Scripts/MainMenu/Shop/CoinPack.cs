using IAP;
using TMPro;
using UnityEngine;

namespace MainMenu.Shop
{
	public class CoinPack : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _coinAmountText;

		private Pack _pack;

		private void Start()
		{
			Init();
		}

		private void Init()
		{
			_pack = IAPPackHelper.GetCoinPack(GetPackId());
			_coinAmountText.text = "x" + _pack.GetCoinAmount();
		}

		private string GetPackId()
		{
			return "coin1";
		}
	}
}