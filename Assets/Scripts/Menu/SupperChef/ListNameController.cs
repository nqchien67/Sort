using System;
using System.Collections.Generic;
using UnityEngine;

public class ListNameController : MonoBehaviour
{
	public static ListNameController Instance { get; private set; }

	[SerializeField] public ListNameControllerDefaults listNameControllerDefaults;

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

	// public void LoadDefaultListNameController()
	// {
	//     string listNameController = FirebaseServiceController.Instance.GetItemNameData();
	//     listNameControllerDefaults = JsonUtility.FromJson<ListNameControllerDefaults>(listNameController);
	// }
	public ListNameControllerDefault GetListNameController(int itemID)
	{
		if (listNameControllerDefaults.items[itemID] != null) return listNameControllerDefaults.items[itemID];
		return null;
	}
}

[Serializable]
public class ListNameControllerDefaults
{
	public List<ListNameControllerDefault> items;
}

[Serializable]
public class ListNameControllerDefault
{
	public List<string> ten;
	public List<string> ho;
}