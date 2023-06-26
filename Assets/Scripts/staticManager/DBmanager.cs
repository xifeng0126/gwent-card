using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data;

public class DBmanager
{
    public static Card selectCardById(int id)
    {
        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456; database=gwentcard;charset=utf8;";
        MySqlConnection conn = new MySqlConnection(connetStr);
        try
        {
            conn.Open();
            string sql = "select * from cards where card_id = " + id;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                //Debug.Log(1);
                if (reader.GetString("card_type") == "Leader")
                {
                    LeaderCard leaderCard = new LeaderCard(reader.GetInt32("card_id"), reader.GetString("name"), reader.GetString("path"), reader.GetInt32("number"), reader.GetString("position"), reader.GetString("camp"), reader.GetInt32("skill1"), reader.GetInt32("skill2"));
                    return leaderCard;
                }
                else if (reader.GetString("card_type") == "Special")
                {
                    SpecialCard specialCard = new SpecialCard(reader.GetInt32("card_id"), reader.GetString("name"), reader.GetString("path"), reader.GetInt32("number"), reader.GetString("position"), reader.GetString("camp"), reader.GetInt32("skill1"), reader.GetInt32("skill2"));
                    return specialCard;
                }
                else if (reader.GetString("card_type") == "Basic")
                {
                    //Debug.Log(2);
                    BasicCard basicCard = new BasicCard(reader.GetInt32("card_id"), reader.GetString("name"), reader.GetInt32("attack_num"), reader.GetString("path"), reader.GetInt32("number"), reader.GetString("position"), reader.GetString("camp"), reader.GetInt32("skill1"), reader.GetInt32("skill2"));
                    return basicCard;
                }
                //else if (reader.GetString("card_type") == "Neutral")
                //{
                //    NeutralCard neutralCard = new NeutralCard(reader.GetInt32("card_id"), reader.GetString("name"), reader.GetInt32("attack_num"), reader.GetString("path"), reader.GetInt32("number"), reader.GetString("position"), reader.GetString("camp"), reader.GetInt32("skill1"), reader.GetInt32("skill2"));
                //    return neutralCard;
                //}
            }
            conn.Close();
            return null;
        }
        catch (MySqlException ex)
        {
            Debug.Log(ex);
            return null;
        }
        finally
        {
            conn.Close();
        }
    }

    public static List<Card> selectCardsByCamp(string camp)
    {
        List<Card> cards = new List<Card>();
        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456; database=gwentcard;charset=utf8;";
        MySqlConnection conn = new MySqlConnection(connetStr);
        try
        {
            conn.Open();
            string sql = "select * from cards where camp = '" + camp + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                //Debug.Log(reader.GetInt32("card_id"));
                cards.Add(selectCardById(reader.GetInt32("card_id")));
            }
            conn.Close();
            return cards;
        }
        catch (MySqlException ex)
        {
            Debug.Log(ex);
            return null;
        }
        finally
        {
            conn.Close();
        }
    }


    public static Deck selectDeckByDeckId(int deck_id) //不存在时返回null
    {
        int player_id = 0;
        string deck_temp = "";
        Card Leader = null;
        List<Card> Special = new List<Card>();
        List<Card> Base = new List<Card>();

        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456; database=gwentcard;charset=utf8;";
        MySqlConnection conn = new MySqlConnection(connetStr);
        try
        {
            conn.Open();
            string sql = "select * from decks where deck_id = '" + deck_id + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                player_id = reader.GetInt32("player_id");
                deck_temp = reader.GetString("deck_temp");
                Leader = selectCardById(reader.GetInt32("Leader"));

                //将用逗号分隔的字符串数字转换为int
                string[] Special_temp = reader.GetString("Special").Split(',');
                foreach (string s in Special_temp)
                {
                    Special.Add(selectCardById(Convert.ToInt32(s)));
                }
                string[] Base_temp = reader.GetString("Base").Split(',');
                foreach (string s in Base_temp)
                {
                    Base.Add(selectCardById(Convert.ToInt32(s)));
                }
                conn.Close();
                return new Deck(deck_id, player_id, deck_temp, Leader, Special, Base);
            }
            conn.Close();
            return null;
        }
        catch (MySqlException ex)
        {
            Debug.Log(ex);
            return null;
        }
        finally
        {
            conn.Close();
        }
    }

    public static void saveDeck(Deck deck)
    {
        Debug.Log(deck.deck_camp);
        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456; database=gwentcard;charset=utf8;";
        MySqlConnection conn = new MySqlConnection(connetStr);
        try
        {
            conn.Open();
            //判断decks中是否已经有deck_id=deck.deck_id的记录
            string sql = "select * from decks where deck_id = '" + deck.deck_id + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                //如果有，更新记录
                reader.Close();

                int Leader_id = 0;
                if (deck.Leader != null)
                    Leader_id = deck.Leader.id;
                else Leader_id = -1;

                string Special_temp = "";
                foreach (Card c in deck.Special)
                {
                    Special_temp += c.id + ",";
                }
                Special_temp = Special_temp.Substring(0, Special_temp.Length - 1);

                string Base_temp = "";
                foreach (Card c in deck.Base)
                {
                    Base_temp += c.id + ",";
                }
                Base_temp = Base_temp.Substring(0, Base_temp.Length - 1);

                string update_sql = "update decks set Leader ='" + Leader_id + "', Special = '" + Special_temp + "',Base = '" + Base_temp + "'where deck_id = '" + deck.deck_id + "'";
                cmd = new MySqlCommand(update_sql, conn);
                cmd.ExecuteNonQuery();
            }
            else
            {
                //如果没有，插入记录
                reader.Close();
                string Special_temp = "";
                foreach (Card c in deck.Special)
                {
                    Special_temp += c.id + ",";
                }
                Special_temp = Special_temp.Substring(0, Special_temp.Length - 1);

                string Base_temp = "";
                foreach (Card c in deck.Base)
                {
                    Base_temp += c.id + ",";
                }
                Base_temp = Base_temp.Substring(0, Base_temp.Length - 1);

                string insert_sql = "insert into decks values('" + deck.deck_id + "','" + deck.player_id + "','" + deck.deck_camp + "','" + deck.Leader.id + "','" + Special_temp + "','" + Base_temp + "')";
                cmd = new MySqlCommand(insert_sql, conn);
                cmd.ExecuteReader();
            }
        }
        catch (MySqlException ex)
        {
            Debug.Log("卡组保存失败");
            Debug.Log(ex.ToString());
        }
        finally
        {
            conn.Close();
        }
    }

    public static int verifyPlayer(string username, string password)
    {
        //验证玩家信息，玩家账户正确且未登录返回玩家id，并且将login字段改为1，匹配失败返回-1，已经登录返回-2，未知错误-3
        //if(username == "1")
        //    return 0;
        //if(username == "2")
        //    return 1;
        //else return -1;
        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456; database=gwentcard;charset=utf8;";
        MySqlConnection conn = new MySqlConnection(connetStr);
        try
        {
            conn.Open();
            string qeury_sql = "select * from players where player_name = '" + username + "'" + " and password ='" + password + "'";
            MySqlCommand cmd = new MySqlCommand(qeury_sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int player_id = reader.GetInt32("player_id");
                int login = reader.GetInt32("login");
                if (login == 0)
                {
                    reader.Close();
                    string update_sql = "update players set login = '" + 1 + "' where player_name = '" + username + "'" + " and password = '" + password + "'";
                    cmd = new MySqlCommand(update_sql, conn);
                    cmd.ExecuteReader();
                    //Debug.Log("登录成功");
                    return player_id;
                }
                else if (login == 1)
                {
                    Debug.Log("用户已登录");
                    return -2;
                }
                else
                {
                    Debug.Log("未知错误");
                    return -3;
                }

            }
            else
            {
                Debug.Log("用户名或密码错误");
                return -1;
            }
        }
        catch (MySqlException ex)
        {
            Debug.Log("信息认证失败");
            Debug.Log(ex.ToString());
        }
        finally
        {
            conn.Close();
        }
        return -3;
    }

    public static bool savePlayer(string username, string password)
    {
        //保存玩家信息,首先检测用户名是否已经存在，
        //然后获取用户数量，新建用户id为已有用户数量，用户的四个卡组id为用户id*4、用户id*4+1、用户id*4+2、用户id*4+3
        //最后将login字段设置为0，表示未登录
        //return true;  //添加成功
        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456; database=gwentcard;charset=utf8;";
        MySqlConnection conn = new MySqlConnection(connetStr);
        try
        {
            conn.Open();
            // 检查用户名是否已经存在
            string checkUsernameSql = "SELECT COUNT(*) FROM players WHERE player_name = '" + username + "'";
            MySqlCommand checkUsernameCmd = new MySqlCommand(checkUsernameSql, conn);
            int existingUserCount = Convert.ToInt32(checkUsernameCmd.ExecuteScalar());
            if (existingUserCount > 0)
            {
                Debug.Log("用户名已存在");
                conn.Close();
                return false;
            }

            // 获取用户数量
            string getUserCountSql = "SELECT COUNT(*) FROM players";
            MySqlCommand getUserCountCmd = new MySqlCommand(getUserCountSql, conn);
            int userCount = Convert.ToInt32(getUserCountCmd.ExecuteScalar());

            // 创建新的用户ID
            int newUserId = userCount;
            int no_id = newUserId * 4;
            int nl_id = no_id + 1;
            int mo_id = no_id + 2;
            int so_id = no_id + 3;

            // 执行插入操作
            string insertSql = "INSERT INTO players  VALUES ('" + newUserId + "', '" + username + "', '" + password + "', '" + no_id + "', '" + nl_id + "', '" + mo_id + "', '" + so_id + "', '" + 0 + "')";
            Debug.Log(insertSql);
            MySqlCommand insertCmd = new MySqlCommand(insertSql, conn);
            insertCmd.ExecuteReader();
            Debug.Log("保存玩家信息成功");
            conn.Close();
            return true;
        }
        catch (MySqlException ex)
        {
            Debug.Log("玩家信息保存失败");
            Debug.Log(ex.ToString());
        }
        finally
        {
            conn.Close();
        }
        return false;

    }

    public static CardPlayer selectPlayerById(int player_id)
    {
        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456; database=gwentcard;charset=utf8;";
        MySqlConnection conn = new MySqlConnection(connetStr);
        try
        {
            conn.Open();
            string sql = "select * from players where player_id = '" + player_id + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string username = reader.GetString("player_name");
                string password = reader.GetString("password");
                int northerndeck = reader.GetInt32("northerndeck");
                int nilfgaardiandeck = reader.GetInt32("nilfgaardiandeck");
                int monsterdeck = reader.GetInt32("monsterdeck");
                int scoiataeldeck = reader.GetInt32("scoiataeldeck");
                conn.Close();
                return new CardPlayer(player_id, username, password, northerndeck, nilfgaardiandeck, monsterdeck, scoiataeldeck);
            }
            conn.Close();
            return null;
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return null;
        }
        finally
        {
            conn.Close();
        }
    }

    public static void exitPlayer(int player_id)
    {
        Debug.Log(player_id);
        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456; database=gwentcard;charset=utf8;";
        MySqlConnection conn = new MySqlConnection(connetStr);
        conn.Open();
        string update_sql = "update players set login = '" + 0 + "' where player_id = '" + player_id + "'";
        MySqlCommand cmd = new MySqlCommand(update_sql, conn);
        cmd.ExecuteReader();
        conn.Close();
    }
}
