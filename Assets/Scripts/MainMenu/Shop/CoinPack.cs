using System;
using IAP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
			_pack = IAPPackHelper.GetPack(GetPackId());
			_coinAmountText.text = "x" + _pack.GetCoinAmount();
		}

		private string GetPackId()
		{
			return "";
		}
	}
}