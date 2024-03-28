using System;
using TMPro;
using UI;
using UnityEngine;

namespace Controllers
{
	public class LevelUIController : SingletonCore<LevelUIController>
	{
		[SerializeField] private TextMeshProUGUI _timerText;
		[SerializeField] private TextMeshProUGUI _levelText;
		[SerializeField] private TextMeshProUGUI _coinText;
		[SerializeField] private TextMeshProUGUI _starText;
		[SerializeField] private TextMeshProUGUI _comboText;
		public TextMeshProUGUI _comboTimeText;

		[SerializeField] private LosePanel _losePanel;
		[SerializeField] private WinPanel _winPanel;

		private void Start()
		{
			_losePanel = GetComponentInChildren<LosePanel>(true);
			_winPanel = GetComponentInChildren<WinPanel>(true);

			string numberString = gameObject.scene.name.Substring(5);
			_levelText.text = "Lv." + int.Parse(numberString);
		}

		public void RenderTimer(int secondsLeft)
		{
			int minutes = secondsLeft / 60;
			int seconds = secondsLeft % 60;

			string minutesString = minutes.ToString("D2");
			string secondsString = seconds.ToString("D2");

			_timerText.text = $"{minutesString}:{secondsString}";
		}

		public void DisplayCoin(int amount)
		{
			_coinText.text = amount.ToString();
		}

		public void DisplayStar(int amount)
		{
			_starText.text = amount.ToString();
		}

		public void DisplayCombo(int combo)
		{
			_comboText.text = "Combo x" + combo;
		}

		public void ShowLosePanel()
		{
			_losePanel.Show();
		}

		public void ShowWinPanel()
		{
			_winPanel.Show();
		}

		public void NextLevel()
		{
			LevelController.Instance.Win();
		}

		public void OnClickReplay()
		{
			LevelController.Instance.Replay();
		}

		public void OnClickHome()
		{
			LevelController.Instance.GoHome();
		}
	}
}