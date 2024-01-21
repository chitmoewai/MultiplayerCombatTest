using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPun
{
    private PlayerMovement playerMovement;

    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject[] bullets;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    [PunRPC]
    private void AttackRPC(string attackerName)
    {
        Attack(attackerName);
    }

    /// <summary>
    /// for handling the attack logic based on the current weapon.
    /// ets the direction based on the player's facing direction.
    /// </summary>
    /// <param name="attackerName"></param>
    private void Attack(string attackerName)
    {
        if (GetComponent<WeaponHolder>().currentWeaponIndex == 0)
        {
            
            bullets[FindBullet()].GetComponent<Projectile>().speed = 50;
            bullets[FindBullet()].GetComponent<Projectile>().bulletDamage = 5;
           
        }
        if (GetComponent<WeaponHolder>().currentWeaponIndex == 1)
        {
            bullets[FindBullet()].GetComponent<Projectile>().speed = 15;
            bullets[FindBullet()].GetComponent<Projectile>().bulletDamage = 50;
            
        }
        bullets[FindBullet()].transform.position = firePos.position;
        bullets[FindBullet()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x), attackerName); // as the direction of player is facing
    }

    private int FindBullet()
    {
        for (int i = 0; i < bullets.Length; i++)
        {
            if (!bullets[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
