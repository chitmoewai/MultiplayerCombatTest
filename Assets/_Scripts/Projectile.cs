using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPun
{

    public float speed;
    public float bulletDamage;
    private BoxCollider2D boxCollider;
    
    private bool hit;
    private float direction; //1 right , -1 left
    private float lifetime;

    private string _attackerName;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit) return;

        if (direction == 1)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        if (direction == -1)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        //float movementSpeed = speed * Time.deltaTime * direction;
        //transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > 3) gameObject.SetActive(false); 


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //hit = true;
        //boxCollider.enabled = false;
        //Deactivate();

        if (!photonView.IsMine)
            return;

       
        PhotonView target = collision.gameObject.GetComponent<PhotonView>();

        if(target != null && (!target.IsMine || target.IsRoomView))
        {
            if(target.tag == "Player")
            {
                target.RPC("ReduceHealth", RpcTarget.AllBuffered, bulletDamage, _attackerName);
            }
            //photonView.RPC("Deactivate",RpcTarget.AllBuffered);
        }
    }

    public void SetDirection(float _direction, string attackerName) //use this method everytime shooting a bullet
    {
        hit = false;
        direction = _direction;
        gameObject.SetActive(true);
        boxCollider.enabled = true;

        lifetime = 0;

        _attackerName = attackerName;
        //Debug.Log($"Direction : {direction} ");

        // bullet sprite ko flip tr basically
        //float localScaleX = transform.localScale.x;
        //if (Mathf.Sign(localScaleX) != _direction)
        //    localScaleX = -localScaleX;

        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

    }


    [PunRPC]
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
