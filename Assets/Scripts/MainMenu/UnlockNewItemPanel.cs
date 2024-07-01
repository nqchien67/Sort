using System.Collections.Generic;
using System.Linq;
using Data;
using InGame.UI;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
	public class UnlockNewItemPanel : Popup
	{
		[SerializeField] private Image _itemIcon;
		[SerializeField] private Button _claimButton;
		[SerializeField] private Sprite[] _itemIcons;

		private int _itemId;
		private UnlockableItemSkin _unlockableItemSkin;

		private void Start()
		{
			List<UnlockableItemSkin> notUnlockedItems = SpritesCollection.Instance.GetNotUnlockedItems();

			_unlockableItemSkin = notUnlockedItems[Random.Range(0, notUnlockedItems.Count)];
			_itemIcon.sprite = _itemIcons[_unlockableItemSkin.Id];

			_unlockableItemSkin.Unlocked = true;
			SpritesCollection.Instance.AddPriorityItem(_unlockableItemSkin);
			DataController.Instance.SaveData();
		}

		public override void Show()
		{
			base.Show();
		}

		public void OnClickClaim()
		{
			Close();
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