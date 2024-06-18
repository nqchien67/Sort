using System;
using Controllers;
using Data;
using IAP;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.EndlessTreasure
{
	public class EndlessTreasureController : MonoBehaviour
	{
		public Pack[] TreasurePacksData;

		[SerializeField] private Button _openPanelButton;
		[SerializeField] private EndlessTreasurePanel _panelPrefab;

		private void Start()
		{
			if (!CanOpenPack())
			{
				TreasurePacksData = LoadData();
				_openPanelButton.gameObject.SetActive(true);
			}
			else
				_openPanelButton.gameObject.SetActive(false);
		}

		public bool CanOpenPack()
		{
			return MainMenuController.Instance.HighestPassedLevel >= 12;
		}

		private Pack[] LoadData()
		{
			string data = FirebaseServiceController.Instance.GetEndlessTreasureData();
			var packsData = JsonUtility.FromJson<EndlessTreasurePacksData>(data);
			return packsData.TreasurePacksData;
		}

		private void OnClickOpenPanel()
		{
			EndlessTreasurePanel panel = Instantiate(_panelPrefab, MainMenuController.Instance.CameraCanvas);
			panel.Show(TreasurePacksData);
		}
	}

	[Serializable]
	public class EndlessTreasurePacksData
	{
		public Pack[] TreasurePacksData;
	}
}