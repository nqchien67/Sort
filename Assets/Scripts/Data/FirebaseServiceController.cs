using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Data
{
	public class FirebaseServiceController : SingletonCore<FirebaseServiceController>
	{
		public bool isTestMode;

		private bool hasCompleteInitFirebase;
		// private FirebaseAuth firebaseAuth;
		// public RemoteCMPData remoteCMPData;

		protected override void Awake()
		{
			base.Awake();
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
			// Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
			// {
			// 	var dependencyStatus = task.Result;
			// 	if (dependencyStatus == Firebase.DependencyStatus.Available)
			// 	{
			// 		firebaseAuth = FirebaseAuth.DefaultInstance;
			// 		InitFirebase();
			// 	}
			// 	else
			// 	{
			// 		Debug.LogError(
			// 			"Could not resolve all Firebase dependencies: " + dependencyStatus);
			// 	}
			// });
		}

		// private void InitFirebase()
		// {
		// 	Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(new Dictionary<string, object>());
		// 	System.Threading.Tasks.Task fetchTask =
		// 		Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
		// 	fetchTask.ContinueWithOnMainThread(OnFetchCompleted);
		// }

		// private void OnFetchCompleted(Task fetchTask)
		// {
		// 	if (fetchTask.IsFaulted)
		// 	{
		// 		//hasCompleteInitFirebase = true;
		// 		Debug.LogError("cant fetch remote config");
		// 		Debug.Log(fetchTask.Exception);
		// 	}
		// 	else if (fetchTask.IsCompleted)
		// 	{
		// 		APIController.Instance.SetUserProperty();
		// 		// StartCoroutine(SetUserProperty());
		// 		Debug.Log("Fetch completed successfully!");
		// 	}
		//
		// 	remoteCMPData = JsonUtility.FromJson<RemoteCMPData>(GetRemoteCMPData());
		//
		// 	SceneManager.LoadScene("Login");
		// 	var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
		// 	switch (info.LastFetchStatus)
		// 	{
		// 		case Firebase.RemoteConfig.LastFetchStatus.Success:
		// 			Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
		// 				.ContinueWithOnMainThread(task =>
		// 				{
		// 					Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
		// 						info.FetchTime));
		// 				});
		//
		// 			break;
		// 		case Firebase.RemoteConfig.LastFetchStatus.Failure:
		// 			switch (info.LastFetchFailureReason)
		// 			{
		// 				case Firebase.RemoteConfig.FetchFailureReason.Error:
		// 					Debug.Log("Fetch failed for unknown reason");
		// 					break;
		// 				case Firebase.RemoteConfig.FetchFailureReason.Throttled:
		// 					Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
		// 					break;
		// 			}
		//
		// 			break;
		// 		case Firebase.RemoteConfig.LastFetchStatus.Pending:
		// 			Debug.Log("Latest Fetch call still pending.");
		// 			break;
		// 	}
		// }

		// private IEnumerator SetUserProperty()
		// {
		//     yield return new WaitForSeconds(0.5f);
		//     string first_date = PlayerPrefs.GetString("first_date", "");
		//     if (first_date == "")
		//     {
		//         APIController.Instance.SetProperty("retent_type", "0");
		//         APIController.Instance.SetProperty("days_played", "1");
		//         PlayerPrefs.SetInt("days_played", 2);
		//         PlayerPrefs.SetString("first_date", DataController.ConvertToUnixTime(DateTime.UtcNow).ToString());
		//     }
		//     else
		//     {
		//         int days_played = PlayerPrefs.GetInt("days_played", 1);
		//         DateTime retent_time = DataController.ConvertFromUnixTime(Convert.ToDouble(first_date));
		//         int retent_date = PlayerPrefs.GetInt("retent_date", 0);
		//         int totalDates = (DateTime.UtcNow - retent_time).Days;
		//         if (Mathf.Abs(totalDates) > retent_date)
		//         {
		//             APIController.Instance.SetProperty("days_played", days_played.ToString());
		//             APIController.Instance.SetProperty("retent_type", totalDates.ToString());
		//             PlayerPrefs.SetInt("retent_date", totalDates);
		//             PlayerPrefs.SetInt("days_played", days_played++);
		//         }
		//     }
		// }


		// public FirebaseAuth GetFirebaseAuth()
		// {
		// 	return firebaseAuth;
		// }

		public string GetLevelsData()
		{
			// string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("piggy_bank").StringValue;
			// if (value == null || value == "")
			// {
			TextAsset data = Resources.Load<TextAsset>("level_data");
			return data.text;
			// }

			// return value;
		}

		public string GetCombosData()
		{
			// string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("piggy_bank").StringValue;
			// if (value == null || value == "")
			// {
			TextAsset data = Resources.Load<TextAsset>("combo_data");
			return data.text;
			// }

			// return value;
		}

		// public string GetCheckInternetConfigDataData()
		// {
		// 	string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("internet_config")
		// 		.StringValue;
		// 	if (value == null || value == "")
		// 	{
		// 		TextAsset data = null;
		// 		data = Resources.Load<TextAsset>("internet_config");
		// 		return data.text;
		// 	}
		//
		// 	return value;
		// }

		// public string GetRemoteCMPData()
		// {
		// 	string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("remote_CMP")
		// 		.StringValue;
		// 	if (value == null || value == "")
		// 	{
		// 		TextAsset data = null;
		// 		data = Resources.Load<TextAsset>("remote_CMP");
		// 		return data.text;
		// 	}
		//
		// 	return value;
		// }

		public string GetShopPacksData()
		{
			// string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("iap_packs").StringValue;
			// if (value == null || value == "")
			// {
			TextAsset data = null;
			data = Resources.Load<TextAsset>("shop_packs");
			return data.text;
			// }
			//
			// return value;
		}

		// public string GetNewUpdateData()
		// {
		// 	string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("new_update")
		// 		.StringValue;
		// 	if (value == null || value == "")
		// 	{
		// 		TextAsset data = null;
		// 		data = Resources.Load<TextAsset>("new_update");
		// 		return data.text;
		// 	}
		//
		// 	return value;
		// }

		// public string GetPiggyBankData()
		// {
		// 	string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("piggy_bank")
		// 		.StringValue;
		// 	if (value == null || value == "")
		// 	{
		// 		TextAsset data = null;
		// 		data = Resources.Load<TextAsset>("piggy_bank");
		// 		return data.text;
		// 	}
		//
		// 	return value;
		// }

		// public string GetVersionApp()
		// {
		// 	string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("version_data")
		// 		.StringValue;
		// 	if (value == null || value == "")
		// 	{
		// 		TextAsset data = null;
		// 		data = Resources.Load<TextAsset>("version_data");
		// 		return data.text;
		// 	}
		//
		// 	return value;
		// }

		// public string GetDeviceId()
		// {
		// 	// TextAsset data = null;
		// 	// data = Resources.Load<TextAsset>("deviceid_data");
		// 	// return data.text;
		// 	string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("deviceid_data")
		// 		.StringValue;
		// 	if (string.IsNullOrEmpty(value))
		// 	{
		// 		TextAsset data = null;
		// 		data = Resources.Load<TextAsset>("deviceid_data");
		// 		return data.text;
		// 	}
		//
		// 	return value;
		// }

		public string GetDailyRewardData()
		{
			// string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("daily_reward_data")
			// 	.StringValue;
			// if (string.IsNullOrEmpty(value))
			// {
			TextAsset data = null;
			data = Resources.Load<TextAsset>("daily_reward_data");
			return data.text;
			// }
			//
			// return value;
		}

		public string GetCollectionTaskData()
		{
			// string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("collection_task_data")
			// 	.StringValue;
			// if (string.IsNullOrEmpty(value))
			// {
			TextAsset data = null;
			data = Resources.Load<TextAsset>("collection_task_data");
			return data.text;
			// }
			//
			// return value;
		}

		public string GetTopChartsRewards()
		{
			TextAsset data = Resources.Load<TextAsset>("top_charts_rewards");
			return data.text;
		}

		public string GetEndlessTreasureData()
		{
			var data = Resources.Load<TextAsset>("endless_treasure_data");
			return data.text;
		}
	}
}