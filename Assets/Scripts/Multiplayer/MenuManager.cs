using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] Menu[] menus;
    private Menu currentlyOpenMenu;

    private void Awake()
    {
        instance = this;
        currentlyOpenMenu = null;
    }

    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                OpenMenu(menus[i]);
                return;
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        currentlyOpenMenu?.Close();
        currentlyOpenMenu = menu;
        currentlyOpenMenu?.Open();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
