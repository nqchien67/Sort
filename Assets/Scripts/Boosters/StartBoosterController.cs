using System;
using System.Collections;
using System.Collections.Generic;
using Boosters.InGame;
using Controllers;
using DG.Tweening;
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

		private void Awake()
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		public void SelectBooster(StartBooster booster)
		{
			SelectedBoosters.Add(booster);
		}

		public void ActiveBooster()
		{
			if (SelectedBoosters.Count == 0)
				return;

			foreach (var booster in SelectedBoosters)
			{
				switch (booster)
				{
					case StartBooster.HugeHammer:
						UseHugeHammer();
						break;
					case StartBooster.Time:
						UseTime();
						break;
					case StartBooster.DoublePoint:
						UseDoublePoint();
						break;
				}
			}
			
			SelectedBoosters.Clear();
		}

		public void DeselectBooster(StartBooster booster)
		{
			SelectedBoosters.Remove(booster);
		}

		public void UseHugeHammer()
		{
			StartCoroutine(HugeHammer());
		}

		public void UseTime()
		{
			LevelController.Instance.LevelTime += 60;
		}

		public void UseDoublePoint()
		{
			LevelController.Instance.DoubleStar = true;
		}

		private IEnumerator HugeHammer()
		{
			LittleHammer littleHammer = FindObjectOfType<LittleHammer>();
			for (int i = 0; i < 3; i++)
			{
				yield return littleHammer.CollectItems();
			}
		}
	}
}