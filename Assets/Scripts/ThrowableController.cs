using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{
    public Transform weaponHoldTransform;
    // public Granade throwable;

    public void Throw()
    {
        Debug.Log("Throw");
        // if (throwable != null)
        // {
        //     Destroy(throwable.gameObject);
        // }
        // throwable = Instantiate(
        //     throwable,
        //     weaponHoldTransform.position,
        //     weaponHoldTransform.rotation,
        //     weaponHoldTransform
        // );
    }
}
