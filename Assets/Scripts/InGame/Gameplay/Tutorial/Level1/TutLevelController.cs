using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Data;
using DG.Tweening;
using Gameplay;
using Gameplay.Tutorial.Level1;
using UnityEngine;
using Utilities;

namespace InGame.Gameplay.Tutorial.Level1
{
	public class TutLevelController : LevelController
	{
		[SerializeField] private Transform _hand;

		protected override IEnumerator Start()
		{
			LevelTime = CalculateTime();
			Ui.DisplayCoin(0);
			Ui.DisplayStar(0);
			yield return StartCoroutine(Tutorial());
		}
 
		private IEnumerator Tutorial()
		{
			yield return null;
			List<TutItem> items = Shelves[0].GetAllItems().Cast<TutItem>().ToList();

			Vector2[] startPos = new Vector2[items.Count];
			for (int i = 0; i < items.Count; i++)
				startPos[i] = items[i].Position;

			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];
				Vector2 correctPos = FindCorrectPosition(item);

				_hand.position = startPos[i];
				_hand.localScale = new Vector3(1.4f, 1.4f, 1);

				Tween handMoveTween = DOTween.Sequence()
					.Append(_hand.DOScale(1, 0.6f))
					.Append(_hand.DOMove(correctPos, 0.8f).SetEase(Ease.InOutQuad))
					.Append(_hand.DOScale(1.3f, 0.4f).SetEase(Ease.InOutQuad))
					.SetLoops(-1, LoopType.Restart);

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
			AddCoin(1);
			AddStar(3);
		}

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
			StartCoroutine(CommonIEnumerator.WaiForSeconds(0.5f, () => Ui.ShowWinPanel()));
		}
	}
}