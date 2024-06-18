using System;
using System.Collections;
using System.Collections.Generic;
using Boosters.InGame;
using Controllers;
using Data;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;


namespace Boosters
{
	public class StartBoosterController : MonoBehaviour
	{
		[Serializable]
		public enum StartBooster
		{
			HugeHammer,
			Time,
			DoublePoint,
		}

		public static StartBoosterController Instance;
		public List<StartBooster> SelectedBoosters = new List<StartBooster>();

		[SerializeField] private SkeletonAnimation _hugeHammerEffect;
		[SerializeField] private SkeletonAnimation _timeEffect;
		[SerializeField] private SkeletonAnimation _doublePointEffect;

		[SerializeField] private Transform _timeIconPrefab;
		[SerializeField] private int _addTimeAmount = 60;
		[SerializeField] private Transform _doublePointExplosionEffect;
		[SerializeField] private Transform _timeExplosionEffect;

		private void Awake()
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		public void SelectBooster(StartBooster booster)
		{
			SelectedBoosters.Add(booster);
		}

		public IEnumerator ActiveBooster()
		{
			if (SelectedBoosters.Count == 0)
				yield break;

			foreach (StartBooster booster in SelectedBoosters)
			{
				switch (booster)
				{
					case StartBooster.HugeHammer:
						DataController.Instance.AddBooster(BoosterType.HugeHammer, -1);
						yield return StartCoroutine(HugeHammer());
						break;
					case StartBooster.Time:
						DataController.Instance.AddBooster(BoosterType.Time, -1);
						yield return StartCoroutine(Time());
						break;
					case StartBooster.DoublePoint:
						DataController.Instance.AddBooster(BoosterType.DoublePoint, -1);
						yield return StartCoroutine(DoublePoint());
						break;
				}
			}

			SelectedBoosters.Clear();
			LevelController.Instance.CanDrag = true;
		}

		public void DeselectBooster(StartBooster booster)
		{
			SelectedBoosters.Remove(booster);
		}

		private IEnumerator Time()
		{
			yield return StartCoroutine(PlayTimeEffect());

			int startTime = LevelController.Instance.LevelTime;
			LevelController.Instance.LevelTime += _addTimeAmount;
			for (int i = 0; i < _addTimeAmount; i++)
			{
				yield return null;
				LevelUIController.Instance.RenderTimer(startTime + i);
			}
		}

		private IEnumerator DoublePoint()
		{
			LevelController.Instance.DoubleStar = true;
			var doublePointEffect = SpawnEffect(_doublePointEffect, Vector3.zero);

			yield return new WaitForSeconds(0.5f);
			yield return doublePointEffect.transform.DOMove(LevelUIController.Instance.StarIcon.position, 1f)
				.WaitForCompletion();

			LevelUIController.Instance.StarIcon.gameObject.SetActive(false);
			LevelUIController.Instance.StarX2Icon.gameObject.SetActive(true);

			Instantiate(_doublePointExplosionEffect, doublePointEffect.transform.position, Quaternion.identity);
			yield return new WaitForSeconds(0.02f);
			Destroy(doublePointEffect.gameObject);
		}

		private IEnumerator HugeHammer()
		{
			Vector2 spawnPos = new Vector2(CameraController.TopRight.x - 3, 0);
			var effect = SpawnEffect(_hugeHammerEffect, spawnPos);
			bool animCompleted = false;
			effect.AnimationState.Complete += entry => animCompleted = true;

			LittleHammer littleHammer = FindObjectOfType<LittleHammer>();
			for (int i = 0; i < 3; i++)
			{
				yield return littleHammer.CollectItems(false);
			}

			yield return new WaitUntil(() => animCompleted);
			yield return effect.transform.DOMoveX(CameraController.BottomLeft.x - 4, 0.4f).WaitForCompletion();
			Destroy(effect.gameObject);
		}

		private IEnumerator PlayTimeEffect()
		{
			var effect = SpawnEffect(_timeEffect, Vector3.zero);
			bool animCompleted = false;

			effect.AnimationState.Complete += entry => animCompleted = true;
			yield return new WaitUntil(() => animCompleted);

			Destroy(effect.gameObject);
			Transform timeIcon = Instantiate(_timeIconPrefab, Vector3.zero, Quaternion.identity);
			yield return timeIcon.DOMove(LevelUIController.Instance.ClockIcon.position, 1f).WaitForCompletion();

			Instantiate(_timeExplosionEffect, timeIcon.position, Quaternion.identity);
			Destroy(timeIcon.gameObject);
		}

		private SkeletonAnimation SpawnEffect(SkeletonAnimation effectPrefab, Vector3 position)
		{
			return Instantiate(effectPrefab, position, Quaternion.identity)
				.GetComponent<SkeletonAnimation>();
			// _spawnedEffect.AnimationState.Complete += OnAnimationComplete;
			// return _spawnedEffect;
		}
	}
}