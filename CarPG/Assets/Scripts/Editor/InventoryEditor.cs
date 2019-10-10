using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Inventory inventory = ((Inventory)this.target);

        InventorySlot[] slots = inventory.gameObject.GetComponentsInChildren<InventorySlot>();

        
        for (int i= 0;i<slots.Length;i++)
        {
            EditorGUI.BeginChangeCheck();

            SerializedObject slot = (new SerializedObject(serializedObject.FindProperty("slots").GetArrayElementAtIndex(i).objectReferenceValue));
            SerializedProperty content = slot.FindProperty("_content");
            EditorGUILayout.PropertyField(content,new GUIContent(slots[i].gameObject.name));

            if (EditorGUI.EndChangeCheck())
            {
                slot.ApplyModifiedProperties();
                slots[i].UpdateGraphic();
                serializedObject.ApplyModifiedProperties();
            }
        }

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponSlot"), new GUIContent("Weapon Slot"));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        inventory.slots = slots;
        
    }
}