using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerMovement : MonoBehaviourPun
{
    public GameObject playerCamera;

    public TextMeshProUGUI playerNameTmp;

    private Rigidbody2D rb;

    [SerializeField] private float moveSpeed = 800f;
    [SerializeField] private float jumpingPower = 16f;

    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform playerNameTransform;
    private float horizontalInput;

    public bool DisableInput = false; // when player die can't move anymore

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (photonView.IsMine)
        {
            playerCamera.gameObject.SetActive(true);
            playerNameTmp.text = PhotonNetwork.NickName;
        }
        else
        {
            playerNameTmp.text = photonView.Owner.NickName;
            playerNameTmp.color = Color.red;
        }
            
    }

    void Update()
    {
        if (photonView.IsMine && !DisableInput)
        {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, rb.velocity.y);

            horizontalInput = rb.velocity.x;

            //Flip player when moving left - right
            if (!isFacingRight && horizontalInput > 0f)
            {
                photonView.RPC("Flip", RpcTarget.AllBuffered);
               
            }
            else if (isFacingRight && horizontalInput < 0f) // left
            {
                photonView.RPC("Flip", RpcTarget.AllBuffered);
            }

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }

            if (Input.GetKeyUp(KeyCode.Space) && (rb.velocity.y > 0f))
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
      
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    [PunRPC]
    private void Flip()
    {
        //isFacingRight = !isFacingRight;
        //playerBodySpriteRender.flipX = !isFacingRight;

        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;


        Vector3 localScalePlayerName = transform.localScale;
        localScalePlayerName.x *= 1f;
        playerNameTransform.localScale = localScalePlayerName;
    }
  

    

}
