using System.Collections;
using Boosters;
using Data;
using DG.Tweening;
using MainMenu;
using MainMenu.DailyReward;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
	public class MainMenuController : SingletonCore<MainMenuController>
	{
		[SerializeField] private TMP_InputField _levelInput;
		[SerializeField] private StartLevelPopup _startLevelPopup;
		[SerializeField] private TextMeshProUGUI _buttonPlayText;

		[Header("Top Bar")] [SerializeField] private Transform _coinIcon;

		[Header("Props")] [SerializeField] private Transform _coinPropPrefab;

		[Header("Tutorials")] [SerializeField] private StartBoosterTutorial _hugeHammerTutorial;
		[SerializeField] private StartBoosterTutorial _addTimeTutorial;
		[SerializeField] private StartBoosterTutorial _doublePointTutorial;
		[SerializeField] private CommonTutorial _luckySpinTutorial;
		[SerializeField] private CommonTutorial _piggyBankTutorial;

		[Space] [SerializeField] private DailyRewardController _dailyRewardController;
		[SerializeField] private UnlockNewItemPanel _unlockNewItemPanelPrefab;

		public Transform CameraCanvas;

		private int _highestPassedLevel;
		private int _nextLevel;
		[HideInInspector] public bool HardLevelComing;

		private void Start()
		{
			_highestPassedLevel = PlayerPrefs.GetInt("level", 0);
			_nextLevel = _highestPassedLevel + 1;
			_buttonPlayText.text = "LEVEL " + _nextLevel;

			HardLevelComing = _nextLevel % 5 == 0 && _highestPassedLevel > 5;

			DataController.Instance.SaveData();
			DisplayMenuPanel();
		}

		public void DisplayMenuPanel()
		{
			if (_dailyRewardController.ShouldShowPanel())
				_dailyRewardController.ShowDailyRewardPanel();
			else if (_highestPassedLevel % 5 == 0 &&
			         _highestPassedLevel / 5 > SkinDataController.Instance.UnlockedItemSkinsCount)
				Instantiate(_unlockNewItemPanelPrefab, CameraCanvas);
			else
				switch (_highestPassedLevel)
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
				}
		}

		public void OnClickPlay()
		{
			_startLevelPopup.Show();

			if (_highestPassedLevel == 4)
				_addTimeTutorial.gameObject.SetActive(true);

			if (_highestPassedLevel == 6)
				_doublePointTutorial.gameObject.SetActive(true);
		}

		public void Play()
		{
			string text = _levelInput.text;
			if (int.TryParse(text, out int level) && level >= 1)
				PlayerPrefs.SetInt("level", _highestPassedLevel);
			else
				level = _highestPassedLevel + 1;

			SceneManager.LoadScene("Level" + level);
		}

		public void StartIncreaseCoin(Vector3 spawnPos, int totalCoin)
		{
			StartCoroutine(IncreaseCoin(spawnPos, totalCoin));
		}

		private IEnumerator IncreaseCoin(Vector3 spawnPos, int totalCoin)
		{
			// isLockUpdateData = true;
			// tmpGold = targetCoin;
			int[] coins = new int[5];
			int averageGold = totalCoin / 5;
			for (int i = 0; i < 4; i++)
				coins[i] = averageGold;

			coins[4] = totalCoin - averageGold * 4;
			for (int i = 0; i < 5; i++)
			{
				int rewardCoin = coins[i];
				StartCoroutine(CoinPropEffect(spawnPos, rewardCoin));

				yield return new WaitForSeconds(0.06f);
			}
			// isLockUpdateData = false;
		}

		private IEnumerator CoinPropEffect(Vector3 spawnPos, int rewardCoin)
		{
			Transform coin = Instantiate(_coinPropPrefab, spawnPos + new Vector3(0, 0, -0.1f), Quaternion.identity);
			Vector3 targetPosition = coin.transform.position +
			                         new Vector3(Random.Range(-1.3f, 1.3f), Random.Range(-1.3f, 1.3f), 0);
			Vector3 coinIconPos = _coinIcon.position;
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
	}
}