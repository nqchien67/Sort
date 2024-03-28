using System;
using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
	public class LosePanel : MonoBehaviour
	{
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
	}
}