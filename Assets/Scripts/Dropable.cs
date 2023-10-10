using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropable : MonoBehaviour
{
    public enum DropableType
    {
        Health,
        PrimaryWeaponAmmo,
        SecondaryWeaponAmmo
    }

    public DropableType type;

    public GameObject healthPrefab;
    public GameObject primaryWeaponAmmoPrefab;
    public GameObject secondaryWeaponAmmoPrefab;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.RegisterDropable(this);
    }

    void SetType(DropableType type)
    {
        this.type = type;
        if (type == DropableType.Health)
        {
            Instantiate(healthPrefab, transform.position, Quaternion.identity, transform);
        }
        else if (type == DropableType.PrimaryWeaponAmmo)
        {
            Instantiate(
                primaryWeaponAmmoPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
        }
        else if (type == DropableType.SecondaryWeaponAmmo)
        {
            Instantiate(
                secondaryWeaponAmmoPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
        }
    }

    public void SetRandomType()
    {
        int random = Random.Range(0, 3);
        if (random == 0)
        {
            SetType(DropableType.Health);
        }
        else if (random == 1)
        {
            SetType(DropableType.PrimaryWeaponAmmo);
        }
        else if (random == 2)
        {
            SetType(DropableType.SecondaryWeaponAmmo);
        }
    }
}
