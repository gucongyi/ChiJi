using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorHelper  {

    public static void SetColor(Text text,string colorValue)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(colorValue,out color))
        {
            text.color = color;
        }
    }
}
