using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Data
{
	public class RewardHelper : SingletonCore<RewardHelper>
	{
		public Sprite[] RewardIcons;

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		public Sprite GetRewardSprite(RewardType rewardType)
		{
			return RewardIcons[(int)rewardType];
		}

		public static bool TryConvertRewardToBooster(RewardType reward, out BoosterType result)
		{
			string valueString = reward.ToString();
			result = default;
			return Enum.TryParse(valueString, true, out result);
		}

		public static bool TryConvertBoosterToReward(BoosterType boosterType, out RewardType result)
		{
			string valueString = boosterType.ToString();
			result = default;
			return Enum.TryParse(valueString, true, out result);
		}

		public static RewardType StringToReward(string value)
		{
			if (Enum.IsDefined(typeof(RewardType), value))
			{
				return (RewardType)Enum.Parse(typeof(RewardType), value);
			}

			Debug.LogError("Can't convert string: \"" + value + "\" to Reward -_-");
			return RewardType.Coin;
		}
	}

	[Serializable]
	public enum RewardType
	{
		Coin,
		HugeHammer,
		Time,
		DoublePoint,
		LittleHammer,
		MagicWand,
		FreezeTime,
		Refresh,
		Energy,
		ShelfSkin,
		Effect,
		ItemSkin,
		X2Coin,
		X2Star
	}

	[Serializable]
	public enum BoosterType
	{
		HugeHammer,
		Time,
		DoublePoint,
		LittleHammer,
		MagicWand,
		FreezeTime,
		Refresh,
	}
	
	[Serializable]
	public class Reward
	{
		[JsonProperty("RewardTypes", ItemConverterType = typeof(StringEnumConverter))]
		public RewardType[] RewardTypes;

		public int[] Quantities;
	}

}