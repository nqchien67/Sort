using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class LanguagesPanel : Popup
{
    // [SerializeField] private Toggle[] toggles;
    // [SerializeField] private GameObject uiLanguagePanel;
    // private bool isScrolled;
    // private int targetScrollValue;
    // [SerializeField] RectTransform ContentRect;
    // [SerializeField] private int index = 0;
    // private SettingPanelController settingPanel;
    // [SerializeField]
    // private string[] LANGUAGE_CODE = { "EN", "FR", "ID", "DE", "ES", "IT", "CN", "KR", "JP", "RU", "PT", "VN" };
    // private bool isShow = false;
    // public override void OnShow(Transform parent)
    // {
    //     UIController.Instance.PushUitoStack(this);
    //     for (int i = 0; i < toggles.Length; i++)
    //     {
    //         if (Lean.Localization.LeanLocalization.CurrentLanguage == toggles[i].name)
    //         {
    //             toggles[i].isOn = Lean.Localization.LeanLocalization.CurrentLanguage == toggles[i].name;
    //             index = i;
    //             break;
    //         }
    //         toggles[i].isOn = Lean.Localization.LeanLocalization.CurrentLanguage == toggles[i].name;
    //         index = i;
    //     }
    //     uiLanguagePanel.SetActive(true);
    //     GetComponent<Animator>().Play("Appear");
    //     isScrolled = true;
    //     isShow = true;
    // }
    // void Start()
    // {
    //     GetLanguageCode();
    //     settingPanel = FindObjectOfType<SettingPanelController>();
    //     isScrolled = true;
    // }
    // public void GetLanguageCode()
    // {
    //     for (int i = 0; i < toggles.Length; i++)
    //     {
    //         if (Lean.Localization.LeanLocalization.CurrentLanguage == toggles[i].name)
    //         {
    //             index = i;
    //             break;
    //         }
    //     }
    // }
    // void Update()
    // {
    //     if (isScrolled)
    //     {
    //         targetScrollValue = (index > 6) ? 168 : 0;
    //         float deltaPos = targetScrollValue - ContentRect.anchoredPosition.y;
    //         float sign = deltaPos / Mathf.Abs(deltaPos);
    //         float baseSpeed = Mathf.Max(600f, 2 * deltaPos * sign);
    //         if (Mathf.Abs(deltaPos) > 20)
    //             ContentRect.anchoredPosition += new Vector2(0, sign * Time.deltaTime * baseSpeed);
    //         else
    //             isScrolled = false;
    //     }
    // }
    // private int GetContentScrollPos(int Index)
    // {
    //     int result = 0;
    //     if (Index / 4 >= 1)
    //         result = 140 + (310 * Mathf.Max(1, Index / 4));
    //     return result;
    // }
    // public void OnClickEnglishToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "English";
    //         PlayerPrefs.SetString("language_code", "EN");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public void OnClickFrenchToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "French";
    //         PlayerPrefs.SetString("language_code", "FR");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public void OnClickIndonesiaToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "Indonesia";
    //         PlayerPrefs.SetString("language_code", "ID");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public void OnClickDeutschToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "German";
    //         PlayerPrefs.SetString("language_code", "DE");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public void OnClickEspanolToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "Spanish";
    //         PlayerPrefs.SetString("language_code", "ES");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public void OnClickItalianToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "Italian";
    //         PlayerPrefs.SetString("language_code", "IT");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public void OnClickChineseToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "Chinese";
    //         PlayerPrefs.SetString("language_code", "CN");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public void OnClickKoreanToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "Korean";
    //         PlayerPrefs.SetString("language_code", "KR");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public void OnClickJapaneseToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "Japanese";
    //         PlayerPrefs.SetString("language_code", "JP");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public void OnClickRussiaToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "Russian";
    //         PlayerPrefs.SetString("language_code", "RU");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public void OnClickPortugalToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "Portuguese";
    //         PlayerPrefs.SetString("language_code", "PT");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public void OnClickVietNamToggle(bool state)
    // {
    //     if (state)
    //     {
    //         Lean.Localization.LeanLocalization.CurrentLanguage = "Vietnamese";
    //         PlayerPrefs.SetString("language_code", "VN");
    //         settingPanel.UpdateLanguage();
    //     }
    // }
    // public override void OnHide()
    // {
    //     GetComponent<Animator>().Play("Disappear");
    //     uiLanguagePanel.SetActive(false);
    //     UIController.Instance.PopUiOutStack();
    //     isShow = false;
    // }
}
