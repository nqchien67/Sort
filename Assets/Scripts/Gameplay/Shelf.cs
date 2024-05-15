using System.Collections.Generic;
using Controllers;
using Gameplay;
using UnityEngine;

public class Shelf : MonoBehaviour
{
	public List<ItemLayer> Layers = new List<ItemLayer>();
	private BoxCollider2D _boxCollider;
	protected SpriteRenderer _renderer;

	public Sprite[] _centerSprites;
	public Sprite[] _leftSprites;
	public Sprite[] _rightSprites;
	
	[SerializeField] private float _centerY;

	public Vector3 Position
	{
		get => transform.position;
		set => transform.position = value;
	}

	[HideInInspector] public Lock Lock;
	[HideInInspector] public bool IsLocked;

	public ItemLayer FrontLayer => Layers[Layers.Count - 1];

	private void Awake()
	{
		_boxCollider = GetComponent<BoxCollider2D>();
		_renderer = GetComponent<SpriteRenderer>();
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
			layer.LocalPosition = new Vector3(0, _centerY + order * 0.06f, order * 0.1f);
			layer.Active(false);

			layer.gameObject.SetActive(i >= Layers.Count - 2);
		}
	}

	public bool CanTakeItem()
	{
		return Layers[Layers.Count - 1].CanTakeItem() && !IsLocked;
	}

	public void TakeItem(Item item)
	{
		int[] desiredIndexes = CalculateDesiredIndexes(item);

		if (Layers[Layers.Count - 1].CanTakeItem())
			Layers[Layers.Count - 1].StartTakeItem(item, desiredIndexes);
	}
// anh chiến ăn cứt 
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
		// Debug.Log(gameObject.name + ": " + layer.gameObject.name);

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
		foreach (var layer in Layers)
		{
			items.AddRange(layer.GetAllItems());
		}

		return items;
	}
}