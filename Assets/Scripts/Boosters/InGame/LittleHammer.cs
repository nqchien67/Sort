using System.Collections;
using System.Collections.Generic;
using Controllers;
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
			return LevelController.CanDrag && !LevelController.AnyItemMoving;
		}

		private Coroutine CollectItems()
		{
			List<Item> items = GetAllFrontItems();
			if (items.Count == 0)
				return null;
			Item randomItem = items[Random.Range(0, items.Count)];
			var foundItems = FindSameItems(randomItem);
			return StartCoroutine(DestroyItems(foundItems));
		}

		private IEnumerator DestroyItems(List<Item> items)
		{
			LevelController.CanDrag = false;
			const float duration = 0.7f;

			foreach (var item in items)
			{
				item.Renderer.sortingOrder = 1;
				item.Renderer.material = LevelController.NormalMaterial;

				var layer = item.Layer;
				layer.RemoveItem(item);
				layer.CheckShouldDestroy();

				DOTween.Sequence()
					.Append(item.transform.DOScale(1.1f, 0.2f))
					.Join(item.transform.DOShakePosition(0.3f, new Vector3(0.15f, 0.15f), 20))
					.Append(item.transform.DOMove(Vector3.zero, duration - 0.2f)
						.SetEase(Ease.InBack))
					.OnComplete(() => Destroy(item.gameObject));
			}

			yield return new WaitForSeconds(duration - 0.3f);
			SpawnEffect();
			yield return new WaitForSeconds(duration - 0.2f);
			CameraController.Instance.StartShake(0.1f, 0.1f);
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
			int currentLevel = LevelController.Instance.LevelIndex;
			return currentLevel > 2;
		}
	}
}