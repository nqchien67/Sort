using System;
using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MainMenu.DailyReward
{
	public class DailyRewardButton : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI dayLabelText, quantityLabelText;
		[SerializeField] private Sprite[] itemSprites;
		[SerializeField] private Sprite[] _frameSprites;
		[SerializeField] private Sprite _day7FrameSprite;
		[SerializeField] private Material _disabledMaterial;
		[SerializeField] private Sprite _multipleRewardSprite;

		[SerializeField] private Image _highlightSprite, _tick, _frame;
		[SerializeField] private Image _itemIcon;

		private UnityAction onCloseCallback;
		private bool canDoubleReward;
		private readonly List<RewardType> _rewards = new List<RewardType>();
		private readonly List<int> _rewardQuantities = new List<int>();
		private int _itemSkinId;
		public bool canClaim;

		public int _date;

		public void Init(RewardType[] items, int[] quantities, int date, int receivedProgress, bool canDouble,
			UnityAction onCloseCallback = null)
		{
			_date = date;
			// dayLabelText.text = Lean.Localization.LeanLocalization.GetTranslationText("daily_reward_day") + date;
			dayLabelText.text = "Day " + date;

			canClaim = receivedProgress + 1 == date && DailyRewardController.CanClaimReward();
			if (date < receivedProgress + 1)
			{
				_itemIcon.gameObject.SetActive(false);
				_tick.gameObject.SetActive(true);
				_frame.sprite = _frameSprites[0];
			}
			else
			{
				_itemIcon.gameObject.SetActive(true);
				SetConsumableIcon(items, quantities);
				ResizeIcon(items[0]);

				if (canClaim)
					_frame.sprite = _frameSprites[1];
			}

			if (date == 7)
				SetDay7Frame();

			canDoubleReward = canDouble;
			this.onCloseCallback = onCloseCallback;
		}

		private void SetConsumableIcon(RewardType[] consumables, int[] quantities)
		{
			if (consumables.Length > 1)
			{
				_itemIcon.sprite = _multipleRewardSprite;
				return;
			}

			RewardType reward = consumables[0];
			int quantity = quantities[0];

			// if (reward == RewardType.ItemSkin)
			// {
			// 	var notPurchasedItemIds = SpritesCollection.Instance.GetNotPurchasedItemsId();
			// 	if (notPurchasedItemIds.Count == 0)
			// 		_itemSkinId = notPurchasedItemIds[Random.Range(0, notPurchasedItemIds.Count)];
			// 	else
			// 		reward = RewardType.Coin;
			// }

			SetRewardQuantities(reward, quantity);
			_itemIcon.sprite = RewardHelper.Instance.GetRewardSprite(reward);
		}
		
		private void ResizeIcon(RewardType item)
		{
			if (item == RewardType.Coin)
				return;

			var rectTransform = _itemIcon.GetComponent<RectTransform>();
			var size = rectTransform.sizeDelta;
			size.x = size.y;
			rectTransform.sizeDelta = size;
		}

		private void SetDay7Frame()
		{
			if (canClaim)
				_frame.sprite = _day7FrameSprite;
			else
				_frame.type = Image.Type.Sliced;

			var rectTransform = GetComponent<RectTransform>();
			rectTransform.sizeDelta = rectTransform.parent.GetComponent<RectTransform>().sizeDelta;
		}

		private void SetRewardQuantities(RewardType rewardType, int quantity)
		{
			_rewards.Add(rewardType);
				_rewardQuantities.Add(quantity);
		}

		public void OnClick()
		{
			if (canClaim)
			{
				int received_progress = PlayerPrefs.GetInt("received_progress", 0);
				PlayerPrefs.SetInt("received_progress", received_progress + 1);
				if ((received_progress + 1) % 7 == 0)
				{
					PlayerPrefs.SetInt("received_progress", 0);
					int weeklyRewardProgress = PlayerPrefs.GetInt("weekly_reward_progress", 0);
					PlayerPrefs.SetInt("weekly_reward_progress", weeklyRewardProgress + 1);
				}

				PlayerPrefs.SetInt("last_received_date", DateTime.Now.DayOfYear);
				PlayerPrefs.SetInt("dayAccumulated", PlayerPrefs.GetInt("dayAccumulated", 0) + 1);

				onCloseCallback?.Invoke();
				if (_rewardQuantities.Count != 0)
					FindObjectOfType<RewardPanelController>().Init(_rewards.ToArray(),
						_rewardQuantities.ToArray(), true, canDoubleReward);
			}
		}
	}
}