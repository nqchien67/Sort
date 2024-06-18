using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSuperChefDataController : MonoBehaviour
{
	public static ItemSuperChefDataController Instance { get; private set; }

	[SerializeField] public ItemSuperChefDefaults itemSuperChefDefaultsData;

	// Start is called before the first frame update
	private void Start()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// public void LoadDefaultItemSuperChefData()
	// {
	//     string itemSuperChefData = FirebaseServiceController.Instance.GetItemSuperChefData();
	//     itemSuperChefDefaultsData = JsonUtility.FromJson<ItemSuperChefDefaults>(itemSuperChefData);
	// }
	public ItemSuperChefList GetItemSuperChefData(int itemID)
	{
		if (itemSuperChefDefaultsData.itemSuperChefDefaults[itemID] != null)
			return itemSuperChefDefaultsData.itemSuperChefDefaults[itemID];
		return null;
	}
}

[Serializable]
public class ItemSuperChef
{
	public string typeReward;
	public int number;
}

[Serializable]
public class ItemSuperChefList
{
	public List<ItemSuperChef> itemsList;
}

[Serializable]
public class ItemSuperChefDefaults
{
	public List<ItemSuperChefList> itemSuperChefDefaults;
}