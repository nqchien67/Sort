#if UNITY_EDITOR
using System.Collections.Generic;
using InGame.Gameplay;
using UnityEditor;
using UnityEngine;

namespace Controllers
{
	[CustomEditor(typeof(LevelController))]
	public class LevelControllerEditor : Editor
	{
		private LevelController script;

		private void Awake()
		{
			script = (LevelController)target;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("Refresh Shelf list"))
			{
				RefreshShelfList();
			}
		}

		private void RefreshShelfList()
		{
			script.Shelves = FindObjectsOfType<Shelf>();
		}
	}
}
#endif