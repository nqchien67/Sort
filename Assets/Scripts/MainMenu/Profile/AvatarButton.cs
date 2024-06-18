using System.Collections.Generic;
using Data;
using MainMenu.Setting.Profile;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;
using Avatar = MainMenu.Setting.Profile.Avatar;

namespace MainMenu.Profile
{
	public class AvatarButton : MonoBehaviour
	{
		[SerializeField] private int _coinPrice;

		[SerializeField] private Image _image;
		[SerializeField] private GameObject _buyWithCoin;
		[SerializeField] private GameObject _buyWithAds;
		private Avatar _avatar;
		private bool _isUnlocked;
		private EditProfilePanel _editProfilePanel;

		public void Init(Avatar avatar, EditProfilePanel editProfilePanel)
		{
			_avatar = avatar;
			_isUnlocked = avatar.howToUnlock == Avatar.HowToUnlock.Free ||
			              DataController.Instance.UnlockedAvatarIds.Contains(_avatar.id);

			_image.sprite = avatar.sprite;
			_editProfilePanel = editProfilePanel;

			if (_isUnlocked)
			{
				Destroy(_buyWithAds);
				Destroy(_buyWithCoin);
			}
			else if (avatar.howToUnlock == Avatar.HowToUnlock.Coin)
			{
				_buyWithCoin.SetActive(true);
				Destroy(_buyWithAds);
			}
			else
			{
				_buyWithAds.SetActive(true);
				Destroy(_buyWithCoin);
			}
		}

		public void OnClick()
		{
			if (_isUnlocked)
				_editProfilePanel.ChangeAvatar(_avatar);
			else
				Unlock();
		}

		private void Unlock()
		{
			if (_avatar.howToUnlock == Avatar.HowToUnlock.Ads)
				UnlockWithAds();
			else
				UnlockWithCoin();
		}

		private void UnlockWithCoin()
		{
			int coinHave = DataController.Instance.Coin;
			if (coinHave < _editProfilePanel.AvatarCoinPrice)
				// _notEnoughMoney.SetActive(true);
				return;

			DataController.Instance.Coin -= 200;
			MainMenuUIController.Instance.Coin.UpdateValue();
			AddData();
			_editProfilePanel.ChangeAvatar(_avatar);
			Destroy(_buyWithCoin);
			_isUnlocked = true;
		}

		private void UnlockWithAds()
		{
			AddData();
			_editProfilePanel.ChangeAvatar(_avatar);
			Destroy(_buyWithAds);
			_isUnlocked = true;
		}

		private void AddData()
		{
			List<int> unlockedAvatarIds = DataController.Instance.UnlockedAvatarIds;
			unlockedAvatarIds.Add(_avatar.id);
			DataController.Instance.UnlockedAvatarIds = unlockedAvatarIds;
		}
	}
}