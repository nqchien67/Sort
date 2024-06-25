using System.Collections;
using System.Collections.Generic;
using Audio;
using Boosters;
using Data;
using DG.Tweening;
using InGame;
using InGame.Gameplay;
using MainMenu.CollectionTask;
using MainMenu.TopCharts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Controllers
{
	public class LevelController : SingletonCore<LevelController>
	{
		public int LevelIndex;
		public Shelf[] Shelves;

		public ItemLayer ItemLayerPrefab;
		public Item ItemPrefab;
		public Lock LockPrefab;

		public Material DisabledMaterial;
		public Material NormalMaterial;

		public LevelData LevelData;
		public List<Shelf> LockedShelves;
		public List<Sprite> RemainItemTypes;

		public bool CanDrag;
		public int LevelTime;
		public bool DoubleStar;

		protected bool _isGameEnd;
		public bool IsShowAds { get; set; }
		private Coroutine _countDown;

		public int Coin;
		public int Star;

		protected LevelUIController Ui => LevelUIController.Instance;
		public bool MovingItem { get; set; }

		private int _secondRemain;
		public GameObject SortEffect;
		public Prop StarProp;
		public Prop CoinProp;

		[Header("Audio")] public AudioClip GameplayMusic;
		public AudioClip PickupItemSfx;
		public AudioClip PutDownItemSfx;
		public AudioClip RemoveASetSfx;
		public AudioClip ComboSfx;
		public AudioClip UseBoosterSfx;

		protected override void Awake()
		{
			base.Awake();

			RefreshShelfList();

			string numberString = gameObject.scene.name.Substring(5);
			LevelIndex = int.Parse(numberString);
			LevelIndex--;
			LevelData = (LevelData)DataController.Instance.LevelsData[LevelIndex].Clone();
		}

		protected virtual IEnumerator Start()
		{
			AudioController.Instance.PlayMusic(GameplayMusic, true);
			_levelSetupManager = GetComponent<LevelSetupManager>();
			_levelSetupManager.SetUpLevel();
			SortEffect = SkinManager.Instance.GetSortEffect();

			Ui.DisplayCoin(0);
			Ui.DisplayStar(0);

			yield return new WaitForSeconds(0.5f);

			LevelTime = CalculateTime();
			Ui.RenderTimer(LevelTime);

			if (StartBoosterController.Instance != null)
				yield return StartCoroutine(StartBoosterController.Instance.ActiveBooster());
			else
				yield return null;

			_countDown = StartCoroutine(CountDown(LevelTime));
			CanDrag = true;
		}

		private void RefreshShelfList()
		{
			Shelves = FindObjectsOfType<Shelf>();
		}

		protected int CalculateTime()
		{
			int seconds;
			if (LevelData.ItemTypes < 25)
				seconds = 4 * 60;
			else if (LevelData.ItemTypes < 40)
				seconds = 5 * 60;
			else
				seconds = 6 * 60;

			seconds += 10 * LevelData.LockShelves;
			return seconds;
		}

		public void ReduceLocksNumber()
		{
			if (LockedShelves == null || LockedShelves.Count <= 0)
				return;

			LockedShelves[0].Lock.ReduceLocksNumber();
			if (LockedShelves[0].Lock.Number <= 0)
				// LockedShelves[0].Lock.Remove();
				LockedShelves.RemoveAt(0);
		}

		[HideInInspector] public bool PausedTime;
		private LevelSetupManager _levelSetupManager;

		private IEnumerator CountDown(int totalSeconds)
		{
			var waitForASecond = new WaitForSeconds(1);

			_secondRemain = totalSeconds;
			while (enabled)
			{
				yield return waitForASecond;
				if (PausedTime)
					continue;

				_secondRemain--;
				Ui.RenderTimer(_secondRemain);

				if (_secondRemain <= 0)
				{
					Lose();
					break;
				}
			}
		}

		public virtual void CheckFull()
		{
			StartCoroutine(CommonIEnumerator.WaitForFrames(1, () =>
			{
				foreach (var shelf in Shelves)
					if (shelf.FrontLayer.ItemsCount < 3)
						return;

				Lose();
			}));
		}

		public void CheckClear()
		{
			foreach (var shelf in Shelves)
			{
				if (shelf.Layers.Count > 1)
					return;

				if (shelf.GetAllItems().Count > 0)
					return;
			}

			Win();
		}

		public virtual void Win()
		{
			if (_isGameEnd)
				return;
			_isGameEnd = true;
			CanDrag = false;

			StopCoroutine(_countDown);

			int currentLevel = LevelIndex;
			currentLevel++;
			if (currentLevel > DataController.Instance.LevelsData.Length - 1)
				currentLevel = 0;
			PlayerPrefs.SetInt("level", currentLevel);
			StartCoroutine(CommonIEnumerator.WaiForSeconds(0.5f, () => Ui.ShowWinPanel()));
			GainReward();

			UpdateTopCharts(Star);

			int currentFreeItemProgress = PlayerPrefs.GetInt("FreeItemProgress", 0);
			currentFreeItemProgress = Mathf.Min(currentFreeItemProgress + 1, 5);
			PlayerPrefs.SetInt("FreeItemProgress", currentFreeItemProgress);

			if (LevelData.IsHardLevel() && IsReducedDifficulty())
				PlayerPrefs.SetInt("ReducedDifficulty", 0);

			CollectionTaskHandle();
		}

		protected void Lose()
		{
			if (_isGameEnd)
				return;

			_isGameEnd = true;
			CanDrag = false;
			StopCoroutine(_countDown);

			if (LevelData.IsHardLevel() && !IsReducedDifficulty())
				PlayerPrefs.SetInt("ReducedDifficulty", 1);

			StartCoroutine(CommonIEnumerator.WaiForSeconds(1, () => Ui.ShowLosePanel()));
		}

		protected void GainReward()
		{
			DataController.Instance.IncreaseOneEnergy();
			if (LevelData.IsHardLevel())
				Coin += IsReducedDifficulty() ? 25 : 50;

			DataController.Instance.Coin += Coin;
			DataController.Instance.Star += Star;

			DataController.Instance.SaveData();
		}

		private void OnApplicationPause(bool pause)
		{
			if (_isGameEnd) return;
			// if (Time.timeScale != 0 && !IsShowAds)
			// {
			// 	Time.timeScale = 0;
			// 	// pausePanel.Init();
			// }
		}

		public void Replay()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		public void ShuffleItems(List<Item> items)
		{
			_levelSetupManager.ShuffleItems(items);
		}

		public void EatASet()
		{
			GainScore();
			ReduceLocksNumber();
			CheckClear();
		}

		protected virtual void AddCoin(int amount)
		{
			Coin += amount;
			Ui.DisplayCoin(Coin);
		}

		protected virtual void AddStar(int amount)
		{
			Star += amount;
			Ui.DisplayStar(Star);
		}

		private int _currentCombo;
		private Coroutine _comboTimer;
		private int _highestCombo;

		protected virtual void GainScore()
		{
			AddCoin(1);

			_currentCombo++;
			_currentCombo = Mathf.Min(_currentCombo, DataController.Instance.CombosData.Length);

			if (_currentCombo % 5 == 0)
				AudioController.Instance.PlaySfx(ComboSfx);

			if (_highestCombo < _currentCombo) _highestCombo = _currentCombo;

			int comboStar = DataController.Instance.CombosData[_currentCombo - 1].Star;

			if (DoubleStar)
				AddStar(2 * (3 + comboStar));
			else
				AddStar(3 + comboStar);

			if (_comboTimer != null)
				StopCoroutine(_comboTimer);
			_comboTimer = StartCoroutine(ComboTimer());
		}

		private IEnumerator ComboTimer()
		{
			Ui.DisplayCombo(_currentCombo);

			float comboTime = DataController.Instance.CombosData[_currentCombo - 1].Time;
			float remainTime = comboTime;

			yield return Ui._comboTimeBar.DOFillAmount(1, 0.1f).WaitForCompletion();
			remainTime -= 0.1f;

			while (remainTime > 0)
			{
				yield return null;
				remainTime -= Time.deltaTime;
				Ui.DisplayComboTimeBar(remainTime, comboTime);
			}

			_currentCombo = 0;
			Ui.DisplayComboTimeBar(0, comboTime);
			Ui.DisplayCombo(_currentCombo);
		}

		public void GoHome()
		{
			StartBoosterController.Instance.Stop();
			SceneController.Instance.LoadScene("MainScene");
		}

		public static bool IsReducedDifficulty()
		{
			return PlayerPrefs.GetInt("ReducedDifficulty", 0) > 0;
		}

		protected void CollectionTaskHandle()
		{
			var cTController = CollectionTaskController.Instance;
			if (!cTController.IsStarted)
				return;

			if (cTController.Current.Type == TaskType.Star)
				cTController.AddProgress(Star);
			else if (cTController.Current.Type == TaskType.Combo) cTController.AddProgress(_highestCombo);
		}

		public void UpdateTopCharts(int star)
		{
			int prevPoint = PlayerPrefs.GetInt("PointUser", 0);
			PlayerPrefs.SetInt("PointUser", prevPoint + star);
			PlayerPrefs.SetInt("PrevPointUser", prevPoint);

			TopChartsPlayerDataManager.Instance.UpdateRank();
		}
	}
}