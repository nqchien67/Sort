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
		}

		public virtual void Close()
		{
			_animator.Play("Disappear");
		}
		
		public virtual void EndCloseAnimationTrigger()
		{
		}
	}
}