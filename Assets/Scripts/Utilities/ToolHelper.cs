using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities
{
	public static class ToolHelper
	{
		public static DateTime ToDateTime(this string datetime, char dateSplitter = '-', char timeSplitter = ':',
			char millisecondSplitter = ',')
		{
			try
			{
				datetime = datetime.Trim();
				datetime = datetime.Replace("  ", " ");
				string[] body = datetime.Split(' ');
				string[] date = body[0].Split(dateSplitter);
				int year = Int32.Parse(date[0]);
				int month = Int32.Parse(date[1]);
				int day = Int32.Parse(date[2]);

				return new DateTime(year, month, day);
			}
			catch
			{
				return new DateTime();
			}
		}

		public static T[] RemoveNulls<T>(T[] array)
		{
			return array.Where(element => element != null).ToArray();
		}
	}

	[Serializable]
	public class Range
	{
		public int Min;
		public int Max;

		public Range(int min, int max)
		{
			Min = min;
			Max = max;
		}
		
		public int GetRandomValue()
		{
			return Random.Range(Min, Max + 1);
		}
	}

	[Serializable]
	public class RangeFloat
	{
		public float Min;
		public float Max;

		public float GetRandomValue()
		{
			return Random.Range(Min, Max);
		}
	}
}