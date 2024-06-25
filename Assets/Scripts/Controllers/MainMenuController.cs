using System;
using System.Collections;
using Audio;
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
		public int NextLevel;
		public bool HardLevelComing;

		public UnityAction<int> OnStartLevelAction;

		[Header("Audio")] [SerializeField] private AudioClip _backgroundMusic;
		public AudioClip CollectCoinSfx;
		public AudioClip OpenRewardSfx;
		public AudioClip ButtonClickSfx;

		protected override void Awake()
		{
			base.Awake();
			HighestPassedLevel = DataController.Instance.HighestPassedLevel;
			NextLevel = HighestPassedLevel + 1;
			HardLevelComing = NextLevel % 5 == 0 && NextLevel >= 10;

			DataController.Instance.SaveData();

			OnStartLevelAction += OnStartLevel;
			AudioController.Instance.PlayMusic(_backgroundMusic, true);

			DisplayMenuPanel();
		}

		private void Start()
		{
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

			AudioController.Instance.PlaySfx(CollectCoinSfx);
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
			PlayClaimRewardEffect(rewardType, spawnPos, MainMenuUIController.Instance.Avatar.Position);
		}

		public void PlayClaimRewardEffect(RewardType rewardType, Vector3 spawnPos, Vector3 targetPos,
			Action onComplete = null)
		{
			var position = spawnPos;
			RewardProp prop = Instantiate(_rewardPropPrefab, position, Quaternion.identity, CameraCanvas);
			prop.Init(rewardType);

			StartCoroutine(CommonIEnumerator.IMove(prop.gameObject, targetPos, 1, onComplete));
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