using Data;
using IAP;
using InGame.UI;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace MainMenu.Shop
{
	public class CoinPackPopup : Popup
	{
		[SerializeField] private TextMeshProUGUI _coinAmountText;
		[SerializeField] private Image _coinIcon;
		[SerializeField] private Sprite[] _coinSprites;

		private Pack _pack;
		private int _coinAmount;

		public void Init(int coinNeed)
		{
			var packs = IAPPackHelper.GetAllCoinPacks();

			foreach (var pack in packs)
			{
				if (pack.Price > 0
				    && pack.GetCoinAmount() >= coinNeed)
				{
					_pack = pack;
					break;
				}
			}

			_coinAmount = _pack.GetCoinAmount();
			_coinAmountText.text = "x" + _coinAmount;
			_coinIcon.sprite = GetCoinSprite(_pack.Id);
		}

		private Sprite GetCoinSprite(string packId)
		{
			int index = ToolHelper.GetLastNumber(packId);
			index = Mathf.Clamp(index, 0, _coinSprites.Length - 1);

			return _coinSprites[index];
		}

		public void OnBuy()
		{
			DataController.Instance.Coin += _coinAmount;
			Close();
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			Destroy(gameObject);
		}
	}
}