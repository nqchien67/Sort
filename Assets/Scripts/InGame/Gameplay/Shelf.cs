using System.Collections.Generic;
using Controllers;
using UnityEngine;

namespace InGame.Gameplay
{
	public class Shelf : MonoBehaviour
	{
		public List<ItemLayer> Layers = new List<ItemLayer>();

		public Sprite[] _centerSprites;
		public Sprite[] _leftSprites;
		public Sprite[] _rightSprites;

		[SerializeField] private float _centerY;

		[HideInInspector] public Lock Lock;
		[HideInInspector] public bool IsLocked;
		private BoxCollider2D _boxCollider;

		private SpriteRenderer _renderer;

		public SpriteRenderer Renderer
		{
			get
			{
				if (_renderer != null)
					return _renderer;
				_renderer = GetComponent<SpriteRenderer>();
				return _renderer;
			}
		}

		public Vector3 Position
		{
			get => transform.position;
			set => transform.position = value;
		}

		public ItemLayer FrontLayer => Layers[Layers.Count - 1];

		private void Awake()
		{
			_boxCollider = GetComponent<BoxCollider2D>();
		}

		public void RenderLayers()
		{
			if (Layers.Count == 0)
				return;

			var frontLayer = FrontLayer;
			frontLayer.Active(true);
			frontLayer.LocalPosition = new Vector3(0, _centerY, 0);

			for (int i = Layers.Count - 2; i >= 0; i--)
			{
				var layer = Layers[i];

				int order = Layers.Count - 1 - i;
				layer.LocalPosition = new Vector3(0, _centerY + order * 0.4f, order * 0.1f);
				layer.Active(false);

				layer.gameObject.SetActive(i >= Layers.Count - 2);
			}
		}

		public bool CanTakeItem()
		{
			return Layers[Layers.Count - 1].CanReceiveItem() && !IsLocked;
		}

		public void ReceiveItem(Item item)
		{
			int[] desiredIndexes = CalculateDesiredIndexes(item);

			if (Layers[Layers.Count - 1].CanReceiveItem())
				Layers[Layers.Count - 1].StartReceiveItem(item, desiredIndexes);
		}

		private int[] CalculateDesiredIndexes(Item item)
		{
			int[] desiredIndexes = new int[3];

			var bounds = _boxCollider.bounds;
			float minX = bounds.min.x;
			float maxX = bounds.max.x;

			float lerpValue = Mathf.InverseLerp(minX, maxX, item.Position.x);

			if (lerpValue <= 1 / 3f)
			{
				desiredIndexes[0] = 0;
				desiredIndexes[1] = 1;
				desiredIndexes[2] = 2;
			}
			else if (lerpValue > 1 / 3f && lerpValue <= 2 / 3f)
			{
				desiredIndexes[0] = 1;
				if (lerpValue < 0.5f)
				{
					desiredIndexes[1] = 0;
					desiredIndexes[2] = 2;
				}
				else
				{
					desiredIndexes[1] = 2;
					desiredIndexes[2] = 0;
				}
			}
			else
			{
				desiredIndexes[0] = 2;
				desiredIndexes[1] = 1;
				desiredIndexes[2] = 0;
			}

			return desiredIndexes;
		}

		public ItemLayer GetLayerAtIndex(int index)
		{
			return Layers.Count <= index ? AddNewLayer() : Layers[index];
		}

		public virtual void RemoveFrontLayer()
		{
			Layers.RemoveAt(Layers.Count - 1);
		}

		public void RemoveLayer(ItemLayer layer)
		{
			Layers.Remove(layer);
		}

		public ItemLayer AddNewLayer()
		{
			ItemLayer layer = Instantiate(LevelController.Instance.ItemLayerPrefab, transform);
			layer.LocalPosition = new Vector3(0, _centerY, 0);

			layer.Init(this);
			layer.gameObject.name = Layers.Count.ToString();
			Layers.Add(layer);

			return layer;
		}

		public void SetUpLock(int lockNumber)
		{
			if (lockNumber <= 0)
				return;
			Lock = Instantiate(LevelController.Instance.LockPrefab, transform);
			Lock.Init(lockNumber, this);
			IsLocked = true;
		}

		public List<Item> GetAllItems()
		{
			List<Item> items = new List<Item>();
			foreach (var layer in Layers) items.AddRange(layer.GetAllItems());

			return items;
		}

		public void RefreshItemsPos()
		{
			foreach (var l in Layers) l.RefreshItemPos();
		}
	}
}