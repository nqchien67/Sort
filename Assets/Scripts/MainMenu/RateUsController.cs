using System.Collections;
using System.Collections.Generic;
using Audio;
using InGame.UI;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class RateUsController : Popup
{
	[SerializeField] private GameObject RateusPanel, feedBackPanel;
	[SerializeField] private GameObject[] stars;
	[SerializeField] private AudioClip popUpClip;
	[SerializeField] private Button continueBtn;
	string rateconfig;
#if UNITY_ANDROID
	private string storeUrl = "market://details?id=com.cooking.truckfest.food.festival";
#elif UNITY_IOS
        private string storeUrl = "itms-apps://itunes.apple.com/app/id1642259256";
#endif
	private int currentStar = 0;

	private void Start()
	{
		//    GetComponent<Animator>().Play("Appear");
		//    AudioController.Instance.PlaySfx(popUpClip);
		//    currentStar = 0;
		//    continueBtn.gameObject.SetActive(false);
	}

	public override void Show()
	{
		RateusPanel.SetActive(true);
		// rateconfig = FirebaseServiceController.Instance.GetRateConfig();
		if (rateconfig == "0")
			GetComponent<Animator>().Play("Appear");
		else
		{
			// GetComponent<Animator>().Play("Appear1");
			GetComponent<Animator>().Play("Appear");
		}

		AudioController.Instance.PlaySfx(popUpClip);
		continueBtn.gameObject.SetActive(false);
	}

	public void Spawn(bool forceSpawn = false)
	{
		if (PlayerPrefs.GetInt("rating", 0) < 5 || forceSpawn)
			Show();
		RateusPanel.GetComponent<CanvasGroup>().alpha = 0;
	}

	public void OnClickStar(int star)
	{
		currentStar = star;
		for (int i = 0; i < stars.Length; i++)
			stars[i].SetActive(i < currentStar);
		GetComponent<Animator>().Play("BtnAppear");
	}

	public void OnClickContinue()
	{
#if UNITY_ANDROID
		PlayerPrefs.SetInt("rating", currentStar);
		if (currentStar < 5)
		{
			feedBackPanel.SetActive(true);
			feedBackPanel.GetComponent<Animator>().Play("Appear");
			// var feedbackController = feedBackPanel.GetComponent<FeedbackController>();
			// if (feedbackController != null)
			// 	UIController.Instance.PushUitoStack(feedbackController);
		}
		else
			Application.OpenURL(storeUrl);
#elif UNITY_IOS
        PlayerPrefs.SetInt("rating", currentStar);
        Application.OpenURL(storeUrl);
#endif
		GetComponent<Animator>().Play("Disappear");
		RateusPanel.SetActive(false);
	}

	public void ShowFeedBack()
	{
		feedBackPanel.SetActive(true);
		StartCoroutine(DelayOnHide());
	}

	IEnumerator DelayOnHide()
	{
		yield return new WaitForSeconds(0.2f);
		Close();
	}

	public override void Close()
	{
		if (rateconfig == "0")
			GetComponent<Animator>().Play("Disappear");
		else
			GetComponent<Animator>().Play("Disappear1");
		RateusPanel.SetActive(false);
	}

	public void OnClickLater()
	{
		GetComponent<Animator>().Play("Disappear");
		RateusPanel.SetActive(false);
	}

	public void OnClickOpenStore()
	{
		Application.OpenURL(storeUrl);
		Close();
	}
}