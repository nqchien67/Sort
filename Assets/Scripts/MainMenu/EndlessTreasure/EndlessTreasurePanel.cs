using System.Collections;
using DG.Tweening;
using IAP;
using MainMenu.Shop;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.EndlessTreasure
{
	public class EndlessTreasurePanel : MonoBehaviour
	{
		private CanvasGroup _canvasGroup;

		[SerializeField] private float _displayPackTime = 0.2f;

		private TreasurePack[] _packs;
		private Pack[] _treasurePacksData;

		private float _initialCanvasAlpha;
		private float _initialPacksAnchorPosX;

		private void Start()
		{
			_canvasGroup = GetComponentInChildren<CanvasGroup>();
			_initialCanvasAlpha = _canvasGroup.alpha;

			ScrollRect layoutGroup = GetComponentInChildren<ScrollRect>();
			_packs = layoutGroup.content.GetComponentsInChildren<TreasurePack>();
			_initialPacksAnchorPosX = _packs[0].RectTransform.anchoredPosition.x;

			_canvasGroup.alpha = 0;

			InitPacks();
		}

		private void InitPacks()
		{
			for (int i = 0; i < _packs.Length; i++)
			{
				_packs[i].Init(_treasurePacksData[i]);
				ResetPackPos(_packs[i]);
			}
		}

		public void Show(Pack[] data)
		{
			_treasurePacksData = data;
			StartCoroutine(DisplayPacks());
		}

		private IEnumerator DisplayPacks()
		{
			StopAllCoroutines();

			yield return _canvasGroup.DOFade(_initialCanvasAlpha, _displayPackTime / 2f).WaitForCompletion();

			float waitTime = _displayPackTime / 2f;
			foreach (var pack in _packs)
			{
				pack.RectTransform.DOAnchorPosX(_initialPacksAnchorPosX, _displayPackTime).SetEase(Ease.OutBack);
				yield return new WaitForSeconds(waitTime);
			}
		}

		private IEnumerator HidePacks()
		{
			StopAllCoroutines();

			float waitTime = _displayPackTime / 2f;

			_canvasGroup.DOFade(0, _displayPackTime * _packs.Length).SetEase(Ease.Linear);
			foreach (var pack in _packs)
			{
				pack.RectTransform.DOAnchorPosX(0, _displayPackTime).SetEase(Ease.OutQuart);
				yield return new WaitForSeconds(waitTime);
			}

			Destroy(gameObject);
		}

		private static void ResetPackPos(TreasurePack shopPack)
		{
			Vector2 anchorPos = shopPack.RectTransform.anchoredPosition;
			anchorPos.x = 0;
			shopPack.RectTransform.anchoredPosition = anchorPos;
		}

		public void Close()
		{
			StartCoroutine(HidePacks());
		}
	}
}