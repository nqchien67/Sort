using Boosters.Start;
using Controllers;
using Data;
using TMPro;
using UI;
using UnityEngine;

namespace MainMenu
{
	public class StartLevelPanel : Popup
	{
		[SerializeField] private StartBoosterButton[] _startBoosterButtons;
		[SerializeField] private GameObject _freeGifButton;
		[SerializeField] private TextMeshProUGUI _levelText;

		private void Start()
		{
			int highestPassedLevel = MainMenuController.Instance.HighestPassedLevel;
			_levelText.text = "Level " + (highestPassedLevel + 1);

			bool isPassedLevel8 = highestPassedLevel >= 8;
			_freeGifButton.SetActive(isPassedLevel8);
		}

		public void OnClickPlayButton()
		{
			MainMenuController.Instance.Play();
		}

		public void OnClickPreGiftButton()
		{
			Debug.Log("Show reward video");

			foreach (var boosterButton in _startBoosterButtons)
			{
				DataController.Instance.AddBooster(boosterButton.boosterType, 1);

				boosterButton.RefreshQuantityText();
			}
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			gameObject.SetActive(false);
		}
	}
}