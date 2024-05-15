using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

public class Loading : SingletonCore<Loading>
{
	[SerializeField] private float _loadTime = 3;
	private bool clicked;

	public void Pause()
	{
		clicked = !clicked;
		Time.timeScale = clicked ? 0 : _startTimeScale;
	}

	private string _splashScene = "SplashScene";
	private string _level1 = "Level1";
	private string _mainScene = "MainScene";

	[SerializeField] private Image _fill;
	[SerializeField] private TextMeshProUGUI _loadingText;
	public TextMeshProUGUI verText;

	private float _fillMaxSizeX;
	private float _startTimeScale;
	private RectTransform _fillRectTransform;

	private void Start()
	{
		StartCoroutine(LoadAsyncScene());
		verText.text = Application.version;
		_fillRectTransform = _fill.GetComponent<RectTransform>();
		_fillMaxSizeX = _fillRectTransform.sizeDelta.x;

		StartCoroutine(LoadingTexAnimation());
	}

	private IEnumerator LoadAsyncScene()
	{
		bool isPassLevel1 = PlayerPrefs.HasKey("level");
		string sceneToLoad = isPassLevel1 ? _mainScene : _level1;

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

		asyncLoad.allowSceneActivation = false;
		yield return null;

		float elapsedTime = 0;
		while (elapsedTime <= _loadTime)
		{
			yield return new WaitForSecondsRealtime(1);
			FillLoadingbar(elapsedTime / _loadTime);
			elapsedTime++;
		}

		asyncLoad.allowSceneActivation = true;

		FillLoadingbar(1);
		while (!asyncLoad.isDone)
			yield return null;

		SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
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

	private IEnumerator LoadingTexAnimation()
	{
		int dotCount = 0;
		while (enabled)
		{
			dotCount++;
			if (dotCount > 3)
				dotCount = 0;

			string dots = "";
			for (int i = 0; i < dotCount; i++)
				dots += ".";

			_loadingText.text = "Loading" + dots;
			yield return new WaitForSeconds(0.3f);
		}
	}

	private void FillLoadingbar(float fillAmount)
	{
		var sizeDelta = _fillRectTransform.sizeDelta;
		sizeDelta.x = Mathf.Lerp(0, _fillMaxSizeX, fillAmount);
		_fillRectTransform.sizeDelta = sizeDelta;
	}
}