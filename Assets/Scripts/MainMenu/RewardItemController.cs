using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
	public class RewardItemController : MonoBehaviour
	{
		[SerializeField] private Image itemImage;
		[SerializeField] private TextMeshProUGUI itemQuantityTxt;

		public void Init(RewardType rewardType, int quantity)
		{
			itemImage.sprite = RewardHelper.Instance.GetRewardSprite(rewardType);
			itemQuantityTxt.text = quantity.ToString();
		}
	}
}