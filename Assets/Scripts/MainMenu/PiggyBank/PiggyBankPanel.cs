using System;
using System.Collections;
using Audio;
using Controllers;
using Data;
using InGame.UI;
using Spine.Unity;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.PiggyBank
{
	public class PiggyBankPanel : Popup
	{
		// [SerializeField] private string packageId = "com.cooking.truckfest.food.festival.piggy1";

		[SerializeField]
		private TextMeshProUGUI rubyStorageText, rubyFullStorageText, timeLeftText, timeleftFullText, minRuby, maxRuby;

		[SerializeField] private Image storageProgress;
		[SerializeField] private SkeletonGraphic pigAnimator;
		[SerializeField] private Animator backgroundAnim;

		[SerializeField] private Button buyBtn;

		// [SerializeField] private CookingIAPButton buyBtnIAPFully;
		// [SerializeField] private CookingIAPButton buyBtnIAP;
		[SerializeField] public GameObject backGround, rewardLayer, inforLayer, fullPiggyPanel;
		[SerializeField] private AudioClip break_piggybank;

		[SerializeField] private Image[] _coinBarFills;
		private float _maxFillBarLength;

		[SerializeField] private TextMeshProUGUI[] _coinBarTexts;
		public bool ForceOpen { get; set; }

		private void Start()
		{
			_maxFillBarLength = _coinBarFills[0].rectTransform.sizeDelta.x;

			backgroundAnim.Play("Appear");
			int pbCoin = DataController.Instance.PiggyBankCoin;
			rubyStorageText.text = pbCoin.ToString();
			rubyFullStorageText.text = pbCoin.ToString();

			UpdateCoinProgressBar(pbCoin, DataController.Instance.CurrentPbStorage);

			float currentFillPercent = (float)pbCoin / DataController.Instance.CurrentPbStorage;
			buyBtn.interactable = currentFillPercent >= 0.6f;

			DataController.Instance.PbTimeDuration -=
				(int)(DataController.ConvertToUnixTime(DateTime.Now) - DataController.Instance.PiggyBankTimeStamp);
			DataController.Instance.PiggyBankTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
			StartCoroutine(CountdownPbTime());
			minRuby.text = ((int)(DataController.Instance.CurrentPbStorage * 0.6f)).ToString();
			maxRuby.text = DataController.Instance.CurrentPbStorage.ToString();
			backGround.SetActive(!DataController.Instance.IsPiggyBankFull());

			if (!DataController.Instance.IsPiggyBankFull())
			{
				fullPiggyPanel.SetActive(false);
				return;
			}

			// var latestPassedLevel = LevelDataController.Instance.lastestPassedLevel;
			// if (latestPassedLevel == null)
			// {
			// 	fullPiggyPanel.SetActive(true);
			// 	return;
			// }

			var latestPassedLevel = PlayerPrefs.GetInt("level", 0);

			if (latestPassedLevel != 6)
			{
				fullPiggyPanel.SetActive(true);
			}
		}

		private IEnumerator CountdownPbTime()
		{
			var delay = new WaitForSeconds(1);
			while (DataController.Instance.PbTimeDuration > 0)
			{
				if (DataController.Instance.PbTimeDuration > 0)
				{
					DataController.Instance.PbTimeDuration -= (int)((DataController.ConvertToUnixTime(DateTime.Now) -
					                                                 DataController.Instance.PiggyBankTimeStamp));
					DataController.Instance.PiggyBankTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
					timeleftFullText.text = String.Format("{0:D2}:{1:D2}:{2:D2}",
						DataController.Instance.PbTimeDuration / 3600,
						(DataController.Instance.PbTimeDuration / 60) % 60,
						DataController.Instance.PbTimeDuration % 60);

					timeLeftText.text = String.Format("{0:D2}:{1:D2}:{2:D2}",
						DataController.Instance.PbTimeDuration / 3600,
						(DataController.Instance.PbTimeDuration / 60) % 60,
						DataController.Instance.PbTimeDuration % 60);
				}
				else
				{
					timeleftFullText.gameObject.SetActive(false);
					timeLeftText.gameObject.SetActive(false);
				}

				yield return delay;
			}
		}

		public void OnClickBuy()
		{
			backGround.SetActive(false);
			fullPiggyPanel.SetActive(false);
			rewardLayer.SetActive(true);
			ClaimReward();
		}

		public void OnClickAddCoinAds()
		{
			DataController.Instance.PiggyBankCoin += 200;
			UpdateCoinProgressBar(DataController.Instance.PiggyBankCoin, DataController.Instance.CurrentPbStorage);
		}

		public void ClaimReward()
		{
			StartCoroutine(DelayClaimReward());
		}

		private IEnumerator DelayClaimReward()
		{
			pigAnimator.AnimationState.SetAnimation(1, "jumpin", false);
			yield return new WaitForSeconds(1.5f);
			AudioController.Instance.PlaySfx(break_piggybank);
			yield return new WaitForSeconds(0.3f);
			pigAnimator.AnimationState.SetAnimation(1, "break", false);
			yield return new WaitForSeconds(4f);
			int coin = DataController.Instance.PiggyBankCoin;

			MainMenuController.Instance.PlayClaimCoinEffect(transform.position, Mathf.Min(coin / 100, 35));
			DataController.Instance.Coin += coin;

			DataController.Instance.PbTimeDuration = 0;
			DataController.Instance.initedPiggy = false;
			DataController.Instance.PiggyBankCoin = 0;
			DataController.Instance.PiggyBankLevel++;
			DataController.Instance.SaveData();

			PiggyBankController piggyBankController = FindObjectOfType<PiggyBankController>();
			piggyBankController.SetupData();
			// APIController.Instance.LogEventEarnRuby(ruby, "buy_IAP");
			// CookingIAPButton iapBtn;
			// if (fullPiggyPanel.activeSelf == true)
			// 	iapBtn = buyBtnIAPFully;
			// else
			// 	iapBtn = buyBtnIAP;
			// // IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", "source", iapBtn.productId, 1);
			// // string request = JsonUtility.ToJson(iapMessageLog);
			// // APIController.Instance.LogMessage(iapMessageLog);
			// APIController.Instance.LogEventPiggyBankTracking(DataController.Instance.PbLevel, ruby);
			Destroy(gameObject);
			if (PlayerPrefs.GetInt("isShowFullPiggy") == 1)
			{
				PlayerPrefs.SetInt("isShowFullPiggy", 0);
			}

			piggyBankController.DisableFullText();
		}

		public void OnClickInforBtn()
		{
			inforLayer.SetActive(true);
		}

		public void OnHideInforLayer()
		{
			inforLayer.SetActive(false);
		}

		public void OnHide()
		{
			backgroundAnim.Play("Disappear");
			StartCoroutine(DelayHide());
			if (PlayerPrefs.GetInt("isShowFullPiggy") == 1)
			{
				PlayerPrefs.SetInt("isShowFullPiggy", 0);
			}
		}

		private IEnumerator DelayHide()
		{
			yield return new WaitForSeconds(0.2f);
			Destroy(gameObject);
		}

		private void UpdateCoinProgressBar(int fillAmount, int total)
		{
			float fillPercent = (float)fillAmount / total;
			fillPercent = Mathf.Min(fillPercent, 1);

			for (int i = 0; i < 2; i++)
			{
				Vector2 sizeDelta = _coinBarFills[i].rectTransform.sizeDelta;
				sizeDelta.x = _maxFillBarLength * fillPercent;

				_coinBarFills[i].rectTransform.sizeDelta = sizeDelta;
				_coinBarTexts[i].text = fillAmount + "/" + total;
			}
		}

		public void Init(IAPData iapData)
		{
			// GetComponent<>()
			buyBtn.GetComponentInChildren<TextMeshProUGUI>().text = iapData.Price.ToString();
		}
	}
}