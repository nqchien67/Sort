using System.Collections;
using System.Collections.Generic;
using MainMenu.BottomBar;
using MainMenu.Shop;
using MainMenu.TopBar;
using UI;
using UI.MainMenu.BottomBar;
using UnityEngine;

namespace MainMenu
{
	public class MainMenuUIController : SingletonCore<MainMenuUIController>
	{
		[Header("Top bar")] public Coin Coin;
		public TopBar.Avatar Avatar;
		[SerializeField] private Transform _topBar;
		public BottomBarController BottomBar;
		[SerializeField] private GameObject _blockInteract;
		public GameObject NotEnoughCoin;

		[HideInInspector] public int InitialTopBarSiblingIndex;

		private Stack<Popup> _popups = new Stack<Popup>();

		private void Start()
		{
			InitialTopBarSiblingIndex = _topBar.GetSiblingIndex();
		}

		public void OpenShop()
		{
			StartCoroutine(CloseAllPopupThenOpenShop());
		}

		private IEnumerator CloseAllPopupThenOpenShop()
		{
			var wait = new WaitForSeconds(0.2f);

			_blockInteract.SetActive(true);
			while (_popups.Count > 0)
			{
				CloseFrontPopup();
				yield return wait;
			}

			BottomBar.OpenShop();
			yield return new WaitForSeconds(0.2f);
			IAPShop.Instance.ScrollToBottom();

			_blockInteract.SetActive(false);
		}

		public void PushToStack(Popup popup)
		{
			_popups.Push(popup);
		}

		private void CloseFrontPopup()
		{
			if (_popups.Count <= 0)
				return;
			Popup popup = _popups.Peek();
			popup.Close();
		}

		public void PopOutOfStack()
		{
			if (_popups.Count > 0)
				_popups.Pop();
		}

		public void SetTopBarSiblingIndex(int index)
		{
			_topBar.SetSiblingIndex(index);
		}

		public void ResetTopBarSiblingIndex()
		{
			_topBar.SetSiblingIndex(InitialTopBarSiblingIndex);
		}

		public void ShowNotEnoughCoin()
		{
			NotEnoughCoin.SetActive(true);
			NotEnoughCoin.transform.SetAsLastSibling();
		}
	}
}