using MainMenu.BottomBar;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.MainMenu.BottomBar
{
	public class BottomBarButton : MonoBehaviour, IPointerUpHandler
	{
		private Button _button;
		private Animator _animator;
		private BottomBarController _bottomBar;
		private static readonly int Normal = Animator.StringToHash("Normal");
		private static readonly int Selected = Animator.StringToHash("Selected");

		[SerializeField] private UnityEvent _onSelect;
		[SerializeField] private UnityEvent _onDeSelect;
		private static readonly int Pressed = Animator.StringToHash("Pressed");

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

			_onSelect.Invoke();
			_bottomBar.SelectingButton = this;
			_animator.SetTrigger(Selected);
		}

		private void DeSelect()
		{
			_animator.ResetTrigger(Selected);
			_animator.SetTrigger(Normal);

			_onDeSelect.Invoke();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			// _animator.ResetTrigger(Pressed);
			_animator.SetTrigger(Selected);
		}
	}
}