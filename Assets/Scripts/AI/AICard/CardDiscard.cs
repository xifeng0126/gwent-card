using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDiscard : MonoBehaviour, IPointerUpHandler,IPointerDownHandler  
{
    [HideInInspector]
    public GameObject controllerObject;
    [HideInInspector]
    public SceneController controller;
    [HideInInspector]
    public GameObject Canvas;
    public GameObject BigImage;
    public GameObject BigEffect;

    public GameObject bigImage;
    public GameObject bigEffect;

    float lastClick = 0f;
    float interval = 0.4f;
    private bool isRightMouseDown = false;

    void Start()
    {
        controllerObject = GameObject.Find("SceneController");
        controller = controllerObject.GetComponent<SceneController>();
        Canvas = GameObject.Find("Main Canvas");
    }
    private void OnDestroy()
    {
        DestroyEffect();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // Left mouse button click
        {
            // Keep the original logic for left-click (double-click) here
            if ((lastClick + interval) > Time.time)
            {
                if (controller.battleState == BattleState.PLAYERTURN && controller.PlayerInfo.CanDiscard && gameObject.CompareTag("Player"))
                {
                    controller.DiscardCard(GetComponent<CardDisplay>().cardStats.GetComponent<CardStats>()._id, true);
                    controller.PlayerInfo.CardsDiscarded++;

                    // Conditions met, a card can be chosen
                    if (controller.PlayerInfo.CardsDiscarded == 2)
                        controller.DeckCardPicker(true);
                }
            }
            else
            {
                //Debug.Log("Single Click! ");
                lastClick = Time.time;
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            isRightMouseDown = true;
            CardStats cardStats = GetComponent<CardDisplay>().cardStats.GetComponent<CardStats>();
            bigImage = Instantiate(BigImage);
            bigEffect = Instantiate(BigEffect);

            bigImage.transform.SetParent(Canvas.transform, false);
            bigEffect.transform.SetParent(Canvas.transform, false);

            bigImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/List/591x380/" + cardStats._id);

            if (cardStats.faction == "Special")
            {// Special AiCard
                bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/" + cardStats._idstr).text;
            }
            else
            {// Unit AiCard or Leader
                if (cardStats.ability == "leader") // Leader AiCard
                    bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/Leader/" + cardStats._id).text;
                else // Unit AiCard
                {
                    if (cardStats.unique)
                    {
                        bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/hero").text;
                    }

                    if (cardStats.ability != "")
                        bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/" + cardStats.ability).text;
                    else if (!cardStats.unique)
                        // No ability (normal unit)
                        if (cardStats.row == "close_range")
                            bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/agile").text;
                        else
                        {
                            Debug.Log(Resources.Load<TextAsset>("Cards/EffectBox/normal_unit"));
                            bigEffect.transform.Find("text").GetComponent<TextMeshProUGUI>().text = Resources.Load<TextAsset>("Cards/EffectBox/normal_unit").text;
                        }

                }
            }

        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) // Right mouse button release
        {
            // Check if the right mouse button was pressed before (using the flag)
            if (isRightMouseDown)
            {
                isRightMouseDown = false;
                DestroyEffect();
            }
        }
    }

    public void DestroyEffect()
    {
        Destroy(bigImage);
        Destroy(bigEffect);
    }
}

