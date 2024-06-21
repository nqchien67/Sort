using System;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.ShopSkin
{
	public abstract class SkinButton : MonoBehaviour
	{
		[SerializeField] private Image _frame;
		[SerializeField] protected Image _skinImage;
		[SerializeField] private GameObject _chosenFrameBorder;
		[SerializeField] protected GameObject _tick;

		[SerializeField] private Sprite _unlockedFrame;

		protected Skin _skin;
		protected SkinGroup SkinGroup;
		protected ShopSkinPanel ShopSkinPanel => SkinGroup.ShopSkinPanel;

		public virtual void Init(Skin skin, SkinGroup skinGroup)
		{
			_skin = skin;
			SkinGroup = skinGroup;

			if (skin.Unlocked) _frame.sprite = _unlockedFrame;
			
			_tick.SetActive(_skin.InUse);

			if (_skin.InUse)
				skinGroup.SkinInUse = this;
		}

		public virtual void Unlock()
		{
			if (_skin.Unlocked)
				return;

			_skin.Unlocked = true;
			_frame.sprite = _unlockedFrame;

			OnClick();
		}

		public void OnClick()
		{
			if (_skin.Unlocked)
				ToggleUse();
			else
				Select();
		}

		public abstract void ToggleUse();

		private void Select()
		{
			ShopSkinPanel.SelectingButton = this;
			_chosenFrameBorder.SetActive(true);
		}

		public virtual void DeSelect()
		{
			_chosenFrameBorder.SetActive(false);
		}
	}
}