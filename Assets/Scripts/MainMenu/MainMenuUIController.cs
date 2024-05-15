using Data;
using MainMenu.TopBar;
using Menu.Setting.Profile;
using UnityEngine;
using UnityEngine.UI;
using Avatar = Menu.Setting.Profile.Avatar;

namespace MainMenu
{
	public class MainMenuUIController : SingletonCore<MainMenuUIController>
	{
		[Header("Top bar")] public Coin Coin;
		[SerializeField] private Image _avatar;


		private void Start()
		{
			Avatar avatar = FindObjectOfType<EditProfilePanel>(true)
				._avatars[DataController.Instance.Profile.CurrentAvatarId];
			_avatar.sprite = avatar.sprite;
		}
	}
}