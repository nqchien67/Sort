using UnityEngine;

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
	}
}