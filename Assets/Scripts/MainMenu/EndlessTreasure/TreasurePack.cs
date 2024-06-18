using Controllers;
using Data;
using IAP;
using MainMenu.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.EndlessTreasure
{
	public class TreasurePack : MonoBehaviour
	{
		public RectTransform RectTransform;
		[SerializeField] private PackContent _packContent;
		[SerializeField] private Button _freeButton;
		[SerializeField] private Button _iapButton;

		private Pack _pack;

		public void Init(Pack pack)
		{
			_pack = pack;
			_packContent.Init(pack);

			if (pack.Price <= 0)
			{
				_freeButton.gameObject.SetActive(true);
				_iapButton.gameObject.SetActive(false);
			}
			else
			{
				_freeButton.gameObject.SetActive(false);
				_iapButton.gameObject.SetActive(true);
			}
		}

		public void OnClickClaim()
		{
			for (int i = 0; i < _pack.RewardTypes.Length; i++)
			{
				RewardType reward = _pack.RewardTypes[i];
				int quantity = _pack.Quantities[i];

				if (quantity <= 0)
					continue;

				if (reward == RewardType.Coin)
					ClaimCoin(quantity);
				else if (reward == RewardType.Energy)
					ClaimEnergy(quantity);
				else
				{
					
				}
			}
		}

		private void ClaimCoin(int quantity)
		{
			DataController.Instance.Coin += quantity;
			MainMenuController.Instance.PlayClaimCoinEffect(_iapButton.transform.position);
		}

		private void ClaimEnergy(int minute)
		{
			//TODO: lam unlimited energy roi lam cai nay
		}
	}
}