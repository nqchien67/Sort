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
		public bool _isMute;
		private Vector3 originScale;

		private void Start()
		{
			if (btnClickAudio != null) return;

			if (MainMenuController.Instance != null)
				btnClickAudio = MainMenuController.Instance.ButtonClickSfx;
			else if (LevelUIController.Instance != null)
				btnClickAudio = LevelUIController.Instance.ButtonClickSfx;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			originScale = transform.localScale;
			if (isNotScale)
				return;

			transform.localScale *= 0.88f;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (isNotScale)
				return;
			transform.localScale = originScale;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (btnClickAudio != null && !_isMute)
				AudioController.Instance.PlaySfx(btnClickAudio);
		}

		public void OnClick()
		{
		}
	}
}