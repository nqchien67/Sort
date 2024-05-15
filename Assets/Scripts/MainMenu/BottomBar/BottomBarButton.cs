using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainMenu.BottomBar
{
	public class BottomBarButton : MonoBehaviour
	{
		private Button _button;
		private Animator _animator;
		private BottomBarController _bottomBar;
		private static readonly int Normal = Animator.StringToHash("Normal");
		private static readonly int Selected = Animator.StringToHash("Selected");

		[SerializeField] private UnityEvent _onDeSelect;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_button = GetComponent<Button>();
			_bottomBar = GetComponentInParent<BottomBarController>();
		}

		private void Start()
		{
			_button.onClick.AddListener(OnClick);
		}

		public void OnClick()
		{
			if (_bottomBar.SelectingButton == this)
				return;

			if (_bottomBar.SelectingButton != null)
				_bottomBar.SelectingButton.DeSelect();

			_bottomBar.SelectingButton = this;
			_animator.SetTrigger(Selected);
		}

		private void DeSelect()
		{
			_animator.ResetTrigger(Selected);
			_animator.SetTrigger(Normal);

			_onDeSelect.Invoke();
		}
	}
}