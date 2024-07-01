using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Object = UnityEngine.Object;

namespace MainMenu.TopCharts
{
	public class PlayerCardListInWinPanel : MonoBehaviour
	{
		private ScrollRect _scrollRect;
		private RectTransform _content;
		[SerializeField] private PlayerCard _prototypeCell;

		private List<PlayerData> PlayersData => TopChartsDataManager.Instance.PlayersData;
		private List<RectTransform> _spawnedCells = new List<RectTransform>();
		private RectTransform _userCell;

		private IEnumerator Start()
		{
			_scrollRect = GetComponent<ScrollRect>();
			_content = _scrollRect.content;

			int rankUser = PlayerPrefs.GetInt("RankUser");

			if (rankUser > 3)
			{
				SpawnCards(rankUser);
				yield return null;
				ScrollToUser();
			}
			else
				SpawnFirst25Cards();
		}

		private void SpawnFirst25Cards()
		{
			_prototypeCell.gameObject.SetActive(true);
			SetTopAnchor(_prototypeCell.GetComponent<RectTransform>());

			const int poolSize = 25;

			for (int i = 0; i < poolSize; i++)
			{
				RectTransform item = SpawnCell();

				PlayerData playerData = TopChartsDataManager.Instance.DisplayPlayersData[i];
				var playerCard = item.GetComponent<PlayerCard>();
				playerCard.InitItem(playerData.Name, playerData.Rank, playerData.AvatarName, playerData.Star);

				_spawnedCells.Add(item);
			}
		}

		private void SpawnCards(int centerIndex)
		{
			_prototypeCell.gameObject.SetActive(true);
			SetTopAnchor(_prototypeCell.GetComponent<RectTransform>());

			//Temps
			int spawnedCellCount = 0;
			const int poolSize = 25;

			SpawnCenterCell();

			// create cells around the center index
			int indexAbove = centerIndex + 1;
			int indexBelow = centerIndex - 1;
			bool toggle = true; // to alternate between above and below

			while (spawnedCellCount < poolSize)
			{
				int currentIndex;
				if (toggle)
				{
					currentIndex = indexAbove;
					indexAbove++;
				}
				else
				{
					currentIndex = indexBelow;
					indexBelow--;
				}

				toggle = !toggle;

				if (currentIndex < 3 || currentIndex >= PlayersData.Count)
				{
					continue;
				}

				RectTransform item = SpawnCell();

				PlayerData playerData = PlayersData[currentIndex];
				var playerCard = item.GetComponent<PlayerCard>();
				playerCard.InitItem(playerData.Name, playerData.Rank, playerData.AvatarName, playerData.Star);

				// _cachedCells.Add(playerCard);

				spawnedCellCount++;
				_spawnedCells.Add(item);
			}

			SetCellsSiblingIndex();

			//Deactivate prototype cell if it is not a prefab(i.e it's present in scene)
			if (_prototypeCell.gameObject.scene.IsValid())
				_prototypeCell.gameObject.SetActive(false);

			void SpawnCenterCell()
			{
				_userCell = SpawnCell("User");
				_spawnedCells.Add(_userCell);

				PlayerData centerPlayerData = PlayersData[centerIndex];
				var centerPlayerCard = _userCell.GetComponent<PlayerCard>();

				centerPlayerCard.InitItem(centerPlayerData.Name, centerPlayerData.Rank, centerPlayerData.AvatarName,
					centerPlayerData.Star);

				spawnedCellCount++;
			}
		}

		private RectTransform SpawnCell(string name = "Cell")
		{
			RectTransform item = Instantiate(_prototypeCell).GetComponent<RectTransform>();
			item.name = name;
			item.SetParent(_content, false);
			return item;
		}

		private void SetCellsSiblingIndex()
		{
			_spawnedCells.Sort((x, y) =>
				x.GetComponent<PlayerCard>()._rank
					.CompareTo(y.GetComponent<PlayerCard>()._rank));
			
			for (int i = 0; i < _spawnedCells.Count; i++)
			{
				var item = _spawnedCells[i];
				item.SetSiblingIndex(i);
			}
		}

		private void ScrollToUser()
		{
			StartCoroutine(_scrollRect.FocusOnItemCoroutine(_userCell, 1));
		}

		private void SetTopAnchor(RectTransform rectTransform)
		{
			rectTransform.anchorMin = new Vector2(0.5f, 1);
			rectTransform.anchorMax = new Vector2(0.5f, 1);
			rectTransform.pivot = new Vector2(0.5f, 1);
		}
	}
}