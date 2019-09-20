using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Inventory inventory = ((Inventory)this.target);

        InventorySlot[] slots = inventory.gameObject.GetComponentsInChildren<InventorySlot>();

        for (int i= 0;i<slots.Length;i++)
        {
            InventorySlot slot = slots[i];

            slot.Content = (Item)EditorGUILayout.ObjectField(slot.gameObject.name,slot.Content,typeof(Item),false);
        }

        inventory.slots = slots;
    }
}