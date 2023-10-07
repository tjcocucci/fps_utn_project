using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{
    public Transform weaponHoldTransform;
    public Granade granadePrefab;
    private Transform bulletContarinerTransform;


    public void Start()
    {
        bulletContarinerTransform = GameObject.Find("BulletContainer").transform;
    }
    public void Throw()
    {
        Debug.Log("Throw");
        Instantiate(
            granadePrefab,
            weaponHoldTransform.position,
            weaponHoldTransform.rotation,
            bulletContarinerTransform
        );

    }
}
