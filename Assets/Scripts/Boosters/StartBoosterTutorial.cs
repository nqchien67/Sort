using System.Diagnostics;
using Boosters.Start;
using DG.Tweening;
using MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Boosters
{
	public class StartBoosterTutorial : MonoBehaviour
	{
		[SerializeField] private StartBoosterButton _booster;
		[SerializeField] private UnlockBoosterPanel _unlockBoosterPanel;
		[SerializeField] private GameObject _useBoosterTutorial;
		[SerializeField] private Button _continueButton;

		[SerializeField] private Button _selectButton;
		[SerializeField] private GameObject _arrow;

		private void Awake()
		{
			_unlockBoosterPanel.Data = _booster.Data;
		}

		private void Start()
		{
			if (PlayerPrefs.GetInt(gameObject.name, 0) != 1)
			{
				_unlockBoosterPanel.OnClickClaim += OnClickClaimBooster;
				_unlockBoosterPanel.Show();
				// _booster.gameObject.SetActive(false);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void OnClickClaimBooster()
		{
			DOVirtual.DelayedCall(0.5f, () =>
			{
				_useBoosterTutorial.SetActive(true);
				_booster.gameObject.SetActive(true);
			});
		}

		public void OnClickBooster()
		{
			_booster.Select();
			_booster.Button.interactable = false;
			_selectButton.interactable = false;
			_arrow.SetActive(false);

			_continueButton.gameObject.SetActive(true);
		}

		public void OnClickContinue()
		{
			FindObjectOfType<StartLevelPopup>().OnClickPlayButton();
			PlayerPrefs.SetInt(gameObject.name, 1);
		}
	}
}