using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSortOrderSCript : MonoBehaviour
{

    private void OnEnable()
    {
       GetComponent<RectTransform>().SetAsLastSibling();
    }
}