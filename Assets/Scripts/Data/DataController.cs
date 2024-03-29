using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
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
		}
		
		public int Ruby
		{
			get => gameData.ruby;
			set
			{
				gameData.ruby = value;
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
		
		public bool isFirstOpenPB = true;
		public int PbTimeDuration
		{
			get => gameData.pbTimeDuration;
			set => gameData.pbTimeDuration = value;
		}

		public double PbTimeStamp
		{
			get => gameData.pbTimeStamp;
			set => gameData.pbTimeStamp = value;
		}

		public int PbRuby
		{
			get { return gameData.pbRuby; }
			set { gameData.pbRuby = Mathf.Clamp(value, 0, pbStorageMilestone[PbLevel - 1]); }
		}

		public int[] pbStorageMilestone
		{
			get { return new int[] { 120, 200, 340, 700, 1600, 4000 }; }
		}

		public int PbLevel
		{
			get
			{
				if (gameData.pbLevel == 0) gameData.pbLevel = 1;
				return Mathf.Clamp(gameData.pbLevel, 1, 6);
			}
			set => gameData.pbLevel = Mathf.Clamp(value, 1, 6);
		}
		
		public int CurrentPbStorage => pbStorageMilestone[PbLevel - 1];
		
		public bool IsFullPB()
		{
			return gameData.pbRuby >= pbStorageMilestone[PbLevel - 1];
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
	}
}