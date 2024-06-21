using System;
using Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Audio
{
	public class ButtonClickAudio : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
	{
		[SerializeField] private AudioClip btnClickAudio;
		public bool isNotScale;
		private Vector3 originScale;

		private void Start()
		{
			if (btnClickAudio == null && MainMenuController.Instance != null)
				btnClickAudio = MainMenuController.Instance.ButtonClickSfx;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			originScale = transform.localScale;
			if (isNotScale)
				return;
		
			transform.localScale *= 0.85f;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (isNotScale)
				return;
			transform.localScale = originScale;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (btnClickAudio != null)
				AudioController.Instance.PlaySfx(btnClickAudio);
		}

		public void OnClick()
		{
		}
	}
}