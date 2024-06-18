using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
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

		private float packTimeStamp, timeStamp;

		private Animator _animator;

		public RecyclableScrollRect ContentSpawn;
		private int count, countCheck;

		private List<Transform> children = new List<Transform>();
		private float posTempContent;

		private Vector2 _initialContentSize;
		private float _playerCardHeight;
		private bool _userCardInView;

		private MainMenuUIController MainMenuUIController => MainMenuUIController.Instance;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		private void Start()
		{
			// if (PlayerPrefs.GetInt("HasOpenSuperChefFirst", 0) == 0)
			// {
			// 	PlayerPrefs.SetInt("HasOpenSuperChefFirst", 1);
			// 	infoLayer2.gameObject.SetActive(true);
			// 	infoLayer2.GetComponent<Animator>().Play("Appear");
			// 	mainUI.gameObject.SetActive(false);
			// }

			// if (PlayerPrefs.GetFloat(SuperChefController.SPC_HASH, 0) <= DataController.ConvertToUnixTime(DateTime.Now))
			// {
			// 	PlayerPrefs.SetFloat(SuperChefController.SPC_HASH,
			// 		PlayerPrefs.GetFloat(SuperChefController.SPC_HASH, 0) + SuperChefController.TimeResetRank);
			// }

			if (ContentSpawn != null)
				_playerCardHeight = ContentSpawn.PrototypeCell.sizeDelta.y;

			userCardColliderCatcher.OnTriggerEnter += () => _userCardInView = true;
			userCardColliderCatcher.OnTriggerExit += () => _userCardInView = false;

			for (int i = 0; i < _topPlayers.Length; i++)
				_topPlayers[i].Init(TopChartsPlayerDataManager.Instance.PlayersData[i]);
		}

		private void Update()
		{
			// Debug.Log(ContentSpawn.verticalNormalizedPosition);
			// if ((float)(packTimeStamp - DataController.ConvertToUnixTime(System.DateTime.Now)) <= 0)
			// {
			// 	StartCoroutine(DelayClosePanel());
			// 	return;
			// }

			if (Time.time - timeStamp > 1)
			{
				timeStamp = Time.time;
				float deltaTime = (float)(packTimeStamp - DataController.ConvertToUnixTime(DateTime.Now));
				if (deltaTime > 0)
				{
					int hours = DataController.SecondToHours(deltaTime);
					int days = DataController.SecondsToDays(deltaTime);
					if (days >= 1)
						_countDownText.text =
							$" {(int)deltaTime / (3600 * 24):D2}d{(int)(deltaTime / 3600) % 24:D2}h{(int)(deltaTime / 60) % 60:D2}m";
					else if (hours >= 1)
						_countDownText.text =
							$" {(int)deltaTime / 3600:D2}h{(int)(deltaTime / 60) % 60:D2}m{(int)deltaTime % 60:D2}s";
					else
						_countDownText.text = $" {(int)(deltaTime / 60) % 60:D2}m{(int)deltaTime % 60:D2}s";
				}
			}

			// if (!TopChartsPlayerDataManager.Instance.isUpdateRank)
			// {
			// 	if (-TopChartsPlayerDataManager.Instance.PlayerCardPosCollection
			// 		    .superChefNameData3[PlayerPrefs.GetInt("RankUser")].pos.y + 0 > -posTempContent &&
			// 	    -posTempContent >
			// 	    -TopChartsPlayerDataManager.Instance.PlayerCardPosCollection
			// 		    .superChefNameData3[PlayerPrefs.GetInt("RankUser")].pos.y - 750)

			// Debug.Log(CalculateCardPos(PlayerPrefs.GetInt("RankUser")));
			// if (posTempContent > CalculateCardPos(PlayerPrefs.GetInt("RankUser"))
			//     && posTempContent < CalculateCardPos(PlayerPrefs.GetInt("RankUser")) + 1690)

			if (_userCardInView)
				_userCard.gameObject.SetActive(false);
			else
				_userCard.gameObject.SetActive(true);
		}

		public void Show()
		{
			gameObject.SetActive(true);

			if (_animator != null)
				_animator.Play("Appear");

			if (MainMenuUIController != null)
				MainMenuUIController.SetTopBarSiblingIndex(transform.GetSiblingIndex());

			packTimeStamp = PlayerPrefs.GetFloat(TopChartsController.TC_HASH, 0);

			int currentRank = PlayerPrefs.GetInt("RankUser");
			if (currentRank < 3)
			{
				_userCard.gameObject.SetActive(false);
				return;
			}

			if (TopChartsPlayerDataManager.Instance.isUpdateRank)
				_userCard.InitItem("You", PlayerPrefs.GetInt("RankUser_Cache"),
					DataController.Instance.Profile.AvatarName, PlayerPrefs.GetInt("PrevPointUser"));
			else
				_userCard.InitItem("You", currentRank,
					DataController.Instance.Profile.AvatarName, PlayerPrefs.GetInt("PointUser"));

			countCheck = PlayerPrefs.GetInt("RankUser_Cache") - currentRank;

			if (TopChartsPlayerDataManager.Instance.isUpdateRank)
			{
				TopChartsPlayerDataManager.Instance.isUpdateRank = false;
				UpdateUserCard();
			}
		}

		public void Move()
		{
			ContentSpawn.content.anchoredPosition = new Vector3(0, 0, 0);

			if (TopChartsPlayerDataManager.Instance.isUpdateRank)
			{
				TopChartsPlayerDataManager.Instance.isUpdateRank = false;
				UpdateUserCard();

				if (PlayerPrefs.GetInt("RankUser") > 3)
					StartCoroutine(ScrollToUser());
			}
		}

		private IEnumerator ScrollToUser()
		{
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
					PlayerPrefs.GetInt("PointUser", 0), 2,
					false))
				.Append(_userCardRank.DOCounter(prevRank, currentRank, 2,
					false))
				.Append(_userCard.transform.DOScaleX(1f, 0.1f).SetEase(Ease.Linear))
				.OnComplete(() => { _userCard.rankTemp = currentRank; });
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

		private float CalculateCardPos(int rank)
		{
			return -rank * _playerCardHeight;
		}
	}
}