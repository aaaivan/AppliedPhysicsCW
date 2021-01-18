using UnityEngine;

public class SwitchMenu : MonoBehaviour
{
    public GameObject[] menus;
    int menuIndex=0;

    private void Start()
    {
        foreach(GameObject go in menus)
        {
            go.SetActive(false);
        }
        menus[menuIndex].SetActive(true);
    }

    //navigate to next menu
    public void NextMenu()
    {
        menus[menuIndex].SetActive(false);
        menuIndex = (menuIndex + 1) % menus.Length;
        menus[menuIndex].SetActive(true);
    }

    //navigate to previous menu
    public void PrevMenu()
    {
        menus[menuIndex].SetActive(false);
        menuIndex = (menuIndex - 1 + menus.Length) % menus.Length;
        menus[menuIndex].SetActive(true);
    }
}
