using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MainMenu
{
	public class CommonTutorial : MonoBehaviour
	{
		[SerializeField] private Transform _target;
		[SerializeField] private UnityEvent _firstAction;

		[SerializeField] private RectTransform _targetCircle;
		[SerializeField] private RectTransform _arrow;
		[SerializeField] private RectTransform _select;

		private void Awake()
		{
			if (PlayerPrefs.GetInt(gameObject.name, 0) == 1)
				Destroy(gameObject);
			else
				PointToTarget();
		}

		public void OnClickFirst()
		{
			_firstAction?.Invoke();
			Destroy(gameObject);
			PlayerPrefs.SetInt(gameObject.name, 1);
		}

		private void PointToTarget()
		{
			_targetCircle.position = _target.position;
			_arrow.position = _target.position;
			_select.position = _target.position;
		}
	}
}