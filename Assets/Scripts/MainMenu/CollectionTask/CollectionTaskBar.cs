using System;
using System.Collections;
using Controllers;
using Data;
using TMPro;
using UnityEngine;

namespace MainMenu.CollectionTask
{
	public class CollectionTaskBar : SingletonCore<CollectionTaskBar>
	{
		[SerializeField] private CollectionTaskPanel _panel;

		[SerializeField] private CTProgressBar _progressBar;
		public TextMeshProUGUI Timer;
		[SerializeField] private NotiDot _notiDot;

		private float ResetTaskTimeStamp
		{
			get => PlayerPrefs.GetFloat(CollectionTaskController.ResetTaskTimeStamp_Hash, 0);
			set => PlayerPrefs.SetFloat(CollectionTaskController.ResetTaskTimeStamp_Hash, value);
		}

		private float _timeRemain;

		private void Start()
		{
			if (!CanShowBar())
			{
				gameObject.SetActive(false);
				return;
			}

			if (!CollectionTaskController.Instance.IsStarted)
				CollectionTaskController.Instance.StartFirstTask();

			InitProgressBar();

			_notiDot = GetComponentInChildren<NotiDot>();
			CheckAndShowNotiDot();

			StartCoroutine(CountDownTime());
		}

		private void InitProgressBar()
		{
			CTTaskData current = CollectionTaskController.Instance.Current;
			_progressBar.Init(current);
		}

		public void CheckAndShowNotiDot()
		{
			_notiDot.SetEnable(CollectionTaskController.Instance.HaveUnclaimedReward());
		}

		private bool CanShowBar()
		{
			bool isPassLevel4 = MainMenuController.Instance.HighestPassedLevel >= 3;
			return isPassLevel4;
		}

		public void ShowPanel()
		{
			_panel.Show();
		}

		private IEnumerator CountDownTime()
		{
			_timeRemain = ResetTaskTimeStamp - (float)DataController.ConvertToUnixTime(DateTime.Now);

			var waitForSecond = new WaitForSeconds(1);
			while (_timeRemain > 0)
			{
				Timer.text = DataController.SecondToHours(_timeRemain) + ":" +
				              DataController.SecondsToMinutes(_timeRemain) + ":" +
				              DataController.GetSeconds(_timeRemain);
				yield return waitForSecond;
				_timeRemain--;
			}

			ResetProgresses();
		}

		private void ResetProgresses()
		{
			CollectionTaskController.Instance.ResetProgresses();
			StartCoroutine(CountDownTime());
			CheckAndShowNotiDot();
		}
	}
}