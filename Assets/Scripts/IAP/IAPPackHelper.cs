using System.Linq;
using Data;
using UnityEngine;

namespace IAP
{
	[System.Serializable]
	public class Packs
	{
		public Pack[] packs;

		public Pack GetPackById(string packId)
		{
			foreach (Pack pack in packs)
				if (pack.Id == packId)
					return pack;
			return null;
		}

		public float GetPackPrice(string packId)
		{
			foreach (var pack in packs)
				if (pack.Id == packId)
					return pack.Price;
			return 0;
		}

		public int GetPackCoinAmount(string packId)
		{
			foreach (var pack in packs)
				if (pack.Id == packId)
					return pack.coinAmount;
			return 0;
		}

		public RewardType[] GetPackReward(string packId)
		{
			foreach (var pack in packs)
				if (pack.Id == packId)
					return pack.RewardTypes;
			return null;
		}

		public int[] GetPackRewardAmounts(string packId)
		{
			foreach (var pack in packs)
				if (pack.Id == packId)
					return pack.Quantities;
			return null;
		}
	}

	[System.Serializable]
	public class Pack
	{
		public string Id;
		public float Price;
		public int coinAmount;
		public RewardType[] RewardTypes;
		public int[] Quantities;
		public string tag;
	}

	[System.Serializable]
	public class GiftPack
	{
		public int RubyAmount, GoldAmount, UnlimitedEnergyTime;
		public int[] ItemId, ItemAmount;
		public bool IsRemoveAds;
	}

	[System.Serializable]
	public class GiftCodeData
	{
		public int giftType;
		public GiftPack gift;
		public string message;
	}

	public class ShowRubySaleInShop
	{
		public int active;
	}

	public class IAPPackHelper
	{
		public const string IAP_VALUE = "user_iap_value";

		public static Pack GetPack(string packId)
		{
			string packsData = FirebaseServiceController.Instance.GetShopPacksData();
			Packs packs = JsonUtility.FromJson<Packs>(packsData);
			return packs.packs.FirstOrDefault(pack => pack.Id == packId);
		}

		public static float GetPackPrice(string packId)
		{
			string packsData = FirebaseServiceController.Instance.GetShopPacksData();
			Packs packs = JsonUtility.FromJson<Packs>(packsData);
			return packs.GetPackPrice(packId);
		}

		public static RewardType[] GetPackReward(string packId)
		{
			string packsData = FirebaseServiceController.Instance.GetShopPacksData();
			Packs packs = JsonUtility.FromJson<Packs>(packsData);
			return packs.GetPackReward(packId);
		}

		public static int[] GetPackRewardAmounts(string packId)
		{
			string packsData = FirebaseServiceController.Instance.GetShopPacksData();
			Packs packs = JsonUtility.FromJson<Packs>(packsData);
			return packs.GetPackRewardAmounts(packId);
		}
		
	}
}