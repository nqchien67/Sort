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
	public class Refresh : InGameBoosterButton
	{
		private List<Item> _items;
		private SkeletonAnimation _effectSkeletonAnimation;

		public override void Use()
		{
			base.Use();
			LevelController.CanDrag = false;
			_items = GetAllItems();
			Debug.Log("_items.Count: " + _items.Count);
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
			CameraController.Instance.StartShake(1.5f, 0.05f);

			foreach (var item in items)
			{
				item.transform.DOMove(Vector3.zero, duration).SetEase(Ease.InBack);
				yield return null;
			}

			yield return new WaitForSeconds(0.4f);
			LevelController.ShuffleItems(items);
			CameraController.Instance.StartShake(0.11f, 0.15f);

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

		protected override bool CanUse()
		{
			return LevelController.CanDrag && !LevelController.MovingItem;
		}

		private List<Item> GetAllItems()
		{
			List<Item> items = new List<Item>();
			foreach (var s in LevelController.Shelves)
			{
				var allItems = s.GetAllItems();
				Debug.Log(s.gameObject.name + " :all items: " + allItems.Count);
				items.AddRange(allItems);
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
			int highestPassedLevel = PlayerPrefs.GetInt("level", 0);
			return highestPassedLevel >= 3;
		}
	}
}