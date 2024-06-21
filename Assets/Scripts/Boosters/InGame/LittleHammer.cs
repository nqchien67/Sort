using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using InGame.Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Boosters.InGame
{
	public class LittleHammer : InGameBoosterButton
	{
		private SkeletonAnimation _effectSkeletonAnimation;

		public override void Use()
		{
			base.Use();
			CollectItems();
			ReduceQuantity();
		}

		protected override Transform SpawnEffect()
		{
			if (_spawnedEffect == null)
			{
				_spawnedEffect = Instantiate(_effectPrefab, Vector3.zero, Quaternion.identity);
				_effectSkeletonAnimation = _spawnedEffect.GetComponentInChildren<SkeletonAnimation>();
				_effectSkeletonAnimation.AnimationState.Complete += OnAnimationComplete;
				return _spawnedEffect;
			}

			_effectSkeletonAnimation.AnimationState.SetAnimation(0, "animation",
				false);
			return _spawnedEffect;
		}

		protected override bool CanUse()
		{
			return LevelController.CanDrag;
		}

		private Coroutine CollectItems(bool playEffect = true)
		{
			List<Item> items = GetAllFrontItems();
			if (items.Count == 0)
				return null;
			Item randomItem = items[Random.Range(0, items.Count)];
			var foundItems = FindSameItems(randomItem);
			return StartCoroutine(DestroyItems(foundItems, playEffect));
		}

		private IEnumerator DestroyItems(List<Item> items, bool playEffect)
		{
			LevelController.CanDrag = false;
			const float duration = 0.4f;

			foreach (var item in items)
			{
				item.Renderer.sortingOrder = 1;
				item.Renderer.material = LevelController.NormalMaterial;

				var layer = item.Layer;
				layer.RemoveItem(item);
				layer.CheckShouldDestroy();

				item.transform.DOMove(Vector3.zero, duration)
					.SetEase(Ease.InBack)
					.OnComplete(() => Destroy(item.gameObject));
			}

			yield return new WaitForSeconds(0.05f);
			if (playEffect)
				SpawnEffect();
			yield return new WaitForSeconds(duration - 0.15f);

			LevelController.EatASet();

			yield return null;
			LevelController.CanDrag = true;
		}

		private void OnAnimationComplete(TrackEntry trackEntry)
		{
			// _effectSkeletonAnimation.AnimationState.ClearTrack(trackEntry.TrackIndex);
		}

		private void OnDestroy()
		{
			if (_spawnedEffect != null)
				_effectSkeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
		}

		protected override bool IsBoosterUnlocked()
		{
			int highestPassedLevel = PlayerPrefs.GetInt("level", 0);
			return highestPassedLevel >= 1;
		}
	}
}