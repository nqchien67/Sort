using System.Diagnostics;
using Boosters.Start;
using Data;
using DG.Tweening;
using MainMenu;
using TMPro;
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
		[SerializeField] private TextMeshProUGUI _description;

		private void Awake()
		{
			_unlockBoosterPanel.Data = _booster.Data;
			if (DataController.Instance.GetBoosterQuantity(_unlockBoosterPanel.Data.Type) < 3)
			{
				DataController.Instance.AddBooster(_unlockBoosterPanel.Data.Type, 3);
				DataController.Instance.SaveData();
			}
		}

		private void Start()
		{
			if (PlayerPrefs.GetInt(gameObject.name, 0) != 1)
			{
				_unlockBoosterPanel.OnClickClaim += OnClickClaimBooster;
				_unlockBoosterPanel.Show();
				// _booster.gameObject.SetActive(false);
				_description.text = _unlockBoosterPanel.Data.Description;
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
			FindObjectOfType<StartLevelPanel>().OnClickPlayButton();
			PlayerPrefs.SetInt(gameObject.name, 1);
		}
	}
}