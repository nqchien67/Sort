using System;

namespace Data
{
	[Serializable]
	public class ComboData
	{
		public int Combo;
		public int Star;
		public float Time;
	}

	public class ComboDataCollection
	{
		public ComboData[] CombosData;
	}
}