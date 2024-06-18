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
		[SerializeField] private Sprite[] itemSprites;

		public void Init(RewardType rewardTypeId, int quantity)
		{
			itemImage.sprite = itemSprites[(int)rewardTypeId];
			itemQuantityTxt.text = quantity.ToString();
		}
	}
}