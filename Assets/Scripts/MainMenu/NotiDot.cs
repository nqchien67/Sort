using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace MainMenu
{
	public class NotiDot : MonoBehaviour
	{
		[SerializeField] private float _blinkDuration;

		public void Enable(bool enable)
		{
			if (enable)
			{
				gameObject.SetActive(true);
				StartCoroutine(Blink());
			}
			else
				Disable();
		}

		private IEnumerator Blink()
		{
			while (enabled)
			{
				yield return transform.DOScale(1.1f, _blinkDuration);
				yield return new WaitForSeconds(0.2f);
			}
		}

		public void Disable()
		{
			gameObject.SetActive(false);
		}
	}
}