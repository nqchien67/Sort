using Controllers;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
	public class ChangeSpriteForHardLevel : MonoBehaviour
	{
		[SerializeField] private Sprite _hardSprite;

		private void Start()
		{
			if (MainMenuController.Instance != null && MainMenuController.Instance.HardLevelComing)
				GetComponent<Image>().sprite = _hardSprite;
			else if (LevelController.Instance != null && LevelController.Instance.LevelData.IsHardLevel())
				GetComponent<Image>().sprite = _hardSprite;
		}
	}
}