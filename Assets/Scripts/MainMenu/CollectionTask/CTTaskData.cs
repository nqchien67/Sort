using System;
using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MainMenu.CollectionTask
{
	[Serializable]
	public class CTTaskData
	{
		public int Id;

		[JsonConverter(typeof(StringEnumConverter))]
		public TaskType Type;

		public int RequestAmount;

		[JsonConverter(typeof(StringEnumConverter))]
		public RewardType RewardType;

		public int RewardAmount;
	}

	public class CollectionTaskData
	{
		public int TimeResetTask; //second
		public CTTaskData[] CollectionTasksData;
	}

	[Serializable]
	public class CTProgress
	{
		public int Id;
		public int Progress;
		public State State;

		public CTProgress(int id)
		{
			Id = id;
			Progress = 0;
			State = State.Lock;
		}

		public void Reset()
		{
			Progress = 0;
			State = State.Lock;
		}
	}

	[Serializable]
	public class CTProgressData
	{
		public CTProgress[] Progresses;

		public int CurrentTaskId;

		public CTProgressData(CTTaskData[] tasks)
		{
			Progresses = new CTProgress[tasks.Length];
			for (int i = 0; i < tasks.Length; i++) Progresses[i] = new CTProgress(tasks[i].Id);

			CurrentTaskId = -1;
		}
	}

	[Serializable]
	public enum TaskType
	{
		Star,
		Combo
	}

	[Serializable]
	public enum State
	{
		Lock,
		Active,
		Complete,
		Claimed
	}
}