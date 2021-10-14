using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHide : MonoBehaviour
{

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

}
