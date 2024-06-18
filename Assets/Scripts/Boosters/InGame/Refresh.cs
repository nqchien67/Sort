using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Boosters.InGame
{
	public class Refresh : InGameBoosterButton
	{
		private List<Item> _items;
		private SkeletonAnimation _effectSkeletonAnimation;

		public override void Active()
		{
			if (!LevelController.CanDrag )
				return;

			LevelController.CanDrag = false;
			_items = GetAllItems();
			foreach (var i in _items)
			{
				i.Layer.RemoveItem(i);
			}

			ReduceQuantity();

			StartCoroutine(ShuffleItems(_items));
		}

		private IEnumerator ShuffleItems(List<Item> items)
		{
			const float duration = 0.3f;

			SpawnEffect();

			foreach (var item in items)
			{
				item.transform.DOMove(Vector3.zero, duration).SetEase(Ease.InBack);
				yield return null;
			}

			yield return new WaitForSeconds(0.4f);
			LevelController.ShuffleItems(_items);
			yield return new WaitForSeconds(0.11f);
			LevelController.CanDrag = true;
		}

		protected override Transform SpawnEffect()
		{
			if (_spawnedEffect == null)
			{
				_spawnedEffect = Instantiate(_effectPrefab, Vector3.zero, Quaternion.identity);
				_effectSkeletonAnimation = _spawnedEffect.GetComponent<SkeletonAnimation>();
				_effectSkeletonAnimation.AnimationState.Complete += OnAnimationComplete;
				return _spawnedEffect;
			}

			_effectSkeletonAnimation.AnimationState.SetAnimation(0, "animation", false);
			return _spawnedEffect;
		}

		private List<Item> GetAllItems()
		{
			List<Item> items = new List<Item>();
			foreach (var s in LevelController.Shelves)
			{
				items.AddRange(s.GetAllItems());
			}

			return items;
		}

		private void OnAnimationComplete(TrackEntry trackEntry)
		{
			_effectSkeletonAnimation.AnimationState.ClearTrack(trackEntry.TrackIndex);
			// _spawnedEffect.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			if (_spawnedEffect != null)
				_effectSkeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
		}
		
		protected override bool IsBoosterUnlocked()
		{
			return true;
			int highestPassedLevel = PlayerPrefs.GetInt("level", 0);
			return highestPassedLevel >= 3;
		}
	}
}