using Controllers;
using UnityEngine;

namespace Shared
{
	public class DestroyWhenOutOfScreen : MonoBehaviour
	{
		[SerializeField] private float padding;
		[SerializeField] private bool up = true;
		[SerializeField] private bool down = true;
		[SerializeField] private bool left = true;
		[SerializeField] private bool right = true;

		private void LateUpdate()
		{
			Vector3 position = transform.position;
			float x = position.x;
			float y = position.y;

			Vector2 topRight = CameraController.TopRight;
			Vector2 bottomLeft = CameraController.BottomLeft;

			bool checkUp = up && y > topRight.y + padding;
			bool checkDown = down && y < bottomLeft.y - padding;
			bool checkLeft = left && x < bottomLeft.x - padding;
			bool checkRight = right && x > topRight.x + padding;

			if (checkUp || checkDown || checkLeft || checkRight)
				Destroy(gameObject);
		}
	}
}