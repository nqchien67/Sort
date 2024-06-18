using Data;
using UnityEngine;

namespace MainMenu.ShopSkin
{
	public class ItemSkinButton : SkinButton
	{
		public override void Init(Skin skin, SkinGroup skinGroup)
		{
			base.Init(skin, skinGroup);
			_skinImage.sprite = SpritesCollection.Instance.GetPurchasableItemSprite(_skin.Id);
		}

		public override void ToggleUse()
		{
			ShopSkinPanel.SelectingButton = null;

			_skin.InUse = !_skin.InUse;
			_tick.SetActive(_skin.InUse);

			if (!_skin.InUse && SpritesCollection.Instance.PriorityItems.Contains((ItemSkin)_skin))
				SpritesCollection.Instance.RemovePriorityItem((ItemSkin)_skin);
			
			DataController.Instance.SaveData();
		}

		public override void Unlock()
		{
			base.Unlock();
			SpritesCollection.Instance.AddPriorityItem((ItemSkin)_skin);
			DataController.Instance.SaveData();
		}
	}
}