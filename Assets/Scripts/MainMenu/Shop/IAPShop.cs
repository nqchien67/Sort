using System;
using System.Collections;
using DG.Tweening;
using MainMenu.TopBar;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace MainMenu.Shop
{
	public class IAPShop : SingletonCore<IAPShop>
	{
		[SerializeField] private float _displayPackTime = 0.2f;
		[SerializeField] private LayoutGroup _layoutGroup;
		[SerializeField] private ScrollRect _scrollRect;

		private CanvasGroup _canvasGroup;
		private float _initialPacksAnchorPosX;
		private ShopBundle[] _shopPacks;
		private float _initialCanvasAlpha;

		protected override void Awake()
		{
			base.Awake();
			_canvasGroup = GetComponentInChildren<CanvasGroup>();
			_initialCanvasAlpha = _canvasGroup.alpha;

			ScrollRect layoutGroup = GetComponentInChildren<ScrollRect>();
			_shopPacks = layoutGroup.content.GetComponentsInChildren<ShopBundle>();

			_canvasGroup.alpha = 0;
		}

		private IEnumerator Start()
		{
			yield return new WaitForEndOfFrame();
			_initialPacksAnchorPosX = _shopPacks[0].RectTransform.anchoredPosition.x;
		}

		private IEnumerator DisplayPacks()
		{
			yield return null;
			_layoutGroup.enabled = false;
			foreach (var shopPack in _shopPacks)
				ResetPackPos(shopPack);

			_canvasGroup.DOFade(_initialCanvasAlpha, _displayPackTime).WaitForCompletion();

			float waitTime = _displayPackTime / 2f;
			foreach (var pack in _shopPacks)
			{
				pack.RectTransform.DOAnchorPosX(_initialPacksAnchorPosX, _displayPackTime).SetEase(Ease.OutBack);
				yield return new WaitForSeconds(waitTime);
			}

			_layoutGroup.enabled = true;
		}

		private IEnumerator HidePacks()
		{
			float waitTime = _displayPackTime / 2f;

			_canvasGroup.DOFade(0, waitTime * _shopPacks.Length).SetEase(Ease.Linear);
			foreach (var pack in _shopPacks)
			{
				pack.RectTransform.DOAnchorPosX(0, _displayPackTime).SetEase(Ease.OutQuart);
				yield return new WaitForSeconds(waitTime);
			}

			gameObject.SetActive(false);
		}

		private static void ResetPackPos(ShopBundle shopBundle)
		{
			Vector2 anchorPos = shopBundle.RectTransform.anchoredPosition;
			anchorPos.x = 0;
			shopBundle.RectTransform.anchoredPosition = anchorPos;
		}

		public void Show()
		{
			gameObject.SetActive(true);

			ResetAnim();
			StartCoroutine(DisplayPacks());
		}

		public void Close()
		{
			ResetAnim();
			StartCoroutine(HidePacks());
		}

		private void ResetAnim()
		{
			StopAllCoroutines();
			_canvasGroup.DOKill();
			foreach (var pack in _shopPacks)
			{
				pack.RectTransform.DOKill();
			}
		}

		private void OnDisable()
		{
			_scrollRect.verticalNormalizedPosition = 1;
		}

		public void ScrollToBottom()
		{
			Vector2 focusPoint = _scrollRect.content.GetCorners()[0];
			StartCoroutine(_scrollRect.FocusAtPointCoroutine(focusPoint, 2));
		}
	}
}