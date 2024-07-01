//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com 

using System;
using System.Collections;
using System.Collections.Generic;
using MainMenu.TopCharts;
using UnityEngine;
using Utilities;
using Object = UnityEngine.Object;

namespace PolyAndCode.UI
{
	/// <summary>
	///     Recyling system for Vertical type.
	/// </summary>
	public class VerticalRecyclingSystem : RecyclingSystem
	{
		//Assigned by constructor
		private readonly int _coloumns;

		//Cell dimensions
		private float _cellWidth, _cellHeight;

		//Pool Generation
		private List<RectTransform> _cellPool;
		private List<PlayerCard> _cachedCells;
		private Bounds _recyclableViewBounds;

		//Temps, Flags 
		private readonly Vector3[] _corners = new Vector3[4];
		private bool _recycling;

		//Trackers
		private int currentItemCount; //item count corresponding to the datasource.
		private int topMostCellIndex, bottomMostCellIndex; //Topmost and bottommost cell in the heirarchy

		private int
			_topMostCellColoumn,
			_bottomMostCellColoumn; // used for recyling in Grid layout. top-most and bottom-most coloumn

		//Cached zero vector 
		private readonly Vector2 zeroVector = Vector2.zero;

		private float _cellDistance = 20;
		private List<PlayerData> PlayersData => TopChartsDataManager.Instance.DisplayPlayersData;

		#region INIT

		public VerticalRecyclingSystem(RectTransform prototypeCell, RectTransform viewport, RectTransform content,
			IRecyclableScrollRectDataSource dataSource, bool isGrid, int coloumns)
		{
			PrototypeCell = prototypeCell;
			Viewport = viewport;
			Content = content;
			DataSource = dataSource;
			IsGrid = isGrid;
			_coloumns = isGrid ? coloumns : 1;
			_recyclableViewBounds = new Bounds();
		}

		/// <summary>
		///     Corotuine for initiazation.
		///     Using coroutine for init because few UI stuff requires a frame to update
		/// </summary>
		/// <param name="onInitialized">callback when init done</param>
		/// <returns></returns>
		/// >
		public override IEnumerator InitCoroutine(Action onInitialized = null)
		{
			SetTopAnchor(Content);
			yield return null;
			SetRecyclingBounds();

			//Cell Poool
			CreateCellPool();
			topMostCellIndex = 0;
			bottomMostCellIndex = _cellPool.Count - 1;

			//Set content height according to no of rows
			int noOfRows = (int)Mathf.Ceil(_cellPool.Count / (float)_coloumns);
			float contentYSize = noOfRows * _cellHeight;
			ContentSize = new Vector2(Content.sizeDelta.x, contentYSize);

			// if (!TopChartsPlayerDataManager.Instance.isUpdateRank)
			Content.sizeDelta = ContentSize;

			SetTopAnchor(Content);

			onInitialized?.Invoke();
		}

		/// <summary>
		///     Sets the uppper and lower bounds for recycling cells.
		/// </summary>
		private void SetRecyclingBounds()
		{
			Viewport.GetWorldCorners(_corners);
			float threshHold = RecyclingThreshold * (_corners[2].y - _corners[0].y);
			_recyclableViewBounds.min = new Vector3(_corners[0].x, _corners[0].y - threshHold);
			_recyclableViewBounds.max = new Vector3(_corners[2].x, _corners[2].y + threshHold);
		}

		/// <summary>
		///     Creates cell Pool for recycling, Caches ICells
		/// </summary>
		private void CreateCellPool()
		{
			//Reseting Pool
			if (_cellPool != null)
			{
				_cellPool.ForEach(item => Object.Destroy(item.gameObject));
				_cellPool.Clear();
				_cachedCells.Clear();
			}
			else
			{
				_cachedCells = new List<PlayerCard>();
				_cellPool = new List<RectTransform>();
			}

			//Set the prototype cell active and set cell anchor as top 
			PrototypeCell.gameObject.SetActive(true);
			if (IsGrid)
				SetTopLeftAnchor(PrototypeCell);
			else
				SetTopAnchor(PrototypeCell);

			//Reset
			_topMostCellColoumn = _bottomMostCellColoumn = 0;

			//Temps
			float currentPoolCoverage = 0;
			int poolSize = 0;
			float posX = 0;
			float posY = 0;

			//set new cell size according to its aspect ratio
			_cellWidth = Content.rect.width / _coloumns;
			// _cellHeight = PrototypeCell.sizeDelta.y / PrototypeCell.sizeDelta.x * _cellWidth;
			_cellHeight = PrototypeCell.sizeDelta.y;

			//Get the required pool coverage and mininum size for the Cell pool
			float requiredCoverage = MinPoolCoverage * Viewport.rect.height;
			int minPoolSize = Math.Min(MinPoolSize, DataSource.GetItemCount());

			//create cells untill the Pool area is covered and pool size is the minimum required
			while ((poolSize < minPoolSize || currentPoolCoverage < requiredCoverage) &&
			       poolSize < DataSource.GetItemCount())
			{
				//Instantiate and add to Pool
				RectTransform item = Object.Instantiate(PrototypeCell.gameObject).GetComponent<RectTransform>();
				item.sizeDelta = new Vector2(_cellWidth, _cellHeight);
				_cellPool.Add(item);
				item.SetParent(Content, false);

				if (IsGrid)
				{
					posX = _bottomMostCellColoumn * _cellWidth;
					item.anchoredPosition = new Vector2(posX, posY);
					if (++_bottomMostCellColoumn >= _coloumns)
					{
						_bottomMostCellColoumn = 0;
						posY -= _cellHeight;
						currentPoolCoverage += item.rect.height;
					}
				}
				else
				{
					item.anchoredPosition = new Vector2(0, posY);
					posY = item.anchoredPosition.y - item.rect.height;
					currentPoolCoverage += item.rect.height;
				}

				PlayerData playerData = PlayersData[poolSize];
				item.GetComponent<PlayerCard>().InitItem(playerData.Name, playerData.Rank, playerData.AvatarName,
					playerData.Star);

				//Setting data for Cell
				_cachedCells.Add(item.GetComponent<PlayerCard>());
				// Debug.Log(_cachedCells.Count + "______________");
				// Debug.Log(_cachedCells.Count - 1 + "______________");
				// Debug.Log(_cachedCells);
				// Debug.Log(_cachedCells[0]);
				// DataSource.SetCell(_cachedCells[_cachedCells.Count - 1], poolSize);

				//Update the Pool size
				poolSize++;
			}

			currentItemCount = _cellPool.Count;

			//Deactivate prototype cell if it is not a prefab(i.e it's present in scene)
			if (PrototypeCell.gameObject.scene.IsValid()) PrototypeCell.gameObject.SetActive(false);

			// TopChartsController.Instance.panel.GetComponent<TopChartsPanel>().Move();
			// Object.FindObjectOfType<TopChartsPanel>().Move();
			// OnValueChangedListener(new Vector2(0, PlayerPrefs.GetInt("RankUser") * 10002));
		}

		private void CreateCellPool(int centerIndex)
		{
			Content.anchoredPosition = Vector3.zero;

			//Reseting Pool
			if (_cellPool != null)
			{
				_cellPool.ForEach(item => Object.Destroy(item.gameObject));
				_cellPool.Clear();
				_cachedCells.Clear();
			}
			else
			{
				_cachedCells = new List<PlayerCard>();
				_cellPool = new List<RectTransform>();
			}

			//Set the prototype cell active and set cell anchor as top 
			PrototypeCell.gameObject.SetActive(true);
			SetTopAnchor(PrototypeCell);

			//Reset
			_topMostCellColoumn = _bottomMostCellColoumn = 0;

			//Temps
			float currentPoolCoverage = 0;
			int poolSize = 0;
			float posX = 0;

			//set new cell size according to its aspect ratio
			_cellWidth = Content.rect.width / _coloumns;
			_cellHeight = PrototypeCell.sizeDelta.y;

			//Get the required pool coverage and minimum size for the Cell pool
			float requiredCoverage = MinPoolCoverage * Viewport.rect.height;
			int minPoolSize = Math.Min(MinPoolSize, DataSource.GetItemCount());

			SpawnCenterCell();

			// create cells around the center index
			int indexAbove = centerIndex + 1;
			int indexBelow = centerIndex - 1;
			bool toggle = true; // to alternate between above and below

			while ((poolSize < minPoolSize || currentPoolCoverage < requiredCoverage) &&
			       poolSize < DataSource.GetItemCount())
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

				if (currentIndex < 0 || currentIndex >= DataSource.GetItemCount())
				{
					continue; // skip invalid indices
				}

				//Instantiate and add to Pool
				RectTransform item = SpawnCell();

				currentPoolCoverage += item.rect.height;

				PlayerData playerData = PlayersData[currentIndex];
				var playerCard = item.GetComponent<PlayerCard>();
				playerCard.InitItem(playerData.Name, playerData.Rank, playerData.AvatarName, playerData.Star);

				_cachedCells.Add(playerCard);

				poolSize++;
			}

			SetCellsSiblingIndex();
			currentItemCount = indexAbove;

			//Deactivate prototype cell if it is not a prefab(i.e it's present in scene)
			if (PrototypeCell.gameObject.scene.IsValid()) PrototypeCell.gameObject.SetActive(false);

			// Object.FindObjectOfType<TopChartsPanel>().Move();

			void SpawnCenterCell()
			{
				RectTransform centerItem = SpawnCell("User");
				// centerItem.anchoredPosition = new Vector2(0, posY);
				// posY = centerItem.anchoredPosition.y - centerItem.rect.height;
				currentPoolCoverage += centerItem.rect.height;

				PlayerData centerPlayerData = PlayersData[centerIndex];
				var centerPlayerCard = centerItem.GetComponent<PlayerCard>();
				centerPlayerCard.InitItem(centerPlayerData.Name, centerPlayerData.Rank, centerPlayerData.AvatarName,
					centerPlayerData.Star);

				_cachedCells.Add(centerPlayerCard);
				poolSize++;
			}
		}

		private RectTransform SpawnCell(string name = "Cell")
		{
			RectTransform item = Object.Instantiate(PrototypeCell.gameObject)
				.GetComponent<RectTransform>();
			item.name = name;
			item.sizeDelta = new Vector2(_cellWidth, _cellHeight);
			_cellPool.Add(item);
			item.SetParent(Content, false);
			return item;
		}

		private void SetCellsSiblingIndex()
		{
			float posY = 0;

			_cellPool.Sort((x, y) =>
				x.GetComponent<PlayerCard>()._rank
					.CompareTo(y.GetComponent<PlayerCard>()._rank));

			for (int i = 0; i < _cellPool.Count; i++)
			{
				var item = _cellPool[i];

				item.SetSiblingIndex(i);
				item.anchoredPosition = new Vector2(0, posY);
				posY = item.anchoredPosition.y - item.rect.height;
			}
		}

		#endregion


		#region RECYCLING

		/// <summary>
		///     Recyling entry point
		/// </summary>
		/// <param name="direction">scroll direction </param>
		/// <returns></returns>
		public override Vector2 OnValueChangedListener(Vector2 direction)
		{
			if (_recycling || _cellPool == null || _cellPool.Count == 0) return zeroVector;

			//Updating Recyclable view bounds since it can change with resolution changes.
			SetRecyclingBounds();
			if (direction.y > 0 && _cellPool[bottomMostCellIndex].MaxY() > _recyclableViewBounds.min.y)
				return RecycleTopToBottom();
			if (direction.y < 0 && _cellPool[topMostCellIndex].MinY() < _recyclableViewBounds.max.y)
				return RecycleBottomToTop();

			return zeroVector;
		}

		/// <summary>
		///     Recycles cells from top to bottom in the List heirarchy
		/// </summary>
		private Vector2 RecycleTopToBottom()
		{
			_recycling = true;

			int n = 0;
			float posY = IsGrid ? _cellPool[bottomMostCellIndex].anchoredPosition.y : 0;
			float posX = 0;

			//to determine if content size needs to be updated
			int additionalRows = 0;
			//Recycle until cell at Top is avaiable and current item count smaller than datasource
			while (_cellPool[topMostCellIndex].MinY() > _recyclableViewBounds.max.y &&
			       currentItemCount < DataSource.GetItemCount())
			{
				posY = _cellPool[bottomMostCellIndex].anchoredPosition.y - _cellPool[bottomMostCellIndex].sizeDelta.y;
				_cellPool[topMostCellIndex].anchoredPosition =
					new Vector2(_cellPool[topMostCellIndex].anchoredPosition.x, posY);
				
				var playerData = PlayersData[currentItemCount];
				_cachedCells[topMostCellIndex].InitItem(playerData.Name, playerData.Rank,
					playerData.AvatarName, playerData.Star);

				_cachedCells[topMostCellIndex].gameObject.name = currentItemCount.ToString();

				//set new indices
				bottomMostCellIndex = topMostCellIndex;
				topMostCellIndex = (topMostCellIndex + 1) % _cellPool.Count;

				currentItemCount++;
				if (!IsGrid) n++;
			}

			//Content anchor position adjustment.
			Vector2 deltaPos = n * _cellPool[topMostCellIndex].sizeDelta.y * Vector2.up;

			_cellPool.ForEach(cell => cell.anchoredPosition += deltaPos);
			Content.anchoredPosition -= deltaPos;

			TopChartsController.Instance.Panel.UpdatePosContent(-deltaPos.y);

			_recycling = false;

			return -new Vector2(0, n * _cellPool[topMostCellIndex].sizeDelta.y);
		}

		/// <summary>
		///     Recycles cells from bottom to top in the List heirarchy
		/// </summary>
		private Vector2 RecycleBottomToTop()
		{
			_recycling = true;

			int n = 0;
			float posY = IsGrid ? _cellPool[topMostCellIndex].anchoredPosition.y : 0;
			float posX = 0;

			//to determine if content size needs to be updated
			int additionalRows = 0;
			//Recycle until cell at bottom is avaiable and current item count is greater than cellpool size
			while (_cellPool[bottomMostCellIndex].MaxY() < _recyclableViewBounds.min.y &&
			       currentItemCount > _cellPool.Count)
			{
				//Move bottom cell to top
				posY = _cellPool[topMostCellIndex].anchoredPosition.y + _cellPool[topMostCellIndex].sizeDelta.y;
				_cellPool[bottomMostCellIndex].anchoredPosition =
					new Vector2(_cellPool[bottomMostCellIndex].anchoredPosition.x, posY);
				n++;

				currentItemCount--;

				//Cell for row at
				// DataSource.SetCell(_cachedCells[bottomMostCellIndex], currentItemCount - _cellPool.Count);
				var playerData = PlayersData[currentItemCount - _cellPool.Count];
				_cachedCells[bottomMostCellIndex].InitItem(playerData.Name, playerData.Rank,
					playerData.AvatarName, playerData.Star);

				//set new indices
				topMostCellIndex = bottomMostCellIndex;
				bottomMostCellIndex = (bottomMostCellIndex - 1 + _cellPool.Count) % _cellPool.Count;
			}

			// if (!TopChartsPlayerDataManager.Instance.isUpdateRank)
			// {
			_cellPool.ForEach(cell =>
				cell.anchoredPosition -= n * _cellPool[topMostCellIndex].sizeDelta.y * Vector2.up);
			Content.anchoredPosition += n * _cellPool[topMostCellIndex].sizeDelta.y * Vector2.up;
			TopChartsController.Instance.Panel
				.UpdatePosContent((n * _cellPool[topMostCellIndex].sizeDelta.y * Vector2.up).y);
			// }

			_recycling = false;
			return new Vector2(0, n * _cellPool[topMostCellIndex].sizeDelta.y);
		}

		#endregion

		#region HELPERS

		/// <summary>
		///     Anchoring cell and content rect transforms to top preset. Makes repositioning easy.
		/// </summary>
		/// <param name="rectTransform"></param>
		private void SetTopAnchor(RectTransform rectTransform)
		{
			//Saving to reapply after anchoring. Width and height changes if anchoring is change. 
			float width = rectTransform.rect.width;
			float height = rectTransform.rect.height;

			//Setting top anchor 
			rectTransform.anchorMin = new Vector2(0.5f, 1);
			rectTransform.anchorMax = new Vector2(0.5f, 1);
			rectTransform.pivot = new Vector2(0.5f, 1);

			//Reapply size
			//rectTransform.sizeDelta = new Vector2(width, height);
		}

		private void SetTopLeftAnchor(RectTransform rectTransform)
		{
			//Saving to reapply after anchoring. Width and height changes if anchoring is change. 
			float width = rectTransform.rect.width;
			float height = rectTransform.rect.height;

			//Setting top anchor 
			rectTransform.anchorMin = new Vector2(0, 1);
			rectTransform.anchorMax = new Vector2(0, 1);
			rectTransform.pivot = new Vector2(0, 1);

			//Reapply size
			//rectTransform.sizeDelta = new Vector2(width, height);
		}

		#endregion

		#region TESTING

		public void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(_recyclableViewBounds.min - new Vector3(2000, 0),
				_recyclableViewBounds.min + new Vector3(2000, 0));
			Gizmos.color = Color.red;
			Gizmos.DrawLine(_recyclableViewBounds.max - new Vector3(2000, 0),
				_recyclableViewBounds.max + new Vector3(2000, 0));
		}

		#endregion
	}
}