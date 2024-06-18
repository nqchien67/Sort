using System;
using System.Collections;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.Shop
{
	public class IAPShop : MonoBehaviour
	{
		[SerializeField] private float _displayPackTime = 0.2f;
		[SerializeField] private Ease _displayPackEase;
		private CanvasGroup _canvasGroup;
		private float _initialPacksAnchorPosX;
		private ShopPack[] _shopPacks;
		private float _initialCanvasAlpha;

		private void Awake()
		{
			_canvasGroup = GetComponentInChildren<CanvasGroup>();
			_initialCanvasAlpha = _canvasGroup.alpha;

			ScrollRect layoutGroup = GetComponentInChildren<ScrollRect>();
			_shopPacks = layoutGroup.content.GetComponentsInChildren<ShopPack>();
			_initialPacksAnchorPosX = _shopPacks[0].RectTransform.anchoredPosition.x;

			_canvasGroup.alpha = 0;
			foreach (var shopPack in _shopPacks)
				ResetPackPos(shopPack);
		}

		private IEnumerator DisplayPacks()
		{
			yield return _canvasGroup.DOFade(_initialCanvasAlpha, _displayPackTime / 2f).WaitForCompletion();

			float waitTime = _displayPackTime / 2f;
			foreach (var pack in _shopPacks)
			{
				pack.RectTransform.DOAnchorPosX(_initialPacksAnchorPosX, _displayPackTime).SetEase(Ease.OutBack);
				yield return new WaitForSeconds(waitTime);
			}
		}

		private IEnumerator HidePacks()
		{
			float waitTime = _displayPackTime / 2f;

			_canvasGroup.DOFade(0, _displayPackTime * _shopPacks.Length).SetEase(Ease.Linear);
			foreach (var pack in _shopPacks)
			{
				pack.RectTransform.DOAnchorPosX(0, _displayPackTime).SetEase(Ease.OutQuart);
				yield return new WaitForSeconds(waitTime);
			}

			gameObject.SetActive(false);
		}

		private static void ResetPackPos(ShopPack shopPack)
		{
			Vector2 anchorPos = shopPack.RectTransform.anchoredPosition;
			anchorPos.x = 0;
			shopPack.RectTransform.anchoredPosition = anchorPos;
		}

		public void Show()
		{
			gameObject.SetActive(true);
			StopAllCoroutines();
			StartCoroutine(DisplayPacks());
		}

		public void Close()
		{
			StopAllCoroutines();
			StartCoroutine(HidePacks());
		}
	}
}