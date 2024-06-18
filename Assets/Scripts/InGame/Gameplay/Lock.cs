using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using DG.Tweening;
using MainMenu.TopCharts;
using TMPro;
using UnityEngine;
using Utilities;

namespace InGame.Gameplay
{
	public class Lock : MonoBehaviour
	{
		[SerializeField] private TextMeshPro _text;
		[SerializeField] private List<Transform> _chains;
		[SerializeField] private List<GameObject> _glass;
		[SerializeField] private Rigidbody2D[] _glassFragments;
		[SerializeField] private RangeFloat _explosionForce;
		[SerializeField] private float _explosionRadius;

		private GameObject _currentGlass;

		public int Number
		{
			get => _number;
			set
			{
				_number = value;
				_text.text = _number.ToString();

				if (_number == 0)
					Shelf.IsLocked = false;
			}
		}

		private int _number;
		public Shelf Shelf;

		private void Start()
		{
			_currentGlass = _glass[_glass.Count - 1];
		}

		public void Init(int number, Shelf shelf)
		{
			Number = number;
			Shelf = shelf;

			int chainsRemove = _chains.Count - number;
			for (int i = 0; i < chainsRemove; i++)
			{
				int index = _chains.Count - 1;
				Transform chain = _chains[index];
				Destroy(chain.gameObject);
				_chains.RemoveAt(index);
			}

			for (int i = number; i < _glass.Count - 2; i++)
			{
				_glass.RemoveAt(i);
			}
		}

		public void ReduceLocksNumber()
		{
			if (Number <= 3)
			{
				int chainIndex = _chains.Count - 1;
				StartCoroutine(ChainsFallAnimation(_chains[chainIndex]));
				_chains.RemoveAt(chainIndex);
			}

			Number--;
			ChangeGlass(Number);

			if (Number == 0)
			{
				ApplyExplosionForce(_currentGlass.transform.position, _glassFragments, _explosionForce.GetRandomValue(),
					_explosionRadius);
			}
		}

		private void ChangeGlass(int index)
		{
			_currentGlass.SetActive(false);
			_currentGlass = _glass[index];
			_currentGlass.SetActive(true);
		}

		private IEnumerator ChainsFallAnimation(Transform chain)
		{
			chain.parent = null;
			yield return chain.DOShakePosition(0.2f, new Vector3(0.2f, 0.2f, 0), 50).SetEase(Ease.Linear)
				.WaitForCompletion();
			chain.DOMoveY(CameraController.BottomLeft.y - 3, 0.7f).SetEase(Ease.InSine);

			if (_number == 0)
				Destroy(gameObject);
		}

		private void ApplyExplosionForce(Vector2 centerPoint, Rigidbody2D[] rigidbodies, float explosionForce,
			float explosionRadius)
		{
			foreach (Rigidbody2D rb in rigidbodies)
			{
				rb.transform.parent = null;

				rb.bodyType = RigidbodyType2D.Dynamic;
				Vector2 direction = rb.position - centerPoint;
				float distance = direction.magnitude;
				direction.Normalize();
				float force = explosionForce * (1 - distance / explosionRadius);
				rb.AddForce(direction * force, ForceMode2D.Impulse);
			}
		}
	}
}