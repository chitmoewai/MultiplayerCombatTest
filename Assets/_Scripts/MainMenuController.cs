using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class MainMenuController : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject LoadingPanel;
    [SerializeField] private GameObject UsernameMenuPanel;
    [SerializeField] private GameObject ConnectPanel;

    [SerializeField] private TMP_InputField UsernameInput;
    [SerializeField] private TMP_InputField CreateGameInput;
    [SerializeField] private TMP_InputField JoinGameInput;

    [SerializeField] private GameObject StartButton;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Start()
    {
        LoadingPanel.SetActive(true);
        //UsernameMenuPanel.SetActive(true);
    }

    public void ChangeUserNameInput()
    {
        if (UsernameInput.text.Length >= 3)
            StartButton.SetActive(true);
        else
            StartButton.SetActive(false);
    }

    public void SetUserName()
    {
        UsernameMenuPanel.SetActive(false);
        PhotonNetwork.NickName = UsernameInput.text;
    }

    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions { MaxPlayers = 5 }, null);
    }

    public void JoinGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.JoinOrCreateRoom(JoinGameInput.text,roomOptions, TypedLobby.Default);
    }

    #region PhotonCallbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
   
        Debug.Log("OnConnectedToMaster");
    }

    public override void OnJoinedLobby()
    {
        LoadingPanel.SetActive(false);
        UsernameMenuPanel.SetActive(true);
        Debug.Log("OnJoinLobby");
        //load lobby scene or show lobby panel
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinRoom");
        PhotonNetwork.LoadLevel("GameScene");
        Debug.Log(PhotonNetwork.CountOfPlayers);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreateRoomCallback");
    }
   
    #endregion
}
