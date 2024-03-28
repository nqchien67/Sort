using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using DG.Tweening;
using UnityEngine;

public class ItemLayer : MonoBehaviour
{
	[SerializeField] private float _itemDistance;
	[SerializeField] private float _placeItemAnimDuration;

	public Item[] Items;

	public Vector3 Position
	{
		get => transform.position;
		set => transform.position = value;
	}

	public Vector3 LocalPosition
	{
		get => transform.localPosition;
		set => transform.localPosition = value;
	}

	public int ItemsCount => Items.Count(i => i != null);
	public Shelf Shelf;

	private void Awake()
	{
		Items = new Item[3];
	}

	public void Init(Shelf shelf)
	{
		Shelf = shelf;
	}

	private void RepositionItems()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			var child = transform.GetChild(i);
			Items[i] = child.GetComponent<Item>();
			child.localPosition = new Vector3(GetItemLocalPosX(i), 0);
		}
	}

	public bool CanTakeItem()
	{
		bool canTakeItem = Items.Any(i => i == null);
		return canTakeItem;
	}

	public void StartTakeItem(Item item, int[] desiredIndexes)
	{
		StartCoroutine(TakeItem(item, desiredIndexes));
	}

	private IEnumerator TakeItem(Item item, int[] desiredIndexes)
	{
		foreach (int index in desiredIndexes)
		{
			if (Items[index] != null)
				continue;

			yield return MoveItemToIndex(item, index);
			yield return null;
			StartCoroutine(CheckCorrect());
			yield break;
		}
	}

	public YieldInstruction MoveItemToIndex(Item item, int index)
	{
		item.Layer = this;
		Items[index] = item;

		return item.LocalMove(new Vector2(GetItemLocalPosX(index), 0), _placeItemAnimDuration);
	}

	public void PlaceItemAtIndex(Item item, int index)
	{
		if (Items[index] != null)
			Debug.LogError("Co item o day roi");

		item.Layer = this;
		Items[index] = item;

		item.LocalPosition = new Vector2(GetItemLocalPosX(index), 0);
	}

	private IEnumerator CheckCorrect()
	{
		for (int i = 0; i < Items.Length - 1; i++)
		{
			var item1 = Items[i];
			var item2 = Items[i + 1];
			if (item1 == null || item2 == null || item1.Type != item2.Type)
				yield break;
		}

		LevelController.Instance.RemainItemTypes.Remove(Items[0].Sprite);

		float duration = 0.3f;
		foreach (var item in Items)
			item.transform.DOScale(0, duration).SetEase(Ease.OutBack).OnComplete(() => Destroy(item.gameObject));

		yield return new WaitForSeconds(duration);
		yield return null;

		CheckShouldDestroy();

		LevelController.Instance.EatASet();
	}

	private static readonly List<int> indexes = new List<int>
	{
		0, 1, 2
	};

	// public void PlaceItemAtRandom(Item item)
	// {
	// 	// Items = new Item[GameController.Instance.ItemNumbEachLayer];
	//
	// 	List<int> shuffledIndexes = indexes.OrderBy(i => Random.value).ToList();
	// 	foreach (int index in shuffledIndexes)
	// 	{
	// 		if (Items[index] == null)
	// 			continue;
	//
	// 		PlaceItemAtIndex(item, index);
	// 		return;
	// 	}
	// }

	public void RemoveItem(Item item)
	{
		for (int i = 0; i < Items.Length; i++)
		{
			if (Items[i] == null || Items[i] != item) 
				continue;

			Items[i].transform.parent = null;
			Items[i] = null;
			return;
		}
	}

	public virtual void CheckShouldDestroy(bool reRenderShelf = true)
	{
		if (ItemsCount != 0 || Shelf.Layers.Count <= 1) return;

		Shelf.RemoveLayer(this);
		if (reRenderShelf)
			Shelf.RenderLayers();
		Destroy(gameObject);
	}

	private float GetItemLocalPosX(int index)
	{
		return index switch
		{
			0 => -_itemDistance,
			1 => 0,
			2 => _itemDistance,
			_ => 0
		};
	}

	public bool IsAlreadyHaveTwoOfThisType(Sprite itemSprite)
	{
		if (ItemsCount < 2)
			return false;

		foreach (var i in Items)
		{
			if (i != null && i.Type != itemSprite.name)
				return false;
		}

		return true;
	}

	public void Active(bool active)
	{
		if (active)
			gameObject.SetActive(true);

		foreach (var i in Items)
		{
			if (i == null)
				continue;

			i.Active(active);
		}
	}

	public List<Item> GetAllItems()
	{
		return Items.Where(item => item != null).ToList();
	}

	public Item GetRandomItem()
	{
		List<Item> list = Items.Where(item => item != null).ToList();
		return list[Random.Range(0, list.Count)];
	}
}