using System;
using System.Collections.Generic;
using Controllers;
using Data;
using MainMenu.DailyReward;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.DailyReward
{
	public class DailyRewardPanel : Popup
	{
		public string adsName = "x2Daily";
		[SerializeField] private AudioClip popUpClip;
		[SerializeField] private GameObject fireworkPrefab;
		[SerializeField] private AccumulateDayButton[] _accumulateButtons;
		[SerializeField] private Transform[] spawnsPos;
		[SerializeField] private Button _claimButton;
		[SerializeField] private Button _closeButton;

		[SerializeField] private Image _progressFill;
		private float _maxFillBarLength;

		private int rewardProgress, lastRewardDay;

		[SerializeField] private DailyRewardButton dailyRewardBtnPrefab;

		private DailyRewardObjects dailyRewards;
		private int weeklyRewardProgress, lastReceivedDate;

		private DailyRewardButton _rewardCanClaim;

		private void Start()
		{
			_maxFillBarLength = _progressFill.rectTransform.sizeDelta.x;

			dailyRewards = LoadDailyRewardData();
			// weeklyRewardProgress = PlayerPrefs.GetInt("weekly_reward_progress", 0);
			// dailyRewards = GetDailyRewardObjectsByWeek(weeklyRewardProgress);
			int receivedProgress = PlayerPrefs.GetInt("received_progress", 0);
			if (receivedProgress == 0)
			{
				PlayerPrefs.SetInt("last_received_date", DateTime.Now.DayOfYear - 1);
			}

			// bool canDouble = AdsController.Instance.CanShowAds(adsName);
			bool canDouble = true;
			for (int i = 0; i < dailyRewards.DailyReward.Count; i++)
			{
				DailyRewardButton dailyRewardBtn = Instantiate(dailyRewardBtnPrefab, spawnsPos[i].position,
					Quaternion.identity, spawnsPos[i]);

				RewardType[] items = DataController.StringsToConsumable(dailyRewards.DailyReward[i].RewardTypes);
				dailyRewardBtn.Init(items, dailyRewards.DailyReward[i].Quantities, i + 1, receivedProgress, canDouble,
					Close /*,dailyRewardExtra.itemId*/);

				if (dailyRewardBtn.canClaim && _rewardCanClaim == null)
					_rewardCanClaim = dailyRewardBtn;
			}

			_claimButton.gameObject.SetActive(DailyRewardController.CanClaimReward());
			_closeButton.gameObject.SetActive(!DailyRewardController.CanClaimReward());
			// AudioController.Instance.PlaySfx(popUpClip);
			// UIController.Instance.PushUitoStack(this);
			InitAccumulation();

			Show();
		}


		public void Claim()
		{
			_rewardCanClaim.OnClick();
		}

		private DailyRewardObjects LoadDailyRewardData()
		{
			var data = FirebaseServiceController.Instance.GetDailyRewardData();
			return JsonUtility.FromJson<DailyRewardObjects>(data);
		}

		public DailyRewardObjects GetDailyRewardObjectsByWeek(int weekIndex)
		{
			DailyRewardObjects drObjects = new DailyRewardObjects();
			int count = 7;
			weekIndex %= 4;
			for (int i = weekIndex * 7; i < dailyRewards.DailyReward.Count; i++)
			{
				if (count > 0)
				{
					drObjects.DailyReward.Add(dailyRewards.DailyReward[i]);
					count--;
				}
				else
					break;
			}

			return drObjects;
		}

		private void InitAccumulation()
		{
			int dayAccumulated = PlayerPrefs.GetInt("dayAccumulated", 0);
			foreach (var button in _accumulateButtons)
			{
				button.Init(dayAccumulated);
			}

			UpdateProgressBar(dayAccumulated, 28);
		}

		private void UpdateProgressBar(int fillAmount, int total)
		{
			float fillPercent = (float)fillAmount / total;
			Vector2 sizeDelta = _progressFill.rectTransform.sizeDelta;
			sizeDelta.x = _maxFillBarLength * fillPercent;
			_progressFill.rectTransform.sizeDelta = sizeDelta;
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			Destroy(gameObject);
		}

		public void ResetAccumulatedDay()
		{
			PlayerPrefs.SetInt("dayAccumulated", 0);

			foreach (var button in _accumulateButtons)
			{
				button.ResetClaimed();
			}
		}
	}
}