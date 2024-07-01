using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace MainMenu.TopCharts
{
	public class TopChartsController : MonoBehaviour
	{
		[SerializeField] private NotiDot _openPanelButtonNotiDot;
		private float clickTimeStamp, packLifeTime, timeStamp;
		public UnityAction OnSeasonEnd;

		public static TopChartsController Instance { get; private set; }

		public TopChartsPanel Panel;

		private TopChartsDataManager TopChartsDataManager => TopChartsDataManager.Instance;
		private List<PlayerData> PlayersData => TopChartsDataManager.PlayersData;

		public bool SeasonEnded { get; private set; }

		public List<Reward> Rewards
		{
			get
			{
				if (_rewards == null || _rewards.Count == 0)
					_rewards = GetRewardsData();

				return _rewards;
			}
		}

		private List<Reward> _rewards;


		private void Start()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);

			CheckUpdateRank();
			StartCoroutine(CheckEndSeason());
		}

		private void CheckUpdateRank()
		{
			bool passedADay = DateTime.Now.Day != PlayerPrefs.GetInt("Day_Cache_SuperChef", DateTime.Now.Day);
			if (passedADay)
			{
				Debug.Log(gameObject.name + ": passedADay");

				IncreasePlayersStar();
				PlayersData.Sort((x, y) => y.Star.CompareTo(x.Star));
				RenameLast250Players();

				TopChartsDataManager.SavePlayersData();
			}
			else
			{
				PlayersData.Sort((x, y) => y.Star.CompareTo(x.Star));
			}

			PlayerPrefs.SetInt("Day_Cache_SuperChef", DateTime.Now.Day);

			for (int i = 0; i < PlayersData.Count; i++)
			{
				PlayersData[i].Rank = i;
				if (PlayersData[i].Name == "You")
				{
					PlayerPrefs.SetInt("RankUser", i);
					PlayerPrefs.SetInt("PointUser", PlayersData[i].Star);
				}
			}

			TopChartsDataManager.Instance.ResetDisplayPlayersData();

			PlayerPrefs.SetInt("RankUser_Cache", PlayerPrefs.GetInt("RankUser"));
		}

		private IEnumerator CheckEndSeason()
		{
			yield return null;
			float timeEndSeason = TopChartsDataManager.TimeEndSeason;
			yield return new WaitUntil(() => DataController.ConvertToUnixTime(DateTime.Now) >= timeEndSeason);
			EndSeason();
		}

		private void EndSeason()
		{
			SeasonEnded = true;
			OnSeasonEnd?.Invoke();
			_openPanelButtonNotiDot.SetEnable(true);
		}

		public void StartNewSeason()
		{
			SeasonEnded = false;
			TopChartsDataManager.Instance.InitNewSeason();
			Panel.ContentSpawn.ClearContent();
			Panel.ContentSpawn.Initialize();
			Panel.InitTopPlayerCards();
			PlayerPrefs.SetInt("PointUser", 0);
			_openPanelButtonNotiDot.SetEnable(false);
			TopChartsDataManager.isUpdateRank = false;
		}

		private void IncreasePlayersStar()
		{
			for (int i = 0; i < PlayersData.Count; i++)
				if (TopChartsDataManager.PlayersDataCollection.PlayersData[i].Name != "You")
					PlayersData[i].Star += TopChartsDataManager.StarPlayerGainPerDay.GetRandomValue();
		}

		private void RenameLast250Players()
		{
			for (int i = PlayersData.Count - 1; i > 250; i--)
			{
				PlayerData newPlayerData = TopChartsDataManager.Instance.PlayerNameGenerator.GetRandomName();
				newPlayerData.Star = PlayersData[i].Star;
			}
		}

		private List<Reward> GetRewardsData()
		{
			var data = FirebaseServiceController.Instance.GetTopChartsRewards();
			return JsonConvert.DeserializeObject<TopChartsRewardCollection>(data).Rewards;
		}
	}

	[Serializable]
	public class TopChartsRewardCollection
	{
		public List<Reward> Rewards;
	}
}