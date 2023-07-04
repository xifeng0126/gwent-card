using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DeckController;

public class CollectionManager : MonoBehaviour
{
    public DeckController deckController;
    public SimpleScrollSnap headerSnap;
    public Toggle toggleBtn;

    public TextMeshProUGUI subtitle;
    public GameObject CCardPrefab;
    public GameObject cardsArea;
    public GameObject cardDetails;

    public int selectedTab;
    public string faction = "NR";
    public TextMeshProUGUI factionPerk;

    public List<Image> currentSprites;
    public List<Sprite> idleSprites;
    public List<Sprite> selectedSprites;//选中的图片

    [HideInInspector]
    public List<AiCard> CardsToDisplay;

    public MiddleManager middleManager;

    IEnumerator Start()
    {
        ClearCardsArea();
        yield return new WaitForSeconds(0.3f); //确保 DeckController 首先启动

        // 收藏卡或玩家牌组
        if (transform.CompareTag("Player"))
        {
            LoadFromDeck();
        }
        else
            CardsToDisplay = deckController.allCards;

        SetCardsArea(UnitMergerHelper(faction, "all"));
    }

    // 卡牌类型点击事件
    public void HandleClick(int index)
    {
        if (selectedTab != index)
        {
            Deselect(selectedTab);
            selectedTab = index;
            Select(selectedTab);
        }
    }

    public void Deselect(int index)
    {
        currentSprites[index].sprite = idleSprites[index];
    }

    public void Select(int index, bool resetScroll = true)
    {
        string selected_row;

        // UI: Text and Icon
        currentSprites[index].sprite = selectedSprites[index];
        switch (index)
        {
            case 0:
                subtitle.text = "所有卡牌";
                selected_row = "all";
                break;
            case 1:
                subtitle.text = "近战单位";
                selected_row = "close";
                break;
            case 2:
                subtitle.text = "远程单位";
                selected_row = "range";
                break;
            case 3:
                subtitle.text = "攻城单位";
                selected_row = "siege";
                break;
            case 4:
                subtitle.text = "英雄卡牌";
                selected_row = "heroes";
                break;
            case 5:
                subtitle.text = "特殊卡牌";
                selected_row = "specials";
                break;
            default:
                selected_row = "none";
                break;
        }

        // 实例化卡牌
        ManageCardsArea(selected_row, resetScroll);
    }

    //------------------------------------------------------------Functions-----------------------------------------------//
    // Sets AiDeck faction and Cards from my_deck
    private void LoadFromDeck()
    {
        faction = deckController.my_deck.Faction;
        CardsToDisplay = deckController.GetCardsFromDeck(deckController.my_deck);
    }

    private void ClearCardsArea()
    {
        for (int i = 0; i < cardsArea.transform.childCount; i++)
        {
            Destroy(cardsArea.transform.GetChild(i).gameObject);
        }

        // Do not remove this, otherwise count will still get the previous frame's count (NOT 0);
        cardsArea.transform.DetachChildren();
    }

    private void SetCardsArea(List<AiCard> cards_list)
    {
        ClearCardsArea();
        foreach (AiCard card in cards_list)
        {
                GameObject instantiatedCard = Instantiate(CCardPrefab);
                instantiatedCard.name = card._id.ToString();
                instantiatedCard.GetComponent<CardStats>().name = card.name;
                instantiatedCard.GetComponent<CardStats>()._id = card._id;
                instantiatedCard.GetComponent<CardStats>()._idstr = card._idstr;
                instantiatedCard.GetComponent<CardStats>().faction = card.faction;
                instantiatedCard.GetComponent<CardStats>().unique = card.unique;
                instantiatedCard.GetComponent<CardStats>().strength = card.strength;
                instantiatedCard.GetComponent<CardStats>().row = card.row;
                instantiatedCard.GetComponent<CardStats>().ability = card.ability;
                instantiatedCard.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/List/591x380/" + card._id);
                instantiatedCard.transform.SetParent(cardsArea.transform, false);
        }
    }

    private void ResetScrolling()
    {
        cardsArea.transform.parent.GetComponent<ScrollRect>().StopMovement();
        cardsArea.transform.position = new Vector2(cardsArea.transform.position.x, 320f);
    }

    private void ManageCardsArea(string selected_row, bool resetScroll = true)
    {
        if (resetScroll)
            ResetScrolling();

        switch (selected_row)
        {
            case "specials":
                List<AiCard> specials_list = deckController.GetSpecialCards(CardsToDisplay);
                specials_list.Sort((a, b) => a._id.CompareTo(b._id));
                SetCardsArea(specials_list);
                break;
            case "heroes":
                SetCardsArea(UnitMergerHelper(faction, "all", true));
                break;
            default:
                switch (selected_row)
                {
                    case "all":
                        SetCardsArea(UnitMergerHelper(faction, "all"));
                        break;
                    case "close":
                        SetCardsArea(UnitMergerHelper(faction, "close"));
                        break;
                    case "range":
                        SetCardsArea(UnitMergerHelper(faction, "range"));
                        break;
                    case "siege":
                        SetCardsArea(UnitMergerHelper(faction, "siege"));
                        break;
                    default:
                        Debug.LogError("ManageCardsArea: Unexpected value: " + selected_row);
                        break;
                }
                break;
        }
    }

    private List<AiCard> UnitMergerHelper(string fc, string rw, bool ishero = false) // 阵营、单位类型、是否英雄
    {
        List<AiCard> returned_list;
        if (toggleBtn.isOn)//展示中立卡牌
        {
            returned_list = deckController.MergedList(
            deckController.GetUnitCards(CardsToDisplay, fc, rw, ishero),
            deckController.GetUnitCards(CardsToDisplay, "N", rw, ishero)
            );
        }
        else
        {
            returned_list = deckController.MergedList(
            deckController.GetUnitCards(CardsToDisplay, fc, rw, ishero)
            );
        }

        // Sort By strength, if strength is equal then sort by id
        // (To get cards with similar names besides each other in deck collection)
        returned_list.Sort((x, y) =>
        {
            var ret = x.strength.CompareTo(y.strength);
            if (ret == 0) ret = x._id.CompareTo(y._id);
            return ret;
        });

        if (rw == "all" && !ishero) // Add special Cards to all cards
        {
            List<AiCard> specials_list = deckController.GetSpecialCards(CardsToDisplay);
            specials_list.Sort((a, b) => a._id.CompareTo(b._id));
            returned_list = deckController.MergedList(returned_list, specials_list);
        }

        return returned_list;
    }

    //----------------------------------------------------Event Handlers---------------------------------------------------//
    // Reference: ScrollSnap Header On Panel Changed
    public void UpdateSelectedFaction()
    {
        switch (headerSnap.CurrentPanel)
        {
            case 0:
                faction = "NR";
                factionPerk.text = "赢得一局后可从牌组中抽一张牌";
                Select(selectedTab);
                break;
            case 1:
                faction = "NF";
                factionPerk.text = "平手时获得胜利";
                Select(selectedTab);
                break;
            case 2:
                faction = "SC";
                factionPerk.text = "战斗开始时，你可以决定谁先行动";
                Select(selectedTab);
                break;
            case 3:
                faction = "M";
                factionPerk.text = "每局过后随机选择一张怪物单位牌待在战场上";
                Select(selectedTab);
                break;
        }

        // Related to AiDeck Cards
        if (transform.CompareTag("Player"))
        {
            //Debug.Log("Updating deck options...");

            // Update the faction of the new empty deck
            deckController.my_deck.Faction = faction;

            // Empty the deck
            deckController.my_deck.Cards.Clear();

            // Sets the deck name to empty
            deckController.my_deck.Name = "__emptyname__";

            // Change the leader to the default of each faction
            switch (faction)
            {
                case "NR":
                    deckController.my_deck.Leader = 51;
                    break;
                case "NF":
                    deckController.my_deck.Leader = 33;
                    break;
                case "SC":
                    deckController.my_deck.Leader = 57;
                    break;
                case "M":
                    deckController.my_deck.Leader = 40;
                    break;
            }

            // Load the new decklists and get the first deck if not empty
            middleManager.LoadDecks();
        }
    }

    // Reference: Toggle On Value Changed
    public void ToggleNeutrals()
    {
        Select(selectedTab, false);
    }

    // DECK CARDS RELATED
    // Changes my_deck and currentDeckPath in DeckController
    public void OnDeckChange(AiDeck selectedDeck, string selectedPath)
    {
        // Replace the loaded deck with the selected one
        deckController.my_deck = selectedDeck;
        deckController.currentDeckPath = selectedPath;
        LoadFromDeck();
        Select(selectedTab);

        // Save to prefs
        SaveDeckToPrefs();
    }

    // From Double Clicking a card
    public void OnDeckEdit()
    {
        LoadFromDeck();
        Select(selectedTab);
        middleManager.InitializeDeck();

        // Save to the current deck
        deckController.WriteDeckToFile(deckController.my_deck, deckController.currentDeckPath);

        // Save to prefs
        SaveDeckToPrefs();
    }


    public void SaveDeckToPrefs()
    {
        // Save deck path to playerprefs
        if (deckController.currentDeckPath.EndsWith(".json") && deckController.my_deck.Name != "__emptyname__")
        {
            // Further check if deck is valid
            if (!deckController.IsDeckValid(deckController.my_deck))
                return;

            PlayerPrefs.SetString("playerDeckPath", deckController.currentDeckPath);
            Debug.Log("New selected deck: " + deckController.currentDeckPath);
        }
    }

}
