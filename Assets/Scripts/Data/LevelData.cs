using System;
using UnityEngine;

namespace Data
{
	[Serializable]
	public class LevelData
	{
		public int id;
		public int TotalShelves;
		public int ItemTypes;
		public int LayerPerShelf;
		public int LockShelves;
		public bool UnknownItem;
		public int TotalItems => 3 * ItemTypes;
		public int LockNumber => Mathf.Min(LockShelves, 4);

		// public LevelData(int totalShelves, int totalItemTypes, int totalLayerPerShelf)
		// {
		// 	TotalShelves = totalShelves;
		// 	TotalItemTypes = totalItemTypes;
		// 	TotalLayerPerShelf = totalLayerPerShelf;
		// }
	}

	public class LevelDataCollection
	{
		public LevelData[] LevelsData;
	}
}