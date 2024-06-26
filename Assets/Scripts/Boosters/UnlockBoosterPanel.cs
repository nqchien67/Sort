using Audio;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Boosters
{
	public class UnlockBoosterPanel : Popup
	{
		public BoosterData Data;
		[SerializeField] private Image _boosterIcon;
		[SerializeField] private TextMeshProUGUI _descriptionText;
		[SerializeField] private Button _claimButton;
		[SerializeField] private AudioClip _openSfx;

		public UnityAction OnClickClaim;

		private void Start()
		{
			_boosterIcon.sprite = Data.Sprite;
			_descriptionText.text = Data.Name;

			OnClickClaim += Close;
			_claimButton.onClick.AddListener(OnClickClaim);

			AudioController.Instance.PlaySfx(_openSfx);
		}
	}
}