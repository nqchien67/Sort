using System.Collections;
using System.Collections.Generic;
using Data;
using MainMenu.TopCharts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
	public Sprite[] avaList, listReward, listRank;
	public Image avaListImg;
	public Image[] hopQuaListImg, bgRankIn1, bgRankIn2, rankBGListImg;
	public TextMeshProUGUI txtRank, txtName, txtPoint;
	public GameObject toolTips;
	private readonly List<Transform> children = new List<Transform>();

	[SerializeField] private GameObject _userBarBg;

	public int rankTemp;

	public void InitItem(string name, int rank, string avatarName, int point)
	{
		rankTemp = rank;
		// if (!DataSuperChefListName.Instance.isUpdateRank)
		// {
		// bgRank[1].SetActive(name == "You");
		// hopQuaListImg[1].gameObject.SetActive(name == "You");
		// rankBGListImg[1].gameObject.SetActive(name == "You");
		// bgRank[0].SetActive(name != "You");
		// hopQuaListImg[0].gameObject.SetActive(name != "You");
		// rankBGListImg[0].gameObject.SetActive(name != "You");
		// }
		// else
		// {
		//     bgRank[0].SetActive(true);
		//     hopQuaListImg[0].gameObject.SetActive(true);
		//     rankBGListImg[0].gameObject.SetActive(true);
		// }

		if (name == "You")
		{
			_userBarBg.SetActive(true);
		}
		else
		{
			_userBarBg.SetActive(false);
		}

		// if (rank >= 3)
		// {
		// 	bgRankIn1[1].gameObject.SetActive(false);
		// 	bgRankIn2[1].gameObject.SetActive(false);
		// 	bgRankIn1[0].gameObject.SetActive(true);
		// 	bgRankIn2[0].gameObject.SetActive(true);
		// 	hopQuaListImg[0].gameObject.SetActive(true);
		// 	hopQuaListImg[1].gameObject.SetActive(true);
		// 	hopQuaListImg[0].sprite = listReward[3];
		// 	hopQuaListImg[1].sprite = listReward[3];
		// 	if (rank > 49)
		// 	{
		// 		hopQuaListImg[0].gameObject.SetActive(false);
		// 		hopQuaListImg[1].gameObject.SetActive(false);
		// 	}
		// }
		// else
		// {
		// 	bgRankIn1[1].gameObject.SetActive(true);
		// 	bgRankIn2[1].gameObject.SetActive(true);
		// 	bgRankIn1[0].gameObject.SetActive(false);
		// 	bgRankIn2[0].gameObject.SetActive(false);
		// 	switch (rank)
		// 	{
		// 		case 0:
		// 		{
		// 			hopQuaListImg[0].sprite = listReward[0];
		// 			hopQuaListImg[1].sprite = listReward[0];
		// 			bgRankIn1[1].sprite = listRank[0];
		// 			bgRankIn2[1].sprite = listRank[0];
		// 		}
		// 			break;
		// 		case 1:
		// 		{
		// 			hopQuaListImg[0].sprite = listReward[1];
		// 			hopQuaListImg[1].sprite = listReward[1];
		// 			bgRankIn1[1].sprite = listRank[1];
		// 			bgRankIn2[1].sprite = listRank[1];
		// 		}
		// 			break;
		// 		case 2:
		// 		{
		// 			hopQuaListImg[0].sprite = listReward[2];
		// 			hopQuaListImg[1].sprite = listReward[2];
		// 			bgRankIn1[1].sprite = listRank[2];
		// 			bgRankIn2[1].sprite = listRank[2];
		// 		}
		// 			break;
		// 	}
		// }


		if (name == "You")
			avaListImg.sprite = SpritesCollection.Instance.CurrentAvatarSprite;
		else
			avaListImg.sprite =  DataController.GetAvatarSprite(avatarName);

		txtPoint.text = point.ToString();
		txtName.text = name;

		if (name == "You" && rank >= TopChartsPlayerDataManager.PlayerCount)
			txtRank.text = TopChartsPlayerDataManager.PlayerCount + "+";
		else
			txtRank.text = (rank + 1).ToString();
	}

	public void SetInforItem(string typeReward)
	{
		// SetTypeReward()
	}



	public void SetupToolTips()
	{
		foreach (Transform child in transform.parent) children.Add(child);

		children.Sort((x, y) => x.GetComponent<PlayerCard>().rankTemp
			.CompareTo(y.GetComponent<PlayerCard>().rankTemp));
		// Thiết lập lại thứ tự trong hierarchy
		for (int i = 0; i < children.Count; i++) children[i].SetSiblingIndex(i);

		if (rankTemp < 5)
			toolTips.GetComponent<ToolTipForSuperChef>()
				.Init(ItemSuperChefDataController.Instance.itemSuperChefDefaultsData.itemSuperChefDefaults[rankTemp]);
		else if (5 <= rankTemp && rankTemp < 50)
			toolTips.GetComponent<ToolTipForSuperChef>()
				.Init(ItemSuperChefDataController.Instance.itemSuperChefDefaultsData.itemSuperChefDefaults[5]);
		else if (50 <= rankTemp && rankTemp < 100)
			toolTips.GetComponent<ToolTipForSuperChef>()
				.Init(ItemSuperChefDataController.Instance.itemSuperChefDefaultsData.itemSuperChefDefaults[6]);
		toolTips.SetActive(true);
		StartCoroutine(OffToolTips());
	}

	private IEnumerator OffToolTips()
	{
		yield return new WaitForSeconds(0.3f);
		toolTips.SetActive(false);
	}
}