using Gameplay;
using InGame.Gameplay;
using UnityEngine;

namespace FallingLevel
{
	public class FallingItemLayer : ItemLayer
	{
		public override void CheckShouldDestroy(bool reRenderShelf = true)
		{
			if (ItemsCount != 0)
				return;
			
			Shelf.RemoveFrontLayer();
			if (reRenderShelf)
				Shelf.RenderLayers();
		}
	}
}