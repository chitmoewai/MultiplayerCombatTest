using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class MainMenuController : MonoBehaviourPunCallbacks
{
    private int MaxPlayersPerRoom = 1;

    [SerializeField] private GameObject ConnectPanel;
    [SerializeField] private GameObject WaitingRoomPanel;
    [SerializeField] private TextMeshProUGUI waitText;


    [SerializeField] private GameObject LoadingPanel;
    [SerializeField] private GameObject UsernameMenuPanel;
   

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

        ConnectPanel.SetActive(true);
      
    }
    public void StartRandomMatchmaking()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    //public void CreateGame()
    //{
    //    PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions { MaxPlayers = 3 }, null);
    //}

    //public void JoinGame()
    //{
    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.MaxPlayers = 3;
    //    PhotonNetwork.JoinOrCreateRoom(JoinGameInput.text,roomOptions, TypedLobby.Default);
    //}


    private void CreateRoom()
    {
        string roomName = "Room" + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = MaxPlayersPerRoom };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
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
        WaitingRoomPanel.SetActive(true);
        if (PhotonNetwork.CurrentRoom.PlayerCount != MaxPlayersPerRoom)
            waitText.text = $"Waiting for {MaxPlayersPerRoom - PhotonNetwork.CurrentRoom.PlayerCount} players...";

        // Synchronize the "GameScene" load operation across all players

        photonView.RPC("LoadGameScene", RpcTarget.AllBufferedViaServer);

    }

    [PunRPC]
    private void LoadGameScene()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersPerRoom)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // If no rooms are available, create a new one
        CreateRoom();
    }

    #endregion
}
