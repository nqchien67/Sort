using System;
using System.Collections;
using System.Globalization;
using Audio;
using Controllers;
using Data;
using DG.Tweening;
using MainMenu;
using MainMenu.TopCharts;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;
using Random = UnityEngine.Random;

namespace InGame.UI
{
	public class WinPanel : MonoBehaviour
	{
		[SerializeField] private SkeletonGraphic _piggyBankAnim;
		[SerializeField] private GameObject _piggyBank;
		[SerializeField] private TextMeshProUGUI _piggyGoldBonusTxt;

		[SerializeField] private Image _progressFill;
		[SerializeField] private TextMeshProUGUI _progressCount;
		[SerializeField] private Button _claimItemButton;
		[SerializeField] private UnlockNewItemPanel _unlockNewItemPanelPrefab;
		[SerializeField] private GameObject _highlight;
		[SerializeField] private TextMeshProUGUI _multiStarAdsText;
		[SerializeField] private TextMeshProUGUI _totalStarAdsText;

		[SerializeField] private AdsMultiplierCatcher _adsMultiplierCatcher;
		[SerializeField] private DOTweenAnimation _pointerTween;

		private Animator _animator;
		private float _maxFillBarLength;
		private int _levelGainedStar;

		[Header("Audio")] [SerializeField] private AudioClip _openSfx;
		[SerializeField] private AudioClip _piggySuckCoinSfx;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_maxFillBarLength = _progressFill.rectTransform.sizeDelta.x;
		}

		public void Show()
		{
			gameObject.SetActive(true);
			StartCoroutine(ShowCoroutine());

			int freeItemProgress = PlayerPrefs.GetInt("FreeItemProgress", 0);
			UpdateUnlockItemProgress(freeItemProgress, 5);

			bool canClaimFreeItem = freeItemProgress >= 5 && SpritesCollection.Instance.GetNotUnlockedItems().Count > 0;
			if (canClaimFreeItem)
			{
				_claimItemButton.enabled = true;
				_claimItemButton.onClick.AddListener(OnClickClaimFreeItem);
				_highlight.SetActive(true);
			}
			else
			{
				_claimItemButton.enabled = false;
			}

			_levelGainedStar = LevelController.Instance.Star;
			AudioController.Instance.StopMusic();
			AudioController.Instance.PlaySfx(_openSfx);
		}

		private IEnumerator ShowCoroutine()
		{
			_animator.Play("Appear");

			yield return new WaitForSeconds(0.2f);
			GetComponentInChildren<TopChartsPanel>(true).Show();

			if (PlayerPrefs.GetInt("level", 0) >= 6 && !DataController.Instance.IsPiggyBankFull())
			{
				_piggyBank.SetActive(true);
				_piggyBankAnim.AnimationState.SetAnimation(1, "jumpin_x", false);

				// int bonusCoinPiggy =
				// 	DataController.Instance.CurrentPbStorage / (DataController.Instance.PiggyBankLevel * 2 + 3);
				int bonusCoinPiggy = LevelController.Instance.Coin;

				bonusCoinPiggy += Random.Range(-1, bonusCoinPiggy / 10 + 1);
				bonusCoinPiggy = Mathf.Max(bonusCoinPiggy, 0);
				int averageGold = bonusCoinPiggy / 12;

				int tmp = 0;
				yield return new WaitForSeconds(1.3f);
				_piggyBankAnim.AnimationState.SetAnimation(1, "suckindiamond_x", false);
				yield return new WaitForSeconds(0.4f);
				var delay = new WaitForSeconds(0.03f);
				while (tmp < bonusCoinPiggy)
				{
					tmp += averageGold;
					_piggyGoldBonusTxt.text = "+" + tmp;
					AudioController.Instance.PlaySfx(_piggySuckCoinSfx);
					yield return delay;
				}

				_piggyGoldBonusTxt.text = "+" + bonusCoinPiggy;
				yield return new WaitForSeconds(0.8f);
				DataController.Instance.PiggyBankCoin += bonusCoinPiggy;
				if (DataController.Instance.IsPiggyBankFull() && DataController.Instance.PbTimeDuration <= 0)
				{
					DataController.Instance.PbTimeDuration = 7200;
					DataController.Instance.PiggyBankTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
					PlayerPrefs.SetInt("open_full_piggy", 0);
				}

				yield return new WaitForSeconds(0.7f);
				_piggyBank.SetActive(false);
			}
		}

		public void OnClickCLose()
		{
			_animator.Play("Disappear");
			StartCoroutine(CommonIEnumerator.WaiForSeconds(0.3f, () => LevelController.Instance.GoHome()));
		}

		public void OnClickClaim()
		{
			if (_claimItemButton.enabled)
				ClaimFreeItemAndClose();
			else
				OnClickCLose();
		}

		public void OnLickClaimAds()
		{
			Debug.Log("Show video ads reward");

			_pointerTween.DOPause();
			int extraStar = _levelGainedStar * _adsMultiplierCatcher.MultiTime - _levelGainedStar;

			LevelController.Instance.UpdateTopCharts(extraStar);

			DataController.Instance.Star += extraStar;
			DataController.Instance.SaveData();

			if (_claimItemButton.enabled)
				ClaimFreeItemAndClose();
			else
				OnClickCLose();
		}

		public void NextLevel()
		{
			int level = PlayerPrefs.GetInt("level", 0);
			level++;
			SceneManager.LoadScene("Level" + level);
		}

		private void UpdateUnlockItemProgress(int fillAmount, int total)
		{
			float newFillPercent = (float)fillAmount / total;

			Vector2 sizeDelta = _progressFill.rectTransform.sizeDelta;
			Vector2 newSizeDelta = sizeDelta;

			sizeDelta.x = 0;
			_progressFill.rectTransform.sizeDelta = sizeDelta;

			newSizeDelta.x = _maxFillBarLength * newFillPercent;
			_progressFill.rectTransform.DOSizeDelta(newSizeDelta, 0.3f);

			DOVirtual.Float(0, fillAmount, 0.3f,
				value => _progressCount.text = Mathf.RoundToInt(value) + "/" + total);
		}

		private UnlockNewItemPanel _unlockNewItemPanel;

		private void ClaimFreeItemAndClose()
		{
			OnClickClaimFreeItem();
			StartCoroutine(CommonIEnumerator.WaitUntil(
				() => _unlockNewItemPanel == null,
				OnClickCLose));
		}

		private void OnClickClaimFreeItem()
		{
			_unlockNewItemPanel = Instantiate(_unlockNewItemPanelPrefab, LevelUIController.Instance.PopupCanvas);
			_unlockNewItemPanel.Show();
			PlayerPrefs.SetInt("FreeItemProgress", 0);
			_claimItemButton.enabled = false;
			_highlight.SetActive(false);
			UpdateUnlockItemProgress(0, 5);
		}

		private void FixedUpdate()
		{
			_multiStarAdsText.text = "Claim x" + _adsMultiplierCatcher.MultiTime;
			_totalStarAdsText.text = (_levelGainedStar * _adsMultiplierCatcher.MultiTime).ToString(CultureInfo
				.InvariantCulture);
		}

		private void EndCloseAnimationTrigger()
		{
		}
	}
}