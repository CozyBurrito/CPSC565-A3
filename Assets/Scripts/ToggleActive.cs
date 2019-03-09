using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActive : MonoBehaviour
{
    public bool isActive;

    public void Start() {
        isActive = false;
        gameObject.SetActive(isActive);
    }

    public void toggleIsActive()
    {
        isActive = !isActive;
        gameObject.SetActive(isActive);
    }
}
