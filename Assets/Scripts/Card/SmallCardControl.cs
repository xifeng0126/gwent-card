using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SmallCardControl : MonoBehaviour
{
    public Card card;
    public setCardManager cardManager;
    public int card_number;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void display(int number)
    {
        card_number = number;
        this.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = card.name;
        this.transform.Find("number").GetComponent<TextMeshProUGUI>().text = number.ToString();
    }

    public void display()
    {
        this.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = card.name;
        this.transform.Find("number").GetComponent<TextMeshProUGUI>().text = card.number.ToString();
    }

}
