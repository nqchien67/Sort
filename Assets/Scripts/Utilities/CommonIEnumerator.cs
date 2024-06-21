using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Utilities
{
	public class CommonIEnumerator
	{
		public static IEnumerator WaitForEndOfFrame(Action action)
		{
			yield return new WaitForEndOfFrame();
			action.Invoke();
		}

		public static IEnumerator WaitForFrames(int frames, Action action)
		{
			for (int i = 0; i < frames; i++)
				yield return null;

			action.Invoke();
		}

		public static IEnumerator WaiForSeconds(float seconds, Action action)
		{
			yield return new WaitForSeconds(seconds);
			action.Invoke();
		}

		public static IEnumerator WaitUntil(Func<bool> predicate, Action action)
		{
			yield return new WaitUntil(predicate);
			action.Invoke();
		}

		public static IEnumerator Wait(YieldInstruction yieldInstruction, Action action)
		{
			yield return yieldInstruction;
			action.Invoke();
		}

		public static IEnumerator IMove(GameObject gameObject, Vector2 destination, float speed,
			Action onComplete = null)
		{
			float time = 0;
			Vector2 middlePos = new Vector2(
				(gameObject.transform.position.x + destination.x) / 2f + Random.Range(-6f, 6f),
				(gameObject.transform.position.y + destination.y) / 3f);
			
			Vector2 tempPos = gameObject.transform.position;
			while (Vector2.Distance(gameObject.transform.position, destination) > 0.3f)
			{
				gameObject.transform.position = CalculateQuadraticBezierPoint(time, tempPos, middlePos, destination);
				time += Time.deltaTime * speed * 2;
				yield return null;
			}

			onComplete?.Invoke();

			yield return new WaitForSeconds(0.05f);
			Object.Destroy(gameObject);
		}
		
		private static Vector3 CalculateQuadraticBezierPoint(float t1, Vector3 p0, Vector3 p1, Vector3 p2)
		{
			float u = 1 - t1;
			float tt = t1 * t1;
			float uu = u * u;
			Vector3 p = uu * p0;
			p += 2 * u * t1 * p1;
			p += tt * p2;
			return p;
		}
	}
}