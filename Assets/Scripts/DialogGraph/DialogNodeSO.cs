using System.Collections.Generic;
using UnityEngine;

public class DialogNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentDialogNodeIDList = new List<string>();
    [HideInInspector] public List<string> childDialogNodeIDList = new List<string>();
    [HideInInspector] public DialogNodeGraphSO dialogNodeGraph;
    [HideInInspector] public DialogNodeTypeListSO dialogNodeTypeList;
    public RoomNodeTypeSO roomNodeType;
}