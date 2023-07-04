using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CCardManager : MonoBehaviour, IPointerClickHandler
{
    public List<Sprite> normalPatch;
    public List<Sprite> uniquePatch;

    float lastClick = 0f;
    float interval = 0.4f;

    [HideInInspector]
    public GameObject deckControllerGO;
    [HideInInspector]
    public DeckController deckController;

    [HideInInspector]
    public GameObject deckCollectionGO;
    [HideInInspector]
    public CollectionManager deckCollection;

    private void Start()
    {
        deckControllerGO = GameObject.Find("DeckController");
        deckController = deckControllerGO.GetComponent<DeckController>();

        deckCollectionGO = GameObject.Find("DeckCollection");
        deckCollection = deckCollectionGO.GetComponent<CollectionManager>();

        UpdateCardGUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 右键显示详细信息
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowBigCard();
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if ((lastClick + interval) > Time.time)
            {
                // 双击
                CardStats cardStats = GetComponent<CardStats>();
                if (transform.parent.parent.parent.CompareTag("Player"))
                {
                    // 从牌组中删除卡
                    Debug.Log("Removing card from deck: " + deckController.my_deck.Name);
                    deckController.my_deck.Cards.Remove(cardStats._id);
                    deckCollection.OnDeckEdit();
                }
                else
                {
                    // 将卡添加到选定的牌组（如果存在）
                    if (deckController.my_deck.Name == "__emptyname__")
                        Debug.Log("未选择牌组!");
                    else
                    {
                        int occurences = deckController.my_deck.Cards.Where(x => x.Equals(cardStats._id)).Count();

                        // 英雄Card
                        if (cardStats.unique)
                        {
                            ImageManager.setDullColor(GetComponent<Image>());
                            if (occurences > 0)
                            {
                                Debug.Log("Cannot add hero card to deck, already contains: " + occurences);
                                ImageManager.setDullColor(GetComponent<Image>());
                                return;
                            }
                        }

                        // 单位卡和特殊卡每张数量不能大于3
                        if (!cardStats.unique)
                        {
                            if (occurences >= 3)
                            {
                                ImageManager.setDullColor(GetComponent<Image>());
                                Debug.Log("Cannot add card to deck, already contains: " + occurences);
                                return;
                            }
                        }

                        //特殊卡总数不能大于10
                        if (cardStats.faction == "Special")
                        {
                            int specialsOcc = deckController.GetSpecialsOccurence(deckController.my_deck);
                            if(specialsOcc == 9)
                            {
                                foreach (Transform child in this.transform.parent.transform)
                                {
                                    ImageManager.setDullColor(child.GetComponent<Image>());
                                }
                            }
                            if (specialsOcc >= 10)
                            {
                                Debug.Log("Cannot add special card to deck, deck already contains " + specialsOcc + " specials.");
                                return;
                            }
                        }

                        //当所有条件都满足时，添加卡片到牌堆
                        deckController.my_deck.Cards.Add(cardStats._id);
                        deckCollection.OnDeckEdit();
                    }
                }
            }
            else
            {
                //Debug.Log("Single Click :)");
            }
            lastClick = Time.time;
        }
    }

    //------------------------------------------------------Functions---------------------------------------------------//
    private void UpdateCardGUI()
    {
        string myText = Regex.Replace(GetComponent<CardStats>().name, @"[\d-]", string.Empty).Trim();
        transform.Find("Name").GetComponent<TextMeshProUGUI>().text = myText;
        if (GetComponent<CardStats>().faction == "N" || GetComponent<CardStats>().faction == "Special")
        {
            float y = transform.Find("Name").GetComponent<RectTransform>().localPosition.y;
            transform.Find("Name").GetComponent<RectTransform>().localPosition = new Vector2(0, y);
            if (GetComponent<CardStats>().unique)
            {
                transform.Find("Patch").GetComponent<Image>().sprite = uniquePatch[1];
            }
            else
            {
                transform.Find("Patch").GetComponent<Image>().sprite = normalPatch[1];
            }
        }
        else
        {
            if (GetComponent<CardStats>().unique)
            {
                transform.Find("Patch").GetComponent<Image>().sprite = uniquePatch[0];
            }
            else
            {
                transform.Find("Patch").GetComponent<Image>().sprite = normalPatch[0];
            }
        }
    }

    public void ShowBigCard()
    {
        GameObject cardDetails = transform.parent.parent.parent.GetComponent<CollectionManager>().cardDetails;
        CardStats cardStats = GetComponent<CardStats>();

        cardDetails.SetActive(true);
        cardDetails.transform.Find("BigImage")
            .GetComponent<Image>()
            .sprite = Resources.Load<Sprite>("Cards/List/591x380/" + cardStats._id);

        if (cardStats.faction == "Special")
        {// Special Card
            cardDetails.transform.Find("BigEffect/text")
                .GetComponent<TextMeshProUGUI>()
                .text = Resources.Load<TextAsset>("Cards/EffectBox/" + cardStats._idstr).text;
        }
        else
        {// Unit Card
                if (cardStats.unique)
                {
                 cardDetails.transform.Find("BigEffect/text")
                .GetComponent<TextMeshProUGUI>()
                .text = Resources.Load<TextAsset>("Cards/EffectBox/hero").text;
                }

                if (cardStats.ability != "")
                    cardDetails.transform.Find("BigEffect/text").
                        GetComponent<TextMeshProUGUI>()
                .text = Resources.Load<TextAsset>("Cards/EffectBox/" + cardStats.ability).text;
                else if (!cardStats.unique)
                    // No ability (normal unit)
                    if (cardStats.row == "close_range")
                        cardDetails.transform.Find("BigEffect/text")
                               .GetComponent<TextMeshProUGUI>()
                .text = Resources.Load<TextAsset>("Cards/EffectBox/agile").text;
                    else
                        cardDetails.transform.Find("BigEffect/text")
                            .GetComponent<TextMeshProUGUI>()
                .text = Resources.Load<TextAsset>("Cards/EffectBox/normal_unit").text;
            
        }
    }

}
