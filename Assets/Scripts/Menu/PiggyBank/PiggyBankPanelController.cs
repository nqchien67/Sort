using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
using UnityEngine.UI;
using System;
using Controllers;
using Data;
using DG.Tweening;


public class PiggyBankPanelController : MonoBehaviour
{
	[SerializeField] private string packageId = "com.cooking.truckfest.food.festival.piggy1";

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

	[SerializeField] private Image _rubyBarFill;
	private float _maxFillBarLength;

	[SerializeField] private TMP_Text _rubyBarText;
	public bool ForceOpen { get; set; }

	private void Start()
	{
		_maxFillBarLength = _rubyBarFill.rectTransform.sizeDelta.x;

		backgroundAnim.Play("Appear");
		int pbRuby = DataController.Instance.PbRuby;
		rubyStorageText.text = pbRuby.ToString();
		rubyFullStorageText.text = pbRuby.ToString();
		UpdateGemBar(pbRuby, DataController.Instance.CurrentPbStorage);

		float currentFillAmount = (1f * pbRuby) / DataController.Instance.CurrentPbStorage;
		storageProgress.rectTransform.sizeDelta = new Vector2(currentFillAmount * 546f, 42f);
		buyBtn.interactable = currentFillAmount >= 0.6f;
		DataController.Instance.PbTimeDuration -=
			(int)((DataController.ConvertToUnixTime(DateTime.Now) - DataController.Instance.PbTimeStamp));
		DataController.Instance.PbTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
		StartCoroutine(CountdownPbTime());
		minRuby.text = ((int)(DataController.Instance.CurrentPbStorage * 0.6f)).ToString();
		maxRuby.text = DataController.Instance.CurrentPbStorage.ToString();
		backGround.SetActive(!DataController.Instance.IsFullPB());
		
		if (!DataController.Instance.IsFullPB())
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
				                                                 DataController.Instance.PbTimeStamp));
				DataController.Instance.PbTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
				timeleftFullText.text = String.Format("{0:D2}:{1:D2}:{2:D2}",
					DataController.Instance.PbTimeDuration / 3600, (DataController.Instance.PbTimeDuration / 60) % 60,
					DataController.Instance.PbTimeDuration % 60);

				timeLeftText.text = String.Format("{0:D2}:{1:D2}:{2:D2}", DataController.Instance.PbTimeDuration / 3600,
					(DataController.Instance.PbTimeDuration / 60) % 60, DataController.Instance.PbTimeDuration % 60);
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

	public void ClaimReward()
	{
		StartCoroutine(DelayClaimReward());
	}

	IEnumerator DelayClaimReward()
	{
		pigAnimator.AnimationState.SetAnimation(1, "jumpin", false);
		yield return new WaitForSeconds(1.5f);
		// AudioController.Instance.PlaySfx(break_piggybank);
		yield return new WaitForSeconds(0.3f);
		pigAnimator.AnimationState.SetAnimation(1, "break", false);
		yield return new WaitForSeconds(4f);
		int ruby = DataController.Instance.PbRuby;
		FindObjectOfType<MainMenuController>().IncreaseCoin(transform.position, ruby);
		DataController.Instance.Ruby += ruby;
		
		DataController.Instance.PbTimeDuration = 0;
		DataController.Instance.isFirstOpenPB = true;
		DataController.Instance.PbRuby = 0;
		DataController.Instance.PbLevel++;
		DataController.Instance.SaveData();
		FindObjectOfType<PiggyBankBtnController>().SetupData();
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

	public void UpdateGemBar(int fillAmount, int total)
	{
		float fillPercent = (float)fillAmount / total;
		
		var sizeDelta = _rubyBarFill.rectTransform.sizeDelta;
		sizeDelta.x = _maxFillBarLength * fillPercent;
		//TODO:

		_rubyBarFill.rectTransform.sizeDelta = sizeDelta;
		_rubyBarText.text = fillAmount + "/" + total;
	}
}