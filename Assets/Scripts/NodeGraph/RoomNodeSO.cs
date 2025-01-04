using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A room node
/// </summary>
public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    public RoomNodeTypeSO roomNodeType;

    #region Editor Code

#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging;
    [HideInInspector] public bool isSelected;

    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle style)
    {
        GUILayout.BeginArea(rect, style);

        EditorGUI.BeginChangeCheck();

        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

        roomNodeType = roomNodeTypeList.list[selection];

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomNodeTypes = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomNodeTypes[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomNodeTypes;
    }

    public void ProcessEvent(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftDragEvent(currentEvent);
        }
    }

    private void ProcessLeftDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;
        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    private void DragNode(Vector2 currentEventDelta)
    {
        rect.position += currentEventDelta;
        // EditorUtility.SetDirty(this) 是 Unity 编辑器脚本中的一个方法，用于标记一个对象为“脏”（dirty），
        // 意思是这个对象已经被修改了，需要重新保存或更新。
        EditorUtility.SetDirty(this);
    }
 
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            isLeftClickDragging = false;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickEvent();
        }
    }

    private void ProcessLeftClickEvent()
    {
        Selection.activeObject = this;
            
        if (isSelected)
            isSelected = false;
        else
            isSelected = true;
    }

#endif

    #endregion
}