using System;
using System.Collections;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
	public class SceneController : SingletonCore<SceneController>
	{
		private const string LoadingScene = "LoadingScene";

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		public void LoadScene(string sceneName, bool saveData = true)
		{
			if (saveData)
				DataController.Instance.SaveData(false);

			StartCoroutine(LoadSceneRoutine(sceneName));
		}

		private IEnumerator LoadSceneRoutine(string sceneName)
		{
			string currentScene = SceneManager.GetActiveScene().name;
			AsyncOperation loadTask = SceneManager.LoadSceneAsync(LoadingScene, LoadSceneMode.Additive);

			while (!loadTask.isDone)
				yield return null;

			LoadingSceneController loadingSceneController = FindObjectOfType<LoadingSceneController>();
			yield return loadingSceneController.FadeIn();
			var unloadTask = SceneManager.UnloadSceneAsync(currentScene);
			loadTask = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			loadTask.allowSceneActivation = false;

			yield return new WaitForSecondsRealtime(0.5f);

			yield return new WaitUntil(() => unloadTask.isDone && loadTask.progress >= 0.85f);
			yield return loadingSceneController.FadeOut();

			loadTask.allowSceneActivation = true;
			yield return new WaitUntil(() => !loadTask.isDone);

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
			yield return null;
			
			SceneManager.UnloadSceneAsync(LoadingScene);
		}
	}
}