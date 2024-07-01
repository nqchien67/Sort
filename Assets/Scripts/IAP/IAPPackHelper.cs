using System.Linq;
using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IAP
{
	[System.Serializable]
	public class PacksData
	{
		public Pack[] Bundles;
		public Pack[] CoinPacks;
	}

	[System.Serializable]
	public class Pack
	{
		public string Id;
		public float Price;

		[JsonProperty("RewardTypes", ItemConverterType = typeof(StringEnumConverter))]
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

		public bool IsCoinPack()
		{
			return RewardTypes.Length == 1 && RewardTypes[0] == RewardType.Coin;
		}
	}

	public static class IAPPackHelper
	{
		public static Pack[] GetAllCoinPacks()
		{
			string stringData = FirebaseServiceController.Instance.GetShopPacksData();
			PacksData packsData = JsonConvert.DeserializeObject<PacksData>(stringData);
			return packsData.CoinPacks;
		}

		public static Pack GetCoinPack(string id)
		{
			string stringData = FirebaseServiceController.Instance.GetShopPacksData();
			PacksData packsData = JsonConvert.DeserializeObject<PacksData>(stringData);
			return packsData.CoinPacks.FirstOrDefault(pack => pack.Id == id);
		}

		public static Pack GetBundle(string id)
		{
			string stringData = FirebaseServiceController.Instance.GetShopPacksData();
			PacksData packsData = JsonConvert.DeserializeObject<PacksData>(stringData);
			return packsData.Bundles.FirstOrDefault(pack => pack.Id == id);
		}
	}
}