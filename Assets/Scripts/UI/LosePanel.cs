using System;
using Boosters.Start;
using Controllers;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
	public class LosePanel : MonoBehaviour
	{
		[SerializeField] private StartBoosterButton[] _startBoosterButtons;
		[SerializeField] private TextMeshProUGUI _levelText;
		private Animator _animator;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		public void Show()
		{
			gameObject.SetActive(true);
			_animator.Play("Appear");
		}

		public void OnClickCLose()
		{
			_animator.Play("Disappear");
			// Destroy(gameObject, 0.25f);
			StartCoroutine(GameManager.WaiForSeconds(0.5f, () => SceneManager.LoadScene("MainScene")));
		}

		public void OnCLickPlay()
		{
			LevelController.Instance.Replay();
		}
		
		public void OnClickPreGiftButton()
		{
			Debug.Log("Show reward video");
			
			foreach (var boosterButton in _startBoosterButtons)
			{
				DataController.Instance.AddConsumable(boosterButton.consumableType, 1);
				boosterButton.RefreshAmountText();
			}
		}
	}
}