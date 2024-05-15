using System;
using System.Collections;
using Data;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace UI
{
	public class WinPanel : MonoBehaviour
	{
		[SerializeField] private SkeletonGraphic _piggyBankAnim;
		[SerializeField] private GameObject _piggyBank;
		[SerializeField] private TextMeshProUGUI _piggyGoldBonusTxt;

		private Animator _animator;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		public void Show()
		{
			gameObject.SetActive(true);
			StartCoroutine(ShowCoroutine());
		}

		private IEnumerator ShowCoroutine()
		{
			_animator.Play("Appear");

			yield return new WaitForSeconds(0.2f);
			if (PlayerPrefs.GetInt("level", 0) >= 6 && !DataController.Instance.IsPiggyBankFull())
			{
				_piggyBank.SetActive(true);
				_piggyBankAnim.AnimationState.SetAnimation(1, "jumpin_x", false);

				int bonusGoldPiggy =
					DataController.Instance.CurrentPbStorage / (DataController.Instance.PiggyBankLevel * 2 + 3);
				bonusGoldPiggy += Random.Range(-1, bonusGoldPiggy / 10 + 1);
				int averageGold = DataController.Instance.CurrentPbStorage /
				                  ((DataController.Instance.PiggyBankLevel * 2 + 3) * 12);
				int tmp = 0;
				yield return new WaitForSeconds(1.3f);
				_piggyBankAnim.AnimationState.SetAnimation(1, "suckindiamond_x", false);
				yield return new WaitForSeconds(0.4f);
				var delay = new WaitForSeconds(0.03f);
				while (tmp < bonusGoldPiggy)
				{
					tmp += averageGold;
					_piggyGoldBonusTxt.text = "+" + tmp;
					// AudioController.Instance.PlaySfx(IncreaseGoldAudio);
					yield return delay;
				}

				_piggyGoldBonusTxt.text = "+" + bonusGoldPiggy;
				yield return new WaitForSeconds(0.8f);
				DataController.Instance.PiggyBankCoin += bonusGoldPiggy;
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
			StartCoroutine(GameManager.WaiForSeconds(0.5f, () => SceneManager.LoadScene("MainScene")));
		}

		public void OnClickClaim()
		{
			//TODO: Nhét piggy bank hiện khi bấm nút này
			OnClickCLose();
		}
		
		public void NextLevel()
		{
			int level = PlayerPrefs.GetInt("level", 0);
			level++;
			SceneManager.LoadScene("Level" + level);
		}

		private void EndCloseAnimationTrigger()
		{
			
		}
	}
}