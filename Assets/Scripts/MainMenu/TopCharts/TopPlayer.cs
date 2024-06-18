using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.TopCharts
{
	public class TopPlayer : MonoBehaviour
	{
		[SerializeField] private Image _avatar;
		[SerializeField] private TextMeshProUGUI _name;
		[SerializeField] private TextMeshProUGUI _star;

		public void Init(PlayerData playerData)
		{
			_avatar.sprite = DataController.GetAvatarSprite(playerData.AvatarName);
			_name.text = playerData.Name;
			_star.text = playerData.Star.ToString();
		}
	}
}