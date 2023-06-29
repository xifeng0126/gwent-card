using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CLeaderManager : MonoBehaviour, IPointerClickHandler
{
    public DeckController deckController;
    public GameObject LeaderPicker;

    float lastClick = 0f;
    float interval = 0.4f;

    public void OnPointerClick(PointerEventData eventData)
    {
        //右键事件
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            GameObject cardDetail = transform.parent.GetComponent<MiddleManager>().cardDetail;
            CardStats cardStats = GetComponent<CardStats>();
            cardDetail.SetActive(true);
            cardDetail.transform.Find("BigImage")
                .GetComponent<Image>()
                .sprite = Resources.Load<Sprite>("Cards/List/591x380/" + cardStats._id);
            Debug.Log(cardStats._id);
            cardDetail.transform.Find("BigEffect/text")
                 .GetComponent<TextMeshProUGUI>()
                 .text = Resources.Load<TextAsset>("Cards/EffectBox/Leader/" + cardStats._id).text;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if ((lastClick + interval) > Time.time)
            {
                // Double Click
                if (deckController.my_deck.Name == "__emptyname__")
                    return;

                LeaderPicker.SetActive(true);
                LeaderPicker.GetComponent<LeaderPicker>().ClearLeaderContent();
                LeaderPicker.GetComponent<LeaderPicker>().SetLeaderContent(GetComponent<CardStats>().faction);
            }
            else
            {
                // Single Click
            }
            lastClick = Time.time;
        }

    }
}
