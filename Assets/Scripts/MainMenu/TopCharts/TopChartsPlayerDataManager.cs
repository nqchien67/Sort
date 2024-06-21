using System;
using System.Collections.Generic;
using System.IO;
using Data;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace MainMenu.TopCharts
{
	public class TopChartsPlayerDataManager : SingletonCore<TopChartsPlayerDataManager>
	{
		public const int PlayerCount = 400;
		public Range StarPlayerGainPerDay;

		[SerializeField] private SuperChefData superChefData;

		private string saveFilePath, saveFilePath2;

		public List<PlayerData> PlayersData
		{
			get => PlayersDataCollection.PlayersData;
			private set => PlayersDataCollection.PlayersData = value;
		}

		public List<PlayerData> DisplayPlayersData;

		public PlayersDataCollection PlayersDataCollection;
		public SuperChefDataNameUserFakesPos PlayerCardPosCollection;

		public PlayerNameGenerator PlayerNameGenerator =>
			_playerNameGenerator ??=
				new PlayerNameGenerator(_nationalFlagAvatars, _animalFlagAvatars, _humanFlagAvatars);

		private PlayerNameGenerator _playerNameGenerator;

		public bool isUpdateRank;

		[SerializeField] private Sprite[] _nationalFlagAvatars;
		[SerializeField] private Sprite[] _animalFlagAvatars;
		[SerializeField] private Sprite[] _humanFlagAvatars;

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
			saveFilePath = Path.Combine(Application.persistentDataPath, "top_charts_data.dat");
			LoadPlayerData();
		}

		public void SavePlayersData()
		{
			string data = JsonUtility.ToJson(PlayersDataCollection);
			File.WriteAllText(saveFilePath, data);
		}

		private void LoadPlayerData()
		{
			if (File.Exists(saveFilePath))
			{
				try
				{
					string loadPlayerData = File.ReadAllText(saveFilePath);
					PlayersDataCollection = JsonUtility.FromJson<PlayersDataCollection>(loadPlayerData);
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
					GenerateNewPLayersData();
				}
			}
			else
			{
				PlayersDataCollection = new PlayersDataCollection();
				GenerateNewPLayersData();
			}

			PlayersData.Sort((x, y) => y.Star.CompareTo(x.Star));
			ResetDisplayPlayersData();
		}

		private void GenerateNewPLayersData()
		{
			const int daysPassed = 15;

			PlayersData = new List<PlayerData>();
			for (int i = 0; i < PlayerCount; i++)
			{
				PlayerData playerData = PlayerNameGenerator.GetRandomName();
				playerData.Star = CalculateRandomStarByDays(daysPassed);

				PlayersData.Add(playerData);
			}

			PlayerData userData = new PlayerData("You", DataController.Instance.Profile.AvatarName);
			PlayersData.Add(userData);

			SavePlayersData();
		}

		private int CalculateRandomStarByDays(int days)
		{
			int result = 0;
			for (int i = 0; i < days; i++)
				result += StarPlayerGainPerDay.GetRandomValue();

			return result;
		}

		public void UpdateRank()
		{
			isUpdateRank = true;
			PlayerPrefs.SetInt("RankUser_Cache", PlayerPrefs.GetInt("RankUser"));

			PlayersData[PlayerPrefs.GetInt("RankUser")].Star = PlayerPrefs.GetInt("PointUser", 0);

			PlayersData.Sort((x, y) => y.Star.CompareTo(x.Star));

			for (int i = 0; i < PlayersData.Count; i++)
			{
				PlayersData[i].Rank = i;
				if (PlayersData[i].Name == "You")
				{
					PlayerPrefs.SetInt("RankUser", i);
				}
			}

			if (PlayerPrefs.GetInt("RankUser") == PlayerPrefs.GetInt("RankUser_Cache"))
			{
				isUpdateRank = false;
			}

			SavePlayersData();
			ResetDisplayPlayersData();
		}

		public void ResetDisplayPlayersData()
		{
			DisplayPlayersData = new List<PlayerData>(PlayersData);
			for (int i = 0; i < 3; i++)
				DisplayPlayersData.RemoveAt(0);
		}
	}

	[System.Serializable]
	public class PlayerData
	{
		public int Rank;
		public int Star;
		public string AvatarName;
		public string Name;

		public PlayerData(string name, string avatarName)
		{
			Name = name;
			AvatarName = avatarName;
		}
	}

	[System.Serializable]
	public class PlayersDataCollection
	{
		public List<PlayerData> PlayersData = new List<PlayerData>();
	}

	[System.Serializable]
	public class DataSuperChefNameUserFakePos
	{
		public Vector3 pos;
	}

	[System.Serializable]
	public class SuperChefDataNameUserFakesPos
	{
		public List<DataSuperChefNameUserFakePos> superChefNameData3 = new List<DataSuperChefNameUserFakePos>();
	}

// SfxScript.SFXins.SFXAudio.Stop();
//         SfxScript.SFXins.SFXAudio.PlayOneShot(SfxScript.SFXins.rankSoud, 2f);
//         DOTween.Sequence()
//         .Append(rankPlayer.transform.DOScaleX(1.1f, 0.25f).SetEase(Ease.Linear))
//         .Append(parentOfRank.transform.DOLocalMoveY(-2928, 2))
//         .Append(rankPlayer.transform.DOScaleX(1f, 0.25f).SetEase(Ease.Linear))
//         .OnComplete(() =>
//         {
//             btnBack.SetActive(true);
//             btnBack2.SetActive(true);
//         }
//         )
//         .Play();
}