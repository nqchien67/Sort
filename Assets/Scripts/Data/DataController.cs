using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
	public class DataController : SingletonCore<DataController>
	{
		public LevelData[] LevelsData;
		public ComboData[] CombosData;

		protected override void Awake()
		{
			base.Awake();
			// DontDestroyOnLoad(gameObject);
			var levelDataCollection = JsonUtility.FromJson<LevelDataCollection>(GetLevelsData());
			LevelsData = levelDataCollection.LevelsData;

			var comboDataCollection = JsonUtility.FromJson<ComboDataCollection>(GetCombosData());
			CombosData = comboDataCollection.CombosData;
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