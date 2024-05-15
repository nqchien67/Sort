using System;
using System.Collections;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.PiggyBank
{
	public class FullPiggyBankPanelController : MonoBehaviour
	{
		private Vector3 PiggyIcon;
		[SerializeField] private SkeletonGraphic pigAnim;
		[SerializeField] private Image progressImg;
		[SerializeField] private GameObject ConfettiBlast;
		[SerializeField] private AudioClip IncreaseGoldAudio;
		private Action Callback;

		public void Init(Vector3 piggyIcon, Action callback)
		{
			PiggyIcon = piggyIcon;
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

			progressImg.rectTransform.DOSizeDelta(new Vector2(275, 42), 2).SetEase(Ease.OutQuint);

			var delay = new WaitForSeconds(0.04f);
			while (tmp < 1.2f)
			{
				tmp += 0.05f;
				// AudioController.Instance.PlaySfx(IncreaseGoldAudio);
				yield return delay;
			}

			yield return new WaitForSeconds(0.3f);
			ConfettiBlast.SetActive(true);
			pigAnim.AnimationState.SetAnimation(1, "jumpin_loop", false);
			progressImg.transform.parent.gameObject.SetActive(false);
			yield return new WaitForSeconds(1f);
			ConfettiBlast.SetActive(false);
			StartCoroutine(IMove(pigAnim.gameObject));
		}

		public IEnumerator IMove(GameObject gameObject)
		{
			Vector3 target = PiggyIcon - new Vector3(0, 0.25f, 0); //generate a random pos
			Vector3 targetScale = new Vector3(0.3f, 0.3f, 1);

			transform.DOMove(target, 0.75f).SetEase(Ease.OutQuint).OnComplete(() =>
			{
				DOVirtual.DelayedCall(0.25f, () =>
				{
					Callback.Invoke();
					Destroy(gameObject);
				});
			});

			yield return null;
			transform.DOScale(targetScale, 0.75f).SetEase(Ease.OutQuint);
		}

		public Vector3 CalculateQuadraticBezierPoint(float t1, Vector3 p0, Vector3 p1, Vector3 p2)
		{
			float u = 1 - t1;
			float tt = t1 * t1;
			float uu = u * u;
			Vector3 p = uu * p0;
			p += 2 * u * t1 * p1;
			p += tt * p2;
			return p;
		}
	}
}