using Boosters.InGame;
using Controllers;
using DG.Tweening;
using UnityEngine;

namespace Boosters
{
	public class InGameBoosterTutorial : MonoBehaviour
	{
		[SerializeField] private InGameBoosterButton _booster;
		[SerializeField] private UnlockBoosterPanel _unlockBoosterPanel;
		[SerializeField] private GameObject _useBoosterTutorial;

		private void Awake()
		{
			_unlockBoosterPanel.Data = _booster.Data;
		}

		private void Start()
		{
			// if (PlayerPrefs.GetInt(gameObject.name, 0) != 1)
			// {
			// 	_unlockBoosterPanel.OnClickClaim += OnClickClaimBooster;
			// 	_unlockBoosterPanel.Show();
			// 	// _booster.gameObject.SetActive(false);
			// 	LevelController.Instance.CanDrag = false;
			// PointToBoosterButton();
			// }
			// else
			// {
			// 	Destroy(gameObject);
			// }
		}

		private void PointToBoosterButton()
		{
			Vector2 buttonPos = _booster.GetComponent<RectTransform>().position;
			var tutTransform = _useBoosterTutorial.transform;
			SetAnchorPos(tutTransform.Find("Target"), buttonPos);
			SetAnchorPos(tutTransform.Find("Arrow"), buttonPos);
			SetAnchorPos(tutTransform.Find("Continue"), buttonPos);
		}

		private void SetAnchorPos(Transform trans, Vector2 pos)
		{
			trans.GetComponent<RectTransform>().anchoredPosition = pos;
		}

		private void OnClickClaimBooster()
		{
			DOVirtual.DelayedCall(0.5f, () =>
			{
				_useBoosterTutorial.SetActive(true);
				// _booster.gameObject.SetActive(true);
			});
		}

		public void OnClickBooster()
		{
			LevelController.Instance.CanDrag = true;
			_booster.Active();
			Destroy(gameObject);
			PlayerPrefs.SetInt(gameObject.name, 1);
		}
	}
}