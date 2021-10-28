using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillTab : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private Image KillTabItem;
    [SerializeField] private GameObject KillTabPanel;

     private GameObject[] Weapons =  new GameObject[3];


    public void InstantiatingAndSetUp(int ItemIndex, string KillerName, string DeadName)
    {
        Image KillTabItemClone = Instantiate(KillTabItem);

        KillTabItemClone.gameObject.SetActive(false);

        KillTabItemClone.transform.parent = KillTabPanel.transform;
        KillTabItemClone.GetComponent<RectTransform>().sizeDelta = KillTabItem.GetComponent<RectTransform>().sizeDelta;
        KillTabItemClone.GetComponent<RectTransform>().localScale = KillTabItem.GetComponent<RectTransform>().localScale;
        KillTabItemClone.GetComponent<RectTransform>().position = KillTabItem.GetComponent<RectTransform>().position;

        foreach(Transform child in KillTabItemClone.transform)
        {
            if (child.name == "Killer")
            {
                child.GetComponent<Text>().text = KillerName;
            }

            if (child.name == "Dead")
            {
                child.GetComponent<Text>().text = DeadName;
            }

            if (child.name == "AK")
            {
                Weapons[0] = child.gameObject;
            }

            if (child.name == "Glock")
            {
                Weapons[1] = child.gameObject;
            }

            if (child.name == "AWP")
            {
                Weapons[2] = child.gameObject;
            }

            /*for (int i = 0; i < Weapons.Length; i++)
            {
                if (i != ItemIndex)
                {
                    Weapons[i].gameObject.SetActive(false);
                }
                else if (i == ItemIndex)
                {
                    Weapons[i].gameObject.SetActive(true);
                }
            }*/
        }

        Debug.Log(ItemIndex);

        KillTabItemClone.gameObject.SetActive(true);

        Destroy(KillTabItemClone.gameObject, 5f);
    }

}
