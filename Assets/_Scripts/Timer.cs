using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Timer : MonoBehaviourPun
{
    public TextMeshProUGUI timerText;
    private float countdownTime = 10f; // 2 minutes in seconds
    private bool IsGameEnd = false;

    void Start()
    {
        photonView.RPC("InitializeCountdownRPC", RpcTarget.AllBuffered, countdownTime);
     
    }

    void Update()
    {
        //if (photonView.IsMine)

        countdownTime -= Time.deltaTime;

        // Ensure the timer doesn't go below zero
        countdownTime = Mathf.Max(countdownTime, 0f);

        UpdateTimerDisplay();

        if (countdownTime <= 0f && !IsGameEnd)
        {
            // Time is up, implement your game end logic here
            photonView.RPC("EndGameRPC", RpcTarget.AllBuffered);
        }
    }

    void UpdateTimerDisplay()
    {
        string minutes = Mathf.Floor(countdownTime / 60).ToString("00");
        string seconds = (countdownTime % 60).ToString("00");
        timerText.text = $"{minutes}:{seconds}";

    }

    [PunRPC]
    void InitializeCountdownRPC(float startTime)
    {
        countdownTime = startTime;
    }

    [PunRPC]
    void EndGameRPC()
    {
        IsGameEnd = true;
        GameManager.Instance.GameOver();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(countdownTime);
        }
        else
        {
            countdownTime = (float)stream.ReceiveNext();
        }
    }
}
