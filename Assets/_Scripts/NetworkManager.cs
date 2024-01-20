using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private int MaxPlayersPerRoom = 2;

    [SerializeField] private GameObject ConnectPanel;
    [SerializeField] private GameObject WaitingRoomPanel;
    [SerializeField] private TextMeshProUGUI waitText;

    [SerializeField] private GameObject GameCanvas;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        ConnectPanel.SetActive(true);
    }

    public void StartRandomMatchmaking()
    {
        PhotonNetwork.JoinRandomRoom();
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
            //PhotonNetwork.LoadLevel("GameScene");
            GameCanvas.SetActive(true);
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // If no rooms are available, create a new one
        CreateRoom();
    }

    private void CreateRoom()
    {
        string roomName = "Room" + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = MaxPlayersPerRoom };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void ReMatchGame()
    {
        photonView.RPC("StartRematchRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void StartRematchRPC()
    {
        // Inform players that the rematch is starting
        // This might involve updating UI or displaying a message

        // Reload the game scene for all players
        PhotonNetwork.LoadLevel("GameScene");
    }


}
