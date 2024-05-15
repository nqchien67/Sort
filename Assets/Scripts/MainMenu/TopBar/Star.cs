using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.TopBar
{
	public class Star : MonoBehaviour
	{
		public TextMeshProUGUI Text;
		public Image Icon;

		private Tween _updateTween;
		private Vector3 _originalScale;

		private int _value;

		public int Value
		{
			get => _value;
			set
			{
				_value = value;
			}
		}

		private void Start()
		{
			Text.text = DataController.Instance.Star.ToString();
			_originalScale = Icon.transform.localScale;
		}
	}
}