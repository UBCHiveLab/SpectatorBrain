using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConfiguration : MonoBehaviour {

    public string appConfigWord;

    private List<string> activeHolograms;
    private List<string> activeMenuItems;

    public List<string> GetActiveHolograms()
    {
        if(activeHolograms != null)
        {
            return activeHolograms;
        } else
        {
            activeHolograms = new List<string>();
            TextAsset holograms = Resources.Load<TextAsset>("Configs/" + appConfigWord + "Holograms");
            string hologramsRaw = holograms.text;
            string[] hologramList = hologramsRaw.Split(',');
            foreach (string s in hologramList)
            {
                activeHolograms.Add(s);
            }
            return activeHolograms;
        }
    }

    public List<string> GetActiveMenuItems()
    {
        if(activeMenuItems != null)
        {
            return activeMenuItems;
        } else
        {
            activeMenuItems = new List<string>();
            TextAsset menuItems = Resources.Load<TextAsset>("Configs/" + appConfigWord + "Menu");
            string menuItemsRaw = menuItems.text;
            string[] menuItemList = menuItemsRaw.Split(',');
            foreach (string s in menuItemList)
            {
                activeMenuItems.Add(s);
            }
            return activeMenuItems;
        }
    }
}
