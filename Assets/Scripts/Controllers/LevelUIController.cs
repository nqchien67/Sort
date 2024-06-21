using Data;
using DG.Tweening;
using InGame.UI;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
	public class LevelUIController : SingletonCore<LevelUIController>
	{
		[SerializeField] private TextMeshProUGUI _timerText;
		[SerializeField] private TextMeshProUGUI _levelText;
		[SerializeField] private TextMeshProUGUI _coinText;
		[SerializeField] private TextMeshProUGUI _starText;
		[SerializeField] private TextMeshProUGUI _comboText;
		public Image _comboTimeBar;

		[SerializeField] private LosePanel _losePanel;
		[SerializeField] private WinPanel _winPanel;

		[SerializeField] private Image _avatarImage;

		public Transform ClockIcon;
		public Transform StarIcon;
		public Transform CoinIcon;
		public GameObject StarX2Icon;
		public Transform Canvas;
		public RectTransform TopBar;
		public RectTransform BottomBar;

		[SerializeField] private Sprite _hardLevelTopBar;
		[SerializeField] private BuyBoosterPanel _buyBoosterPanelPrefab;
		public GameObject NotEnoughCoin;

		private void Start()
		{
			_avatarImage.sprite = SpritesCollection.Instance.CurrentAvatarSprite;

			if (_avatarImage.sprite == null)
				_avatarImage.gameObject.SetActive(false);

			_losePanel = GetComponentInChildren<LosePanel>(true);
			_winPanel = GetComponentInChildren<WinPanel>(true);

			string numberString = gameObject.scene.name.Substring(5);
			_levelText.text = "Lv." + int.Parse(numberString);

			DisplayCombo(0);
			DisplayComboTimeBar(0, 1);

			if (LevelController.Instance.LevelData.IsHardLevel())
				TopBar.Find("Background").GetComponent<Image>().sprite = _hardLevelTopBar;
		}

		public void RenderTimer(int secondsLeft)
		{
			int minutes = secondsLeft / 60;
			int seconds = secondsLeft % 60;

			string minutesString = minutes.ToString("D2");
			string secondsString = seconds.ToString("D2");

			_timerText.text = $"{minutesString}:{secondsString}";
		}

		public void DisplayCoin(int amount)
		{
			_coinText.text = amount.ToString();
		}

		public void DisplayStar(int amount)
		{
			_starText.text = amount.ToString();
		}

		public void DisplayCombo(int combo)
		{
			if (combo == 0)
			{
				_comboText.text = "";
			}
			else
			{
				_comboText.text = "Combo x" + combo;
				_comboText.transform.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo);
			}
		}

		public void DisplayComboTimeBar(float timeRemain, float totalTime)
		{
			_comboTimeBar.fillAmount = timeRemain / totalTime;
		}

		public void ShowLosePanel()
		{
			_losePanel.Show();
		}

		public void ShowWinPanel()
		{
			_winPanel.Show();
		}

		public void NextLevel()
		{
			LevelController.Instance.Win();
		}

		public void OnClickHome()
		{
			LevelController.Instance.GoHome();
		}

		private Tween _starIconBlink;
		private Tween _coinIconBlink;

		public void BlinkStarIcon()
		{
			_starIconBlink?.Kill();
			_starIconBlink = DOTween.Sequence()
				.Append(StarIcon.DOScale(new Vector3(1.4f, 1.4f, 1), 0.1f))
				.Append(StarIcon.DOScale(1, 0.1f));
		}

		public void BlinkCoinIcon()
		{
			_coinIconBlink?.Kill();
			_coinIconBlink = DOTween.Sequence()
				.Append(CoinIcon.DOScale(new Vector3(1.4f, 1.4f, 1), 0.1f))
				.Append(CoinIcon.DOScale(1, 0.1f));
		}

		public BuyBoosterPanel SpawnBuyBoosterPanel()
		{
			return Instantiate(_buyBoosterPanelPrefab, Canvas);
		}

		public void ShowNotEnoughCoin()
		{
			NotEnoughCoin.SetActive(true);
			NotEnoughCoin.transform.SetAsLastSibling();
		}
	}
}