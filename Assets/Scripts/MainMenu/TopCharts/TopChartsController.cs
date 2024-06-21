using System;
using System.Collections.Generic;
using Audio;
using Data;
using TMPro;
using UnityEngine;
using Utilities;

namespace MainMenu.TopCharts
{
	public class TopChartsController : MonoBehaviour
	{
		public const string TC_HASH = "spc_hash",
			SPC_DONE_EVENT = "spc_done_event",
			SPC_FIRST_TUTORIAL = "spc_FirstTutorial";

		public const int TimeResetRank = 2592000; //30 days

		[SerializeField] private GameObject cnPanelPrefabs;
		[SerializeField] private AudioClip popUpClip;
		[SerializeField] public GameObject cnBtn;
		[SerializeField] public GameObject cnBtn0;
		[SerializeField] private TextMeshProUGUI cnCountdown, cnCountdownFirstTime;
		private float clickTimeStamp, packLifeTime, packTimeStamp, timeStamp;
		public DateTime timeStart;
		public DateTime timeEnd;
		[Header("Tutorial")] public GameObject tutOpenPrefab0;
		public bool isDoneEvent;

		public static TopChartsController Instance { get; private set; }

		public TopChartsPanel Panel
		{
			get
			{
				if (_panel == null)
					_panel = FindObjectOfType<TopChartsPanel>();

				return _panel;
			}
		}

		private TopChartsPanel _panel;

		public int[] itemRewards;
		public int[] itemRewardsQuantity;

		private TopChartsPlayerDataManager TopChartsPlayerDataManager => TopChartsPlayerDataManager.Instance;
		private List<PlayerData> PlayersData => TopChartsPlayerDataManager.PlayersData;
		private List<PlayerData> DisPlayersData => TopChartsPlayerDataManager.DisplayPlayersData;

		private void Start()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);

			if (SuperChefDataController.Instance != null)
			{
				timeStart = SuperChefDataController.Instance.GetDayStartEvent().ToDateTime('/');
				timeEnd = SuperChefDataController.Instance.GetDayEndEvent().ToDateTime('/');
			}

			if (PlayerPrefs.GetInt("initedTopCharts", 0) == 0)
			{
				const int daysPassed = 15;
				const int secondsPassed = daysPassed * 24 * 60 * 60;
				const int timeRemain = TimeResetRank - secondsPassed;

				PlayerPrefs.SetFloat(TC_HASH,
					(float)DataController.ConvertToUnixTime(DateTime.Now) + timeRemain);
				PlayerPrefs.SetInt("initedTopCharts", 1);
			}

			packTimeStamp = PlayerPrefs.GetFloat(TC_HASH, 0);
			// CheckDoneEvent();
			CheckUpdateRank();
		}

		public void CheckUpdateRank()
		{
			// CalculateTimeAndDisplay();
			if (DateTime.Now.Day != PlayerPrefs.GetInt("Day_Cache_SuperChef", DateTime.Now.Day) &&
			    PlayerPrefs.GetInt(SPC_DONE_EVENT, 0) != 1)
			{
				Debug.Log("Test____________2");
				PlayerPrefs.SetInt("Day_Cache_SuperChef", DateTime.Now.Day);

				for (int i = 0; i < PlayersData.Count; i++)
					if (TopChartsPlayerDataManager.PlayersDataCollection.PlayersData[i].Name != "You")
					{
						PlayersData[i].Star +=
							TopChartsPlayerDataManager.StarPlayerGainPerDay.GetRandomValue();
					}

				PlayersData.Sort((x, y) => y.Star.CompareTo(x.Star));
				RenameLast150Players();

				TopChartsPlayerDataManager.SavePlayersData();
			}
			else
			{
				PlayerPrefs.SetInt("Day_Cache_SuperChef", DateTime.Now.Day);
				PlayersData.Sort((x, y) => y.Star.CompareTo(x.Star));
			}

			for (int i = 0; i < PlayersData.Count; i++)
			{
				PlayersData[i].Rank = i;
				if (PlayersData[i].Name == "You")
				{
					PlayerPrefs.SetInt("RankUser", i);
					PlayerPrefs.SetInt("PointUser", PlayersData[i].Star);
				}
			}

			TopChartsPlayerDataManager.Instance.ResetDisplayPlayersData();

			PlayerPrefs.SetInt("RankUser_Cache", PlayerPrefs.GetInt("RankUser"));
		}

		private void RenameLast150Players()
		{
			for (int i = PlayersData.Count - 1; i > 250; i--)
			{
				PlayerData newPlayerData = TopChartsPlayerDataManager.Instance.PlayerNameGenerator.GetRandomName();
				newPlayerData.Star = PlayersData[i].Star;
			}
		}

		// public void RewardEndEv()
		// {
		// 	if (PlayerPrefs.GetInt("IsCollelctDoneEventSuperChef", 0) == 0)
		// 	{
		// 		Debug.Log("TEst3");
		//
		// 		PlayerPrefs.SetInt("IsCollelctDoneEventSuperChef", 1);
		// 		var itemSuperChefDataController = ItemSuperChefDataController.Instance;
		// 		if (PlayerPrefs.GetInt("RankUser") == 0)
		// 		{
		// 			var itemSuperChefData = itemSuperChefDataController.GetItemSuperChefData(0).itemsList;
		// 			itemRewards = new int[3];
		// 			itemRewardsQuantity = new int[3];
		//
		// 			itemRewards[0] = SetReward(itemSuperChefData[0].typeReward);
		// 			itemRewards[1] = SetReward(itemSuperChefData[1].typeReward);
		// 			itemRewards[2] = SetReward(itemSuperChefData[2].typeReward);
		// 			itemRewardsQuantity[0] = itemSuperChefData[0].number;
		// 			itemRewardsQuantity[1] = itemSuperChefData[1].number;
		// 			itemRewardsQuantity[2] = itemSuperChefData[2].number;
		//
		// 			FindObjectOfType<RewardPanelController>()
		// 				.Init(
		// 					0,
		// 					30,
		// 					itemRewards,
		// 					itemRewardsQuantity,
		// 					false,
		// 					null,
		// 					() => { });
		// 		}
		// 		else if (PlayerPrefs.GetInt("RankUser") == 1)
		// 		{
		// 			itemRewards = new int[2];
		// 			itemRewardsQuantity = new int[2];
		// 			var itemSuperChefData1 = itemSuperChefDataController.GetItemSuperChefData(1);
		// 			itemRewards[0] = SetReward(itemSuperChefData1.itemsList[0]
		// 				.typeReward);
		// 			itemRewards[1] = SetReward(itemSuperChefData1.itemsList[1]
		// 				.typeReward);
		// 			itemRewardsQuantity[0] =
		// 				itemSuperChefData1.itemsList[0].number;
		// 			itemRewardsQuantity[1] =
		// 				itemSuperChefData1.itemsList[1].number;
		// 			FindObjectOfType<RewardPanelController>()
		// 				.Init(
		// 					0,
		// 					0,
		// 					30,
		// 					itemRewards,
		// 					itemRewardsQuantity,
		// 					false,
		// 					null,
		// 					() => { });
		// 		}
		// 		else if (PlayerPrefs.GetInt("RankUser") == 2)
		// 		{
		// 			itemRewards = new int[2];
		// 			itemRewardsQuantity = new int[2];
		// 			itemRewards[0] = SetReward(itemSuperChefDataController.GetItemSuperChefData(2).itemsList[0]
		// 				.typeReward);
		// 			itemRewards[1] = SetReward(itemSuperChefDataController.GetItemSuperChefData(2).itemsList[1]
		// 				.typeReward);
		// 			itemRewardsQuantity[0] =
		// 				itemSuperChefDataController.GetItemSuperChefData(2).itemsList[0].number;
		// 			itemRewardsQuantity[1] =
		// 				itemSuperChefDataController.GetItemSuperChefData(2).itemsList[1].number;
		// 			FindObjectOfType<RewardPanelController>()
		// 				.Init(
		// 					0,
		// 					0,
		// 					25,
		// 					itemRewards,
		// 					itemRewardsQuantity,
		// 					false,
		// 					null,
		// 					() => { });
		// 		}
		// 		else if (PlayerPrefs.GetInt("RankUser") == 3)
		// 		{
		// 			itemRewards = new int[1];
		// 			itemRewardsQuantity = new int[1];
		// 			itemRewards[0] = SetReward(itemSuperChefDataController.GetItemSuperChefData(3).itemsList[0]
		// 				.typeReward);
		// 			itemRewardsQuantity[0] =
		// 				itemSuperChefDataController.GetItemSuperChefData(3).itemsList[0].number;
		// 			FindObjectOfType<RewardPanelController>()
		// 				.Init(
		// 					0,
		// 					0,
		// 					20,
		// 					itemRewards,
		// 					itemRewardsQuantity,
		// 					false,
		// 					null,
		// 					() => { });
		// 		}
		// 		else if (PlayerPrefs.GetInt("RankUser") == 4)
		// 		{
		// 			itemRewards = new int[1];
		// 			itemRewardsQuantity = new int[1];
		// 			itemRewards[0] = SetReward(itemSuperChefDataController.GetItemSuperChefData(4).itemsList[0]
		// 				.typeReward);
		// 			itemRewardsQuantity[0] =
		// 				itemSuperChefDataController.GetItemSuperChefData(4).itemsList[0].number;
		// 			FindObjectOfType<RewardPanelController>()
		// 				.Init(
		// 					0,
		// 					0,
		// 					15,
		// 					itemRewards,
		// 					itemRewardsQuantity,
		// 					false,
		// 					null,
		// 					() => { });
		// 		}
		// 		else if (5 <= PlayerPrefs.GetInt("RankUser") && PlayerPrefs.GetInt("RankUser") < 50)
		// 		{
		// 			itemRewards = Array.Empty<int>();
		// 			itemRewardsQuantity = Array.Empty<int>();
		// 			FindObjectOfType<RewardPanelController>()
		// 				.Init(
		// 					0,
		// 					0,
		// 					10,
		// 					itemRewards,
		// 					itemRewardsQuantity,
		// 					false,
		// 					null,
		// 					() => { });
		// 		}
		// 		else if (50 <= PlayerPrefs.GetInt("RankUser") && PlayerPrefs.GetInt("RankUser") < 100)
		// 		{
		// 			go = Instantiate(cnPanelPrefabs, MainMenuController.Instance.CameraCanvas);
		// 			AudioController.Instance.PlaySfx(popUpClip);
		// 		}
		//
		// 		Debug.Log("TEst4");
		// 	}
		// }

		public int SetReward(string typeRewardFree)
		{
			if (typeRewardFree == "2 Chance")
				return 100300;
			if (typeRewardFree == "Add Customer")
				return 100100;
			if (typeRewardFree == "Add Time")
				return 100200;
			if (typeRewardFree == "Anti Over")
				return 100000;
			if (typeRewardFree == "Auto Serve")
				return 130000;
			if (typeRewardFree == "Avatar")
				return 9;
			if (typeRewardFree == "Caramen")
				return 101000;
			if (typeRewardFree == "Customer New")
				return 310000; // tammmmmm
			if (typeRewardFree == "Double Coin")
				return 110000;
			if (typeRewardFree == "Energy")
				return 121212; //tammmmmm
			if (typeRewardFree == "Instant")
				return 120000;
			if (typeRewardFree == "Avatar 2")
				return 50;
			return 0;
		}

		public void CheckDoneEvent()
		{
			if (CanDisplayPack())
				CalculateTimeAndDisplay();
			else
				cnBtn.SetActive(false);
		}

		public void CalculateTimeAndDisplay()
		{
			if (PlayerPrefs.GetFloat(TC_HASH, 0) <= DataController.ConvertToUnixTime(DateTime.Now) &&
			    PlayerPrefs.GetInt("HasShowSuperChef") == 1)
			{
				// Debug.Log("CalculateTimeAndDisplayYYYYYYYYYYYYYYYYYYYYYYYYYYY");
				if (PlayerPrefs.GetInt("SPC_Set_Active_False", 0) == 0)
					PlayerPrefs.SetInt("SPC_Set_Active_False", 1);
				PlayerPrefs.SetInt(SPC_DONE_EVENT, 1);
			}

			packTimeStamp = PlayerPrefs.GetFloat(TC_HASH, 0);
			if (SuperChefDataController.Instance.GetActiveEvent()
			    && DataController.ConvertToUnixTime(DateTime.Now) - DataController.ConvertToUnixTime(timeStart) > 0
			    && PlayerPrefs.GetInt("HasShowSuperChef", 0) == 1)
				cnBtn.gameObject.SetActive(true);
			else
				cnBtn.gameObject.SetActive(false);
		}

		public bool CanDisplayPack()
		{
			// return true;
			return DateTime.Now >= timeStart && DateTime.Now < timeEnd || isDoneEvent == false;
		}

		public void OnClickOpenPanel()
		{
			if (Time.time - clickTimeStamp < 0.5f) return;
			clickTimeStamp = Time.time;
			OpenPanel();
		}

		public void OpenPanel()
		{
			AudioController.Instance.PlaySfx(popUpClip);
		}

		private void Update()
		{
			// if ((float)(packTimeStamp - DataController.ConvertToUnixTime(System.DateTime.Now)) <= 0 ||
			//     PlayerPrefs.GetInt(SPC_DONE_EVENT) == 1)
			// {
			// 	cnBtn.SetActive(false);
			// 	return;
			// }

			// if (Time.time - timeStamp > 1)
			// {
			// 	if (cnBtn.gameObject.activeInHierarchy && cnCountdown.gameObject.activeInHierarchy)
			// 	{
			// 		float deltaTime = (float)(packTimeStamp - DataController.ConvertToUnixTime(DateTime.Now));
			// 		int hours = DataController.SecondToHours(deltaTime);
			// 		int days = DataController.SecondsToDays(deltaTime);
			//
			// 		if (days >= 1)
			// 			cnCountdown.text = string.Format(" {0:D2}d {1:D2}h", (int)deltaTime / (3600 * 24),
			// 				(int)(deltaTime / 3600) % 24);
			// 		else if (hours >= 1)
			// 			cnCountdown.text = string.Format(" {0:D2}h {1:D2}m", (int)deltaTime / 3600,
			// 				(int)(deltaTime / 60) % 60);
			// 		else
			// 			cnCountdown.text = string.Format(" {0:D2}m {1:D2}s", (int)(deltaTime / 60) % 60,
			// 				(int)deltaTime % 60);
			// 	}
			//
			// 	if (cnBtn0.gameObject.activeInHierarchy && cnCountdownFirstTime.gameObject.activeInHierarchy)
			// 	{
			// 		float deltaTime = (float)(packTimeStamp - DataController.ConvertToUnixTime(DateTime.Now));
			// 		int hours = DataController.SecondToHours(deltaTime);
			// 		int days = DataController.SecondsToDays(deltaTime);
			//
			// 		if (days >= 1)
			// 			cnCountdownFirstTime.text = string.Format(" {0:D2}d{1:D2}h", (int)deltaTime / (3600 * 24),
			// 				(int)(deltaTime / 3600) % 24);
			// 		else if (hours >= 1)
			// 			cnCountdownFirstTime.text = string.Format(" {0:D2}h{1:D2}m", (int)deltaTime / 3600,
			// 				(int)(deltaTime / 60) % 60);
			// 		else
			// 			cnCountdownFirstTime.text = string.Format(" {0:D2}m{1:D2}s", (int)(deltaTime / 60) % 60,
			// 				(int)deltaTime % 60);
			// 	}
			// }
		}
	}
}