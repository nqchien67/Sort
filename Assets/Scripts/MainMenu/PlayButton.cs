using Controllers;
using TMPro;
using UnityEngine;

namespace MainMenu
{
	public class PlayButton : MonoBehaviour
	{
		private TextMeshProUGUI _text;

		private void Awake()
		{
			_text = GetComponentInChildren<TextMeshProUGUI>();
		}

		private void Start()
		{
			_text.text = "LEVEL " + MainMenuController.Instance.NextLevel;
		}
	}
}