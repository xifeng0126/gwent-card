using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager
{

    public static void setDullColor(Image image)
    {
        Color c = new((140f / 255), (140f / 255), (140f / 255), (255f / 255));
        image.color = c;
    }

    public static void setBrightColor(Image image)
    {
        Color c = new((255f / 255), (255f / 255), (255f / 255), (255f / 255));
        image.color = c;
    }

}
