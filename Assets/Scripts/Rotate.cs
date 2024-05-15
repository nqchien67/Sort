using UnityEngine;

public class Rotate : MonoBehaviour
{
	private void Update()
	{
		transform.Rotate(Vector3.back, Time.deltaTime * 100);
	}
}