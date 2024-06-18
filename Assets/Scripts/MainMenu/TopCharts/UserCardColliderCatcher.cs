using System;
using UnityEngine;
using UnityEngine.Events;

namespace MainMenu.TopCharts
{
	[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
	public class UserCardColliderCatcher : MonoBehaviour
	{
		public UnityAction OnTriggerEnter;
		public UnityAction OnTriggerExit;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("UserTopChartsCard"))
				OnTriggerEnter?.Invoke();
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.CompareTag("UserTopChartsCard"))
				OnTriggerExit?.Invoke();
		}
	}
}