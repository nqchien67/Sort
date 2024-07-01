using System;
using IAP;
using UnityEngine;

namespace MainMenu.Shop
{
	public class ShopBundle : MonoBehaviour
	{
		[SerializeField] private PackContent _packContent;

		public RectTransform RectTransform;
		private Pack _pack;

		private void Start()
		{
			Init();
		}

		private void Init()
		{
			_pack = IAPPackHelper.GetBundle(GetPackId());
			if (_packContent != null)
				_packContent.Init(_pack);
		}

		private string GetPackId()
		{
			return "basicbundle";
		}
	}
}