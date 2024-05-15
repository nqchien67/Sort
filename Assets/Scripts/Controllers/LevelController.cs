using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Boosters;
using Data;
using DG.Tweening;
using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using Item = Gameplay.Item;
using Random = UnityEngine.Random;

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
		public Material UnknownMaterial;

		public LevelData LevelData;
		public List<Shelf> LockedShelves;
		public List<Sprite> RemainItemTypes;

		public bool CanDrag = true;
		public int LevelTime;
		public bool DoubleStar;

		protected bool _isGameEnd;
		public bool IsShowAds { get; set; }
		private Coroutine _countDown;

		private int _coin;
		private int _star;

		protected LevelUIController Ui => LevelUIController.Instance;
		private int _secondRemain;

		protected override void Awake()
		{
			base.Awake();

			RefreshShelfList();

			string numberString = gameObject.scene.name.Substring(5);
			LevelIndex = int.Parse(numberString);
			LevelIndex--;
			LevelData = DataController.Instance.LevelsData[LevelIndex];
		}

		protected virtual void Start()
		{
			_levelSetupManager = GetComponent<LevelSetupManager>();
			_levelSetupManager.SetUpLevel();
			LevelTime = CalculateTime();

			if (StartBoosterController.Instance != null)
				StartBoosterController.Instance.ActiveBooster();

			_countDown = StartCoroutine(CountDown(LevelTime));
			Ui.DisplayCoin(0);
			Ui.DisplayStar(0);
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

			LockedShelves[0].Lock.Number--;
			if (LockedShelves[0].Lock.Number <= 0)
			{
				LockedShelves[0].Lock.Remove();
				LockedShelves.RemoveAt(0);
			}
		}

		[HideInInspector] public bool PausedTime;
		private LevelSetupManager _levelSetupManager;

		private IEnumerator CountDown(int totalSeconds)
		{
			var waitForASecond = new WaitForSeconds(1);

			_secondRemain = totalSeconds;
			Ui.RenderTimer(_secondRemain);
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

		public void CheckFull()
		{
			foreach (var shelf in Shelves)
			{
				if (shelf.FrontLayer.ItemsCount < 3)
					return;
			}

			Lose();
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

			StopCoroutine(_countDown);

			int currentLevel = LevelIndex;
			currentLevel++;
			if (currentLevel > DataController.Instance.LevelsData.Length - 1)
				currentLevel = 0;
			PlayerPrefs.SetInt("level", currentLevel);
			StartCoroutine(GameManager.WaiForSeconds(0.5f, () => Ui.ShowWinPanel()));

			SaveData();
		}

		private void SaveData()
		{
			int coin = PlayerPrefs.GetInt("coin", 0);
			int star = PlayerPrefs.GetInt("star", 0);

			coin += _coin;
			star += star;
			PlayerPrefs.SetInt("coin", coin);
			PlayerPrefs.SetInt("star", star);
		}

		private void Lose()
		{
			if (_isGameEnd)
				return;
			_isGameEnd = true;
			StartCoroutine(GameManager.WaiForSeconds(1, () => Ui.ShowLosePanel()));
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
			_coin += amount;
			Ui.DisplayCoin(_coin);
		}

		protected virtual void AddStar(int amount)
		{
			_star += amount;
			Ui.DisplayStar(_star);
		}

		private int _currentCombo;
		private Coroutine _comboTimer;

		public virtual void GainScore()
		{
			AddCoin(1);

			_currentCombo++;
			_currentCombo = Mathf.Min(_currentCombo, DataController.Instance.CombosData.Length);

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
			SceneManager.LoadScene("MainScene");
		}
	}
}