using System;
using UnityEngine;
using UnityEngine.Events;

namespace MainMenu
{
	public class CommonTutorial : MonoBehaviour
	{
		[SerializeField] private UnityEvent _firstAction;

		private void Start()
		{
			if (PlayerPrefs.GetInt(gameObject.name, 0) == 1)
			{
				Destroy(gameObject);
			}
		}

		public void OnClickFirst()
		{
			_firstAction?.Invoke();
			Destroy(gameObject);
			PlayerPrefs.SetInt(gameObject.name, 1);
		}
	}
}