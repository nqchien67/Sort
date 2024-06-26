using System;
using System.Collections.Generic;
using Audio;
using Data;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MainMenu.LuckySpin
{
	public class LuckySpinPanel : Popup
	{
		public int numberSpin;
		public int timeToFreeSpin;
		public Transform wheel;
		public Button exitButton, freeButton, adsButton, iapButton;
		public TextMeshProUGUI timeToFreeSpinText;
		public LuckySpinItem finalItem;
		public List<LuckySpinItem> luckySpinItems = new List<LuckySpinItem>();

		[SerializeField] private TextMeshProUGUI _fakeAdsCountText;
		public AudioClip spinClip;

		[SerializeField] private Image _wheelImage;
		[SerializeField] private Sprite _adsWheelSprite;
		private Sprite _freeWheelSprite;

		[SerializeField] private GameObject _tutorial;
		public UnityAction OnHaveFreeSpin;
		public UnityAction OnUseFreeSpin;

		private string TimeLineResetFreeSpin
		{
			get => PlayerPrefs.GetString("TimeLineResetFreeSpin", "");
			set => PlayerPrefs.SetString("TimeLineResetFreeSpin", value);
		}

		private int AdsCount
		{
			get => PlayerPrefs.GetInt("FreeSpinAdsCount", 0);
			set => PlayerPrefs.SetInt("FreeSpinAdsCount", value);
		}

		private void Start()
		{
			_freeWheelSprite = _wheelImage.sprite;

			SwitchAdsAndFreeButton();
			GetComponent<Animator>().Play("Appear");
			// UIController.Instance.PushUitoStack(this);

			string timeLineResetFreeSpin = TimeLineResetFreeSpin;
			if (timeLineResetFreeSpin != "")
				if ((DateTime.Today.AddDays(1) - DateTime.Parse(timeLineResetFreeSpin)).TotalSeconds < 86400f)
				{
					timeToFreeSpin = (int)(DateTime.Parse(timeLineResetFreeSpin) - DateTime.Now).TotalSeconds;
					StartCountDownTimeToSpinFree();
				}

			if (PlayerPrefs.GetInt("level", 0) == 5 && PlayerPrefs.GetInt("LuckySpinPanelTutorial", 0) != 1)
				_tutorial.SetActive(true);

			ChangeWheelAndRewardValue();
		}

		private void SwitchAdsAndFreeButton()
		{
			if (TimeLineResetFreeSpin == "")
			{
				freeButton.gameObject.SetActive(true);
				adsButton.gameObject.SetActive(false);

				timeToFreeSpinText.gameObject.SetActive(false);
			}
			else
			{
				if ((DateTime.Today.AddDays(1) - DateTime.Parse(TimeLineResetFreeSpin)).TotalSeconds >= 86400f)
				{
					freeButton.gameObject.SetActive(true);
					adsButton.gameObject.SetActive(false);

					timeToFreeSpinText.gameObject.SetActive(false);
				}
				else
				{
					freeButton.gameObject.SetActive(false);
					adsButton.gameObject.SetActive(true);

					timeToFreeSpinText.gameObject.SetActive(true);
				}
			}

			_fakeAdsCountText.gameObject.SetActive(adsButton.gameObject.activeSelf);
			_fakeAdsCountText.text = $"({AdsCount}/3)";
		}

		public void OnSpin(Action onComplete = null)
		{
			AudioController.Instance.PlaySfx(spinClip);
			exitButton.enabled = false;
			freeButton.enabled = false;
			adsButton.enabled = false;
			iapButton.enabled = false;
			float probability = Random.Range(0f, 100f);
			for (int i = 0; i < luckySpinItems.Count; i++)
				if (probability <= luckySpinItems[i].probability)
				{
					finalItem = luckySpinItems[i];
					break;
				}
				else
				{
					probability -= luckySpinItems[i].probability;
				}

			finalItem.OnCollect();
			float timeRotate = 0.4f * (numberSpin + finalItem.rotationZ / 360f);
			wheel.DOLocalRotate(new Vector3(0, 0, -360f * numberSpin - finalItem.rotationZ), timeRotate)
				.SetEase(Ease.OutQuint)
				.OnComplete(() =>
				{
					exitButton.enabled = true;
					freeButton.enabled = true;
					adsButton.enabled = true;
					iapButton.enabled = true;
					finalItem.OnOpenRewardPanel();
					onComplete?.Invoke();
				});
		}

		public void OnSpinFree()
		{
			ResetTimeFreeSpin();
			SwitchAdsAndFreeButton();
			OnSpin(ChangeWheelAndRewardValue);
			OnUseFreeSpin?.Invoke();
		}

		private void ResetTimeFreeSpin()
		{
			TimeLineResetFreeSpin = DateTime.Today.AddDays(1).ToString();
			timeToFreeSpin = (int)(DateTime.Today.AddDays(1) - DateTime.Now).TotalSeconds;
			StartCountDownTimeToSpinFree();
		}

		public void OnSpinAds()
		{
			// bool isWatchedAds = false;
			// AdsController.Instance.ShowVideoReward
			// (
			//     () =>
			//     {
			//         isWatchedAds = true;
			//     },
			//     () =>
			//     {
			//         if (isWatchedAds)
			//         {
			OnSpin();

			int adsCount = AdsCount;
			adsCount++;
			if (adsCount >= 3)
				adsCount = 0;
			_fakeAdsCountText.text = $"({adsCount}/3)";
			AdsCount = adsCount;

			//             GSMController.Instance.AdsLogReward(DataController.Instance.GetMaxPassedLevelToInt(), "lucky_spin_ads");
			//             isWatchedAds = false;
			//         }
			//     },
			//     "lucky_spin_ads"
			// );
			// APIController.Instance.LogEventAdsClick("Lucky_Spin_Ads");
		}

		public void OnSpinIAP()
		{
			OnSpin();
		}

		private void CounterTimeToSpinFree()
		{
			timeToFreeSpin--;
			timeToFreeSpinText.text = DataController.SecondToHours(timeToFreeSpin) + "h:" +
			                          DataController.SecondsToMinutes(timeToFreeSpin) + "m:" +
			                          DataController.GetSeconds(timeToFreeSpin) + "s";
			if (timeToFreeSpin == 0)
			{
				StopCountDownTimeToSpinFree();
				freeButton.gameObject.SetActive(true);
				adsButton.gameObject.SetActive(false);
				timeToFreeSpinText.gameObject.SetActive(false);

				ChangeWheelAndRewardValue();
				OnHaveFreeSpin?.Invoke();
			}
		}

		public void StartCountDownTimeToSpinFree()
		{
			InvokeRepeating(nameof(CounterTimeToSpinFree), 0, 1);
		}

		public void StopCountDownTimeToSpinFree()
		{
			CancelInvoke(nameof(CounterTimeToSpinFree));
		}

		public void OnHide()
		{
			// StartCoroutine(DelayClosePanel());
			// UIController.Instance.PopUiOutStack();
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			Destroy(gameObject);
		}

		public void OnClickContinueTut()
		{
			OnSpinFree();
			Destroy(_tutorial);
			PlayerPrefs.SetInt("LuckySpinPanelTutorial", 1);
		}

		private void ChangeWheelAndRewardValue()
		{
			if (adsButton.gameObject.activeSelf)
			{
				_wheelImage.sprite = _adsWheelSprite;
				foreach (LuckySpinItem item in luckySpinItems)
					item.SetAdsRewardValue();
			}
			else
			{
				_wheelImage.sprite = _freeWheelSprite;
				foreach (LuckySpinItem item in luckySpinItems)
					item.SetFreeRewardValue();
			}
		}
	}
}