using System;
using Controllers;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.CollectionTask
{
	public class Task : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _taskOrder;
		[SerializeField] private Image _rewardImage;
		[SerializeField] private TextMeshProUGUI _rewardAmount;
		[SerializeField] private GameObject _lock;
		[SerializeField] private GameObject _tick;
		[SerializeField] private Image _taskOrderImage;
		[SerializeField] private Sprite[] _taskOrderSprites;

		[SerializeField] private Image _boxImage;
		[SerializeField] private Sprite[] _boxSprites;

		[SerializeField] private Button _button;
		[SerializeField] private GameObject _highlight;
		[SerializeField] private Color _completedLineColor;
		[SerializeField] private GameObject _claimButton;
		public GameObject Line;

		private CTProgress _progress;
		private CTTaskData _task;
		private Image _lineImage;

		private void Awake()
		{
			_lineImage = Line.GetComponent<Image>();
		}

		public void Init(CTTaskData task, CTProgress progress)
		{
			_progress = progress;
			_task = task;
			_taskOrder.text = task.Id.ToString();
			DisplayByState(progress.State);
			_rewardImage.sprite = RewardHelper.Instance.GetRewardSprite(task.RewardType);
			_rewardAmount.text = "x" + task.RewardAmount;

			if (progress.Id < CollectionTaskController.Instance.CurrentTaskIndex)
				_lineImage.color = _completedLineColor;
		}

		public void OnClickClaim()
		{
			if (_progress.State != State.Complete)
				return;

			_progress.State = State.Claimed;
			DisplayByState(_progress.State);

			RewardType rewardType = _task.RewardType;
			if (rewardType == RewardType.Coin)
			{
				DataController.Instance.Coin += _task.RewardAmount;
				MainMenuController.Instance.PlayClaimCoinEffect(_rewardImage.transform.position);
			}
			else if (rewardType == RewardType.Energy)
			{
				DataController.Instance.AddUnlimitedEnergy(_task.RequestAmount);
				EnergyController.Instance.PlayUnlimitedEnergyEffect();
			}
			else if (RewardHelper.TryConvertRewardToBooster(rewardType, out BoosterType boosterType))
			{
				DataController.Instance.AddBooster(boosterType, _task.RewardAmount);
				MainMenuController.Instance.PlayClaimRewardEffect(rewardType, transform.position);
			}

			CollectionTaskBar.Instance.CheckAndShowNotiDot();
			CollectionTaskController.Instance.SaveData();
			DataController.Instance.SaveData();
		}

		private void DisplayByState(State state)
		{
			if (state == State.Complete || state == State.Claimed)
			{
				_tick.SetActive(state == State.Claimed);
				_rewardImage.gameObject.SetActive(state == State.Complete);
				_rewardAmount.gameObject.SetActive(state == State.Complete);
				_taskOrderImage.sprite = _taskOrderSprites[0];
				_boxImage.sprite = _boxSprites[0];

				_lock.SetActive(false);
				_button.enabled = true;
				_highlight.SetActive(state == State.Complete);
				_claimButton.gameObject.SetActive(state == State.Complete);
			}
			else
			{
				_tick.SetActive(false);
				_rewardImage.gameObject.SetActive(true);
				_rewardAmount.gameObject.SetActive(true);
				_taskOrderImage.sprite = _taskOrderSprites[1];
				_boxImage.sprite = _boxSprites[1];

				_lock.SetActive(state == State.Lock);
				_button.enabled = false;
				_highlight.SetActive(false);
				_claimButton.gameObject.SetActive(false);
			}
		}
	}
}