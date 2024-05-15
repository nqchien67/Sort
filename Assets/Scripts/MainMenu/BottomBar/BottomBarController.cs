using UnityEngine;

namespace MainMenu.BottomBar
{
	public class BottomBarController : MonoBehaviour
	{
		[SerializeField] private BottomBarButton[] _buttons;
		[SerializeField] private int _initialSelectedButton = 2;

		public BottomBarButton SelectingButton;

		private void Start()
		{
			_buttons[_initialSelectedButton].OnClick();
		}
	}
}