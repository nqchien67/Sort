using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
	public class CameraController : SingletonCore<CameraController>
	{
		public static Camera Camera
		{
			get
			{
				if (_camera == null)
				{
					_camera = FindObjectOfType<Camera>();
					// if (_instance == null)
					// {
					//     GameObject obj = new GameObject();
					//     _instance = obj.AddComponent<T>();
					// }
				}

				return _camera;
			}
		}

		private static Camera _camera;

		public static Vector2 TopRight => Camera.ViewportToWorldPoint(Vector2.one);
		public static Vector2 BottomLeft => Camera.ViewportToWorldPoint(Vector2.zero);

		protected override void Awake()
		{
			base.Awake();
			_camera = GetComponent<Camera>();
		}

		public float rotationMultiplier = 7.5f;

		private Vector3 originPos;
		private float shakeFadeTime;
		private float shakePower;
		private float shakeRotation;

		private float shakeTimeRemaining;

		private void Start()
		{
			originPos = transform.position;
		}

		private void Update()
		{
			transform.position = originPos;
		}

		private void LateUpdate()
		{
			if (Time.timeScale == 0)
				return;
			
			if (shakeTimeRemaining > 0)
			{
				shakeTimeRemaining -= Time.deltaTime;

				var xAmount = Random.Range(-1f, 1f) * shakePower;
				var yAmount = Random.Range(-1f, 1f) * shakePower;

				transform.position += new Vector3(xAmount, yAmount, 0f);

				shakePower = Mathf.MoveTowards(shakePower, 0f, shakeFadeTime * Time.deltaTime);
				shakeRotation =
					Mathf.MoveTowards(shakeRotation, 0f, shakeFadeTime * rotationMultiplier * Time.deltaTime);
			}

			transform.rotation = Quaternion.Euler(0f, 0f, shakeRotation * Random.Range(-1f, 1f));
		}

		public void StartShake(float length, float shakePower)
		{
			if (shakePower > this.shakePower)
			{
				shakeTimeRemaining = length;
				this.shakePower = shakePower;
				shakeFadeTime = shakePower / length;
				shakeRotation = shakePower * rotationMultiplier;
			}
		}
	}
}