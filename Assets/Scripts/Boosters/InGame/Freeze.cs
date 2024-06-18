using System.Collections;
using Controllers;
using Data;
using Spine.Unity;
using UnityEngine;

namespace Boosters.InGame
{
	public class Freeze : InGameBoosterButton
	{
		[SerializeField] private float _freezeTime;

		private GameObject _effectGO;

		public override void Active()
		{
			if (!LevelController.PausedTime && _quantity > 0)
			{
				StartCoroutine(Freezing());
				ReduceQuantity();
			}
		}

		protected override Transform SpawnEffect()
		{
			_effectGO = Instantiate(_effectPrefab, LevelUIController.Instance.Canvas).gameObject;
			return null;
		}

		protected override bool IsBoosterUnlocked()
		{
			int highestPassedLevel = PlayerPrefs.GetInt("level", 0);
			return highestPassedLevel >= 7;
		}

		private IEnumerator Freezing()
		{
			LevelController.PausedTime = true;

			SpawnEffect();
			var waitForASecond = new WaitForSeconds(1);
			for (int i = 0; i < _freezeTime; i++)
			{
				yield return waitForASecond;
			}

			Destroy(_effectGO);
			LevelController.PausedTime = false;
		}
	}
}