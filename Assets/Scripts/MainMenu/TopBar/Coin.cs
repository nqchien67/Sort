using System;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.TopBar
{
	public class Coin : MonoBehaviour
	{
		public TextMeshProUGUI Text;
		public Image Icon;

		private Tween _updateTween;
		private Vector3 _originalScale;

		[SerializeField] private bool _updateConstantly;
		[SerializeField] private float _updateDuration = 0.3f;
		public Vector2 IconPosition => Icon.transform.position;


		private void Start()
		{
			_originalScale = Icon.transform.localScale;
			if (!_updateConstantly)
				UpdateValue();
		}

		private void Update()
		{
			if (_updateConstantly)
			{
				Text.text = DataController.Instance.Coin.ToString();
			}
		}

		public void UpdateValue()
		{
			int currentValue = int.Parse(Text.text);
			int endValue = DataController.Instance.Coin;

			if (currentValue != endValue)
				PlayUpdateAnim(currentValue, endValue);
		}

		private void PlayUpdateAnim(int currentValue, int endValue)
		{
			if (_updateTween != null && _updateTween.IsActive())
			{
				_updateTween.Kill();
				Icon.transform.localScale = _originalScale;
			}

			_updateTween = DOTween.Sequence()
				.Join(Text.DOCounter(currentValue, endValue, _updateDuration, false))
				.Join(Icon.transform.DOScale(_originalScale * 1.25f, 0.06f).SetLoops(2, LoopType.Yoyo))
				.SetUpdate(true);
		}
	}
}