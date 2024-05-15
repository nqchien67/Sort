using System.Collections;
using UnityEngine;

namespace Boosters.InGame
{
	public class Freeze : InGameBoosterButton
	{
		[SerializeField] private float _freezeTime;

		public override void Active()
		{
			StartCoroutine(Freezing());
		}

		private IEnumerator Freezing()
		{
			var waitForASecond = new WaitForSeconds(1);

			LevelController.PausedTime = true;
			for (int i = 0; i < _freezeTime; i++)
			{
				yield return waitForASecond;
			}

			LevelController.PausedTime = false;
		}
	}
}