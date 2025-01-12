using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 节点页面编辑器逻辑
/// </summary>
public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph; // Room node graph window
    private static RoomNodeTypeListSO roomNodeTypeList; // Context menu item list
    private RoomNodeSO currentRoomNode;

    private Vector2 graphDrag;
    private Vector2 graphOffset;
    
    // G rid spacing
    private const float gridLarge = 100f;
    private const float gridSmall = 25;
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    /// <summary>
    /// Initialise the room node style and room node type list 
    /// </summary>
    private void OnEnable()
    {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        Selection.selectionChanged += InspectorSelectionChanged;

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
 
    private void OnDisable()
    {
        Selection.selectionChanged = InspectorSelectionChanged;
    }

    /// <summary>
    /// Open the Room Node Graph Editor window
    /// </summary>
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    /// <summary>
    /// Double-click on the Room Node Graph asset to open the editor window
    /// </summary>
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }

        return false;
    }

    private void OnGUI()
    {
        if (currentRoomNodeGraph != null)
        {
            // DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            // DrawBackgroundGrid(gridLarge, 0.2f, Color.gray);
            
            DrawDraggedLine();

            ProcessEvents(Event.current); // Process events

            DrawRoomNodeConnections();

            DrawRoomNodes(); // Draw room nodes
        }

        if (GUI.changed)
            Repaint();
    }

    #region Process Event

    private void  ProcessEvents(Event currentEvent)
    {
        graphDrag = Vector2.zero; 
        
        // 如果当前节点为空或者当前节点不在左击拖拽中就检查鼠标在那个节点上面
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        // 如果当前节点为空或者划线值不为空就处理视图界面事件
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvent(currentEvent);
        }
        // 处理当前节点事件
        else
        {
            currentRoomNode.ProcessEvent(currentEvent);
        }
    }

    private void ProcessRoomNodeGraphEvent(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent); // Process mouse down event
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
        }
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            RoomNodeSO node = IsMouseOverRoomNode(currentEvent);

            if (node != null && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
            {
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(node.id))
                {
                    node.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // If it is the right click mouse dragging.
        if (currentEvent.button == 1)
        {
            ProcessRightClickMouseDragging(currentEvent);
        }
        else if (currentEvent.button == 0)
        {
            ProcessLeftClickMouseDragging(currentEvent.delta);
        }
    }

    private void ProcessLeftClickMouseDragging(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }
        
        GUI.changed = true;
    }
 
    private void ProcessRightClickMouseDragging(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DrawConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // If the right mouse button is clicked
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition); // Show context menu
        }
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    private void ClearAllSelectedRoomNodes()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }

    #endregion

    #region Context Menu

    /// <summary>
    /// Show context menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Node"), false, SelectAllRoomNode);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);

        menu.ShowAsContext();
    }

    private void DeleteSelectedRoomNodeLinks()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i > -1; i--)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        ClearAllSelectedRoomNodes();
    }

    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeDeleteQueue = new Queue<RoomNodeSO>();

        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.roomNodeType.isEntrance == false)
            {
                roomNodeDeleteQueue.Enqueue(roomNode);

                foreach (var childID in roomNode.childRoomNodeIDList)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childID);

                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                foreach (var parentID in roomNode.parentRoomNodeIDList)
                {
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentID);

                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        while (roomNodeDeleteQueue.Count > 0)
        {
            RoomNodeSO roomNodeSo = roomNodeDeleteQueue.Dequeue();

            currentRoomNodeGraph.roomNodeList.Remove(roomNodeSo);
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeSo.id);
            DestroyImmediate(roomNodeSo, true);
            AssetDatabase.SaveAssets();
        }
    }

    private void SelectAllRoomNode()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }

        GUI.changed = true;
    }

    /// <summary>
    /// Create context menu item for creating a room node
    /// </summary>
    /// <param name="mousePositionObj"></param>
    private void CreateRoomNode(object mousePositionObj)
    {
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            // Create entrance
            CreateRoomNode(new Vector2(200, 200), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousePositionObj, roomNodeTypeList.list.Find(x => x.isNone));
    }

    /// <summary>
    /// Create specific room node
    /// </summary>
    private void CreateRoomNode(object mousePositionObj, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObj;

        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph,
            roomNodeType);

        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        currentRoomNodeGraph.OnValidate();
    }

    #endregion

    #region Draw Line

    private void DrawConnectingLine(Vector2 currentEventDelta)
    {
        currentRoomNodeGraph.linePosition += currentEventDelta;
    }

    private void DrawRoomNodeConnections()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                foreach (var childID in roomNode.childRoomNodeIDList)
                {
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childID]);
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        Vector2 startPos = parentRoomNode.rect.center;
        Vector2 endPos = childRoomNode.rect.center;

        Vector2 midPos = (endPos + startPos) / 2;
        Vector2 dir = endPos - startPos;


        Vector2 arrowTailPoint1 = midPos - new Vector2(-dir.y, dir.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPos + new Vector2(-dir.y, dir.x).normalized * connectingLineArrowSize;

        Vector2 arrowHeadPoint = midPos + dir.normalized * connectingLineArrowSize;

        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null,
            connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null,
            connectingLineWidth);

        Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.white, null, connectingLineWidth);

        GUI.changed = true;
    }

    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
                currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white,
                null, connectingLineWidth);
        }
    }

    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    #endregion

    /// <summary>
    /// Draw current room node graph's room nodes
    /// </summary>
    private void DrawRoomNodes()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }

        GUI.changed = true;
    }

    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i > -1; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
    }

    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraphSo = Selection.activeObject as RoomNodeGraphSO;
        if (roomNodeGraphSo != null)
        {
            currentRoomNodeGraph = roomNodeGraphSo;
            GUI.changed = true;
        }
    }
}