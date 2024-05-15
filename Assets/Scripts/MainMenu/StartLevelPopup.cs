using System;
using Boosters.Start;
using Controllers;
using Data;
using TMPro;
using UI;
using UnityEngine;

namespace MainMenu
{
	public class StartLevelPopup : Popup
	{
		[SerializeField] private StartBoosterButton[] _startBoosterButtons;
		[SerializeField] private GameObject _freeGifButton;
		[SerializeField] private TextMeshProUGUI _levelText;

		private void Start()
		{
			int highestPassedLevel = PlayerPrefs.GetInt("level", 0);
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
				DataController.Instance.AddConsumable(boosterButton.consumableType, 1);
				boosterButton.RefreshAmountText();
			}
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			gameObject.SetActive(false);
		}
	}
}