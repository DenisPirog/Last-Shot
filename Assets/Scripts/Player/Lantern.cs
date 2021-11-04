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

    public void LaternOn()
    {
        if (lantern.activeSelf)
        {
            lantern.SetActive(false);
        }           
        else
        {
            lantern.SetActive(true);
        }            
    }
}
