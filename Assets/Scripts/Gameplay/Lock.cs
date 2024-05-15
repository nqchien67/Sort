using System;
using TMPro;
using UnityEngine;

public class Lock : MonoBehaviour
{
	private TextMeshPro _text;

	public int Number
	{
		get => _number;
		set
		{
			_number = value;
			_text.text = _number.ToString();
			
			if (_number == 0)
			{
				Shelf.IsLocked = false;
				Destroy(gameObject);
			}
		}
	}
// anh chiến ăn cứt 
	private int _number;
	public Shelf Shelf;

	private void Awake()
	{
		_text = GetComponentInChildren<TextMeshPro>();
	}

	public void Init(int number, Shelf shelf)
	{
		Number = number;
		Shelf = shelf;
	}

	public void Remove()
	{
		Destroy(gameObject);
	}
}