using System;
using Controllers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(LevelUIController))]
public class LevelUiEditor : Editor
{
	private LevelUIController script;

	private void Awake()
	{
		script = (LevelUIController)target;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (GUILayout.Button("Set canvas camera"))
		{
			script.Canvas.GetComponent<Canvas>().worldCamera = Camera.main;
			script.PopupCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

			EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
		}
	}
}