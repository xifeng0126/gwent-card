using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class loadManager : MonoBehaviourPunCallbacks
{
    public GameObject SignIn;
    public GameObject SignUp;
    static bool flag = false;
    //public GameObject Lobby;
    public GameObject notice;
    //public GameObject miusic;


    // Start is called before the first frame update
    void Start()
    {
        //Lobby.SetActive(false);
        SignIn.SetActive(true);
        SignUp.SetActive(false);
       // miusic.GetComponent<AudioManager>().PlayAudio();
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
        if (userId >= 0)
        {
            //显示消息登录成功
            MessageController.ShowMessage("登录成功！", notice);
            PhotonNetwork.AuthValues = new AuthenticationValues(userId.ToString());
            PhotonNetwork.ConnectUsingSettings();
        }
        else if (userId == -1)
        {
            MessageController.ShowMessage("用户或密码错误！", notice);
        }
        else if (userId == -2)
        {
            MessageController.ShowMessage("该用户已登录！", notice );
        }
        else if (userId == -3)
        {

            MessageController.ShowMessage("数据库发生未知错误！", notice);

        }
        else
            Debug.Log("未知错误！");
    }

    public void OnSingUpClick() //点击注册按钮
    {
        string username = SignUp.transform.Find("userName").GetComponent<TMP_InputField>().text;
        string password = SignUp.transform.Find("password").GetComponent<TMP_InputField>().text;
        if (DBmanager.savePlayer(username, password))
        {
            MessageController.ShowMessage("注册成功，请重新登录！", notice);
            OnSettingBtnClick();
        }
        else
        {
            MessageController.ShowMessage("注册失败,用户名可能已存在！", notice);

        }
    }

    public void OnSettingBtnClick()  //登录/注册界面的切换按钮
    {
        if (!flag)
        {
            TextMeshProUGUI btntext = GameObject.Find("Canvas/settingBtn/text").GetComponent<TextMeshProUGUI>();
            //Debug.Log(btntext);
            btntext.text = "登录";
            SignUp.SetActive(true);
            SignIn.SetActive(false);
            flag = !flag;
        }
        else
        {
            TextMeshProUGUI btntext = GameObject.Find("Canvas/settingBtn/text").GetComponent<TextMeshProUGUI>();
            //Debug.Log(btntext);
            btntext.text = "注册";
            SignUp.SetActive(false);
            SignIn.SetActive(true);
            flag = !flag;
        }


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
