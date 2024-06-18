using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToolTipForSuperChef : MonoBehaviour
{
    public GameObject[] goReward;
    public string[] strItemName = {"2 Chance", "Add Customer", "Add Time", "Anti Over", "Auto Serve",
                                    "Avatar CN", "Caramen", "Gem", "Coin", "Customer New", "Double Coin",
                                    "Energy", "Limitless Energy", "Instant", "Avatar CN 2"};
    public Sprite[] listReward;
    public void SetTypeReward(string typeReward, Image img)
    {
        for (int i = 0; i < strItemName.Length; i++)
            if (typeReward == strItemName[i])
                img.sprite = listReward[i];
    }
    public void Init(ItemSuperChefList listData)
    {
        for (int i = 0; i < 4; i++)
        {
            goReward[i].SetActive(false);
        }
        for (int i = 0; i < listData.itemsList.Count; i++)
        {
            goReward[i].SetActive(true);
            SetTypeReward(listData.itemsList[i].typeReward, goReward[i].GetComponent<Image>());
            goReward[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x" + listData.itemsList[i].number.ToString();
            if (listData.itemsList[i].number == 0)
            { goReward[i].SetActive(false); }
            Debug.Log(listData.itemsList[i].number);
        }
    }
}
