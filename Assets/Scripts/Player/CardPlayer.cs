using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPlayer
{
    public int player_id;
    public string player_name;
    public string password;

    public int northerndeck;
    public int nilfgaardiandeck;
    public int monsterdeck;
    public int scoiataeldeck;

    public CardPlayer(int player_id,string player_name,string password,int northerndeck,int nilfgaardiandeck,int monsterdeck, int scoiataeldeck)
    {
        this.player_id = player_id;
        this.player_name = player_name;
        this.password = password;
        this.northerndeck = northerndeck;
        this.nilfgaardiandeck = nilfgaardiandeck;
        this.monsterdeck = monsterdeck;
        this.scoiataeldeck = scoiataeldeck;
    }


}
