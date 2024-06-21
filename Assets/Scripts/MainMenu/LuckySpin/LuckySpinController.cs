using System;
using Controllers;
using UnityEngine;
using Utilities;

namespace MainMenu.LuckySpin
{
	public class LuckySpinController : MonoBehaviour
	{
		[SerializeField] private LuckySpinPanel luckySpinPanelPrefab;
		[SerializeField] private AudioClip popUpClip;
		public GameObject luckySpinBtn;
		[SerializeField] private NotiDot _notiDot;
		private float clickTimeStamp;
		private float packLifeTime;
		private float packTimeStamp;
		private float timeStamp;

		private string TimeLineResetFreeSpin
		{
			get => PlayerPrefs.GetString("TimeLineResetFreeSpin", "");
			set => PlayerPrefs.SetString("TimeLineResetFreeSpin", value);
		}

		private void Start()
		{
			bool isReachLevel5 = MainMenuController.Instance.HighestPassedLevel >= 5;
			if (isReachLevel5)
			{
				luckySpinBtn.SetActive(true);

				bool haveFreeSpin = TimeLineResetFreeSpin == ""
				                    || (DateTime.Today.AddDays(1) - DateTime.Parse(TimeLineResetFreeSpin))
				                    .TotalSeconds >= 86400f;

				_notiDot.SetEnable(haveFreeSpin);
			}
			else
			{
				luckySpinBtn.SetActive(false);
			}
		}

		public void OnClickOpenPanel()
		{
			if (Time.time - clickTimeStamp < 0.5f)
				return;

			clickTimeStamp = Time.time;
			OpenPanel();
		}

		private void OpenPanel()
		{
			// AudioController.Instance.PlaySfx(popUpClip);
			var panel = Instantiate(luckySpinPanelPrefab, MainMenuController.Instance.CameraCanvas);

			panel.OnUseFreeSpin += () => _notiDot.SetEnable(false);
			panel.OnHaveFreeSpin += () => _notiDot.SetEnable(true);
		}

		public void OpenAndSpinIAP()
		{
			var luckySpinPanel = Instantiate(luckySpinPanelPrefab, MainMenuController.Instance.CameraCanvas);
			luckySpinPanel.OnSpinIAP();
			// AudioController.Instance.PlaySfx(popUpClip);
		}
	}
}