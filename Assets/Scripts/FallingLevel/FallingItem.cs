using Controllers;
using InGame.Gameplay;
using UnityEngine;

namespace FallingLevel
{
	public class FallingItem : Item
	{
		public override bool CanDrag()
		{
			var levelUIController = LevelUIController.Instance;
			bool insideBound = Position.y > levelUIController.MinY && Position.y < levelUIController.MaxY;
			return insideBound && base.CanDrag();
		}
	}
}