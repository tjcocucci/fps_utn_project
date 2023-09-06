using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Weapon[] weaponList;
    public Weapon weapon;
    private int weaponIndex = 0;
    public Transform weaponHoldTransform;

    public void EquipWeapon(int index)
    {
        if (weapon != null)
        {
            Destroy(weapon.gameObject);
        }
        weaponIndex = index;
        weapon = Instantiate(weaponList[weaponIndex], weaponHoldTransform.position, weaponHoldTransform.rotation, weaponHoldTransform);
    }
}
