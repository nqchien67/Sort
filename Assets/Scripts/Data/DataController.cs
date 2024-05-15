using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
using MainMenu;
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
		private string dataPath = "";

		protected void Awake()
		{
			DontDestroyOnLoad(gameObject);
			var levelDataCollection = JsonUtility.FromJson<LevelDataCollection>(GetLevelsData());
			LevelsData = levelDataCollection.LevelsData;

			var comboDataCollection = JsonUtility.FromJson<ComboDataCollection>(GetCombosData());
			CombosData = comboDataCollection.CombosData;
		}

		private void Start()
		{
			if (Instance == null)
			{
				Instance = this;
				dataPath = Path.Combine(Application.persistentDataPath, "data.dat");

				// DontDestroyOnLoad(gameObject);
			}

			LoadData();
			Coin = 100000;
		}

		public void LoadData()
		{
			LoadLocalData();
		}

		public void LoadLocalData()
		{
			if (File.Exists(dataPath))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
				{
					try
					{
						string data = (string)binaryFormatter.Deserialize(fileStream);
						gameData = JsonUtility.FromJson<GameData>(data);
					}
					catch (Exception e)
					{
						Debug.LogError(e.Message);
						ResetData();
					}
				}
			}
			else
				ResetData();
		}

		public void ResetData()
		{
			gameData = new GameData
			{
				SkinData = SkinDataController.Instance.InitSkinData()
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
						Debug.Log("save data");
					}


					// if (postData)
					// 	DatabaseController.Instance.PostData();
				}
			);
		}

		public int GetItemQuantity(ConsumableType consumableType)
		{
			string itemId = consumableType.ToString();
			return (from itemState in gameData.ConsumableStates where itemState.Id == itemId select itemState.Quantity)
				.FirstOrDefault();
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

		public string GetLevelsData()
		{
			// string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("piggy_bank").StringValue;
			// if (value == null || value == "")
			// {
			TextAsset data = Resources.Load<TextAsset>("LevelsData");
			return data.text;
			// }

			// return value;
		}

		public string GetCombosData()
		{
			// string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("piggy_bank").StringValue;
			// if (value == null || value == "")
			// {
			TextAsset data = Resources.Load<TextAsset>("CombosData");
			return data.text;
			// }

			// return value;
		}

		public void AddConsumable(ConsumableType consumableTypeId, int quantity)
		{
			gameData.AddConsumable(consumableTypeId, quantity);
		}

		public SkinData SkinData
		{
			get => gameData.SkinData;
			set => gameData.SkinData = value;
		}

		public int[] PurchasedBackgroundIds => gameData.PurchasedBackgroundIds;

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

		public static ConsumableType StringToItem(string value)
		{
			if (Enum.IsDefined(typeof(ConsumableType), value))
			{
				return (ConsumableType)Enum.Parse(typeof(ConsumableType), value);
			}

			Debug.LogError("Khong the convert string: \"" + value + "\" sang Item duoc -_-");
			return ConsumableType.Coin;
		}

		public static ConsumableType[] StringsToItems(string[] values)
		{
			ConsumableType[] items = new ConsumableType[values.Length];

			for (int i = 0; i < values.Length; i++)
			{
				items[i] = StringToItem(values[i]);
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
	}
}