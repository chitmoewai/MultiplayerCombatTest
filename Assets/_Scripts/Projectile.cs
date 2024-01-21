using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPun
{

    public float speed;
    public float bulletDamage;
    private BoxCollider2D boxCollider;
   
    private float direction; //1 right , -1 left
    private float lifetime;

    private string _attackerName;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (direction == 1)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        if (direction == -1)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }

        lifetime += Time.deltaTime;
        if (lifetime > 3) gameObject.SetActive(false); 


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine)
            return;

        PhotonView target = collision.gameObject.GetComponent<PhotonView>();

        if(target != null && (!target.IsMine || target.IsRoomView))
        {
            if(target.tag == "Player")
            {
                target.RPC("ReduceHealth", RpcTarget.AllBuffered, bulletDamage, _attackerName);
            }
           
        }
    }

    public void SetDirection(float _direction, string attackerName) //use this method everytime shooting a bullet
    {
        direction = _direction;
        gameObject.SetActive(true);
        boxCollider.enabled = true;

        lifetime = 0;

        _attackerName = attackerName;
      
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

    }

}
