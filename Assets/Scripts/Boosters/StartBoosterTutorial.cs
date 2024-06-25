using System.Diagnostics;
using Boosters.Start;
using Controllers;
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
		private StartBoosterButton _tutBoosterButton;

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
				_description.text = _unlockBoosterPanel.Data.Description;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void OnClickClaimBooster()
		{
			if (DataController.Instance.GetBoosterQuantity(_unlockBoosterPanel.Data.Type) < 3)
				Give3();
			DOVirtual.DelayedCall(0.5f, DisplayUseBoosterTutorial);
		}

		private void DisplayUseBoosterTutorial()
		{
			_useBoosterTutorial.SetActive(true);
			_tutBoosterButton = Instantiate(_booster, _useBoosterTutorial.transform);
			// _tutBoosterButton.enabled = false;

			Transform buttonTf = _tutBoosterButton.transform;

			buttonTf.position = _booster.transform.position;
			buttonTf.SetSiblingIndex(1);

			PointToBoosterButton(buttonTf.position);
		}

		private void Give3()
		{
			DataController.Instance.AddBooster(_unlockBoosterPanel.Data.Type, 3);
			DataController.Instance.SaveData();
			_booster.RefreshQuantityText();

			// if (RewardHelper.TryConvertBoosterToReward(_unlockBoosterPanel.Data.Type, out var rewardType))
			// {
			// 	MainMenuController.Instance.PlayClaimRewardEffect(rewardType, _unlockBoosterPanel.transform.position,
			// 		_booster.transform.position, () => _booster.RefreshQuantityText());
			// }
		}

		public void OnClickBooster()
		{
			// _booster.Select();
			_booster.Button.interactable = false;
			_selectButton.interactable = false;

			_arrow.SetActive(false);
			_tutBoosterButton.Select();
			_continueButton.gameObject.SetActive(true);
		}

		public void OnClickContinue()
		{
			FindObjectOfType<StartLevelPanel>().OnClickPlayButton();
			PlayerPrefs.SetInt(gameObject.name, 1);
		}

		private void PointToBoosterButton(Vector2 position)
		{
			var tutTransform = _useBoosterTutorial.transform;
			SetAnchorPos(tutTransform.Find("Arrow"), position);
			SetAnchorPos(tutTransform.Find("Select"), position);
		}

		private void SetAnchorPos(Transform trans, Vector2 pos)
		{
			trans.GetComponent<RectTransform>().position = pos;
		}
	}
}