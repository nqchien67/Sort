using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.TopBar
{
	public class Star : MonoBehaviour
	{
		public TextMeshProUGUI Text;
		public Image Icon;

		private Tween _updateTween;
		private Vector3 _originalScale;

		private int _value;

		[SerializeField] private float _updateDuration = 0.3f;
		
		private void Start()
		{
			_originalScale = Icon.transform.localScale;
			UpdateValue();
		}
		
		public void UpdateValue()
		{
			int currentValue = int.Parse(Text.text);
			int endValue = DataController.Instance.Star;

			if (_updateTween != null && _updateTween.IsActive())
			{
				_updateTween.Kill();
				Icon.transform.localScale = _originalScale;
			}

			_updateTween = DOTween.Sequence()
				.Join(Text.DOCounter(currentValue, endValue, _updateDuration, false))
				.Join(Icon.transform.DOScale(_originalScale * 1.25f, 0.06f).SetLoops(2, LoopType.Yoyo));
		}
	}
}