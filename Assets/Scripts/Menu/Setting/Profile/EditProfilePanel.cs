using System;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Setting.Profile
{
	public class EditProfilePanel : MonoBehaviour
	{
		[SerializeField] private Sprite[] _sprites;
		public Avatar[] _avatars;

		[SerializeField] private Image _avatarImage;
		[SerializeField] private RectTransform _avatarListPopup;
		[SerializeField] private RectTransform _closePopupButton;
		[SerializeField] private Vector2 _popupMinSize;
		[SerializeField] private GridLayoutGroup _avatarsGroup;
		[SerializeField] private Button _editNameButton;
		[SerializeField] private TMP_InputField _nameInputField;
		private Image _nameIFBox;
		private TextMeshProUGUI _nameText;

		public int AvatarCoinPrice = 200;

		private Vector2 _initialSize;
		private bool _initialized;

		// private void OnValidate()
		// {
		// _avatars = _sprites.Select((sprite, i) => new Avatar { id = i, sprite = sprite }).ToArray();
		// }

		private void Start()
		{
			_initialSize = _avatarListPopup.sizeDelta;
			_avatarListPopup.sizeDelta = _popupMinSize;
			_closePopupButton.localScale = new Vector3(0, 0, 1);

			_avatarListPopup.gameObject.SetActive(false);

			LoadProfile();
			_nameInputField.interactable = false;
			_editNameButton.interactable = true;
			_nameIFBox = _nameInputField.GetComponent<Image>();
			_nameText = _nameInputField.GetComponentInChildren<TextMeshProUGUI>();
		}

		private void LoadProfile()
		{
			var profile = DataController.Instance.Profile;
			_nameInputField.text = profile.Name;
			_avatarImage.sprite = _avatars[profile.CurrentAvatarId].sprite;
		}

		public void OnClickShowAvatarList()
		{
			if (_avatarListPopup.gameObject.activeSelf)
				return;

			_avatarListPopup.gameObject.SetActive(true);

			Vector2 maxWithSize = new Vector2(_initialSize.x, _popupMinSize.y);
			DOTween.Sequence()
				.Append(_avatarListPopup.DOSizeDelta(maxWithSize, 0.1f))
				.Append(_avatarListPopup.DOSizeDelta(_initialSize, 0.2f).SetEase(Ease.OutBack))
				.Join(_closePopupButton.DOScale(1, 0.1f).SetEase(Ease.OutBack));

			if (!_initialized)
			{
				InitAvatarButtons();
				_initialized = true;
			}
		}

		private void InitAvatarButtons()
		{
			AvatarButton _avatarButtonSample = _avatarsGroup.GetComponentInChildren<AvatarButton>();

			foreach (var avatar in _avatars)
			{
				var button = Instantiate(_avatarButtonSample, _avatarsGroup.transform);
				button.Init(avatar, this);
			}

			Destroy(_avatarButtonSample.gameObject);
		}

		public void ChangeAvatar(Avatar avatar)
		{
			_avatarImage.sprite = avatar.sprite;
			DataController.Instance.Profile.CurrentAvatarId = avatar.id;
			DataController.Instance.SaveData();
		}

		public void OnClickCloseAvatarList()
		{
			if (!_avatarListPopup.gameObject.activeSelf)
				return;

			Vector2 minHeightSize = new Vector2(_initialSize.x, _popupMinSize.y);
			DOTween.Sequence()
				.Append(_avatarListPopup.DOSizeDelta(minHeightSize, 0.1f))
				.Append(_avatarListPopup.DOSizeDelta(_popupMinSize, 0.05f))
				.Join(_closePopupButton.DOScale(0, 0.1f))
				.OnComplete(() => _avatarListPopup.gameObject.SetActive(false));
		}

		private string _currentName;

		public void OnClickEditName()
		{
			if (_nameInputField.interactable)
				return;

			_nameInputField.interactable = true;
			_nameIFBox.enabled = true;
			_editNameButton.interactable = false;
			_nameText.overflowMode = TextOverflowModes.Overflow;

			_nameInputField.ActivateInputField();
			// StartCoroutine(GameManager.WaiForSeconds(0.1f, () =>
			// {
			// 	_nameInputField.MoveToEndOfLine(false, true);
			// } ));
			// _nameInputField.Select();
			_currentName = _nameInputField.text;
		}

		public void OnEndNameEdit()
		{
			_nameIFBox.enabled = false;
			_nameText.overflowMode = TextOverflowModes.Ellipsis;
			_editNameButton.interactable = true;
			_editNameButton.enabled = false;
			_nameText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

			StartCoroutine(GameManager.WaitForEndOfFrame(() =>
			{
				_nameInputField.interactable = false;
				_editNameButton.enabled = true;
			}));

			if (_nameInputField.text != _currentName)
			{
				DataController.Instance.Profile.Name = _nameInputField.text;
				DataController.Instance.SaveData();
			}
		}
	}

	[Serializable]
	public class Avatar
	{
		public int id;
		public Sprite sprite;
		public HowToUnlock howToUnlock;

		public enum HowToUnlock
		{
			Free,
			Ads,
			Coin
		}
	}
}