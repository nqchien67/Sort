using System;
using System.Collections.Generic;
using Data;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utilities;

namespace MainMenu.CollectionTask
{
	public class CollectionTaskPanel : Popup
	{
		private CollectionTaskBar Bar => CollectionTaskBar.Instance;
		[SerializeField] private ScrollRect _tasksScrollRect;
		[SerializeField] private TextMeshProUGUI _timer;
		private CollectionTaskController CollectionTaskController => CollectionTaskController.Instance;
		private CTTaskData[] Tasks => CollectionTaskController.Tasks;
		public CTProgress[] Progresses => CollectionTaskController.Progresses;
		[SerializeField] private CTProgressBar _progressBar;

		private List<Task> _tasksUI;

		private void Start()
		{
			Inits();
		}

		private void Update()
		{
			_timer.text = Bar.Timer.text;
		}

		private void Inits()
		{
			InitTasks();
			CTTaskData current = CollectionTaskController.Instance.Current;
			_progressBar.Init(current);
		}

		public override void Show()
		{
			base.Show();

			StartCoroutine(CommonIEnumerator.WaiForSeconds(0.5f, () =>
			{
				RectTransform currentTask =
					_tasksUI[CollectionTaskController.CurrentTaskId].GetComponent<RectTransform>();
				_tasksScrollRect.FocusOnItem(currentTask);
			}));
		}

		private void InitTasks()
		{
			_tasksUI = new List<Task>();

			Task taskPrefab = _tasksScrollRect.content.GetComponentInChildren<Task>();
			_tasksUI.Add(taskPrefab);

			for (int i = 1; i < Tasks.Length; i++)
			{
				Task task = Instantiate(taskPrefab, _tasksScrollRect.content);
				task.Init(Tasks[i], Progresses[i]);
				_tasksUI.Add(task);
			}

			taskPrefab.Init(Tasks[0], Progresses[0]);

			_tasksUI[_tasksUI.Count - 1].Line.SetActive(false);
		}

		public override void EndCloseAnimationTrigger()
		{
			base.EndCloseAnimationTrigger();
			gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			CollectionTaskController.Instance.OnResetProgress += Inits;
		}

		private void OnDisable()
		{
			CollectionTaskController.Instance.OnResetProgress -= Inits;
		}
	}
}