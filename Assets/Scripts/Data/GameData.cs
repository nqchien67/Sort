using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
	[Serializable]
	public class GameData
	{
		public int pbTimeDuration;
		public long SaveTime;
		public int DailyRewardTimeStamp;
		public double PiggyBankTimeStamp;
		public int PiggyBankCoin;
		public int PiggyBankLevel = 1;
		public int Coin;
		public int Star;

		public Consumable[] ConsumableStates = Array.Empty<Consumable>();

		public SkinData SkinData;
		public int[] PurchasedBackgroundIds;

		public int[] UnlockedAvatarIds;
		public Profile Profile = new Profile();

		public void AddConsumable(ConsumableType consumableType, int amount)
		{
			foreach (Consumable consumableState in ConsumableStates)
				if (consumableState.Id == consumableType.ToString())
				{
					consumableState.Quantity += amount;
					return;
				}

			List<Consumable> consumableStatesTmp = new List<Consumable>(ConsumableStates)
				{ new Consumable(consumableType.ToString(), amount) };
			ConsumableStates = consumableStatesTmp.ToArray();
		}
	}

	[Serializable]
	public class Consumable
	{
		public string Id;
		public int Quantity = 0;

		public Consumable(string consumableId)
		{
			Id = consumableId;
		}

		public Consumable(string consumableId, int quantity)
		{
			Id = consumableId;
			Quantity = quantity;
		}
	}

	[Serializable]
	public class Profile
	{
		public int CurrentAvatarId = 0;
		public string Name = "Player";
	}
}
