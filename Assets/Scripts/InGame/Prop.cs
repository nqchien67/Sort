using System;
using MainMenu.TopCharts;
using UnityEngine;
using Utilities;

namespace InGame
{
	public class Prop : MonoBehaviour
	{
		[SerializeField] private RangeFloat _speed;

		public void Init(Vector3 destination, Action onComplete = null)
		{
			float randomSpeed = _speed.GetRandomValue();
			StartCoroutine(CommonIEnumerator.IMove(gameObject, destination, randomSpeed, () =>
			{
				onComplete?.Invoke();
				Destroy(gameObject, 0.07f);
			}));
		}
	}
}