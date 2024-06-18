using System;
using System.Collections;
using Boosters;
using Data;
using DG.Tweening;
using MainMenu;
using MainMenu.CollectionTask;
using MainMenu.DailyReward;
using TMPro;
using UI;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utilities;
using Random = UnityEngine.Random;

namespace Controllers
{
	public class MainMenuController : SingletonCore<MainMenuController>
	{
		[SerializeField] private TMP_InputField _levelInput;
		[SerializeField] private StartLevelPanel startLevelPanel;
		[SerializeField] private TextMeshProUGUI _buttonPlayText;

		[Header("Top Bar")] [SerializeField] private Transform _coinIcon;

		[Header("Props")] [SerializeField] private Transform _coinPropPrefab;
		[SerializeField] private RewardProp _rewardPropPrefab;

		[Header("Tutorials")] [SerializeField] private StartBoosterTutorial _hugeHammerTutorial;
		[SerializeField] private StartBoosterTutorial _addTimeTutorial;
		[SerializeField] private StartBoosterTutorial _doublePointTutorial;
		[SerializeField] private CommonTutorial _luckySpinTutorial;
		[SerializeField] private CommonTutorial _piggyBankTutorial;
		[SerializeField] private CommonTutorial _endlessTreasureTutorial;

		[Space] [SerializeField] private DailyRewardController _dailyRewardController;

		public Transform CameraCanvas;

		public int HighestPassedLevel;
		private int _nextLevel;
		[HideInInspector] public bool HardLevelComing;

		public UnityAction<int> OnStartLevelAction;

		protected override void Awake()
		{
			HighestPassedLevel = DataController.Instance.HighestPassedLevel;
			base.Awake();
			// PlayerPrefs.SetInt("ReducedDifficulty", 0);
		}

		private void Start()
		{
			_nextLevel = HighestPassedLevel + 1;
			_buttonPlayText.text = "LEVEL " + _nextLevel;

			HardLevelComing = _nextLevel % 5 == 0 && HighestPassedLevel > 5;

			DataController.Instance.SaveData();

			OnStartLevelAction += OnStartLevel; 
			DisplayMenuPanel();
		}

		public void DisplayMenuPanel()
		{
			if (_dailyRewardController.ShouldShowPanel())
				_dailyRewardController.ShowDailyRewardPanel();
			else
				switch (HighestPassedLevel)
				{
					case 2:
						OnClickPlay();
						_hugeHammerTutorial.gameObject.SetActive(true);
						break;
					case 5:
						if (_luckySpinTutorial != null)
							_luckySpinTutorial.gameObject.SetActive(true);
						break;
					case 6:
						if (_luckySpinTutorial != null)
							_piggyBankTutorial.gameObject.SetActive(true);
						break;
					case 12:
						if (_endlessTreasureTutorial != null)
							_endlessTreasureTutorial.gameObject.SetActive(true);
						break;
				}
		}

		public void OnClickPlay()
		{
			startLevelPanel.Show();

			if (HighestPassedLevel == 4 && _addTimeTutorial != null)
				_addTimeTutorial.gameObject.SetActive(true);

			if (HighestPassedLevel == 6 && _doublePointTutorial != null)
				_doublePointTutorial.gameObject.SetActive(true);
		}

		public void Play()
		{
			if (DataController.Instance.TryUseEnergy())
			{
				string text = _levelInput.text;
				if (int.TryParse(text, out int level) && level >= 1)
					PlayerPrefs.SetInt("level", HighestPassedLevel);
				else
					level = HighestPassedLevel + 1;

				SceneController.Instance.LoadScene("Level" + level);
				OnStartLevelAction?.Invoke(level);
			}
			else
				EnergyController.Instance.OpenBuyEnergyPanel();
		}

		public void PlayClaimCoinEffect(Vector3 spawnPos)
		{
			StartCoroutine(IncreaseCoin(spawnPos));
		}

		private IEnumerator IncreaseCoin(Vector3 spawnPos)
		{
			// isLockUpdateData = true;

			for (int i = 0; i < 5; i++)
			{
				StartCoroutine(CoinPropEffect(spawnPos));

				yield return new WaitForSeconds(0.06f);
			}
			// isLockUpdateData = false;
		}

		private IEnumerator CoinPropEffect(Vector3 spawnPos)
		{
			Transform coin = Instantiate(_coinPropPrefab, spawnPos + new Vector3(0, 0, -0.1f), Quaternion.identity);
			Vector3 targetPosition = coin.transform.position +
			                         new Vector3(Random.Range(-1.3f, 1.3f), Random.Range(-1.3f, 1.3f), 0);
			Vector3 coinIconPos = MainMenuUIController.Instance.Coin.IconPosition;
			yield return coin.DOMove(targetPosition, 0.75f)
				.SetEase(Ease.OutQuint)
				.WaitForCompletion();

			yield return coin.DOMove(new Vector3(coinIconPos.x, coinIconPos.y, spawnPos.z - 0.1f), 0.25f)
				.SetEase(Ease.InQuad)
				.WaitForCompletion();

			// AudioController.Instance.PlaySfx(coinIncreaseAudio);
			MainMenuUIController.Instance.Coin.UpdateValue();
			// goldTimeStamp = Time.time;
			Destroy(coin.gameObject);
		}

		public void PlayClaimRewardEffect(RewardType rewardType, Vector3 spawnPos)
		{
			var position = spawnPos;
			RewardProp prop = Instantiate(_rewardPropPrefab, position, Quaternion.identity, CameraCanvas);
			prop.Init(rewardType);

			StartCoroutine(CommonIEnumerator.IMove(prop.gameObject, MainMenuUIController.Instance.Avatar.Position, 1));
		}
		
		private void OnStartLevel(int level)
		{
			// if (level == 4)
			// {
			// 	CollectionTaskController.Instance.
			// }
		}
	}
}