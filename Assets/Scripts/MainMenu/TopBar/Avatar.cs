using System;
using Controllers;
using Data;
using MainMenu.Setting.Profile;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.TopBar
{
	public class Avatar : MonoBehaviour
	{
		[SerializeField] private Image _avatarImage;
		[SerializeField] private EditProfilePanel _editProfilePanel;

		public Vector2 Position => transform.position;
		
		private void Start()
		{
			_avatarImage.sprite = DataController.Instance.Profile.Avatar;
			SpritesCollection.Instance.CurrentAvatarSprite = _avatarImage.sprite;
		}

		public void OnClick()
		{
			EditProfilePanel editProfilePanel =
				Instantiate(_editProfilePanel, MainMenuController.Instance.CameraCanvas);

			editProfilePanel.Show();
			editProfilePanel.OnChangeAvatar += newAvatar => _avatarImage.sprite = newAvatar;
		}
	}
}