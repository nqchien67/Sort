using MainMenu;
using UI.MainMenu;
using UnityEngine;

namespace UI
{
	public class Popup : MonoBehaviour
	{
		protected Animator _animator;

		protected virtual void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		public virtual void Show()
		{
			gameObject.SetActive(true);
			_animator.Play("Appear");

			if (MainMenuUIController.Instance != null)
				MainMenuUIController.Instance.PushToStack(this);
		}

		public virtual void Close()
		{
			if (MainMenuUIController.Instance != null)
				MainMenuUIController.Instance.PopOutOfStack();
			_animator.Play("Disappear");
		}

		public virtual void EndCloseAnimationTrigger()
		{
		}
	}
}