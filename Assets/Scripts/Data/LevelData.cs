using System;
using UnityEngine;

namespace Data
{
	[Serializable]
	public class LevelData: ICloneable
	{
		public int id;
		public int TotalShelves;
		public int ItemTypes;
		public int LayerPerShelf;
		public int LockShelves;
		public bool HaveUnknownItems;
		public int TotalItems => 3 * ItemTypes;
		public int LockNumber => Mathf.Min(LockShelves, 4);

		public bool IsHardLevel()
		{
			if (id < 10)
				return false;

			return id % 5 == 0;
		}

		public static bool IsHardLevel(int levelId)
		{
			if (levelId < 10)
				return false;

			return levelId % 5 == 0;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}

	public class LevelDataCollection
	{
		public LevelData[] LevelsData;
	}
}