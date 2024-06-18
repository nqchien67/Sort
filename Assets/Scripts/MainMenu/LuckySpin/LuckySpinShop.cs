using System.Collections;
using System.Collections.Generic;
using MainMenu.LuckySpin;
using UnityEngine;

public class LuckySpinShop : MonoBehaviour
{
    public void OnSpinIAP()
    {
        FindObjectOfType<LuckySpinController>().OpenAndSpinIAP();
        // FindObjectOfType<ShopController>().OnHide();
    }
}
