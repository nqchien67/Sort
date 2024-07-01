using Audio;
using InGame.UI;
using MainMenu.Setting.Profile;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.Setting
{
	public class SettingPanel : Popup
	{
		[SerializeField] private LanguagesPanel languagePanelPrefab;

		[SerializeField] private GameObject settingPanel,
			newVersionNotification,
			saveProcessPanelPrefab /*, settingNotification, btnLanguage, SignOutAppleIDBtn*/,
			BtnFBLogin,
			BtnFBLogged;

		[SerializeField] private RateUsController rateUsPrefab;
		[SerializeField] private AudioClip popUpClip;
		[SerializeField] private GameObject noInternetPanel, GiftCodePanel;
		[SerializeField] private TMP_InputField ChapterInf, LevelInf;
		[SerializeField] Image musicIcon, sfxIcon, vibrationIcon;
		[SerializeField] private GameObject TestGroup;
		[SerializeField] Sprite musicIconOn, musicIconOff, sfxIconOn, sfxIconOff, vibrationIconOn, vibrationIconOff;
		[SerializeField] private GameObject GiftCode;
		[SerializeField] TextMeshProUGUI textLanguage;
		private AudioController audioController;
		bool isIOS, isShowRate, isShowFeedBack;
		private const string _policyURL = "";

		// private EditProfilePanel _editProfilePanel;
		
		protected override void Awake()
		{
			base.Awake();
			audioController = AudioController.Instance;
			// _editProfilePanel = GetComponent<EditProfilePanel>();
		}

		public override void Show()
		{
			base.Show();
			
			// newVersionNotification.SetActive(DataController.Instance.CheckVersion());
			musicIcon.sprite = audioController.Music ? musicIconOn : musicIconOff;
			sfxIcon.sprite = audioController.SFX ? sfxIconOn : sfxIconOff;
			vibrationIcon.sprite = audioController.Vibration ? vibrationIconOn : vibrationIconOff;
			// UpdateLanguage();
			AudioController.Instance.PlaySfx(popUpClip);
			// if (TestGroup != null)
			// {
			//     TestGroup.gameObject.SetActive(DataController.Instance.deviceIdData.CheckDeviceId(DataController.Instance.GetGameData().userID) || DataController.Instance.deviceIdData.CheckDeviceId(SystemInfo.deviceUniqueIdentifier));
			// }
		}
		
		public void ChangeMusic()
		{
			if (audioController.Music)
			{
				audioController.Music = false;
				musicIcon.sprite = musicIconOff;
			}
			else
			{
				audioController.Music = true;
				musicIcon.sprite = musicIconOn;
			}
		}

		public void ChangeSfx()
		{
			if (audioController.SFX)
			{
				audioController.SFX = false;
				sfxIcon.sprite = sfxIconOff;
			}
			else
			{
				audioController.SFX = true;
				sfxIcon.sprite = sfxIconOn;
			}
		}

		public void ChangeVibration()
		{
			if (audioController.Vibration)
			{
				audioController.Vibration = false;
				vibrationIcon.sprite = vibrationIconOff;
			}
			else
			{
				audioController.Vibration = true;
				vibrationIcon.sprite = vibrationIconOn;
			}
		}

		public void OnClickSaveProcess()
		{
			// if (Application.internetReachability != NetworkReachability.NotReachable)
			// {
			//     Instantiate(saveProcessPanelPrefab, transform.parent).GetComponent<LoginPanelController>();
			// }
			// else
			// {
			//     Instantiate(noInternetPanel, transform.parent);
			// }
		}

		public void OnClickLogOut()
		{
			// DatabaseController.Instance.LogOutFacebook();
			// BtnFBLogin.SetActive(true);
			// BtnFBLogged.SetActive(false);
			// SceneController.Instance.LoadScene("MainMenu", false);
		}

		public void OnClickLanguageBtn()
		{
			// languagePanelPrefab.OnShow(transform.parent);
			//Instantiate(languagePanelPrefab, transform.parent);
		}

		public void OnClickRateUs()
		{
			// FindObjectOfType<MainMenuController>().UIRateUsController.Spawn(transform.parent, true);
			 rateUsPrefab.Spawn(true);
		}

		public void OnClickContactUs()
		{
			// Application.OpenURL("mailto:" + FeedbackController.EMAIL_ADDRESS + "?subject=" + FeedbackController.SUBJECT + DataController.Instance.GetGameData().userID + "_" + SystemInfo.deviceUniqueIdentifier + "_" + SystemInfo.deviceModel + "_" + Application.version +"_"+ APIController.Instance.TypeUser()+ "&body=" + FeedbackController.BODY);
		}

		public void OnClickNewVersion()
		{
#if UNITY_ANDROID
			Application.OpenURL("market://details?id=com.cooking.truckfest.food.festival");
#elif UNITY_IOS
        Application.OpenURL("itms-apps://itunes.apple.com/app/id1642259256");
#endif
		}

		// public void OnHide()
		// {
		// 	StartCoroutine(DelayClose());
		// }
		//
		// private IEnumerator DelayClose()
		// {
		// 	GetComponent<Animator>().Play("Disappear");
		// 	yield return new WaitForSeconds(0.4f);
		// 	settingPanel.SetActive(false);
		// }

		// public void OnClickClearAllData()
		// {
		//     DataController.Instance.ResetAllData();
		// }
		// public void CheatRuby1()
		// {
		//     DataController.Instance.CheatRuby();
		// }
		// public void RemoveRuby()
		// {
		//     DataController.Instance.RemoveRuby();
		// }
		// public void PlayAnyLevel()
		// {
		//     if (ChapterInf.text != null && LevelInf.text != null)
		//     {
		//         int chapter=1, level=1;
		//         try
		//         {
		//             chapter = Int32.Parse(ChapterInf.text);
		//             level = Int32.Parse(LevelInf.text);
		//             LevelDataController.Instance.LoadLevel(chapter, level);
		//             DataController.Instance.currentChapter = chapter;
		//             LevelDataController.Instance.lastestPassedLevel = null;
		//             LevelDataController.Instance.collectedGold = 0;
		//             string gameScene = DataController.Instance.GetGamePlayScene(LevelDataController.Instance.currentLevel.chapter);
		//             SceneController.Instance.LoadScene(gameScene);
		//         }
		//         catch
		//         {
		//             Debug.Log("Can't Parse");
		//         }
		//     }
		// }
		public void UpdateLanguage()
		{
			string language = PlayerPrefs.GetString("language_code", "");
			if (language.Equals("")) language = "EN";
			textLanguage.text = language;
		}

		public void ResetFlashSale()
		{
			PlayerPrefs.SetFloat("fs_waittime", 0);
			PlayerPrefs.SetFloat("FLASH_SALE", 0);
		}

		public void OnClickGiftCodeBtn()
		{
			Instantiate(GiftCodePanel, transform.parent);
			settingPanel.SetActive(false);
		}
		
		public void BTN_ClickPolicy()
		{
			Application.OpenURL(_policyURL);
		}

		// public override void Close()
		// {
		// 	base.Close();
		// 	_editProfilePanel.OnClickCloseAvatarList();
		// }

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			gameObject.SetActive(false);
		}
	}
}