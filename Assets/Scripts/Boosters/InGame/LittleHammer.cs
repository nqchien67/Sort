using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay;
using UnityEngine;

namespace Boosters.InGame
{
	public class LittleHammer : InGameBoosterButton
	{
		public override void Active()
		{
			if (LevelController.CanDrag)
				CollectItems();
		}

		public Coroutine CollectItems()
		{
			List<Item> items = GetAllFrontItems();
			if (items.Count == 0)
				return null;
			Item randomItem = items[Random.Range(0, items.Count)];
			var foundItems = FindSameItems(randomItem);
			return StartCoroutine(DestroyItems(foundItems));
		}

		protected IEnumerator DestroyItems(List<Item> items)
		{
			LevelController.CanDrag = false;
			const float duration = 0.3f;

			foreach (var item in items)
			{
				item.Renderer.sortingOrder = 1;
				item.Renderer.material = LevelController.NormalMaterial;

				var layer = item.Layer;
				layer.RemoveItem(item);
				layer.CheckShouldDestroy();

				item.transform.DOMove(Vector3.up, duration)
					.SetEase(Ease.InBack)
					.OnComplete(() => Destroy(item.gameObject));
			}

			yield return new WaitForSeconds(duration);
			LevelController.EatASet();

			yield return null;
			LevelController.CanDrag = true;
		}
	}
}