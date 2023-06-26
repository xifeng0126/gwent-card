using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    public int player_id;
    public int deck_id;
    public string deck_camp;

    public Card Leader;
    public List<Card> Special;
    public List<Card> Base;

    public Deck(int deck_id, int player_id, string deck_camp)
    {
        this.deck_camp = deck_camp;
        this.deck_id = deck_id;
        this.player_id = player_id;

        Leader= null;
        Special = new List<Card>();
        Base = new List<Card>();
    }
    public Deck(int deck_id, int player_id, string deck_camp,Card Leader, List<Card> Special, List<Card> Base)
    {
        this.deck_camp = deck_camp;
        this.deck_id = deck_id;
        this.player_id = player_id;

        this.Leader = Leader;
        this.Special = Special;
        this.Base = Base;
    }

    public int getLeaderNum()
    {
        if(Leader== null)
            return 0;
        else
            return 1;
    }

    public int getSpecialNum()
    {
        if (Special == null)
            return 0;
        else
            return Special.Count;
    }

    public int getBaseNum()
    {
        if (Base == null)
            return 0;
        else
            return Base.Count;
    }

    public bool isFull()
    {
        if (getLeaderNum() == 1 && getSpecialNum() == 10 && getBaseNum() == 25)
        {
            return true;
        }
        else return false;
    }
}
