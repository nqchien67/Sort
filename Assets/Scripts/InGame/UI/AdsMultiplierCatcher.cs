using System;
using UnityEngine;

namespace InGame.UI
{
	public class AdsMultiplierCatcher : MonoBehaviour
	{
		public int MultiTime;

		private void OnTriggerEnter2D(Collider2D other)
		{
			switch (other.tag)
			{
				case "x2":
					MultiTime = 2;
					break;
				case "x3":
					MultiTime = 3;
					break;
				case "x4":
					MultiTime = 4;
					break;
			}
		}
	}
}