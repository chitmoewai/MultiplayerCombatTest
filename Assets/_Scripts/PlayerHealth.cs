using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviourPun
{
    public float HealthAmount;

    public Image fillImage;


    public PlayerMovement playerMovement;
    public GameObject playerCanvas;

    public Rigidbody2D rb;
    public GameObject character;
    public Collider2D playerCollider;
    public SpriteRenderer spriteRender;
    public GameObject weaponHolder;

    private string _killerName;
    private int killCount = 0;
    private static List<PlayerHealth> allPlayersHealth = new List<PlayerHealth>();


    private void Awake()
    {
        allPlayersHealth.Add(this);
      
        if (photonView.IsMine)
        {
            GameManager.Instance.localPlayer = this.gameObject;
        }
    }

  
    [PunRPC]
    public void ReduceHealth(float amount, string attackerName)
    {
        _killerName = attackerName;
        ModifyHealth(amount);
    }

    private void CheckHealth()
    {
        fillImage.fillAmount = HealthAmount / 100f;

        if(photonView.IsMine && HealthAmount <= 0) //Player is dead
        {
            playerMovement.DisableInput = true;

            IncrementKillCount(_killerName);

            photonView.RPC("Dead", RpcTarget.AllBuffered); // Send RPC to notify all players about the death

        }
    }

    public void EnablePlayerMovementInput()
    {
        playerMovement.DisableInput = false;
    }

    [PunRPC]
    private void Dead()
    {
        rb.gravityScale = 0;
        playerCollider.enabled = false;
        spriteRender.enabled = false;
        weaponHolder.SetActive(false);
        character.SetActive(false);

        playerCanvas.SetActive(false);

        if(photonView.IsMine)
            GameManager.Instance.EnableRespawn();

    }

    [PunRPC]
    private void Respawn()
    {
        rb.gravityScale = 1;
        playerCollider.enabled = true;
        spriteRender.enabled = true;
        weaponHolder.SetActive(true);
        playerCanvas.SetActive(true);
        character.SetActive(true);

        fillImage.fillAmount = 1f;
        HealthAmount = 100;

    }

    private void ModifyHealth(float amount)
    {
        //fillImage.fillAmount -= amount;
        if (photonView.IsMine)
        {
            HealthAmount -= amount;
            fillImage.fillAmount -= amount;
        }
        else
        {
            HealthAmount -= amount;
            fillImage.fillAmount -= amount;
        }
        CheckHealth();
    }

    void IncrementKillCount(string playerName)
    {
        killCount++;
        photonView.RPC("UpdateKillCount", RpcTarget.AllBuffered, playerName,killCount);// Update kill count for all players
    }

    [PunRPC]
    void UpdateKillCount(string playerName, int newKillCount)
    {
        killCount = newKillCount;
        GameManager.Instance.UpdateKillCount(playerName, killCount);
    }



}
