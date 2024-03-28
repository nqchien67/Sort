using System;
using System.Collections;
using UnityEngine;

public class GameManager : SingletonCore<MonoBehaviour>
{
	private void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	public static IEnumerator WaitForEndOfFrame(Action action )
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
}