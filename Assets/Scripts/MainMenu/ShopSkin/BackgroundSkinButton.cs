using Data;
using UnityEngine;

namespace MainMenu.ShopSkin
{
	public class BackgroundSkinButton : SkinButton
	{
		public override void Init(Skin skin, SkinGroup skinGroup)
		{
			base.Init(skin, skinGroup);
			_skinImage.sprite = SpritesCollection.Instance.GetShelfSkinIcon(_skin.Id);
		}

		public override void ToggleUse()
		{
			ShopSkinPanel.SelectingButton = null;
			if (SkinGroup.SkinInUse != null)
			{
				if (SkinGroup.SkinInUse == this)
					return;

				((BackgroundSkinButton)SkinGroup.SkinInUse).RemoveInUse();
			}

			SkinGroup.SkinInUse = this;
			
			_skin.InUse = true;
			_tick.SetActive(true);
			DataController.Instance.SaveData();
		}

		private void RemoveInUse()
		{
			_skin.InUse = false;
			_tick.SetActive(false);
		}
	}
}