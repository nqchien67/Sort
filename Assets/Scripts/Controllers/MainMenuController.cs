using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
	public class MainMenuController : SingletonCore<MainMenuController>
	{
		[SerializeField] private TMP_InputField _levelInput;
		[SerializeField] private Popup _startLevelPopup;
		[SerializeField] private TextMeshProUGUI _buttonPlayText;

		public Transform CameraCanvas;
		private int _highestPassedLevel;

		private void Start()
		{
			_highestPassedLevel = PlayerPrefs.GetInt("level", 0);
			_buttonPlayText.text = "LEVEL" + _highestPassedLevel;
		}

		public void OnClickPlay()
		{
			_startLevelPopup.Show();
		}

		public void Play()
		{
			string text = _levelInput.text;
			if (int.TryParse(text, out int level) && level >= 1)
				PlayerPrefs.SetInt("level", _highestPassedLevel);
			else
				level = _highestPassedLevel + 1;

			SceneManager.LoadScene("Level" + level);
		}

		public void IncreaseCoin(Vector3 spawnPos, int totalCoin)
		{
			int coin = PlayerPrefs.GetInt("coin", 0);
			coin += totalCoin;
			PlayerPrefs.SetInt("coin", coin);
		}
	}
}