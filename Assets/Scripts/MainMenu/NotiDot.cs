using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MainMenu
{
	public class NotiDot : MonoBehaviour
	{
		[SerializeField] private float _blinkDuration;
		[SerializeField] private float _blinkScale;

		private void Start()
		{
			StartCoroutine(Blink());
		}

		public void SetEnable(bool enable)
		{
			if (enable)
			{
				gameObject.SetActive(true);
				StopAllCoroutines();
				StartCoroutine(Blink());
			}
			else
				Disable();
		}

		private IEnumerator Blink()
		{
			yield return new WaitForSeconds(Random.value);
			while (enabled)
			{
				yield return transform.DOScale(_blinkScale, _blinkDuration).WaitForCompletion();
				yield return new WaitForSeconds(_blinkDuration * 2f);
				yield return transform.DOScale(1, _blinkDuration).WaitForCompletion();
			}
		}

		public void Disable()
		{
			gameObject.SetActive(false);
		}
	}
}