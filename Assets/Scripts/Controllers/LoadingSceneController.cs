using Audio;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
	public class LoadingSceneController : MonoBehaviour
	{
		[SerializeField] private Transform _logo;
		[SerializeField] private Image _background;
		[SerializeField] private float _fadeDuration;
		[SerializeField] private float _logoInDuration;
		[SerializeField] private float _logoOutDuration;
		[SerializeField] private float _amplitude;
		[SerializeField] private Sprite _hardLevelBackground;
		[SerializeField] private AudioClip _sfx;
		
		private float _logoOutY;

		private void Start()
		{
			if (LevelData.IsHardLevel(DataController.Instance.HighestPassedLevel + 1))
				_background.sprite = _hardLevelBackground;

			Color color = _background.color;
			color.a = 0;
			_background.color = color;

			_logoOutY = -_logo.localPosition.y - 200;

			AudioController.Instance.StopMusic();
		}

		public YieldInstruction FadeIn()
		{
			return DOTween.Sequence()
				.Append(_background.DOFade(1, _fadeDuration))
				.Append(_logo.DOLocalMoveY(0, _logoInDuration).SetEase(Ease.OutElastic, _amplitude, 0))
				.Append(_logo.DOScale(1.2f, 0.2f).SetDelay(0.3f).SetEase(Ease.InBack))
				.SetUpdate(true)
				.WaitForCompletion();
		}

		public YieldInstruction FadeOut()
		{
			AudioController.Instance.PlaySfx(_sfx);
			return DOTween.Sequence()
				.Append(_logo.DOScale(1, 0.3f).SetEase(Ease.InBack))
				.Append(_logo.DOLocalMoveY(_logoOutY, _logoOutDuration))
				.Append(_background.DOFade(0, _fadeDuration))
				.SetUpdate(true)
				.WaitForCompletion();
		}
	}
}