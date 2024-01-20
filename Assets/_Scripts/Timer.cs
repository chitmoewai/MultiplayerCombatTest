using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Timer : MonoBehaviourPun
{
    public TextMeshProUGUI timerText;
    private float countdownTime = 60f; // 2 minutes in seconds
    private bool IsGameEnd = false;

    void Start()
    {
        photonView.RPC("InitializeCountdownRPC", RpcTarget.AllBuffered, countdownTime);
     
    }

    void Update()
    {
        countdownTime -= Time.deltaTime;

        countdownTime = Mathf.Max(countdownTime, 0f);// Ensure the timer doesn't go below zero

        UpdateTimerDisplay();

        if (countdownTime <= 0f && !IsGameEnd)// Time is up
        {
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
