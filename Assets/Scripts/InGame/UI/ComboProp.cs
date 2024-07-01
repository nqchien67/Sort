using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
	public class ComboProp : MonoBehaviour
	{
		[SerializeField] private Sprite[] _sprites;
		[SerializeField] private float _moveDuration = 0.5f;
		[SerializeField] private float _fadeDuration = 1;
		[SerializeField] private Image _image;

		private Vector2 _initialScale;

		private void Awake()
		{
			_initialScale = transform.localScale;
		}

		public void Init(int combo)
		{
			transform.localScale = _initialScale;
			_image.color = Color.white;

			gameObject.SetActive(true);
			int index = combo / 5 - 1;
			_image.sprite = _sprites[index];

			StartCoroutine(Effect());
		}

		private IEnumerator Effect()
		{
			transform.DOScale(1, _moveDuration);
			yield return transform.DOMove(Vector2.zero, _moveDuration).SetEase(Ease.OutBack).WaitForCompletion();
			yield return new WaitForSeconds(0.1f);
			yield return _image.DOFade(0, _fadeDuration).SetEase(Ease.OutCubic).WaitForCompletion();
			gameObject.SetActive(false);
		}
	}
}