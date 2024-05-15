using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.ShopSkin
{
	public class SkinGroup : MonoBehaviour
	{
		[SerializeField] private ScrollRect _scrollRect;
		public ShopSkinPanel ShopSkinPanel;

		public SkinButton SkinInUse;
		
		public void Active(bool isActive)
		{
			_scrollRect.vertical = isActive;
		}

		public void InitSkinButtons<T>(List<T> skins) where T : Skin
		{
			SkinButton _skinButton = GetComponentInChildren<SkinButton>();
			
			for (int i = 1; i < skins.Count; i++)
			{
				SkinButton newButton = Instantiate(_skinButton, _scrollRect.content).GetComponent<SkinButton>();
				newButton.gameObject.name = i.ToString(); 
				newButton.Init(skins[i], this);
			}

			_skinButton.Init(skins[0], this);
		}
	}
}