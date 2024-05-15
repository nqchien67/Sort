using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Data;
using DG.Tweening;
using UnityEngine;

namespace Gameplay.Tutorial.Level1
{
	public class TutLevelController : LevelController
	{
		[SerializeField] private Transform _hand;

		protected override void Start()
		{
			LevelTime = CalculateTime();
			StartCoroutine(Tutorial());
		}

		private IEnumerator Tutorial()
		{
			yield return null;
			List<TutItem> items = Shelves[0].GetAllItems().Cast<TutItem>().ToList();
			foreach (var item in items)
			{
				Vector2 correctPos = FindCorrectPosition(item);
				Vector2 startPos = item.Position;

				_hand.position = startPos;
				Tween handMoveTween = DOTween.Sequence()
					.Append(_hand.DOMove(correctPos, 0.8f).SetEase(Ease.InOutQuad))
					.Append(_hand.DOMove(startPos, 0.8f).SetEase(Ease.InOutQuad))
					.SetLoops(-1, LoopType.Yoyo);

				yield return new WaitUntil(() => item == null);
				handMoveTween.Kill();
			}
			
			_hand.gameObject.SetActive(false);
		}

		private Vector2 FindCorrectPosition(TutItem item)
		{
			ItemLayer correctLayer = FindCorrectLayer(item);

			if (correctLayer == null)
				return Vector2.zero;

			for (int i = 0; i < correctLayer.Items.Length; i++)
			{
				if (correctLayer.Items[i] != null)
					continue;
				Vector2 localPos = new Vector2(correctLayer.GetItemLocalPosX(i), 0);
				return correctLayer.transform.TransformPoint(localPos);
			}

			return Vector2.zero;
		}

		private ItemLayer FindCorrectLayer(TutItem item)
		{
			for (int i = 1; i < Shelves.Length; i++)
			{
				var layer = Shelves[i].FrontLayer;
				var allItems = layer.GetAllItems();
				if (allItems[0].Type == item.Type)
					return layer;
			}

			return null;
		}

		public override void GainScore()
		{
		}
// anh chiến ăn cứt 
		public override void Win()
		{
			if (_isGameEnd)
				return;
			_isGameEnd = true;
			
			int currentLevel = LevelIndex;
			currentLevel++;
			if (currentLevel > DataController.Instance.LevelsData.Length - 1)
				currentLevel = 0;
			PlayerPrefs.SetInt("level", currentLevel);
			StartCoroutine(GameManager.WaiForSeconds(0.5f, () => Ui.ShowWinPanel()));
		}
	}
}