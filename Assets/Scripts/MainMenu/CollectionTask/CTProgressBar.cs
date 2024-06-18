using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.CollectionTask
{
	public class CTProgressBar : MonoBehaviour
	{
		[SerializeField] private RectTransform _progressBarFill;
		[SerializeField] private TextMeshProUGUI _progressAmountText;

		[SerializeField] private GameObject _taskStar;
		[SerializeField] private GameObject _taskCombo;
		[SerializeField] private TextMeshProUGUI _taskComboText;

		[SerializeField] private Image _rewardIcon;

		public void Init(CTTaskData current)
		{
			DisplayTaskType(current.Type, current.RequestAmount);
			_rewardIcon.sprite = RewardHelper.Instance.GetRewardSprite(current.RewardType);

			if (current.Type == TaskType.Combo)
			{
				UpdateProgressBar(0, 1);
			}
			else if (current.Type == TaskType.Star)
			{
				int currentProgress = CollectionTaskController.Instance.CurrentTaskProgress;
				UpdateProgressBar(currentProgress, current.RequestAmount);
			}
		}

		private void DisplayTaskType(TaskType taskType, int requestAmount)
		{
			if (taskType == TaskType.Combo)
			{
				_taskCombo.SetActive(true);
				_taskStar.SetActive(false);
				_taskComboText.text = "Combo " + requestAmount;
			}
			else if (taskType == TaskType.Star)
			{
				_taskCombo.SetActive(false);
				_taskStar.SetActive(true);
			}
		}

		private void UpdateProgressBar(int currentAmount, int requestAmount)
		{
			Debug.Log(currentAmount + ", " + requestAmount);
			var sizeDelta = _progressBarFill.sizeDelta;
			var maxFillBarLength = sizeDelta.x;

			float fillPercent = (float)currentAmount / requestAmount;
			sizeDelta.x = maxFillBarLength * fillPercent;

			_progressBarFill.sizeDelta = sizeDelta;
			_progressAmountText.text = currentAmount + "/" + requestAmount;
		}
	}
}