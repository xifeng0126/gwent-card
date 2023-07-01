using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Card
{
    public int id;
    public string name;
    public string image;
    public int number;
    public string position;
    public string camp;
    public int skill1;
    public int skill2;

    public Card(int id, string name, string image, int number, string position, string camp, int skill1, int skill2)
    {
        this.id = id;
        this.name = name;
        this.image = image;
        this.number = number;
        this.position = position;
        this.camp = camp;
        this.skill1 = skill1;
        this.skill2 = skill2;
    }

}

public class LeaderCard : Card
{
    public LeaderCard(int id, string name, string image, int number, string position, string camp, int skill1, int skill2) : base(id, name, image, number, position, camp, skill1, skill2)
    {
    }
}

public class SpecialCard : Card
{
    public SpecialCard(int id, string name, string image, int number, string position, string camp, int skill1, int skill2) : base(id, name, image, number, position, camp, skill1, skill2)
    {
    }
}

public class BasicCard : Card
{
    public int attack;
    
    public BasicCard(int id, string name, int attack, string image, int number, string position, string camp, int skill1, int skill2) : base(id, name, image, number, position, camp, skill1, skill2)
    {
        this.attack = attack;
    }
}

//public class NeutralCard : Card
//{
//    public int attack;
//    public NeutralCard(int id, string name, int attack, string image, int number, string position, string camp, int skill1, int skill2) : base(id, name, image, number, position, camp, skill1, skill2)
//    {
//        this.attack = attack;
//    }
//}

