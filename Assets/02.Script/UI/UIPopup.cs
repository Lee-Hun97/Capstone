using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopup : MonoBehaviour
{
    [SerializeField]private GameObject PopupUI;

    public void PopUI()
    {
        PopupUI.SetActive(true);
    }

    public void DeleteUI()
    {
        PopupUI.SetActive(false);
    }
}
