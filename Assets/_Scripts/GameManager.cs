using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public GameObject PlayerPrefab;
    public GameObject disconnectUIPanel;
    public GameObject SceneCamera;

    private bool Off = false;

    [HideInInspector] public GameObject localPlayer;
    public TextMeshProUGUI respawnTimerText;
    public GameObject respawnMenuPanel;
    private float respawnTimerAmount = 5f;
    private bool RunSpawnTimer = false;

    private WeaponHolder _weaponHolder;

    public GameObject timeUpPanel;

    public Transform playerInfoHolder;

    public GameObject winnerPanel;
    public TextMeshProUGUI winnerNameText;

    // Dictionary to store kill counts for each player
    private Dictionary<string, int> playerKillCounts = new Dictionary<string, int>(); 

    private void Awake()
    {
        Instance = this;

        SpawnPlayer();

        photonView.RPC("UpdatePlayersInfo", RpcTarget.AllBuffered);
    }
    
    private void Update()
    {
        if (RunSpawnTimer)
        {
            StartRespawn();
        }

        if (Off && Input.GetKeyDown(KeyCode.Escape))
        {
            disconnectUIPanel.SetActive(false);
            Off = false;
        }
        else if (!Off && Input.GetKeyDown(KeyCode.Escape))
        {
            disconnectUIPanel.SetActive(true);
            Off = true;
        }
    }

    public void EnableRespawn()
    {
        respawnTimerAmount = 5f;
        RunSpawnTimer = true;
        respawnMenuPanel.SetActive(true);
    }

    private void RespawnLocation()
    {
        float randomValue = Random.Range(-5, 5f);
        localPlayer.transform.localPosition = new Vector3(randomValue, 3f);
    }

    private void StartRespawn()
    {
        respawnTimerAmount -= Time.deltaTime;
        respawnTimerText.text = "Respawing in " + respawnTimerAmount.ToString("F0");

        if(respawnTimerAmount <= 0)
        {
            localPlayer.GetComponent<PhotonView>().RPC("Respawn", RpcTarget.AllBuffered);
            localPlayer.GetComponent<PlayerHealth>().EnablePlayerMovementInput();
            RespawnLocation();
            respawnMenuPanel.SetActive(false);
            RunSpawnTimer = false;

        }
    }

    public void SpawnPlayer()
    {
        float randomValue = Random.Range(-5f, 5f);
        PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector2(transform.position.x + randomValue, transform.position.y), Quaternion.identity,0);

        SceneCamera.SetActive(false);
    }

    public void OnSwitchWeaponClicked(int newWeaponIndex)
    {
        _weaponHolder = localPlayer.GetComponent<WeaponHolder>();
        _weaponHolder.SwitchWeaponButtonClicked(newWeaponIndex);
    }

    [PunRPC]
    public void UpdatePlayersInfo()
    {
        if (PhotonNetwork.InRoom)
        {
            Player[] players = PhotonNetwork.PlayerList;

            for(int i=0; i< players.Length; i++)
            {
                if (!playerKillCounts.ContainsKey(players[i].NickName)) // If the player is not in the dictionary, add them with an initial kill count of 0
                {
                    playerKillCounts[players[i].NickName] = 0;
                }
                playerInfoHolder.GetChild(i).transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = players[i].NickName;
                playerInfoHolder.GetChild(i).transform.Find("KillCount").GetComponent<TextMeshProUGUI>().text = $"{ playerKillCounts[players[i].NickName]}";
                playerInfoHolder.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    // Method to update the kill count for a specific player
    public void UpdateKillCount(string playerName, int newKillCount)
    {
        if (playerKillCounts.ContainsKey(playerName))
        {
            playerKillCounts[playerName] = newKillCount;
        }
        
        photonView.RPC("UpdatePlayersInfo", RpcTarget.AllBuffered);// Update the player list UI
    }

    public void Shoot()
    {
        localPlayer.GetComponent<PhotonView>().RPC("AttackRPC", RpcTarget.AllBuffered, photonView.Owner.NickName);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public void GameOver()
    {
        StartCoroutine(ShowTimesUp());
    }
    
    string FindPlayerWithMaxKillCount() //find the player with the maximum kill count
    {
        if (playerKillCounts.Count == 0) return null;

        var maxPlayer = playerKillCounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

        return maxPlayer;
    }

    IEnumerator ShowTimesUp()
    {
        timeUpPanel.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        timeUpPanel.SetActive(false);
      
        ShowWinnerPanel();
    }

    void ShowWinnerPanel()
    {
        string winner = FindPlayerWithMaxKillCount();

        if (winner != null)
        {
            int maxKillCount = playerKillCounts[winner];
        }
        else
        {
            Debug.Log("No players in the dictionary.");
        }
        winnerNameText.text = winner;
        winnerPanel.SetActive(true);
    }

    public void GoToLobby()
    {
        LeaveRoom();
       
    }

    public void Rematch()
    {
        Debug.Log($"To rematch : check current player count in this room" + PhotonNetwork.CurrentRoom.PlayerCount);
        photonView.RPC("LoadGameScene", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void LoadGameScene()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
        else
        {
            Debug.Log("Other Player Left the room");
        }
    }
}
