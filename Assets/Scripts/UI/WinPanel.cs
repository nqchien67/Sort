using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
	public class WinPanel : Popup
	{
		public void OnClickCLose()
		{
			_animator.Play("Disappear");
			StartCoroutine(GameManager.WaiForSeconds(0.5f, () => SceneManager.LoadScene("MainScene")));
		}

		public void OnClickNext()
		{
			int level = PlayerPrefs.GetInt("level", 0);
			level++;
			SceneManager.LoadScene("Level" + level);
		}
	}
}