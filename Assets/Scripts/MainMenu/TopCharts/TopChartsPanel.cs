using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using Newtonsoft.Json;
using PolyAndCode.UI;
using TMPro;
using UnityEngine;

namespace MainMenu.TopCharts
{
	public class TopChartsPanel : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _countDownText;
		[SerializeField] private PlayerCard _userCard;
		[SerializeField] private TextMeshProUGUI _userCardRank, _userCardStar;
		[SerializeField] private UserCardColliderCatcher userCardColliderCatcher;
		[SerializeField] private TopPlayer[] _topPlayers;

		private float timeStamp;
		private float TimeEndSeason => TopChartsDataManager.TimeEndSeason;

		private Animator _animator;

		public RecyclableScrollRect ContentSpawn;
		private int count, countCheck;

		private float posTempContent;

		private Vector2 _initialContentSize;
		private float _playerCardHeight;
		private bool _userCardInView;

		[SerializeField] private bool _isInWinPanel;
		private TopChartsDataManager TopChartsDataManager => TopChartsDataManager.Instance;

		private MainMenuUIController MainMenuUIController => MainMenuUIController.Instance;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		private void Start()
		{
			if (ContentSpawn != null)
				_playerCardHeight = ContentSpawn.PrototypeCell.sizeDelta.y;

			userCardColliderCatcher.OnTriggerEnter += () => _userCardInView = true;
			userCardColliderCatcher.OnTriggerExit += () => _userCardInView = false;

			InitTopPlayerCards();
		}

		public void InitTopPlayerCards()
		{
			if (_isInWinPanel)
			{
				for (int i = 0; i < _topPlayers.Length; i++)
					_topPlayers[i].Init(TopChartsDataManager.PlayersData[i]);
			}
			else
			{
				for (int i = 0; i < _topPlayers.Length; i++)
					_topPlayers[i].Init(TopChartsDataManager.PlayersData[i],
						TopChartsController.Instance.Rewards[i]);
			}
		}

		private void Update()
		{
			if (Time.time - timeStamp > 1)
			{
				timeStamp = Time.time;
				float timeRemain = (float)(TimeEndSeason - DataController.ConvertToUnixTime(DateTime.Now));
				if (timeRemain >= 0)
				{
					int hours = DataController.SecondToHours(timeRemain);
					int days = DataController.SecondsToDays(timeRemain);
					if (days >= 1)
						_countDownText.text =
							$" {(int)timeRemain / (3600 * 24):D2}d{(int)(timeRemain / 3600) % 24:D2}h{(int)(timeRemain / 60) % 60:D2}m";
					else if (hours >= 1)
						_countDownText.text =
							$" {(int)timeRemain / 3600:D2}h{(int)(timeRemain / 60) % 60:D2}m{(int)timeRemain % 60:D2}s";
					else
						_countDownText.text = $" {(int)(timeRemain / 60) % 60:D2}m{(int)timeRemain % 60:D2}s";
				}
			}

			if (_userCardInView)
				_userCard.gameObject.SetActive(false);
			else if(PlayerPrefs.GetInt("RankUser") >= 3)
				_userCard.gameObject.SetActive(true);
		}

		public void Show()
		{
			gameObject.SetActive(true);

			if (_animator != null)
				_animator.Play("Appear");

			if (MainMenuUIController != null)
				MainMenuUIController.SetTopBarSiblingIndex(transform.GetSiblingIndex());

			int currentRank = PlayerPrefs.GetInt("RankUser");
			if (currentRank < 3)
			{
				_userCard.gameObject.SetActive(false);
				TopChartsDataManager.isUpdateRank = false;
				return;
			}

			if (TopChartsDataManager.isUpdateRank)
				_userCard.InitItem("You", PlayerPrefs.GetInt("RankUser_Cache"),
					DataController.Instance.Profile.AvatarName, PlayerPrefs.GetInt("PrevPointUser"));
			else
				_userCard.InitItem("You", currentRank,
					DataController.Instance.Profile.AvatarName, PlayerPrefs.GetInt("PointUser"));

			countCheck = PlayerPrefs.GetInt("RankUser_Cache") - currentRank;

			if (TopChartsDataManager.isUpdateRank)
			{
				TopChartsDataManager.isUpdateRank = false;
				UpdateUserCard();
			}
		}

		private IEnumerator ScrollToUser()
		{
			ContentSpawn.content.anchoredPosition = new Vector3(0, 0, 0);
			while (_userCard.gameObject.activeSelf && enabled)
			{
				float initialPos = ContentSpawn.verticalNormalizedPosition;

				float elapsedTime = 0;
				const float duration = 0.02f;
				while (elapsedTime < duration)
				{
					if (_userCardInView)
						yield break;

					ContentSpawn.verticalNormalizedPosition
						= Mathf.Lerp(initialPos, 0, elapsedTime / duration);

					elapsedTime += Time.deltaTime;
					yield return null;
				}

				if (ContentSpawn.verticalNormalizedPosition <= 0.01f)
					break;
			}
		}

		private void UpdateUserCard()
		{
			int currentRank = PlayerPrefs.GetInt("RankUser") + 1;
			int prevRank = PlayerPrefs.GetInt("RankUser_Cache") + 1;

			DOTween.Sequence()
				.Append(_userCard.transform.DOScaleX(1.1f, 0.1f).SetEase(Ease.Linear))
				.Append(_userCardStar.DOCounter(PlayerPrefs.GetInt("PrevPointUser", 0),
					PlayerPrefs.GetInt("PointUser", 0), 2, false))
				.Append(_userCardRank.DOCounter(prevRank, currentRank, 2, false))
				.Append(_userCard.transform.DOScaleX(1f, 0.1f).SetEase(Ease.Linear))
				.OnComplete(() => _userCard._rank = currentRank);
		}

		public void UpdateSttRank()
		{
			if (countCheck == 0)
			{
				CancelInvoke(nameof(UpdateSttRank));
				return;
			}

			count++;
			_userCardRank.text = (PlayerPrefs.GetInt("RankUser_Cache") - count + 1).ToString();
			if (count == countCheck)
			{
				CancelInvoke(nameof(UpdateSttRank));
				count = 0;
			}
		}

		public void Close()
		{
			_animator.Play("Disappear");
		}

		private void EndCloseAnimationTrigger()
		{
			gameObject.SetActive(false);

			if (MainMenuUIController != null)
				MainMenuUIController.ResetTopBarSiblingIndex();
		}

		public void UpdatePosContent(float a)
		{
			posTempContent += a;
		}

		public void ClearPlayerCards()
		{
		}
	}
}