using System;
using System.Collections;
using Data;
using DG.Tweening;
using MainMenu;
using TMPro;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Controllers
{
	public class EnergyController : SingletonCore<EnergyController>
	{
		private const int ENERGY_REPLENISH_TIME = 1200;
		public TextMeshProUGUI energyCount, timeCountDown;
		[SerializeField] private BuyEnergyPanel buyEnergyPanelPrefab;
		[SerializeField] private GameObject _normalEnergyIcon;
		[SerializeField] private GameObject _unlimitedEnterIcon;

		[SerializeField] private GameObject energyPropPrefab, unlimitedEnergyPropPrefab;
		private Transform CameraCanvas => MainMenuController.Instance.CameraCanvas;
		[SerializeField] private Button buyEnergyBtn;
		private int currentEnergy;

		private void Start()
		{
			DataController.Instance.Energy += CalculateGainOfflineEnergy();
			currentEnergy = DataController.Instance.Energy;
			energyCount.text = currentEnergy.ToString();
			buyEnergyBtn.gameObject.SetActive(DataController.Instance.Energy < PlayerPrefs.GetInt("MAX_ENERGY", 5) &&
			                                  DataController.Instance.UnlimitedEnergyDuration <= 0);
			StopAllCoroutines();
			StartCoroutine(ReplenishEnergy());
		}

		private IEnumerator ReplenishEnergy()
		{
			var delay = new WaitForSecondsRealtime(1);
			double saveTimeStamp = DataController.Instance.EnergyTimeStamp;
			double deltaTime;
			int remainTime;
			while (true)
			{
				DataController.Instance.UnlimitedEnergyDuration -= CalculateTimePassUnlimitedEnergy();
				SetHeartIcon();

				energyCount.gameObject.SetActive(DataController.Instance.UnlimitedEnergyDuration <= 0);
				if (DataController.Instance.UnlimitedEnergyDuration > 0)
				{
					if (DataController.Instance.UnlimitedEnergyDuration < 0)
						DataController.Instance.UnlimitedEnergyDuration = 0;

					DataController.Instance.UnlimitedEnergyTimeStamp =
						DataController.ConvertToUnixTime(DateTime.UtcNow);

					timeCountDown.text = string.Format("{0:D2}:{1:D2}",
						DataController.Instance.UnlimitedEnergyDuration / 60,
						DataController.Instance.UnlimitedEnergyDuration % 60);
				}

				deltaTime = DataController.ConvertToUnixTime(DateTime.UtcNow) - saveTimeStamp;
				if (deltaTime < -1800)
				{
					DataController.Instance.EnergyTimeStamp =
						DataController.ConvertToUnixTime(DateTime.UtcNow); //user hack time
					deltaTime = 0;
				}

				if (deltaTime >= ENERGY_REPLENISH_TIME)
				{
					saveTimeStamp = DataController.ConvertToUnixTime(DateTime.UtcNow);
					DataController.Instance.Energy++; //add a energy
					currentEnergy = DataController.Instance.Energy;
					if (DataController.Instance.UnlimitedEnergyDuration <= 0)
					{
						buyEnergyBtn.gameObject.SetActive(DataController.Instance.Energy == 0);
						energyCount.text = DataController.Instance.Energy.ToString();
					}
				}

				if (DataController.Instance.Energy <
				    PlayerPrefs.GetInt("MAX_ENERGY", 5)) //calculate the remain time and convert to minute and second
				{
					remainTime = (int)(ENERGY_REPLENISH_TIME - deltaTime);
					if (remainTime < 0) remainTime = 0;
					if (DataController.Instance.UnlimitedEnergyDuration <= 0)
						timeCountDown.text = $"{remainTime / 60:D2}:{remainTime % 60:D2}";
				}
				else
				{
					if (DataController.Instance.UnlimitedEnergyDuration <= 0)
					{
						// _normalEnergyIcon.sprite = _imageEnergyIcons[0];
						// timeCountDown.text = Lean.Localization.LeanLocalization.GetTranslationText("energy_full");
						timeCountDown.text = "Full";
					}
				}

				yield return delay;
			}
		}

		private int CalculateTimePassUnlimitedEnergy()
		{
			double deltaTime = DataController.ConvertToUnixTime(DateTime.UtcNow) -
			                   DataController.Instance.UnlimitedEnergyTimeStamp;
			return Mathf.Max(0, (int)deltaTime);
		}

		private int CalculateGainOfflineEnergy() //this function calculate the offline energy of player
		{
			double deltaTime = DataController.ConvertToUnixTime(DateTime.UtcNow) -
			                   DataController.Instance.EnergyTimeStamp;
			return Mathf.Min((int)(deltaTime / ENERGY_REPLENISH_TIME), PlayerPrefs.GetInt("MAX_ENERGY", 5));
		}

		public void OpenBuyEnergyPanel()
		{
			// buyEnergyPanelPrefab.Spawn(CameraCanvas, PlayIncreaseEnergyEffect);
			BuyEnergyPanel buyEnergyPanel = Instantiate(buyEnergyPanelPrefab, CameraCanvas);
			buyEnergyPanel.Init(PlayIncreaseEnergyEffect);
		}

		public void SetTextEnergyCount()
		{
			energyCount.text = DataController.Instance.Energy.ToString();
		}

		private void PlayIncreaseEnergyEffect()
		{
			energyCount.text = DataController.Instance.Energy.ToString();
			int amount = DataController.Instance.Energy - currentEnergy;
			currentEnergy = DataController.Instance.Energy;
			energyCount.text = DataController.Instance.Energy.ToString();
			buyEnergyBtn.gameObject.SetActive(DataController.Instance.Energy == 0);
			for (int i = 0; i < amount; i++)
			{
				GameObject energyProp = Instantiate(energyPropPrefab, CameraCanvas.position + new Vector3(0, 0, -0.1f),
					Quaternion.identity, CameraCanvas);
				StartCoroutine(IMove(energyProp, _normalEnergyIcon.transform.position, 1));
			}
		}

		public IEnumerator IMove(GameObject gameObject, Vector2 pos, float speed)
		{
			float time = 0;
			Vector2 middlePos =
				new Vector2((gameObject.transform.position.x + pos.x) / 2f + Random.Range(-6f, 0f),
					(gameObject.transform.position.y + pos.y) / 3f);
			Vector2 tempPos = gameObject.transform.position;
			while (Vector2.Distance(gameObject.transform.position, pos) > 0.3f)
			{
				gameObject.transform.position = CalculateQuadraticBezierPoint(time, tempPos, middlePos, pos);
				time += Time.deltaTime * speed * 2;
				yield return null;
			}

			DOVirtual.DelayedCall(0.05f, () => { Destroy(gameObject); });
		}

		public Vector3 CalculateQuadraticBezierPoint(float t1, Vector3 p0, Vector3 p1, Vector3 p2)
		{
			float u = 1 - t1;
			float tt = t1 * t1;
			float uu = u * u;
			Vector3 p = uu * p0;
			p += 2 * u * t1 * p1;
			p += tt * p2;
			return p;
		}

		public void PlayUnlimitedEnergyEffect(Transform parent = null)
		{
			if (parent == null)
				parent = CameraCanvas;
			buyEnergyBtn.gameObject.SetActive(false);
			GameObject energyProp = Instantiate(unlimitedEnergyPropPrefab,
				CameraCanvas.position + new Vector3(0, 0, -1f), Quaternion.identity, parent);
			Vector3 target = energyProp.transform.position + new Vector3(Random.Range(-1.5f, 1.5f),
				Random.Range(-1.5f, 1.5f), 0);

			energyProp.transform.DOMove(target, 0.5f).SetEase(Ease.OutQuint).OnComplete(() =>
			{
				energyProp.transform.DOMove(_normalEnergyIcon.transform.position, 0.5f).SetEase(Ease.InQuad).OnComplete(
					() =>
					{
						StopAllCoroutines();
						DataController.Instance.UnlimitedEnergyTimeStamp =
							DataController.ConvertToUnixTime(DateTime.UtcNow);
						DataController.Instance.UnlimitedEnergyDuration -= CalculateTimePassUnlimitedEnergy();
						if (DataController.Instance.UnlimitedEnergyDuration > 0)
						{
							StartCoroutine(ReplenishEnergy());
						}

						SetHeartIcon();

						energyCount.gameObject.SetActive(DataController.Instance.UnlimitedEnergyDuration <= 0);
						Destroy(energyProp);
					});
			});
		}

		private void SetHeartIcon()
		{
			bool haveUnlimitedEnergy = DataController.Instance.UnlimitedEnergyDuration > 0;

			_normalEnergyIcon.SetActive(!haveUnlimitedEnergy);
			_unlimitedEnterIcon.SetActive(haveUnlimitedEnergy);
		}
	}
}