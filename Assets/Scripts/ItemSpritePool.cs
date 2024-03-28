using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpritePool : MonoBehaviour
{
	[SerializeField] private List<Sprite> _pool;
	public List<Sprite> _unplacedItems;
	
	public void InitUnplacedItems(int totalItemTypes)
	{
		_unplacedItems = new List<Sprite>();
		for (int i = 0; i < totalItemTypes; i++)
			_unplacedItems.Add(GetRandomItem());
	}

	public Sprite GetNextUnplacedItem()
	{
		if (_unplacedItems.Count == 0)
			return null;

		Sprite sprite = _unplacedItems[0];
		_unplacedItems.RemoveAt(0);
		return sprite;
	}

	private Sprite GetRandomItem()
	{
		int randomIndex = Random.Range(0, _pool.Count);

		Sprite result = _pool[randomIndex];
		_pool.RemoveAt(randomIndex);

		return result;
	}
}