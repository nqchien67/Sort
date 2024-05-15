using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainMenu.ShopSkin
{
	public class TabButton : MonoBehaviour
	{
		private Button _button;
		private Image _image;

		[SerializeField] private Sprite _selectedSprite;
		[SerializeField] private Sprite _unSelectedSprite;

		public UnityAction OnPressTab;
		
		private bool _isSelected;

		private void Awake()
		{
			_image = GetComponent<Image>();
			_button = GetComponent<Button>();
		}

		private void Start()
		{
			_button.onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			if (_isSelected)
				return;
			
			OnPressTab.Invoke();
		}

		public void Select()
		{
			_image.sprite = _selectedSprite;
			_image.SetNativeSize();
		}

		public void DeSelect()
		{
			_image.sprite = _unSelectedSprite;
			_image.SetNativeSize();
		}
	}
}