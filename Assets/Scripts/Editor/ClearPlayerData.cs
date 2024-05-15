using System.IO;
using UnityEditor;
using UnityEngine;

	public class ClearPlayerData : MonoBehaviour
	{
		[MenuItem("Tools/Clear Data")]
		private static void Clear()
		{
			PlayerPrefs.DeleteAll();
			string[] filePaths = Directory.GetFiles(Application.persistentDataPath);
			foreach (string filePath in filePaths)
				if (filePath.Contains(".dat"))
					File.Delete(filePath);
		}
	}
