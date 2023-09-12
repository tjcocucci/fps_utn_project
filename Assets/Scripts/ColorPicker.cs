using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    public Color color;

    private void OnValidate()
    {
        ApplyColor();
    }

    void OnEnable()
    {
        ApplyColor();
    }

    void Start()
    {
        ApplyColor();
    }

    void ApplyColor()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            if (r.sharedMaterial == null)
            {
                r.sharedMaterial = new Material(Shader.Find("Standard"));
            }
            r.sharedMaterial.color = color;
        }
    }
}
