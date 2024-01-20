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
    public BoxCollider2D boxCollider;
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

        //UpdateKillCountUI();
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

        //Player is dead
        if(photonView.IsMine && HealthAmount <= 0)
        {
            playerMovement.DisableInput = true;

            IncrementKillCount(_killerName);

            // Send RPC to notify all players about the death
            photonView.RPC("Dead", RpcTarget.AllBuffered);

            //GameManager.Instance.EnableRespawn();
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
        boxCollider.enabled = false;
        spriteRender.enabled = false;
        weaponHolder.SetActive(false);

        playerCanvas.SetActive(false);

        if(photonView.IsMine)
            GameManager.Instance.EnableRespawn();//

    }

    [PunRPC]
    private void Respawn()
    {
        //respawn nay tone mhane thwr ag
        rb.gravityScale = 1;
        boxCollider.enabled = true;
        spriteRender.enabled = true;
        weaponHolder.SetActive(true);
        playerCanvas.SetActive(true);

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

        // Update kill count for all players
        photonView.RPC("UpdateKillCount", RpcTarget.AllBuffered, playerName,killCount);
    }

    [PunRPC]
    void UpdateKillCount(string playerName, int newKillCount)
    {
        // Called on all players when a kill occurs
        killCount = newKillCount;
        GameManager.Instance.UpdateKillCount(playerName, killCount);
        //UpdateUI();
    }

    ////Player is Dead in water
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Water")
    //    {
    //        Debug.Log("Player is dead in water");
    //        photonView.RPC("Dead", RpcTarget.AllBuffered);
    //    }
    //}


}
