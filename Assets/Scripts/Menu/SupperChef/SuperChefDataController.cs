using System;
using MainMenu.TopCharts;
using UnityEngine;
using Utilities;

public class SuperChefDataController : MonoBehaviour
{
	public static SuperChefDataController Instance { get; private set; }

	[SerializeField] private SuperChefData collectHeartData;

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

	// public void LoadDefaultSuperChefData()
	// {
	// 	string strSuperChefData = FirebaseServiceController.Instance.GetSuperChefData();
	// 	collectHeartData = JsonUtility.FromJson<SuperChefData>(strSuperChefData);
	// }

	public string GetNameEvent()
	{
		if (collectHeartData.nameEvent != null)
			return collectHeartData.nameEvent;
		return null;
	}

	public int[] GetLevelOpenEvent()
	{
		if (collectHeartData.level != null)
		{
			string[] unlocks = collectHeartData.level.Split('_');
			return new[] { int.Parse(unlocks[0]), int.Parse(unlocks[1]) };
		}

		return new[] { 2, 5 };
	}

	public int GetTimeForLimitEvent()
	{
		if (collectHeartData.time != 0) return collectHeartData.time;

		return 1728000;
	}

	public string GetDayStartEvent()
	{
		if (collectHeartData.dayStart != null)
			return collectHeartData.dayStart;
		return null;
	}

	public string GetDayEndEvent()
	{
		if (collectHeartData.dayEnd != null)
			return collectHeartData.dayEnd;
		return null;
	}
}

[Serializable]
public class SuperChefData
{
	public string nameEvent;
	public bool active;
	public string level;
	public int time;
	public string dayStart;
	public string dayEnd;
}