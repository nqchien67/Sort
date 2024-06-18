using System;
using System.IO;
using System.Linq;
using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace MainMenu.CollectionTask
{
	public class CollectionTaskController : SingletonCore<CollectionTaskController>
	{
		public const string ResetTaskTimeStamp_Hash = "ResetTaskTimeStamp";

		private string _saveFilePath;

		public CTTaskData[] Tasks;

		public CTProgressData ProgressData;
		public CTProgress[] Progresses => ProgressData.Progresses;

		public int CurrentTaskId
		{
			get => ProgressData.CurrentTaskId;
			private set => ProgressData.CurrentTaskId = value;
		}

		public CTTaskData Current => Tasks[CurrentTaskId];

		public int CurrentTaskProgress
		{
			get => Progresses[CurrentTaskId].Progress;
			private set => Progresses[CurrentTaskId].Progress = value;
		}

		public int TimeResetTask;
		public bool IsStarted => CurrentTaskId >= 0;

		public UnityAction OnResetProgress;

		private void Start()
		{
			_saveFilePath = Application.persistentDataPath + "/collection_task_progress.dat";
			LoadTasksData();
			LoadProgressData();
		}

		public void StartFirstTask()
		{
			CurrentTaskId = 0;
			Progresses[CurrentTaskId].State = State.Active;
			SaveData();
			PlayerPrefs.SetFloat(ResetTaskTimeStamp_Hash,
				(float)DataController.ConvertToUnixTime(DateTime.Now) + TimeResetTask);
		}

		private void LoadTasksData()
		{
			string data = FirebaseServiceController.Instance.GetCollectionTaskData();
			var collectionTaskCollection = JsonConvert.DeserializeObject<CollectionTaskData>(data);
			Tasks = collectionTaskCollection.CollectionTasksData;
			TimeResetTask = collectionTaskCollection.TimeResetTask;
		}

		private void LoadProgressData()
		{
			if (File.Exists(_saveFilePath))
			{
				string data = File.ReadAllText(_saveFilePath);
				ProgressData = JsonUtility.FromJson<CTProgressData>(data);
			}
			else
			{
				GenerateNewProgressData();
			}
		}

		private void GenerateNewProgressData()
		{
			ProgressData = new CTProgressData(Tasks);
			SaveData();
		}

		public void SaveData()
		{
			string data = JsonUtility.ToJson(ProgressData);
			File.WriteAllText(_saveFilePath, data);
		}

		public void AddProgress(int amount)
		{
			if (Current.Type == TaskType.Star)
			{
				CurrentTaskProgress = Mathf.Min(CurrentTaskProgress + amount, Current.RequestAmount);
				if (CurrentTaskProgress < Current.RequestAmount)
				{
					SaveData();
					return;
				}
			}
			else if (Current.Type == TaskType.Combo
			         && amount < Current.RequestAmount)
			{
				return;
			}

			Progresses[CurrentTaskId].State = State.Complete;
			ChangeToNextTask();
		}

		private void ChangeToNextTask()
		{
			CurrentTaskId++;
			Progresses[CurrentTaskId].State = State.Active;
			SaveData();
		}

		public bool HaveUnclaimedReward()
		{
			return Progresses.Any(progress => progress.State == State.Complete);
		}

		public void ResetProgresses()
		{
			foreach (CTProgress progress in Progresses) 
				progress.Reset();

			StartFirstTask();
			OnResetProgress?.Invoke();
		}
	}
}