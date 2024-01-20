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
    public GameObject gameStartUIPanel;
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

    private Dictionary<string, int> playerKillCounts = new Dictionary<string, int>(); // Dictionary to store kill counts for each player

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
        float randomValue = Random.Range(-3, 5f);
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
        float randomValue = Random.Range(-1f, 1f);
        PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector2(transform.position.x*randomValue, transform.position.y), Quaternion.identity,0);

        gameStartUIPanel.SetActive(false);
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
                // If the player is not in the dictionary, add them with an initial kill count of 0
                if (!playerKillCounts.ContainsKey(players[i].NickName))
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

        yield return new WaitForSeconds(2);

        timeUpPanel.SetActive(false);
      
        ShowWinnerPanel();
    }

    void ShowWinnerPanel()
    {
        string winner = FindPlayerWithMaxKillCount();

        if (winner != null)
        {
            int maxKillCount = playerKillCounts[winner];
            Debug.Log($"Player with the maximum kill count: {winner}, Kills: {maxKillCount}");
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
}
