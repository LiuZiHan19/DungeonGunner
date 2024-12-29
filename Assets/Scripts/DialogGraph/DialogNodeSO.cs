using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentDialogNodeIDList = new List<string>();
    [HideInInspector] public List<string> childDialogNodeIDList = new List<string>();
    [HideInInspector] public DialogNodeGraphSO dialogNodeGraph;
    [HideInInspector] public DialogNodeTypeListSO dialogNodeTypeList;
    public DialogNodeTypeSO dialogNodeType;

    #region Editor Code

#if UNITY_EDITOR

    [HideInInspector] public Rect rect;

    public void Initialise(Rect rect, DialogNodeGraphSO nodeGraph, DialogNodeTypeSO dialogNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "DialogNode";
        this.dialogNodeGraph = nodeGraph;
        this.dialogNodeType = dialogNodeType;

        dialogNodeTypeList = GameResources.Instance.dialogNodeTypeList;
    }

    public void Draw(GUIStyle style)
    {
        GUILayout.BeginArea(rect, style);

        EditorGUI.BeginChangeCheck();

        int selected = dialogNodeTypeList.list.FindIndex(x => x == dialogNodeType);

        int selection = EditorGUILayout.Popup("", selected, GetDialogNodeTypesToDisplay());

        dialogNodeType = dialogNodeTypeList.list[selection];

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    public string[] GetDialogNodeTypesToDisplay()
    {
        string[] roomNodeTypes = new string[dialogNodeTypeList.list.Count];

        for (int i = 0; i < dialogNodeTypeList.list.Count; i++)
        {
            if (dialogNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomNodeTypes[i] = dialogNodeTypeList.list[i].dialogNodeTypeName;
            }
        }

        return roomNodeTypes;
    }

#endif

    #endregion
}