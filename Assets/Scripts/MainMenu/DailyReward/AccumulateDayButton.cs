using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.DailyReward
{
	public class AccumulateDayButton : MonoBehaviour
	{
		[SerializeField] private DailyRewardPanel _dailyRewardPanel;
		[SerializeField] private int _numberOfDays;
		[SerializeField] private Sprite _openChest;
		[SerializeField] private GameObject _highlight;
	[SerializeField]	private Image _image;

		private Button _button;
		private bool _canClaim;

		private void Awake()
		{
			_button = GetComponent<Button>();
		}

		private void Start()
		{
			_button.onClick.AddListener(OnClick);
		}

		public void Init(int accumulateProgress)
		{
			bool claimed = PlayerPrefs.GetInt("claimedAccumulatedDay_" + _numberOfDays, 0) == 1;
			_canClaim = _numberOfDays <= accumulateProgress && claimed;
			_highlight.SetActive(_canClaim);
			
			if (claimed)
				_image.sprite = _openChest;
		}

		private void OnClick()
		{
			if (!_canClaim)
				return;

			_image.sprite = _openChest;
			_canClaim = false;
			_highlight.SetActive(false);
			PlayerPrefs.SetInt("claimedAccumulatedDay_" + _numberOfDays, 1);

			if (_numberOfDays == 28)
				_dailyRewardPanel.ResetAccumulatedDay();
		}

		public void ResetClaimed()
		{
			PlayerPrefs.SetInt("claimedAccumulatedDay_" + _numberOfDays, 0);
		}
	}
}