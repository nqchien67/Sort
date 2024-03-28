using System;
using System.Collections;
using System.Collections.Generic;
using Boosters.InGame;
using UnityEngine;
using UnityEngine.UI;

namespace Boosters.Start
{
	[RequireComponent(typeof(Button))]
	public class StartBoosterButton : MonoBehaviour
	{
		private bool _selected;
		private Image _image;
		private Button _button;

		[SerializeField] private StartBoosterController.StartBooster _booster;
		[SerializeField] private Color _selectedColor;
		private Color _normalColor;

		private void Awake()
		{
			_image = GetComponent<Image>();
			_button = GetComponent<Button>();
		}

		private void Start()
		{
			_normalColor = _image.color;
			_button.onClick.AddListener(Select);
		}

		private void Select()
		{
			_selected = !_selected;

			if (_selected)
			{
				StartBoosterController.Instance.SelectBooster(_booster);
				_image.color = _selectedColor;
			}
			else
			{
				StartBoosterController.Instance.DeselectBooster(_booster);
				_image.color = _normalColor;
			}
		}
	}
}