using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class loadManager : MonoBehaviourPunCallbacks
{
    public GameObject SignIn;
    public GameObject SignUp;
    //public GameObject Lobby;

    

    // Start is called before the first frame update
    void Start()
    {
        //Lobby.SetActive(false);
        SignIn.SetActive(true);
        SignUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSingInClick() //点击登录按钮
    {
        string username = SignIn.transform.Find("userName").GetComponent<TMP_InputField>().text;
        string password = SignIn.transform.Find("password").GetComponent<TMP_InputField>().text;
        int userId = DBmanager.verifyPlayer(username, password);
        if (userId == -1)
        {
            //显示消息登录失败
        }
        else{
            //显示消息登录成功
            PhotonNetwork.AuthValues = new AuthenticationValues(userId.ToString());
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnSingUpClick() //点击注册按钮
    {
        string username = SignUp.transform.Find("userName").GetComponent<UnityEngine.UI.InputField>().text;
        string password = SignUp.transform.Find("password").GetComponent<UnityEngine.UI.InputField>().text;
        if (DBmanager.savePlayer(username, password))
        {

        }
        else
        {
            
        }
    }

    public void OnSettingBtnClick()  //登录/注册界面的切换按钮
    {
        
    }

    public override void OnConnectedToMaster()
    {
        //Debug.Log("连接成功");
        //Lobby.SetActive(true);
        SignIn.SetActive(false);
        SignUp.SetActive(false);
        TypedLobby lobby = new TypedLobby("GwentLobby", LobbyType.SqlLobby);    //大厅可搜索
        PhotonNetwork.JoinLobby(lobby);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
    }

    public override void OnJoinedLobby()
    {
        //Debug.Log("加入大厅");
        PhotonNetwork.LoadLevel("Lobby");
    }

}
