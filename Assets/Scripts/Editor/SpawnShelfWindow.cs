using InGame.Gameplay;
using UnityEditor;
using UnityEngine;

public class SpawnShelfWindow : EditorWindow
{
	private const int _rowCount = 5;
	private const int _columnCount = 3;

	private bool[,] toggleStates = new bool[_columnCount, _rowCount];

	private GameObject parentObject;
	private GameObject prefab;

	private float _cellSizeX = 1f;
	private float _cellSizeY = 1f;
	private Vector2 _parentPos;

	private Sprite[] _centerSprites;
	private Sprite[] _leftSprites;
	private Sprite[] _rightSprites;

	[MenuItem("Tools/Spawn Shelf")]
	public static void ShowWindow()
	{
		GetWindow(typeof(SpawnShelfWindow));
	}

	private void OnGUI()
	{
		for (int y = _rowCount - 1; y >= 0; y--)
		{
			EditorGUILayout.BeginHorizontal();

			for (int x = 0; x < _columnCount; x++)
				toggleStates[x, y] = EditorGUILayout.Toggle(toggleStates[x, y]);

			EditorGUILayout.EndHorizontal();
		}

		parentObject =
			EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true) as GameObject;

		prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false) as GameObject;

		_cellSizeX = EditorGUILayout.FloatField("Cell Size X", _cellSizeX);
		_cellSizeY = EditorGUILayout.FloatField("Cell Size Y", _cellSizeY);

		_parentPos = EditorGUILayout.Vector2Field("Parent Position", _parentPos);

		EditorPrefs.SetFloat("CellSizeX", _cellSizeX);
		EditorPrefs.SetFloat("CellSizeY", _cellSizeY);
		EditorPrefs.SetFloat("ParentPositionX", _parentPos.x);
		EditorPrefs.SetFloat("ParentPositionY", _parentPos.y);
		
		if (prefab != null)
		{
			string prefabPath = AssetDatabase.GetAssetPath(prefab);
			EditorPrefs.SetString("Prefab", prefabPath);
		}
		else
		{
			EditorPrefs.SetString("Prefab", "");
		}

		if (GUILayout.Button("Spawn Objects"))
		{
			SpawnObjects();
		}
	}

	private void OnEnable()
	{
		_cellSizeX = EditorPrefs.GetFloat("CellSizeX", 1f);
		_cellSizeY = EditorPrefs.GetFloat("CellSizeY", 1f);
		_parentPos.x = EditorPrefs.GetFloat("ParentPositionX", 1f);
		_parentPos.y = EditorPrefs.GetFloat("ParentPositionY", 1f);

		string prefabPath = EditorPrefs.GetString("Prefab", "");
		if (!string.IsNullOrEmpty(prefabPath))
		{
			prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
		}

		Shelf shelf = prefab.GetComponent<Shelf>();
		_centerSprites = shelf._centerSprites;
		_leftSprites = shelf._leftSprites;
		_rightSprites = shelf._rightSprites;
	}

	private void SpawnObjects()
	{
		if (parentObject == null)
		{
			parentObject = GameObject.Find("Shelves");
			parentObject.transform.position = _parentPos;
		}

		DestroyAllChildren();

		if (prefab != null)
		{
			for (int y = 0; y < _rowCount; y++)
			for (int x = 0; x < _columnCount; x++)
			{
				if (!toggleStates[x, y])
					continue;

				var cell = new Vector2Int(x, y);
				GameObject obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

				if (parentObject != null)
					obj.transform.SetParent(parentObject.transform);

				obj.transform.position = ConvertWorldPosition(cell);
				SetShelfSprite(obj, cell);
				Undo.RegisterCreatedObjectUndo(obj, "Create Object");
			}
		}
		else
		{
			Debug.LogWarning("Please select a Prefab to spawn.");
		}
	}

	private void SetShelfSprite(GameObject shelf, Vector2Int cell)
	{
		var renderer = shelf.GetComponent<SpriteRenderer>();
		Sprite[] sprites = cell.x switch
		{
			0 => _leftSprites,
			1 => _centerSprites,
			_ => _rightSprites
		};

		if (cell.y <= 4)
			renderer.sprite = sprites[4 - cell.y];
		else
			renderer.sprite = sprites[1];
	}

	private Vector3 ConvertWorldPosition(Vector2Int cellPos)
	{
		var transform = parentObject.transform;

		var result = new Vector3(cellPos.x * _cellSizeX, cellPos.y * _cellSizeY, cellPos.y * -0.005f);

		if (cellPos.x == 1)
			result.z += -0.002f;

		result += transform.position + 0.5f * new Vector3(_cellSizeX, _cellSizeY);
		return result;
	}

	public bool IsOutsideBound(Vector2Int cell)
	{
		return cell.x < 0 || cell.x >= _columnCount || cell.y < 0 || cell.y >= _rowCount;
	}

	private void DestroyAllChildren()
	{
		foreach (Transform child in parentObject.transform)
		{
			DestroyImmediate(child.gameObject);
		}
	}
}