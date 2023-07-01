using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class cardBackControl : MonoBehaviour
{
    public int cardNum;

    public void setCardNum(int num)
    {
        cardNum = num;
        this.transform.Find("num").GetComponent<TextMeshProUGUI>().text = num.ToString();
    }

    public void subCard(int num)
    {
        cardNum -= num;
        setCardNum(cardNum);
    }

}
