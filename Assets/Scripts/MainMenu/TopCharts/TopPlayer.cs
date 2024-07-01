using Controllers;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.TopCharts
{
	public class TopPlayer : MonoBehaviour
	{
		[SerializeField] private Image _avatar;
		[SerializeField] private TextMeshProUGUI _nameText;
		[SerializeField] private TextMeshProUGUI _starText;
		[SerializeField] private Button _chestButton;
		[SerializeField] private NotiDot _notiDot;

		private Reward _reward;
		private string _name;

		public void Init(PlayerData playerData, Reward reward)
		{
			Init(playerData);
			_name = playerData.Name;

			_reward = reward;
			if (TopChartsController.Instance.SeasonEnded)
				OnEndSeason();
			else
				TopChartsController.Instance.OnSeasonEnd += OnEndSeason;
		}

		public void Init(PlayerData playerData)
		{
			_avatar.sprite = DataController.GetAvatarSprite(playerData.AvatarName);
			_nameText.text = playerData.Name;
			_starText.text = playerData.Star.ToString();
		}

		private void OnEndSeason()
		{
			if (_name == "You")
			{
				_chestButton.onClick.AddListener(ClaimReward);
				_notiDot.SetEnable(true);
			}
		}

		private void ClaimReward()
		{
			MainMenuController.Instance.RewardPanelController.Init(_reward.RewardTypes, _reward.Quantities, false);
			_notiDot.SetEnable(false);
			MainMenuUIController.Instance.GoHome();
			TopChartsController.Instance.StartNewSeason();
		}
	}
}