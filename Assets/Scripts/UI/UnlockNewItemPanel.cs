using System.Collections.Generic;
using System.Linq;
using Controllers;
using Data;
using DG.Tweening;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class UnlockNewItemPanel : Popup
	{
		[SerializeField] private Image _itemIcon;
		[SerializeField] private Button _claimButton;

		private int _itemId;
		private UnlockableItemSkin _unlockableItemSkin;
		
		private void Start()
		{
			List<UnlockableItemSkin> notUnlockedItems = SkinDataController.Instance.GetNotUnlockedItems();

			_unlockableItemSkin = notUnlockedItems[Random.Range(0, notUnlockedItems.Count)];
			_itemIcon.sprite = SkinDataController.Instance.GetUnlockableItemSprite(_unlockableItemSkin.Id);
		}
		
		public void OnClickClaim()
		{
			// SkinDataController.Instance.AddUnlockedItemSkin(_itemId);
			_unlockableItemSkin.Unlocked = true;
			DataController.Instance.SaveData();
			Close();
			MainMenuController.Instance.DisplayMenuPanel();
		}

		private bool ArrayContains(int[] array, int value)
		{
			return array.Any(t => t == value);
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			Destroy(gameObject);
		}
	}
}