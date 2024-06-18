using System;
using System.Collections.Generic;
using System.Linq;
using MainMenu.CollectionTask;
using MainMenu.TopCharts;
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

		public Booster[] BoostersState;

		public SkinData SkinData;

		public int[] UnlockedAvatarIds;
		public Profile Profile;

		public int Energy;
		public double EnergyTimeStamp;
		public int UnlimitedEnergyTime;
		public double UnlimitedEnergyTimeStamp;

		public GameData()
		{
			Array boosterTypes = Enum.GetValues(typeof(BoosterType));

			List<Booster> listBoosters = new List<Booster>();
			foreach (BoosterType boosterType in boosterTypes)
				listBoosters.Add(new Booster(boosterType));

			BoostersState = listBoosters.ToArray();
		}

		public void AddBooster(BoosterType boosterType, int quantity)
		{
			foreach (Booster consumableState in BoostersState)
				if (consumableState.Type == boosterType)
				{
					consumableState.Quantity = Mathf.Max(0, consumableState.Quantity + quantity);
					return;
				}

			List<Booster> consumableStatesTmp = new List<Booster>(BoostersState)
				{ new Booster(boosterType, quantity) };
			BoostersState = consumableStatesTmp.ToArray();
		}
	}

	[Serializable]
	public class Booster
	{
		public BoosterType Type;
		public int Quantity;

		public Booster(BoosterType boosterType)
		{
			Type = boosterType;
			Quantity = 0;
		}

		public Booster(BoosterType boosterType, int quantity)
		{
			Type = boosterType;
			Quantity = quantity;
		}
	}

	[Serializable]
	public class Profile
	{
		public string AvatarName = "49";
		public string Name;

		public Sprite Avatar => Resources.Load<Sprite>("Avatars/" + AvatarName);
	}
}