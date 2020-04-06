using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public void Loadpopup(GameObject popup)
    {
        popup.SetActive(true);
       
    }

    public void ClosePopup(GameObject popup)
    {
        popup.SetActive(false);

    }
}
