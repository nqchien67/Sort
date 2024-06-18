using Data;
using UnityEngine;

namespace Boosters
{
	[CreateAssetMenu(fileName = "Booster", menuName = "ScriptableObject/Booster", order = 0)]
	public class BoosterData : ScriptableObject
	{
		public string Name;
		public BoosterType Type;
		public Sprite Sprite;
		public string Description;
	}
}