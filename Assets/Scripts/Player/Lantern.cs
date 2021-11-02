using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Lantern : MonoBehaviour
{
    [SerializeField] private GameObject lantern;

    private void Start()
    {        
        lantern.SetActive(false);
    }

    [PunRPC]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (lantern.active)
                lantern.SetActive(false);
            else
                lantern.SetActive(true);
        }
    }
}
