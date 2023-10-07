using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int primaryWeaponAmmo;
    public int secondaryWeaponAmmo;
    public int primaryWeaponAmmoCarryCapacity;
    public int secondaryWeaponAmmoCarryCapacity;
    public int granadeCount;
    public int granadeCarryCapacity;
    public int healthPacks;
    public int healthPackCarryCapacity;

    public int GetAvailableAmmo(int weaponIndex) {
        if (weaponIndex == 0) {
            return primaryWeaponAmmo;
        } else {
            return secondaryWeaponAmmo;
        }
    }
    public void SetAvailableAmmo(int weaponIndex, int ammo) {
        if (weaponIndex == 0) {
            primaryWeaponAmmo = ammo;
        } else {
            secondaryWeaponAmmo = ammo;
        }
    }
}
