using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

public class Loading : SingletonCore<Loading>
{
	private bool clicked;

	public void Pause()
	{
		clicked = !clicked;
		Time.timeScale = clicked ? 0 : _startTimeScale;
	}

	public void DoneLoadRoad()
	{
		SceneManager.UnloadSceneAsync(_splashScene);
	}

	private string _splashScene = "SplashScene";
	private string _mainScene = "MainScene";

	public Image sliderBar;
	public TextMeshProUGUI verText;

	void Start()
	{
		StartCoroutine(LoadAsyncScene());
		verText.text = Application.version;
	}

	private float _startTimeScale;

	private IEnumerator LoadAsyncScene()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_mainScene, LoadSceneMode.Additive);

		asyncLoad.allowSceneActivation = false;
		yield return null;

		float elapsedTime = 0;
		const float loadTime = 3;
		while (elapsedTime <= loadTime)
		{
			yield return new WaitForSecondsRealtime(1);
			sliderBar.fillAmount = elapsedTime / loadTime;
			elapsedTime ++;
		}
		asyncLoad.allowSceneActivation = true;

		sliderBar.fillAmount = 1;
		while (!asyncLoad.isDone)
			yield return null;

		SceneManager.SetActiveScene(SceneManager.GetSceneByName(_mainScene));
		SceneManager.UnloadSceneAsync(_splashScene);

		// string currentScene = SceneManager.GetActiveScene().name;
		// var asyncTask = SceneManager.LoadSceneAsync(_mainScene, LoadSceneMode.Additive);
		// while (!asyncTask.isDone)
		// {
		// 	sliderBar.fillAmount = 0.5f + asyncTask.progress / 2;
		// 	yield return null;
		// }
		//
		// SceneManager.SetActiveScene(SceneManager.GetSceneByName(_mainScene));
		// sliderBar.fillAmount = 1;
		// SceneManager.UnloadSceneAsync(_splashScene);
	}
}