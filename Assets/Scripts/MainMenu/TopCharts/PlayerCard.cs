using System.Collections;
using System.Collections.Generic;
using Controllers;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.TopCharts
{
	public class PlayerCard : MonoBehaviour
	{
		public Image avaListImg;
		public TextMeshProUGUI txtRank, txtName, txtPoint;
		public GameObject toolTips;
		private readonly List<Transform> children = new List<Transform>();

		[SerializeField] private GameObject _userBarBg;
		[SerializeField] private Button _claimButton;
		[SerializeField] private Image _starIcon;

		public int _rank;

		public void InitItem(string name, int rank, string avatarName, int point)
		{
			_rank = rank;

			if (name == "You")
			{
				_userBarBg.SetActive(true);
				avaListImg.sprite = SpritesCollection.Instance.CurrentAvatarSprite;

				if (rank > 2 && TopChartsController.Instance != null)
				{
					if (TopChartsController.Instance.SeasonEnded)
						ActiveClaimButton();
					else
						TopChartsController.Instance.OnSeasonEnd += ActiveClaimButton;
				}
			}
			else
			{
				_userBarBg.SetActive(false);
				avaListImg.sprite = DataController.GetAvatarSprite(avatarName);
			}

			txtPoint.text = point.ToString();
			txtName.text = name;

			if (name == "You" && rank >= TopChartsDataManager.PlayerCount)
				txtRank.text = TopChartsDataManager.PlayerCount + "+";
			else
				txtRank.text = (rank + 1).ToString();
		}

		private void ActiveClaimButton()
		{
			_starIcon.gameObject.SetActive(false);
			txtPoint.gameObject.SetActive(false);

			_claimButton.gameObject.SetActive(true);
			Reward reward = GetRewardByRank(_rank);
			_claimButton.onClick.AddListener(() =>
			{
				MainMenuController.Instance.RewardPanelController.Init(reward.RewardTypes, reward.Quantities, false);
				DeActiveClaimButton();
				MainMenuUIController.Instance.GoHome();
				TopChartsController.Instance.StartNewSeason();
			});
		}

		private void DeActiveClaimButton()
		{
			_starIcon.gameObject.SetActive(true);
			txtPoint.gameObject.SetActive(true);
			Debug.Log(gameObject.name);
			_claimButton.gameObject.SetActive(false);
		}

		private Reward GetRewardByRank(int rank)
		{
			rank += 1;
			var rewards = TopChartsController.Instance.Rewards;

			if (rank >= 300)
				return rewards[5];
			if (rank >= 200)
				return rewards[4];

			return rewards[3];
		}

		public void SetupToolTips()
		{
			foreach (Transform child in transform.parent) children.Add(child);

			children.Sort((x, y) => x.GetComponent<PlayerCard>()._rank
				.CompareTo(y.GetComponent<PlayerCard>()._rank));
			// Thiết lập lại thứ tự trong hierarchy
			for (int i = 0; i < children.Count; i++) children[i].SetSiblingIndex(i);

			// if (rankTemp < 5)
			// 	toolTips.GetComponent<ToolTipForSuperChef>()
			// 		.Init(TopChartRewardController.Instance.itemSuperChefDefaultsData.itemSuperChefDefaults[rankTemp]);
			// else if (5 <= rankTemp && rankTemp < 50)
			// 	toolTips.GetComponent<ToolTipForSuperChef>()
			// 		.Init(TopChartRewardController.Instance.itemSuperChefDefaultsData.itemSuperChefDefaults[5]);
			// else if (50 <= rankTemp && rankTemp < 100)
			// 	toolTips.GetComponent<ToolTipForSuperChef>()
			// 		.Init(TopChartRewardController.Instance.itemSuperChefDefaultsData.itemSuperChefDefaults[6]);
			// toolTips.SetActive(true);
			StartCoroutine(OffToolTips());
		}

		private IEnumerator OffToolTips()
		{
			yield return new WaitForSeconds(0.3f);
			toolTips.SetActive(false);
		}
	}
}