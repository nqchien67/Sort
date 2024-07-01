using System.Collections;
using System.Net.NetworkInformation;
using Boosters.InGame;
using Controllers;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities;

namespace Boosters
{
	public class InGameBoosterTutorial : MonoBehaviour
	{
		[SerializeField] private InGameBoosterButton _booster;
		[SerializeField] private UnlockBoosterPanel _unlockBoosterPanel;
		[SerializeField] private GameObject _useBoosterTutorial;
		[SerializeField] private GameObject _boosterClaimEffectPrefab;
		[SerializeField] private TextMeshProUGUI _boosterDescription;

		private void Awake()
		{
			_unlockBoosterPanel.Data = _booster.Data;
		}

		private IEnumerator Start()
		{
			if (PlayerPrefs.GetInt(gameObject.name, 0) != 1)
			{
				yield return new WaitUntil(() => LevelController.Instance.CanDrag);

				_unlockBoosterPanel.OnClickClaim += OnClickClaimBooster;
				_unlockBoosterPanel.Show();

				LevelController.Instance.CanDrag = false;
				LevelController.Instance.PausedTime = true;
				_boosterDescription.text = _booster.Data.Description;
			}
			else
			{
				yield return null;
				_booster.Unlock();
				Destroy(gameObject);
			}
		}

		private void SetAnchorPos(Transform trans, Vector2 pos)
		{
			trans.GetComponent<RectTransform>().position = pos;
		}

		private void OnClickClaimBooster()
		{
			const int boosterQuantity = 3;
			DataController.Instance.AddBooster(_unlockBoosterPanel.Data.Type, boosterQuantity);
			DataController.Instance.SaveData();

			_booster.Unlock();

			StartCoroutine(ClaimBoosterRoutine(boosterQuantity));

			DOVirtual.DelayedCall(1f, DisplayUseBoosterTutorial);
		}

		private void DisplayUseBoosterTutorial()
		{
			_useBoosterTutorial.SetActive(true);
			InGameBoosterButton tutBoosterButton = Instantiate(_booster, _useBoosterTutorial.transform);
			Transform buttonTf = tutBoosterButton.transform;

			buttonTf.position = _booster.transform.position;
			tutBoosterButton.enabled = false;
			buttonTf.SetSiblingIndex(1);

			buttonTf.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
			PointToBoosterButton(buttonTf.position);
		}


		public void OnClickBooster()
		{
			LevelController.Instance.CanDrag = true;
			LevelController.Instance.PausedTime = false;

			_booster.Use();
			Destroy(gameObject);
			PlayerPrefs.SetInt(gameObject.name, 1);

			DataController.Instance.AddBooster(_unlockBoosterPanel.Data.Type, 1);
			_booster.RefreshQuantity();
			DataController.Instance.SaveData();
		}

		private void PointToBoosterButton(Vector2 position)
		{
			var tutTransform = _useBoosterTutorial.transform;
			SetAnchorPos(tutTransform.Find("Arrow"), position);
			SetAnchorPos(tutTransform.Find("Continue"), position);
		}

		private IEnumerator ClaimBoosterRoutine(int boosterQuantity)
		{
			for (int i = 0; i < boosterQuantity; i++)
			{
				GameObject itemProp = Instantiate(_boosterClaimEffectPrefab,
					transform.position + new Vector3(0, 0, -0.1f),
					Quaternion.identity, transform.parent);
				itemProp.GetComponent<SpriteRenderer>().sprite = _booster.Data.Sprite;

				StartCoroutine(CommonIEnumerator.IMove(itemProp, _booster.transform.position, 1,
					() => _booster.transform.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo)));

				yield return new WaitForSeconds(0.2f);
			}
		}
	}
}