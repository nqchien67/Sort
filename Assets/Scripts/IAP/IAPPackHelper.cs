using System.Linq;
using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IAP
{
	[System.Serializable]
	public class PacksData
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

		[JsonProperty("RewardTypes", ItemConverterType=typeof(StringEnumConverter))]
		public RewardType[] RewardTypes;
		public int[] Quantities;
		public string tag;

		public int GetCoinAmount()
		{
			for (int i = 0; i < RewardTypes.Length; i++)
			{
				if (RewardTypes[i] == RewardType.Coin)
					return Quantities[i];
			}

			return 0;
		}
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

	public static class IAPPackHelper
	{
		// public const string IAP_VALUE = "user_iap_value";

		public static Pack GetPack(string packId)
		{
			string stringData = FirebaseServiceController.Instance.GetShopPacksData();
			PacksData packsData = JsonConvert.DeserializeObject<PacksData>(stringData);
			return packsData.packs.FirstOrDefault(pack => pack.Id == packId);
		}

		public static float GetPackPrice(string packId)
		{
			string stringData = FirebaseServiceController.Instance.GetShopPacksData();
			PacksData packsData = JsonConvert.DeserializeObject<PacksData>(stringData);
			return packsData.GetPackPrice(packId);
		}

		public static RewardType[] GetPackReward(string packId)
		{
			string stringData = FirebaseServiceController.Instance.GetShopPacksData();
			PacksData packsData = JsonConvert.DeserializeObject<PacksData>(stringData);
			return packsData.GetPackReward(packId);
		}

		public static int[] GetPackRewardAmounts(string packId)
		{
			string stringData = FirebaseServiceController.Instance.GetShopPacksData();
			PacksData packsData = JsonConvert.DeserializeObject<PacksData>(stringData);
			return packsData.GetPackRewardAmounts(packId);
		}
	}
}