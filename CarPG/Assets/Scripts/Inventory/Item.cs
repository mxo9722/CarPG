using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[System.Serializable]
[CreateAssetMenu(fileName = "item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        eBumper,
        eCarmor,
        eMisc,
        eWeapon,
        eOther,
        any
    }

    [SerializeField]
    public GUIStyleObject styleObject;

    [SerializeField]
    public string name;
    [SerializeField]
    public Sprite sprite;
    [SerializeField]
    public ItemType itemType;
    [Header("Weapon Properties")]
    [SerializeField]
    public GameObject prefab;
    [Header("Carmor Properties")]
    [SerializeField]
    public Material carmorMaterial;
    [SerializeField]
    public float thresholdBonus=0;
    [SerializeField]
    public float mass = 0;

    float titleHeight=0;
    float contentHeight=0;
    float boxHeight=0;
    string[] details=new string[0];

    public void Awake()
    {
        OnValidate();
    }

    public void OnEnable()
    {
        OnValidate();
    }

    public void OnDisable()
    {
        OnValidate();
    }

    public void OnValidate()
    {

        titleHeight = styleObject.titleStyle.CalcSize(new GUIContent("SUBTITLE")).y;
        contentHeight = styleObject.subTitleStyle.CalcSize(new GUIContent("SUBTITLE")).y;

        details = new string[0];

        switch (itemType)
        {
            case ItemType.eBumper:
                
                details = new string[] { "Shock Absorption - " + prefab.GetComponent<Cushioned>().impulseDivider};
                break;
            case ItemType.eCarmor:
                if (thresholdBonus!=0)
                {
                    details = new string[] { "Mass - " + mass + " kg.", "Safety Rating - "+thresholdBonus };
                }
                else
                {
                    details = new string[] {"Mass - "+mass+" kg."};
                }
                details[0]=("Mass - " + (mass));
                break;
            case ItemType.eMisc:
                details = new string[] { };
                break;
            case ItemType.eOther:
                details = new string[] { };
                break;
            case ItemType.eWeapon:
                details = new string[] { "Mass - " + prefab.GetComponent<Rigidbody>().mass, "Damage - "+prefab.GetComponent<Weapon>().damage };
                break;
        }

        boxHeight = titleHeight + 12 + details.Length * (contentHeight+2);
    }

    public void DisplayItemInfo(Vector2 mouse)
    {
        float windowWidth = 160;

        GUI.BeginGroup(new Rect(mouse.x+8,mouse.y+8, windowWidth, boxHeight));

        float curY = 6;

        GUI.Box(new Rect(0, 0, windowWidth, boxHeight),"");
        GUI.TextArea(new Rect(6,curY, windowWidth-12, titleHeight),name,styleObject.titleStyle);

        curY += titleHeight;

        for(int i = 0; i < details.Length; i++)
        {
            string subtitle = details[i].Substring(0, details[i].IndexOf("-"));
            string content = details[i].Replace(subtitle,"");

            var subtitleSize = styleObject.subTitleStyle.CalcSize(new GUIContent(subtitle));
            GUI.TextArea(new Rect(6, curY+2, subtitleSize.x, contentHeight), subtitle, styleObject.subTitleStyle);
            GUI.TextArea(new Rect(6+ subtitleSize.x, curY + 2, windowWidth-6-subtitleSize.x, contentHeight), content, styleObject.contentStyle);
            curY += contentHeight+2;
        }

        GUI.EndGroup();
    }
}
