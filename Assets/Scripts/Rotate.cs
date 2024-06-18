using UnityEngine;

public class Rotate : MonoBehaviour
{
	[SerializeField] private float _speed = 100;
	
	private void Update()
	{
		transform.Rotate(Vector3.back, Time.deltaTime * _speed);
	}
}