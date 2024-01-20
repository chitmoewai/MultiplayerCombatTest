using UnityEngine;
using Photon.Pun;

public class WeaponHolder : MonoBehaviourPun
{
    int totalWeapons = 1;

    public int currentWeaponIndex;

    public GameObject[] weapons;
    public GameObject weaponHolder;
    public GameObject currentWeapon;

    private void Start()
    {
        if (photonView.IsMine)
        {
            totalWeapons = weapons.Length;

            for (int i = 0; i < totalWeapons; i++)
            {
                weapons[i].SetActive(false);
            }
            weapons[0].SetActive(true);
            currentWeapon = weapons[0];
            currentWeaponIndex = 0;
        }
    }
    private void Update()
    {
        if (!photonView.IsMine)
            return;

        //next Weapon
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentWeaponIndex < totalWeapons - 1)
            {
                weapons[currentWeaponIndex].SetActive(false);
                currentWeaponIndex += 1;
            //    weapons[currentWeaponIndex].SetActive(true);

            //    currentWeapon = weapons[currentWeaponIndex];
            }
            //if (photonView.IsMine)
            //photonView.RPC("SwitchNextWeaponRPC", RpcTarget.AllBuffered);
        }
        //Previous Weapon
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentWeaponIndex > 0)
            {
                weapons[currentWeaponIndex].SetActive(false);
                currentWeaponIndex -= 1;
            //    weapons[currentWeaponIndex].SetActive(true);
            //    currentWeapon = weapons[currentWeaponIndex];
            }
          
        }
        photonView.RPC("SwitchWeaponRPC", RpcTarget.All, currentWeaponIndex);

    }

    public void SwitchWeaponButtonClicked(int newWeaponIndex)
    {
        photonView.RPC("SwitchWeaponRPC", RpcTarget.All, newWeaponIndex);
    }

    [PunRPC]
    void SwitchWeaponRPC(int newWeaponIndex)
    {
        SwitchWeapon(newWeaponIndex);
    }


    void SwitchWeapon(int newWeaponIndex)
    {
        //// Disable the current weapon
        weapons[currentWeaponIndex].SetActive(false);

        // Enable the new weapon
        weapons[newWeaponIndex].SetActive(true);

        // Update the current weapon index
        currentWeaponIndex = newWeaponIndex;
    }


}
