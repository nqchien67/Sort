using System;
using System.Collections.Generic;
using Controllers;
using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace MainMenu.DailyReward
{
	public class DailyRewardController : MonoBehaviour
	{
		[SerializeField] private GameObject _dailyRewardButton, notify, timeText;
		[SerializeField] private DailyRewardPanel _dailyRewardPanelPrefab;
		private bool _isPassLevel4;

		private void Start()
		{
			_dailyRewardButton.SetActive(_isPassLevel4);
		}

		public bool ShouldShowPanel()
		{
			_isPassLevel4 = MainMenuController.Instance.HighestPassedLevel >= 4;
			return _isPassLevel4 && CanClaimReward();
		}

		public void OnClickDailyRewardButton()
		{
			ShowDailyRewardPanel();
		}

		public void ShowDailyRewardPanel()
		{
			Instantiate(_dailyRewardPanelPrefab, MainMenuController.Instance.CameraCanvas);
		}

		public static bool CanClaimReward()
		{
			int _lastRewardDay = PlayerPrefs.GetInt("last_received_date", -1);
			bool isClaimed = _lastRewardDay == DateTime.Now.DayOfYear;
			return !isClaimed;
		}
	}
	
	[Serializable]
	public class DailyRewardCollections
	{
		[SerializeField] public List<Reward> DailyReward;
	}
}