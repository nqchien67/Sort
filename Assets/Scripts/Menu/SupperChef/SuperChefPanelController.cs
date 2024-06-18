using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Controllers;
using Data;
using UnityEngine.UI;
using DG.Tweening;
using MainMenu.TopCharts;
using Spine.Unity;
using UnityEngine.Events;
using Utilities;

public class SuperChefPanelController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI countDownText, countDownText2;
	private float packTimeStamp, timeStamp = 0;
	private DateTime timeStart;
	private DateTime timeEnd;

	[Header("Setup Item")] public GameObject pfSuperChefItem;

	// public Transform tfContentUp, tfContentDown;
	public List<PlayerCard> SuperChefItems = new List<PlayerCard>();
	[Header("Other")] public GameObject infoLayer, infoLayer2, mainUI;
	[SerializeField] private AudioClip popUpClip;
	[Header("Tutorials")] public GameObject firstplayTutPrefab0, firstplayTutPrefab1, imgReward, imgReward2;
	public PlayerCard itemNameUser;
	public TextMeshProUGUI rank;

	public ScrollRect contentSpawn;
	private int count, countCheck;
	private float posTempContent = 0;
	List<Transform> children = new List<Transform>();

	private void Start()
	{
		//Inititems();
		if (PlayerPrefs.GetInt("HasOpenSuperChefFirst", 0) == 0)
		{
			PlayerPrefs.SetInt("HasOpenSuperChefFirst", 1);
			infoLayer2.gameObject.SetActive(true);
			infoLayer2.GetComponent<Animator>().Play("Appear");
			mainUI.gameObject.SetActive(false);
		}

		timeStart = SuperChefDataController.Instance.GetDayStartEvent().ToDateTime('/');
		timeEnd = SuperChefDataController.Instance.GetDayEndEvent().ToDateTime('/');
		if (PlayerPrefs.GetFloat(TopChartsController.TC_HASH, 0) <= DataController.ConvertToUnixTime(DateTime.Now))
		{
			PlayerPrefs.SetFloat(TopChartsController.TC_HASH,
				PlayerPrefs.GetFloat(TopChartsController.TC_HASH, 0) + TopChartsController.TimeResetRank);
		}

		packTimeStamp = PlayerPrefs.GetFloat(TopChartsController.TC_HASH, 0);
		GetComponent<Animator>().Play("Appear");

		if (TopChartsPlayerDataManager.Instance.isUpdateRank)
		{
			itemNameUser.InitItem("You", PlayerPrefs.GetInt("RankUser_Cache"),
				DataController.Instance.Profile.AvatarName, PlayerPrefs.GetInt("PointUser"));
		}
		else
		{
			itemNameUser.InitItem("You", PlayerPrefs.GetInt("RankUser"),
				DataController.Instance.Profile.AvatarName, PlayerPrefs.GetInt("PointUser"));
		}

		countCheck = PlayerPrefs.GetInt("RankUser_Cache") - PlayerPrefs.GetInt("RankUser");
	}

	public void Move1()
	{
		contentSpawn.content.anchoredPosition = new Vector3(0, 0, 0);
		if (TopChartsPlayerDataManager.Instance.isUpdateRank)
		{
			Sequence seq = DOTween.Sequence();
			seq
				.Append(contentSpawn.content.transform.DOLocalMoveY(
					-TopChartsPlayerDataManager.Instance.PlayerCardPosCollection
						.superChefNameData3[PlayerPrefs.GetInt("RankUser_Cache")].pos.y - 588f, 2f))
				.OnComplete(ActionUpdateRank);
		}
		else
		{
			// Sequence seq = DOTween.Sequence();
			// seq
			// .Append(contentSpawn.content.transform.DOLocalMoveY(-DataSuperChefListName.Instance.dataSuperChefNameUserFake1Pos.superChefNameData3[PlayerPrefs.GetInt("RankUser")].pos.y - 588f, 2f))
			// .OnComplete(() =>
			// {
			//     ActionUpdateRank();
			// });
		}
	}

	public void UpdatePosContent(float a)
	{
		posTempContent += a;
	}

	public void ActionUpdateRank()
	{
		if (TopChartsPlayerDataManager.Instance.isUpdateRank)
		{
			InvokeRepeating("updateSttRank", 0.25f, 0.05f);
			Sequence seq = DOTween.Sequence();
			seq
				.Append(itemNameUser.transform.DOScaleX(1.1f, 0.25f).SetEase(Ease.Linear))
				.Append(contentSpawn.content.transform.DOLocalMoveY(
					-TopChartsPlayerDataManager.Instance.PlayerCardPosCollection
						.superChefNameData3[PlayerPrefs.GetInt("RankUser")].pos.y - 588f, 2f))
				.Append(itemNameUser.transform.DOScaleX(1f, 0.25f).SetEase(Ease.Linear))
				.OnComplete(() =>
				{
					contentSpawn.content.sizeDelta = new Vector2(1076.037f, 1079.521f);
					contentSpawn.content.anchoredPosition = new Vector3(0, 0, 0);
					foreach (Transform child in contentSpawn.content)
					{
						children.Add(child);
					}

					children.Sort((x, y) =>
						x.GetComponent<PlayerCard>().rankTemp
							.CompareTo(y.GetComponent<PlayerCard>().rankTemp));
					// Thiết lập lại thứ tự trong hierarchy
					for (int i = 0; i < children.Count; i++)
					{
						children[i].SetSiblingIndex(i);
						children[i].transform.localPosition = new Vector2(0,
							TopChartsPlayerDataManager.Instance.PlayerCardPosCollection.superChefNameData3[i].pos
								.y);
					}

					posTempContent -= (120 * PlayerPrefs.GetInt("RankUser") - 120 * 6);
					itemNameUser.gameObject.SetActive(true);
					contentSpawn.content.anchoredPosition += new Vector2(0, 120);
					TopChartsPlayerDataManager.Instance.isUpdateRank = false;
				});
		}
	}

	public void updateSttRank()
	{
		if (countCheck == 0)
		{
			CancelInvoke("updateSttRank");
			return;
		}

		count++;
		rank.text = (PlayerPrefs.GetInt("RankUser_Cache") - count + 1).ToString();
		if (count == countCheck)
		{
			CancelInvoke("updateSttRank");
			count = 0;
		}
	}

	private void Update()
	{
		if ((float)(packTimeStamp - DataController.ConvertToUnixTime(System.DateTime.Now)) <= 0)
		{
			StartCoroutine(DelayClosePanel());
			return;
		}

		if (Time.time - timeStamp > 1)
		{
			timeStamp = Time.time;
			float deltaTime = (float)(packTimeStamp - DataController.ConvertToUnixTime(System.DateTime.Now));
			if (deltaTime > 0)
			{
				int hours = DataController.SecondToHours(deltaTime);
				int days = DataController.SecondsToDays(deltaTime);
				if (days >= 1)
				{
					countDownText.text = System.String.Format(" {0:D2}d{1:D2}h{2:D2}m", ((int)deltaTime / (3600 * 24)),
						(int)(deltaTime / 3600) % 24, (int)(deltaTime / 60) % 60);
					countDownText2.text = System.String.Format(" {0:D2}d{1:D2}h{2:D2}m", ((int)deltaTime / (3600 * 24)),
						(int)(deltaTime / 3600) % 24, (int)(deltaTime / 60) % 60);
				}
				else if (days < 1 && hours >= 1)
				{
					countDownText.text = System.String.Format(" {0:D2}h{1:D2}m{2:D2}s", (int)deltaTime / 3600,
						(int)(deltaTime / 60) % 60, (int)deltaTime % 60);
					countDownText2.text = System.String.Format(" {0:D2}h{1:D2}m{2:D2}s", (int)deltaTime / 3600,
						(int)(deltaTime / 60) % 60, (int)deltaTime % 60);
				}
				else if (hours < 1 && days < 1)
				{
					countDownText.text = System.String.Format(" {0:D2}m{1:D2}s", (int)(deltaTime / 60) % 60,
						(int)deltaTime % 60);
					countDownText2.text = System.String.Format(" {0:D2}m{1:D2}s", (int)(deltaTime / 60) % 60,
						(int)deltaTime % 60);
				}
			}
		}

		if (!TopChartsPlayerDataManager.Instance.isUpdateRank)
		{
			if (-TopChartsPlayerDataManager.Instance.PlayerCardPosCollection
				    .superChefNameData3[PlayerPrefs.GetInt("RankUser")].pos.y + 0 > -posTempContent && -posTempContent >
			    -TopChartsPlayerDataManager.Instance.PlayerCardPosCollection
				    .superChefNameData3[PlayerPrefs.GetInt("RankUser")].pos.y - 750)
			{
				itemNameUser.gameObject.SetActive(false);
			}
			else
			{
				itemNameUser.gameObject.SetActive(true);
			}
		}
	}

	private IEnumerator DelayClosePanel()
	{
		GetComponent<Animator>().Play("Disappear");
		yield return new WaitForSeconds(.3f);
		Destroy(gameObject);
	}

	public void OnHide()
	{
		GetComponent<Animator>().Play("Disappear");
		Destroy(gameObject, 0.2f);
	}

	#region Button

	public void BTN_Click_OpenInfoLayer()
	{
		infoLayer.SetActive(true);
	}

	public void BTN_Click_CloseInfoLayer()
	{
		infoLayer.SetActive(false);
	}

	#endregion

	#region Tutorial

	public void BTN_Click_FirstTut0_DONE()
	{
		Destroy(firstplayTutPrefab0);
		firstplayTutPrefab1.SetActive(true);
	}

	public void BTN_Click_FirstTut1_DONE()
	{
		PlayerPrefs.SetInt(TopChartsController.SPC_FIRST_TUTORIAL, 2);
		Destroy(firstplayTutPrefab1);
	}

	#endregion

	public void BTN_Click_GO()
	{
		this.OnHide();
		// FindObjectOfType<MainMenuController>().OnOpenMaxResInZone();
	}
	// Cup drop
	// idle 1
	// idle 2
	// idle 3
	// lifted ball
}