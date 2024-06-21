using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
using MainMenu;
using MainMenu.CollectionTask;
using MainMenu.TopCharts;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Events;

namespace Data
{
	public class DataController : MonoBehaviour
	{
		public static DataController Instance;

		[SerializeField] private GameData gameData;
		[HideInInspector] public UnityEvent onDataChange;

		public LevelData[] LevelsData;
		public ComboData[] CombosData;
		public int HighestPassedLevel => PlayerPrefs.GetInt("level", 0);

		private string dataPath = "";

		private void Start()
		{
			if (Instance == null)
			{
				Instance = this;
				dataPath = Path.Combine(Application.persistentDataPath, "data.dat");

				DontDestroyOnLoad(gameObject);
			}
			new StringEnumConverter();
			LoadData();

#if UNITY_EDITOR
			AddBooster(BoosterType.LittleHammer, 100);
#endif
		}

		public void LoadData()
		{
			var levelDataCollection =
				JsonUtility.FromJson<LevelDataCollection>(FirebaseServiceController.Instance.GetLevelsData());
			LevelsData = levelDataCollection.LevelsData;

			var comboDataCollection =
				JsonUtility.FromJson<ComboDataCollection>(FirebaseServiceController.Instance.GetCombosData());
			CombosData = comboDataCollection.CombosData;

			LoadLocalData();
		}

		private void LoadLocalData()
		{
			if (File.Exists(dataPath))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				using FileStream fileStream = File.Open(dataPath, FileMode.Open);
				try
				{
					string data = (string)binaryFormatter.Deserialize(fileStream);
					gameData = JsonUtility.FromJson<GameData>(data);
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
					GenerateNewData();
				}
			}
			else
				GenerateNewData();
		}

		private void GenerateNewData()
		{
			gameData = new GameData
			{
				SkinData = SpritesCollection.Instance.InitSkinData(),
				Profile = new Profile
				{
					Name = PlayerNameGenerator.GenerateDefaultName()
				}
			};

			SaveData(false);
		}

		public int Coin
		{
			get => gameData.Coin;
			set
			{
				gameData.Coin = value;
				onDataChange?.Invoke();
			}
		}

		public int Star
		{
			get => gameData.Star;
			set
			{
				gameData.Star = value;
				onDataChange?.Invoke();
			}
		}

		public static double ConvertToUnixTime(DateTime time)
		{
			DateTime epoch = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return (time - epoch).TotalSeconds;
		}

		public void SaveData(bool postData = true)
		{
			DOVirtual.DelayedCall(0.1f, () =>
				{
					gameData.SaveTime = DateTime.Now.Ticks;
					string origin = JsonUtility.ToJson(gameData);
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
					{
						binaryFormatter.Serialize(fileStream, origin);
					}


					// if (postData)
					// 	DatabaseController.Instance.PostData();
				}
			);
		}

		public int GetBoosterQuantity(BoosterType boosterType)
		{
			foreach (var booster in gameData.BoostersState)
			{
				if (booster.Type == boosterType)
					return booster.Quantity;
			}

			return 0;
		}

		public bool isFirstOpenPB = true;

		public int PbTimeDuration
		{
			get => gameData.pbTimeDuration;
			set => gameData.pbTimeDuration = value;
		}

		public double PiggyBankTimeStamp
		{
			get => gameData.PiggyBankTimeStamp;
			set => gameData.PiggyBankTimeStamp = value;
		}

		public int PiggyBankCoin
		{
			get => gameData.PiggyBankCoin;
			set => gameData.PiggyBankCoin = Mathf.Clamp(value, 0, PiggyBankStorageMilestone[PiggyBankLevel - 1]);
		}

		public int[] PiggyBankStorageMilestone
			=> new[] { 3000, 6000, 9000, 15000, 25000, 45000 };

		public int PiggyBankLevel
		{
			get
			{
				if (gameData.PiggyBankLevel == 0)
					gameData.PiggyBankLevel = 1;
				return Mathf.Clamp(gameData.PiggyBankLevel, 1, 6);
			}
			set => gameData.PiggyBankLevel = Mathf.Clamp(value, 1, 6);
		}

		public int CurrentPbStorage => PiggyBankStorageMilestone[PiggyBankLevel - 1];

		public bool IsPiggyBankFull()
		{
			return gameData.PiggyBankCoin >= PiggyBankStorageMilestone[PiggyBankLevel - 1];
		}

		public void AddBooster(BoosterType boosterType, int quantity)
		{
			gameData.AddBooster(boosterType, quantity);
		}

		public SkinData SkinData
		{
			get => gameData.SkinData;
			set => gameData.SkinData = value;
		}

		public static int SecondsToDays(float totalSeconds)
		{
			return (int)(totalSeconds / (3600f * 24f));
		}

		public static int SecondToHours(float totalSeconds)
		{
			return (int)(totalSeconds / 3600f);
		}

		public static int SecondsToMinutes(float totalSeconds)
		{
			return (int)(totalSeconds / 60 % 60);
		}

		public static int GetSeconds(float totalSeconds)
		{
			return (int)(totalSeconds % 60);
		}

		public static RewardType[] StringsToConsumable(string[] values)
		{
			RewardType[] items = new RewardType[values.Length];

			for (int i = 0; i < values.Length; i++)
			{
				items[i] = RewardHelper.StringToReward(values[i]);
			}

			return items;
		}

		public List<int> UnlockedAvatarIds
		{
			get => gameData.UnlockedAvatarIds.ToList();
			set => gameData.UnlockedAvatarIds = value.ToArray();
		}

		public Profile Profile
		{
			get => gameData.Profile;
			set => gameData.Profile = value;
		}

		public static Sprite GetAvatarSprite(string avatarName) => Resources.Load<Sprite>("Avatars/" + avatarName);

		#region Energy

		public int Energy
		{
			get => gameData.Energy;
			set
			{
				if (value > PlayerPrefs.GetInt("MAX_ENERGY", 5) ||
				    gameData.Energy == PlayerPrefs.GetInt("MAX_ENERGY", 5) &&
				    value == PlayerPrefs.GetInt("MAX_ENERGY", 5) - 1)
				{
					gameData.EnergyTimeStamp = ConvertToUnixTime(DateTime.UtcNow);
				}

				gameData.Energy = Mathf.Clamp(value, 0, PlayerPrefs.GetInt("MAX_ENERGY", 5));
				SaveData(false);
			}
		}

		public int UnlimitedEnergyDuration
		{
			get => gameData.UnlimitedEnergyTime;
			set => gameData.UnlimitedEnergyTime = Mathf.Max(0, value);
		}

		public double EnergyTimeStamp
		{
			get => gameData.EnergyTimeStamp;
			set => gameData.EnergyTimeStamp = value;
		}

		public double UnlimitedEnergyTimeStamp
		{
			set => gameData.UnlimitedEnergyTimeStamp = value;
			get => gameData.UnlimitedEnergyTimeStamp;
		}

		public void IncreaseOneEnergy()
		{
			gameData.Energy = Mathf.Clamp(gameData.Energy + 1, 0, PlayerPrefs.GetInt("MAX_ENERGY", 5));
		}

		public bool HaveUnlimitedEnergy()
		{
			double deltaTime = ConvertToUnixTime(DateTime.UtcNow) - gameData.UnlimitedEnergyTimeStamp;
			int remainTime = gameData.UnlimitedEnergyTime - Mathf.Max(0, (int)deltaTime);
			return remainTime > 0;
		}

		public bool TryUseEnergy()
		{
			if (HaveUnlimitedEnergy())
				return true;

			if (Energy <= 0) return false;

			Energy--;
			return true;
		}

		#endregion
	}
}