using System;
using System.Collections;
using Audio;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.PiggyBank
{
	public class FullPiggyBankPanelController : MonoBehaviour
	{
		private Vector3 _piggyBtnPosition;
		[SerializeField] private SkeletonGraphic pigAnim;
		[SerializeField] private Image progressImg;
		[SerializeField] private GameObject ConfettiBlast;
		[SerializeField] private AudioClip _piggySuckCoinSfx;
		[SerializeField] private Image _background;
		private Action Callback;

		private Vector2 _maxFillBarLength;

		private void Awake()
		{
			var sizeDelta = progressImg.rectTransform.sizeDelta;
			_maxFillBarLength = sizeDelta;

			sizeDelta = new Vector2(0, sizeDelta.y);
			progressImg.rectTransform.sizeDelta = sizeDelta;
		}

		public void Init(Vector3 piggyBtnPosition, Action callback)
		{
			_piggyBtnPosition = piggyBtnPosition;
			StartCoroutine(PlayAnim());
			Callback = callback;
		}

		private IEnumerator PlayAnim()
		{
			pigAnim.AnimationState.SetAnimation(1, "jumpin_x", false);
			yield return new WaitForSeconds(1.3f);
			pigAnim.AnimationState.SetAnimation(1, "suckindiamond_x", false);
			yield return new WaitForSeconds(0.3f);
			progressImg.transform.parent.gameObject.SetActive(true);
			float tmp = 0;

			// progressImg.rectTransform.DOSizeDelta(new Vector2(275, 42), 2).SetEase(Ease.OutQuint);
			progressImg.rectTransform.DOSizeDelta(_maxFillBarLength, 2).SetEase(Ease.OutQuint);

			var delay = new WaitForSeconds(0.04f);
			while (tmp < 1.2f)
			{
				tmp += 0.05f;
				AudioController.Instance.PlaySfx(_piggySuckCoinSfx);
				yield return delay;
			}

			yield return new WaitForSeconds(0.3f);
			ConfettiBlast.SetActive(true);
			pigAnim.AnimationState.SetAnimation(1, "jumpin_loop", false);
			progressImg.transform.parent.gameObject.SetActive(false);
			yield return new WaitForSeconds(1f);
			ConfettiBlast.SetActive(false);
			yield return _background.DOFade(0, 0.2f).WaitForCompletion();

			StartCoroutine(IMove(pigAnim.gameObject));
		}

		public IEnumerator IMove(GameObject gameObject)
		{
			Vector3 target = _piggyBtnPosition; //generate a random pos
			Vector3 targetScale = new Vector3(0.3f, 0.3f, 1);

			transform.DOMove(target, 0.75f).SetEase(Ease.OutQuint).OnComplete(() =>
			{
				DOVirtual.DelayedCall(0.05f, () =>
				{
					Callback.Invoke();
					Destroy(gameObject);
				});
			});

			yield return null;
			transform.DOScale(targetScale, 0.75f).SetEase(Ease.OutQuint);
		}
	}
}