using UnityEngine;

namespace Boosters
{
	[CreateAssetMenu(fileName = "Booster", menuName = "ScriptableObject/Booster", order = 0)]
	public class BoosterData : ScriptableObject
	{
		public Sprite Sprite;
		public string Description;
	}
}