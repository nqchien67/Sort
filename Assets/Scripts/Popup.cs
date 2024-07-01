using Audio;
using Controllers;
using MainMenu;
using UnityEngine;

public class Popup : MonoBehaviour
{
	protected Animator _animator;
	[SerializeField] protected AudioClip _showSfx;

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

		PlayShowSfx();
	}

	private void PlayShowSfx()
	{
		if (_showSfx == null)
			_showSfx = AudioController.Instance.PopupShowSfx;
		
		AudioController.Instance.PlaySfx(_showSfx);
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