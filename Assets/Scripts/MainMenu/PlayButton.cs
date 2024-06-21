using System;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
	public class PlayButton : MonoBehaviour
	{
		private Image _image;
		private TextMeshProUGUI _text;
		[SerializeField] private Sprite _hardSprite;

		private void Awake()
		{
			_image = GetComponent<Image>();
			_text = GetComponentInChildren<TextMeshProUGUI>();
		}

		private void Start()
		{
			_text.text = "LEVEL " + MainMenuController.Instance.NextLevel;
			if (MainMenuController.Instance.HardLevelComing)
				_image.sprite = _hardSprite;
		}
	}
}