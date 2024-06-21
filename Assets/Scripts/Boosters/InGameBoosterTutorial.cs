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
				Destroy(gameObject);
			}
		}

		private void PointToBoosterButton()
		{
			Vector2 buttonPos = _booster.GetComponent<RectTransform>().position;
			var tutTransform = _useBoosterTutorial.transform;
			// SetAnchorPos(tutTransform.Find("Target"), buttonPos);
			SetAnchorPos(tutTransform.Find("Arrow"), buttonPos);
			SetAnchorPos(tutTransform.Find("Continue"), buttonPos);
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

			_booster.RefreshQuantity();

			StartCoroutine(ClaimBoosterRoutine(boosterQuantity));

			DOVirtual.DelayedCall(1f, DisplayUseBoosterTutorial);
		}

		private void DisplayUseBoosterTutorial()
		{
			_useBoosterTutorial.SetActive(true);
			var tutBoosterButton = Instantiate(_booster, _useBoosterTutorial.transform);
			tutBoosterButton.transform.position = _booster.transform.position;
			tutBoosterButton.enabled = false;
			tutBoosterButton.transform.SetSiblingIndex(1);

			tutBoosterButton.transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
			PointToBoosterButton();
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

		private IEnumerator ClaimBoosterRoutine(int boosterQuantity)
		{
			for (int i = 0; i < boosterQuantity; i++)
			{
				GameObject itemProp = Instantiate(_boosterClaimEffectPrefab,
					transform.position + new Vector3(0, 0, -0.1f),
					Quaternion.identity, transform.parent);
				itemProp.GetComponent<SpriteRenderer>().sprite = _booster.Data.Sprite;

				StartCoroutine(CommonIEnumerator.IMove(itemProp, _booster.transform.position, 1));
				//TODO: Them anim scale cho nut booster
				yield return new WaitForSeconds(0.2f);
			}
		}
	}
}