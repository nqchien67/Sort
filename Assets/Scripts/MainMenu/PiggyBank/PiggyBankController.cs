using System;
using System.Collections;
using Controllers;
using Data;
using Menu.PiggyBank;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.PiggyBank
{
	public class PiggyBankController : MonoBehaviour
	{
		[SerializeField] private GameObject piggyBankBtn, notify, timeText;
		[SerializeField] private Image progressImg;
		[SerializeField] private GameObject piggyBankPanelPrefab;
		[SerializeField] private IAPData[] _iapDatas;

		[SerializeField] private GameObject[] tutorialPanels;

		[SerializeField] private FullPiggyBankPanelController fullPiggyPanel;

		private bool isPassLevel6;
		private GameObject tutorialGo;
		private MainMenuController _mainMenu;

		private void Start()
		{
			// if (PlayerClassifyController.Instance.piggyBankData.piggybank.piggyactive == true)
			SetupData();
		}

		public void SetupData()
		{
			_mainMenu = FindObjectOfType<MainMenuController>();
			DataController.Instance.PbTimeDuration -=
				(int)(DataController.ConvertToUnixTime(DateTime.Now) - DataController.Instance.PiggyBankTimeStamp);
			if (DataController.Instance.isFirstOpenPB && DataController.Instance.PbTimeDuration <= 0)
			{
				PlayerPrefs.SetInt("has_full_pb", 0);
				PlayerPrefs.SetInt("open_full_piggy", 0);
				DataController.Instance.PiggyBankCoin = 0;
				DataController.Instance.PbTimeDuration = 86400;
				DataController.Instance.PiggyBankTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
				DataController.Instance.isFirstOpenPB = false;
				Debug.Log("piggy setup data");
				DataController.Instance.SaveData();
			}

			//DataController.Instance.PbTimeDuration -= (int)(DataController.ConvertToUnixTime(DateTime.Now) - DataController.Instance.PbTimeStamp);
			DataController.Instance.PiggyBankTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
			isPassLevel6 = PlayerPrefs.GetInt("level", 0) >= 6;
			bool isFullPB = DataController.Instance.IsPiggyBankFull();
			SetActiveIcon(isPassLevel6);
			if (isFullPB && DataController.Instance.PbTimeDuration <= 0)
				SetActiveIcon(false);
			float currentFillAmount =
				1f * DataController.Instance.PiggyBankCoin / DataController.Instance.CurrentPbStorage;
			progressImg.fillAmount = currentFillAmount;
			notify.SetActive(isPassLevel6 && currentFillAmount >= 0.6f);
			if (currentFillAmount == 1)
				timeText.SetActive(true);
		}

		public void ShowIntroTutorial()
		{
			tutorialGo = Instantiate(tutorialPanels[0], _mainMenu.CameraCanvas);
			tutorialGo.GetComponentInChildren<Button>().onClick.AddListener(OnClickIntroTutorial);
			Time.timeScale = 0;
		}

		public void OnClickIntroTutorial()
		{
			Time.timeScale = 1;
			Destroy(tutorialGo);
			StartCoroutine(DelayShowIntroTut2());
		}

		private IEnumerator DelayShowIntroTut2()
		{
			yield return new WaitForSeconds(0.2f);
			tutorialGo = Instantiate(tutorialPanels[1], _mainMenu.CameraCanvas);
			tutorialGo.GetComponentInChildren<Button>().onClick.AddListener(OnClickIntroTut2);
			Time.timeScale = 0;
		}

		public void OnClickIntroTut2()
		{
			Time.timeScale = 1;
			Destroy(tutorialGo);
			StartCoroutine(DelayShowPanel());
		}

		public void OnClickTurnOffTutorials()
		{
			Time.timeScale = 1;
			SetupData();
			Destroy(tutorialGo);
		}

		private IEnumerator DelayShowPanel()
		{
			yield return new WaitForSeconds(0.2f);
			OnClickPiggyBankBtn();
			yield return new WaitForSeconds(0.2f);
			tutorialGo = Instantiate(tutorialPanels[2], _mainMenu.CameraCanvas);
			tutorialGo.GetComponentInChildren<Button>().onClick.AddListener(OnClickTurnOffTutorials);
			Time.timeScale = 0;
		}

		public bool CanShowFullPiggyPanel()
		{
			if (PlayerPrefs.GetInt("open_full_piggy", 0) == 0
			    && DataController.Instance.IsPiggyBankFull())
			{
				PlayerPrefs.SetInt("open_full_piggy", 1);
				return true;
			}

			return false;
		}

		public void SetActiveIcon(bool status)
		{
			piggyBankBtn.SetActive(status);
		}

		public void ShowFullPiggyPanel()
		{
			var panel = Instantiate(fullPiggyPanel, MainMenuController.Instance.CameraCanvas);
			panel.Init(piggyBankBtn.transform.position, OnClickPiggyBankBtn);
		}

		public void OnClickPiggyBankBtn()
		{
			int piggyBankLevel = DataController.Instance.PiggyBankLevel - 1;
			GameObject piggy = Instantiate(piggyBankPanelPrefab, MainMenuController.Instance.CameraCanvas);

			var piggyBankPanelController = piggy.GetComponent<PiggyBankPanel>();
			piggyBankPanelController.ForceOpen = false;
			piggyBankPanelController.Init(_iapDatas[piggyBankLevel]);
		}

		public void OnForceClickPiggyBankBtn()
		{
			int piggyBankLevel = DataController.Instance.PiggyBankLevel - 1;
			var piggy = Instantiate(piggyBankPanelPrefab, MainMenuController.Instance.CameraCanvas);

			var piggyBankPanelController = piggy.GetComponent<PiggyBankPanel>();
			piggyBankPanelController.ForceOpen = true;
			piggyBankPanelController.Init(_iapDatas[piggyBankLevel]);
		}

		public void DisableFullText()
		{
			timeText.SetActive(false);
		}
	}

	[Serializable]
	public struct IAPData
	{
		public string IAPId;
		public float Price;
	}
}