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

		public void Show()
		{
			gameObject.SetActive(true);
			_animator.Play("Appear");
		}
	}
}