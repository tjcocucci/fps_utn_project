using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{

    public float rotationSpeed = 50;
    public LayerMask enemyMask;
    public SpriteRenderer dot;

    void Start() {
        Cursor.visible = false;
    }
    void Update() {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
