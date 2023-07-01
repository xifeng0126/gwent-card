using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardListController : MonoBehaviour
{
    setCardManager setCardManager;
    UImanager uimanager;

    public GameObject smallCard;
    public GameObject smallCards;

    public TextMeshProUGUI Leadernum;
    public TextMeshProUGUI Specialnum;
    public TextMeshProUGUI Basenum;

    public GameObject notice;

    void Start()
    {
        setCardManager=this.GetComponent<setCardManager>();
        uimanager=this.GetComponent<UImanager>();
        showList(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showList(int camp_id)
    {
        clearList();
        setCardsNum();

        Deck deck;
        switch (camp_id)
        {
            case 0:
                deck=setCardManager.northernDeck;
                break;
            case 1:
                deck = setCardManager.nilfgaardianDeck;
                break; 
            case 2:
                deck = setCardManager.monsterDeck;
                break; 
            case 3:
                deck = setCardManager.scoiataelDeck;
                break;
            default:
                deck = null;
                break;
        }
        if(deck.Leader != null)
            instanceSmallCard(deck.Leader,1);

        if(deck.Special != null)
        {
            var uniqueCards = deck.Special.DistinctBy(card => card.id).ToList(); //根据card.id去重
            foreach (Card card in uniqueCards)
            {
                int num = deck.Special.Count(c => c.id == card.id);  //获取和deck相同元素的数量
                instanceSmallCard(card, num);
            }
        }

        if(deck.Base != null)
        {
            var uniqueCards = deck.Base.DistinctBy(card => card.id).ToList();
            foreach (Card card in uniqueCards)
            {
                //Debug.Log(2);
                int num = deck.Base.Count(c => c.id == card.id);
                instanceSmallCard(card, num);
            }
        }
    }

    public void setCardsNum()
    {
        int camp_id = uimanager.camp_id;
        switch (camp_id)
        {
            case 0:
                Leadernum.text = setCardManager.northernDeck.getLeaderNum().ToString();
                Specialnum.text = setCardManager.northernDeck.getSpecialNum().ToString();
                Basenum.text = setCardManager.northernDeck.getBaseNum().ToString();
                break;
            case 1:
                Leadernum.text = setCardManager.nilfgaardianDeck.getLeaderNum().ToString();
                Specialnum.text = setCardManager.nilfgaardianDeck.getSpecialNum().ToString();
                Basenum.text = setCardManager.nilfgaardianDeck.getBaseNum().ToString();
                break;
            case 2:
                Leadernum.text = setCardManager.monsterDeck.getLeaderNum().ToString();
                Specialnum.text = setCardManager.monsterDeck.getSpecialNum().ToString();
                Basenum.text = setCardManager.monsterDeck.getBaseNum().ToString();
                break;
            case 3:
                Leadernum.text = setCardManager.scoiataelDeck.getLeaderNum().ToString();
                Specialnum.text = setCardManager.scoiataelDeck.getSpecialNum().ToString();
                Basenum.text = setCardManager.scoiataelDeck.getBaseNum().ToString();
                break;
            default:
                Leadernum.text = "0";
                Specialnum.text = "0";
                Basenum.text = "0";
                break;
        }
    }

    public void instanceSmallCard(Card card,int num)
    {
        GameObject instance = Instantiate(smallCard, smallCards.transform.position, smallCards.transform.rotation, smallCards.transform);
        SmallCardControl smallCardControl=instance.GetComponent<SmallCardControl>();
        smallCardControl.card = card;
        smallCardControl.display(num);

        Button button = instance.AddComponent<Button>();
        button.onClick.AddListener(() =>
        {
            onSmallCardClick(instance);
        });
    }

    public void onSmallCardClick(GameObject instance)
    {
        Card card = instance.GetComponent<SmallCardControl>().card;
        instance.GetComponent<SmallCardControl>().card_number--;
        int num=instance.GetComponent<SmallCardControl>().card_number;
        instance.GetComponent<SmallCardControl>().display(num);

        removeSmallCard(card);
        uimanager.freshCards();
        setCardsNum();

        if (num == 0)
        {
            Destroy(instance );
        }
    }

    public void removeSmallCard(Card card)
    {
        int camp_id = uimanager.camp_id;
        Card cardToRemove = null;
        if (card is LeaderCard)
        {
            switch (camp_id)
            {
                case 0:
                    setCardManager.northernDeck.Leader = null;
                    setCardManager.northernCardsList[card.id]++;
                    break;
                case 1:
                    setCardManager.nilfgaardianDeck.Leader = null;
                    setCardManager.nilfgaardianCardsList[card.id]++;
                    break;
                case 2:
                    setCardManager.monsterDeck.Leader = null;
                    setCardManager.monsterCardsList[card.id]++;
                    break;
                case 3:
                    setCardManager.scoiataelDeck.Leader = null;
                    setCardManager.scoiataelCardsList[card.id]++;
                    break;
                default:
                    break;
            }
        }
        else if (card is SpecialCard)
        {
            switch (camp_id)
            {
                case 0:
                    cardToRemove = setCardManager.northernDeck.Special.FirstOrDefault(cardd => cardd.id == card.id);
                    if (cardToRemove != null)
                    {
                        setCardManager.northernDeck.Special.Remove(cardToRemove);
                    }
                    setCardManager.northernCardsList[card.id]++;
                    break;
                case 1:
                    cardToRemove = setCardManager.nilfgaardianDeck.Special.FirstOrDefault(cardd => cardd.id == card.id);
                    if (cardToRemove != null)
                    {
                        setCardManager.nilfgaardianDeck.Special.Remove(cardToRemove);
                    }
                    setCardManager.nilfgaardianCardsList[card.id]++;
                    break;
                case 2:
                    cardToRemove = setCardManager.monsterDeck.Special.FirstOrDefault(cardd => cardd.id == card.id);
                    if (cardToRemove != null)
                    {
                        setCardManager.monsterDeck.Special.Remove(cardToRemove);
                    }
                    setCardManager.monsterCardsList[card.id]++;
                    break;
                case 3:
                    cardToRemove = setCardManager.scoiataelDeck.Special.FirstOrDefault(cardd => cardd.id == card.id);
                    if (cardToRemove != null)
                    {
                        setCardManager.scoiataelDeck.Special.Remove(cardToRemove);
                    }
                    setCardManager.scoiataelCardsList[card.id]++;
                    break;
                default:
                    break;
            }
        }
        else if (card is BasicCard)
        {
            switch (camp_id)
            {
                case 0:
                    cardToRemove = setCardManager.northernDeck.Base.FirstOrDefault(cardd => cardd.id == card.id); //找到第一个和card.id相同的card
                    if (cardToRemove != null)
                    {
                        setCardManager.northernDeck.Base.Remove(cardToRemove);
                    }
                    setCardManager.northernCardsList[card.id]++;
                    break;
                case 1:
                    cardToRemove = setCardManager.nilfgaardianDeck.Base.FirstOrDefault(cardd => cardd.id == card.id);
                    if (cardToRemove != null)
                    {
                        setCardManager.nilfgaardianDeck.Base.Remove(cardToRemove);
                    }
                    setCardManager.nilfgaardianCardsList[card.id]++;
                    break;
                case 2:
                    cardToRemove = setCardManager.monsterDeck.Base.FirstOrDefault(cardd => cardd.id == card.id);
                    if (cardToRemove != null)
                    {
                        setCardManager.monsterDeck.Base.Remove(cardToRemove);
                    }
                    setCardManager.monsterCardsList[card.id]++;
                    break;
                case 3:
                    cardToRemove = setCardManager.scoiataelDeck.Base.FirstOrDefault(cardd => cardd.id == card.id);
                    if (cardToRemove != null)
                    {
                        setCardManager.scoiataelDeck.Base.Remove(cardToRemove);
                    }
                    setCardManager.scoiataelCardsList[card.id]++;
                    break;
                default:
                    break;
            }
        }
    }

    public void clearList()
    {
        Leadernum.text = "0";
        Specialnum.text = "0";
        Basenum.text = "0";
        foreach (Transform child in smallCards.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnSaveBtnClick()
    {
        Deck deck = null;
        switch (uimanager.camp_id)
        {
            case 0:
                deck = setCardManager.northernDeck;
                break;
            case 1:
                deck = setCardManager.nilfgaardianDeck;
                break;
            case 2:
                deck = setCardManager.monsterDeck;
                break;
            case 3:
                deck = setCardManager.scoiataelDeck;
                break;
            default:
                break;
        }
        if (deck.isFull())
        {
            DBmanager.saveDeck(deck);
            MessageController.ShowMessage("保存成功",notice);
        }
        else
        {
            MessageController.ShowMessage("卡组不完整", notice);
        }

    }
}
