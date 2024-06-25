using Audio;
using Boosters.Start;
using Controllers;
using Data;
using MainMenu;
using TMPro;
using UI;
using UnityEngine;
using Utilities;

namespace InGame.UI
{
	public class LosePanel : Popup
	{
		[SerializeField] private StartBoosterButton[] _startBoosterButtons;
		[SerializeField] private TextMeshProUGUI _levelText;
		[SerializeField] private BuyEnergyPanel _buyEnergyPanelPrefab;

		[Header("Audio")] [SerializeField] private AudioClip _openSfx;

		public override void Show()
		{
			base.Show();
			_levelText.text = "Level" + (LevelController.Instance.LevelIndex + 1);
			AudioController.Instance.StopMusic();
			AudioController.Instance.PlaySfx(_openSfx);
		}

		public void OnClickCLose()
		{
			_animator.Play("Disappear");
			StartCoroutine(CommonIEnumerator.WaiForSeconds(0.4f, LevelController.Instance.GoHome));
		}

		public void OnCLickPlay()
		{
			if (DataController.Instance.TryUseEnergy())
			{
				LevelController.Instance.Replay();
			}
			else
			{
				var buyEnergyPanel = Instantiate(_buyEnergyPanelPrefab, transform.parent);
				buyEnergyPanel.Init(null);
			}
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
		}
	}
}