using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHover : MonoBehaviour
{
    [HideInInspector]
    public GameObject Canvas;
    public GameObject BigImage;
    public GameObject BigEffect;

    public GameObject bigImage;
    public GameObject bigEffect;

    public bool isHoverable = true;
    public bool isCardUp = false;

    private void Start()
    {
        Canvas = GameObject.Find("Main Canvas");
    }

    //private void OnDestroy()
    //{
    //    DestroyEffect();
    //}

    public void OnHoverEnter()
    {
       // ShowBigInfo();

        if (isHoverable && !isCardUp)
        {
            TranslateUp();
        }
    }

    public void OnHoverExit()
    {
       // DestroyEffect();

        if (isHoverable && isCardUp)
        {
            TranslateDown();
        }
    }

    public void TranslateUp()
    {
        transform.Translate(0, 10, 0);
        isCardUp = true;
    }
    public void TranslateDown()
    {
        transform.Translate(0, -10, 0);
        isCardUp = false;
    }

    //public void DestroyEffect()
    //{
    //    Destroy(bigImage);
    //    Destroy(bigEffect);
    //}


    ////--------------------------------------------Big info on the right--------------------------------------------//
    //private void ShowBigInfo()
    //{
    //    CardStats cardStats = GetComponent<CardDisplay>().cardStats.GetComponent<CardStats>();
    //    bigImage = Instantiate(BigImage);
    //    bigEffect = Instantiate(BigEffect);

    //    bigImage.transform.SetParent(Canvas.transform, false);
    //    bigEffect.transform.SetParent(Canvas.transform, false);

    //    bigImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/List/591x380/" + cardStats._id);

    //    if (cardStats.faction == "Special")
    //    {// Special AiCard
    //        bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/" + cardStats._idstr).text;
    //    }
    //    else
    //    {// Unit AiCard or Leader
    //        if (cardStats.ability == "leader") // Leader AiCard
    //            bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/Leader/" + cardStats._id).text;
    //        else // Unit AiCard
    //        {
    //            if (cardStats.unique)
    //            {
    //                bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/hero").text;
    //            }

    //            if (cardStats.ability != "")
    //                bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/" + cardStats.ability).text;
    //            else if (!cardStats.unique)
    //                // No ability (normal unit)
    //                if (cardStats.row == "close_range")
    //                     bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/agile").text;
    //                else
    //                {
    //                    Debug.Log(Resources.Load<TextAsset>("Cards/EffectBox/normal_unit"));
    //                     bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/normal_unit").text;
    //                }
   
    //        }
    //    }

    //}
}
