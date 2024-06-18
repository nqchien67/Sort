using Controllers;
using UnityEngine;

namespace MainMenu.LuckySpin
{
	public class LuckySpinController : MonoBehaviour
	{
		[SerializeField] private GameObject luckySpinPanelPrefab;
		[SerializeField] private AudioClip popUpClip;
		public GameObject luckySpinBtn;
		private float clickTimeStamp = 0, packLifeTime, packTimeStamp, timeStamp;

		public void OnClickOpenPanel()
		{
			if (Time.time - clickTimeStamp < 0.5f) 
				return;
		
			clickTimeStamp = Time.time;
			OpenPanel();
		}

		public void OpenPanel()
		{
			Instantiate(luckySpinPanelPrefab, MainMenuController.Instance.CameraCanvas);
			// AudioController.Instance.PlaySfx(popUpClip);
		}

		public void OpenAndSpinIAP()
		{
			GameObject luckySpinPanel = Instantiate(luckySpinPanelPrefab, MainMenuController.Instance.CameraCanvas);
			luckySpinPanel.GetComponent<LuckySpinPanel>().OnSpinIAP();
			// AudioController.Instance.PlaySfx(popUpClip);
		}

		private void Start()
		{
			//Debug.LogError(DataController.Instance.GetLevelState(PlayerClassifyController.Instance.luckySpinData.chapter, PlayerClassifyController.Instance.luckySpinData.level) >= 1);
			// if (PlayerClassifyController.Instance.luckySpinData.active
			//     && DataController.Instance.GetLevelState(PlayerClassifyController.Instance.luckySpinData.chapter, PlayerClassifyController.Instance.luckySpinData.level) >= 1
			//     && DateTime.Now >= PlayerClassifyController.Instance.luckySpinData.timeStart.ToDateTime('/')
			//     && DateTime.Now < PlayerClassifyController.Instance.luckySpinData.timeEnd.ToDateTime('/'))
			// {
			bool isReachLevel5 = MainMenuController.Instance.HighestPassedLevel >= 5;
			luckySpinBtn.SetActive(isReachLevel5);
			// }
			// else
			// {
			// 	luckySpinBtn.SetActive(false);
			// }
		}
	}
}