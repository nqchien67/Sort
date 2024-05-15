using Controllers;
using UnityEngine;

namespace MainMenu.DailyQuest
{
	public class DailyQuestController : MonoBehaviour
	{
		[SerializeField] private GameObject _dailyQuestButton;
		[SerializeField] private DailyQuestPanel _dailyQuestPanelPrefab;

		private void Start()
		{
			bool _isPassLevel10 = PlayerPrefs.GetInt("level", 0) >= 10;
			_dailyQuestButton.SetActive(_isPassLevel10);
		}

		public void OnClickDailyQuestBtn()
		{
			var dailyQuestPanel = Instantiate(_dailyQuestPanelPrefab, MainMenuController.Instance.CameraCanvas);
			dailyQuestPanel.Show();
		}
	}
}