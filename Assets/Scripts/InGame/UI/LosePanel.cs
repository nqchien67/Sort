using Audio;
using Boosters.Start;
using Controllers;
using Data;
using MainMenu;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace InGame.UI
{
	public class LosePanel : Popup
	{
		[SerializeField] private StartBoosterButton[] _startBoosterButtons;
		[SerializeField] private TextMeshProUGUI _levelText;
		[SerializeField] private BuyEnergyPanel _buyEnergyPanelPrefab;

		[Header("Audio")] [SerializeField] private AudioClip _openSfx;
		[SerializeField] private Button _freeGifButton;
		[SerializeField] private RewardPanelController _rewardPanel;

		public override void Show()
		{
			base.Show();
			_levelText.text = "Level" + LevelController.Instance.LevelIndex;

			bool isLevel9 = LevelController.Instance.LevelIndex >= 9;
			_freeGifButton.gameObject.SetActive(isLevel9);

			AudioController.Instance.StopMusic();
			AudioController.Instance.PlaySfx(_openSfx);
		}

		public void OnClickCLose()
		{
			_animator.Play("Disappear");
		}

		public void OnCLickPlay()
		{
			if (DataController.Instance.TryUseEnergy())
			{
				LevelController.Instance.Replay();
			}
			else
			{
				ShowBuyEnergyPanel();
			}
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
					var rewardPanel = Instantiate(_rewardPanel, LevelUIController.Instance.PopupCanvas);
					rewardPanel.Init(new[] { rewardType }, new[] { 1 }, false, false, null, 1,
						() =>
						{
							randomBoosterButton.RefreshQuantityText();
							randomBoosterButton.Select();
							LevelController.Instance.Replay();
						});
				}
			}
			else
			{
				ShowBuyEnergyPanel();
			}
		}

		private void ShowBuyEnergyPanel()
		{
			var buyEnergyPanel = Instantiate(_buyEnergyPanelPrefab, transform.parent);
			buyEnergyPanel.Init(null);
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			LevelController.Instance.GoHome();
		}
	}
}