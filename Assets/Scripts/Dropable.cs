using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dropable : MonoBehaviour
{
    public enum DropableType
    {
        Health,
        PrimaryWeaponAmmo,
        SecondaryWeaponAmmo
    }

    public DropableType type;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.RegisterDropable(this);
        Debug.Log("Dropable registered");
    }

    void SetType(DropableType type)
    {
        this.type = type;
        if (type == DropableType.Health)
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else if (type == DropableType.PrimaryWeaponAmmo)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else if (type == DropableType.SecondaryWeaponAmmo)
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
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
