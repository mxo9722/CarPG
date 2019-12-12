using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public Item content;

    public void SetContent(Item i)
    {
        content = i;
        gameObject.name = i.name;
    }

    public Item GetContent()
    {
        return content;
    }

    public static GameObject CreateItemDrop(Vector3 position,Item i)
    {
        if (i == null)
            return null;

        var drop = Instantiate(Resources.Load("ItemDrop", typeof(GameObject)),position,Quaternion.identity) as GameObject;

        drop.GetComponent<ItemHolder>().SetContent(i);

        return drop;
    }
}
