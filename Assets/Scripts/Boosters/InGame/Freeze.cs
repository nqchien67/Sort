using System.Collections;
using Controllers;
using Data;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Boosters.InGame
{
	public class Freeze : InGameBoosterButton
	{
		[SerializeField] private float _freezeTime;

		private Transform _effect;

		public override void Use()
		{
			base.Use();
			StartCoroutine(Freezing());
			ReduceQuantity();
		}

		protected override Transform SpawnEffect()
		{
			var effect = Instantiate(_effectPrefab, LevelUIController.Instance.Canvas);
			effect.transform.SetAsFirstSibling();
			return effect;
		}

		protected override bool CanUse()
		{
			return !LevelController.PausedTime;
		}

		protected override bool IsBoosterUnlocked()
		{
			int highestPassedLevel = PlayerPrefs.GetInt("level", 0);
			return highestPassedLevel >= 7;
		}

		private IEnumerator Freezing()
		{
			LevelController.PausedTime = true;

			_effect = SpawnEffect();

			SkeletonGraphic[] anims = _effect.GetComponentsInChildren<SkeletonGraphic>(true);
			foreach (var anim in anims)
			{
				anim.AnimationState.SetAnimation(0, "appear", false);
				anim.AnimationState.Complete += track => anim.AnimationState.SetAnimation(0, "loop", true);
			}

			yield return new WaitForSeconds(_freezeTime - 0.5f);

			for (int i = 1; i < anims.Length; i++)
			{
				var anim = anims[i];
				anim.DOFade(0, 1);
			}

			yield return anims[0].DOFade(0, 1).WaitForCompletion();

			Destroy(_effect.gameObject);
			LevelController.PausedTime = false;
		}
	}
}