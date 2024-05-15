using Data;
using UnityEngine;

namespace MainMenu.ShopSkin
{
	public class ItemSkinButton : SkinButton
	{
		public override void Init(Skin skin, SkinGroup skinGroup)
		{
			base.Init(skin, skinGroup);
			_skinImage.sprite = SkinDataController.Instance.GetPurchasableItemSprite(_skin.Id);
		}

		public override void ToggleUse()
		{
			ShopSkinPanel.SelectingButton = null;

			_skin.InUse = !_skin.InUse;
			_tick.SetActive(_skin.InUse);
			DataController.Instance.SaveData();
		}
	}
}