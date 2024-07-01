using UnityEngine;

public interface IDraggable
{
	public void OnStartDrag();
	public void OnEndDrag();
	public bool CanDrag();
	public Vector2 ClampDragZone(Vector2 position);
}