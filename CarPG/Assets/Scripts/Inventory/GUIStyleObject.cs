using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "style", menuName = "ScriptableObjects/Style", order = 1)]
public class GUIStyleObject : ScriptableObject
{

    [SerializeField]
    public GUIStyle titleStyle = new GUIStyle();
    [SerializeField]
    public GUIStyle subTitleStyle = new GUIStyle();
    [SerializeField]
    public GUIStyle contentStyle = new GUIStyle();
}
