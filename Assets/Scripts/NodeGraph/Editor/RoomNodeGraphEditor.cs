using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// 节点页面编辑器逻辑
/// </summary>
public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph; // Room node graph window
    private static RoomNodeTypeListSO roomNodeTypeList; // Context menu item list

    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    /// <summary>
    /// Initialise the room node style and room node type list
    /// </summary>
    private void OnEnable()
    {
        roomNodeStyle = new GUIStyle
        {
            normal =
            {
                background = EditorGUIUtility.Load("Node1") as Texture2D, // 设置节点的背景纹理
                textColor = Color.white // 设置文字颜色
            },
            padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding), // 设置内边距
            border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder) // 设置边框宽度
        };

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
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
            ProcessEvents(Event.current); // Process events
            DrawRoomNodes(); // Draw room nodes
        }

        if (GUI.changed)
            Repaint();
    }

    #region Process Event

    /// <summary>
    /// Process events
    /// </summary>
    private void ProcessEvents(Event currentEvent)
    {
        ProcessRoomNodeGraphEvent(currentEvent);
    }

    /// <summary>
    /// Process room node graph events
    /// </summary>
    private void ProcessRoomNodeGraphEvent(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent); // Process mouse down event
                break;
        }
    }

    /// <summary>
    /// Process mouse down event
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // If the right mouse button is clicked
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition); // Show context menu
        }
    }

    #endregion

    #region Context menu

    /// <summary>
    /// Show context menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);

        menu.ShowAsContext();
    }

    /// <summary>
    /// Create context menu item for creating a room node
    /// </summary>
    /// <param name="mousePositionObj"></param>
    private void CreateRoomNode(object mousePositionObj)
    {
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
    }

    #endregion

    /// <summary>
    /// Draw current room node graph's room nodes
    /// </summary>
    private void DrawRoomNodes()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(roomNodeStyle);
        }

        GUI.changed = true;
    }
}