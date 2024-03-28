using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
	public class MainSceneController : SingletonCore<MainSceneController>
	{
		[SerializeField] private TMP_InputField _levelInput;
		[SerializeField] private Popup _startLevelPopup;

		public void OnClickPlay()
		{
			_startLevelPopup.Show();
		}

		public void Play()
		{
			string text = _levelInput.text;
			if (int.TryParse(text, out int level) && level >= 1)
			{
				PlayerPrefs.SetInt("level", level);
			}
			else
			{
				level = PlayerPrefs.GetInt("level", 0);
				level++;
			}

			SceneManager.LoadScene("Level" + level);
		}
	}
}