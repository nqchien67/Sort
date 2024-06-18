using Controllers;
using UnityEngine;

namespace Shared
{
	public class ResizeCameraForVerticalScreen : MonoBehaviour
	{
		private void Start()
		{
			Camera cam = GetCamera();

			float screenRatio = cam.orthographicSize / (2400f / 1080f);
			float actualAspectRatio = Screen.height / (float)Screen.width;

			actualAspectRatio = Mathf.Max(1920 / 1080f, actualAspectRatio);

			cam.orthographicSize = screenRatio * actualAspectRatio;
		}

		private Camera GetCamera()
		{
			return CameraController.Camera != null
				? CameraController.Camera
				: Camera.main;
		}
	}
}