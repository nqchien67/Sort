using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame
{
	public class PausePanel : Popup
	{
		[SerializeField] private Sprite musicIconOn;
		[SerializeField] private Sprite musicIconOff;
		[SerializeField] private Sprite sfxIconOn;
		[SerializeField] private Sprite sfxIconOff;
		[SerializeField] private Sprite vibrationIconOn;
		[SerializeField] private Sprite vibrationIconOff;

		[SerializeField] private Image musicIcon;
		[SerializeField] private Image sfxIcon;
		[SerializeField] private Image vibrationIcon;

		[SerializeField] private AudioClip _popUpClip;

		[SerializeField] private Button _continueButton;
		[SerializeField] private GameObject _confirmQuit;
		[SerializeField] private TextMeshProUGUI _coinCount;
		[SerializeField] private TextMeshProUGUI _energyCount;
		[SerializeField] private TextMeshProUGUI _starCount;

		private AudioController AudioController => AudioController.Instance;
		private bool _quiting;

		public override void Show()
		{
			base.Show();

			Time.timeScale = 0;

			musicIcon.sprite = AudioController.Music ? musicIconOn : musicIconOff;
			sfxIcon.sprite = AudioController.SFX ? sfxIconOn : sfxIconOff;
			vibrationIcon.sprite = AudioController.Vibration ? vibrationIconOn : vibrationIconOff;
			// UpdateLanguage();
			AudioController.Instance.PlaySfx(_popUpClip);
		}

		public void ChangeMusic()
		{
			if (AudioController.Music)
			{
				AudioController.Music = false;
				musicIcon.sprite = musicIconOff;
			}
			else
			{
				AudioController.Music = true;
				musicIcon.sprite = musicIconOn;
			}
		}

		public void ChangeSfx()
		{
			if (AudioController.SFX)
			{
				AudioController.SFX = false;
				sfxIcon.sprite = sfxIconOff;
			}
			else
			{
				AudioController.SFX = true;
				sfxIcon.sprite = sfxIconOn;
			}
		}

		public void ChangeVibration()
		{
			if (AudioController.Vibration)
			{
				AudioController.Vibration = false;
				vibrationIcon.sprite = vibrationIconOff;
			}
			else
			{
				AudioController.Vibration = true;
				vibrationIcon.sprite = vibrationIconOn;
			}
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			Time.timeScale = 1;
			gameObject.SetActive(false);
			_confirmQuit.SetActive(false);
			_continueButton.gameObject.SetActive(true);
		}

		public void OnClickQuit()
		{
			if (_quiting)
			{
				LevelController.Instance.GoHome();
				Time.timeScale = 1;
			}
			else
				ShowConfirmQuit();
		}

		public void OnClickContinue()
		{
			_quiting = false;
			Close();
		}

		private void ShowConfirmQuit()
		{
			_continueButton.gameObject.SetActive(false);
			_confirmQuit.SetActive(true);
			_coinCount.text = LevelController.Instance.Coin.ToString();
			_energyCount.text = "1";
			_starCount.text = LevelController.Instance.Star.ToString();
			_quiting = true;
		}
	}
}