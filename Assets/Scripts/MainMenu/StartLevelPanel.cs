using Boosters.Start;
using Controllers;
using Data;
using TMPro;
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
			if (DataController.Instance.TryUseEnergy())
				MainMenuController.Instance.Play();
			else
				EnergyController.Instance.OpenBuyEnergyPanel();
		}

		public void OnClickPreGiftButton()
		{
			if (DataController.Instance.TryUseEnergy())
			{
				Debug.Log("Show reward video");

				StartBoosterButton randomBoosterButton =
					_startBoosterButtons[Random.Range(0, _startBoosterButtons.Length)];
				BoosterType boosterType = randomBoosterButton.boosterType;

				if (RewardHelper.TryConvertBoosterToReward(boosterType, out var rewardType))
				{
					// 	MainMenuController.Instance.PlayClaimRewardEffect(rewardType, _freeGifButton.transform.position,
					// 		randomBoosterButton.transform.position, () => randomBoosterButton.RefreshQuantityText());
					FindObjectOfType<RewardPanelController>()
						.Init(new[] { rewardType }, new[] { 1 }, false, false, null, 1, () =>
						{
							randomBoosterButton.RefreshQuantityText();
							randomBoosterButton.Select();
							MainMenuController.Instance.Play();
						});
				}
			}
			else
				EnergyController.Instance.OpenBuyEnergyPanel();
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			gameObject.SetActive(false);
		}
	}
}