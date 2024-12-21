using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
    // GUI样式，用于设置节点外观
    private GUIStyle roomNodeStyle;

    // 当前正在编辑的房间节点图
    private static RoomNodeGraphSO currentRoomNodeGraph;

    // 房间节点类型列表
    private static RoomNodeTypeListSO roomNodeTypeList;

    // 房间节点的默认宽度和高度
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;

    // 节点的内边距和边框
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // 当编辑器窗口启用时调用
    private void OnEnable()
    {
        // 设置节点的样式（背景、文字颜色、内边距、边框）
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

        // 获取房间节点类型列表
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    // 菜单项：点击菜单项打开 Room Node Graph 编辑器窗口
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        // 获取或创建一个 RoomNodeGraphEditor 编辑器窗口实例
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    // 在双击资产时调用此方法
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        // 通过 InstanceID 获取房间节点图对象
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        // 如果获取到有效的房间节点图对象，打开编辑器窗口并设置当前节点图
        if (roomNodeGraph != null)
        {
            OpenWindow(); // 打开窗口
            currentRoomNodeGraph = roomNodeGraph; // 设置当前房间节点图
            return true; // 表示处理了这个双击事件
        }

        return false; // 未处理该事件
    }

    // GUI 绘制方法
    private void OnGUI()
    {
        // 如果当前房间节点图不为空，则绘制节点
        if (currentRoomNodeGraph != null)
        {
            ProcessEvents(Event.current); // 处理用户输入事件
            DrawRoomNodes(); // 绘制所有房间节点
        }

        // 如果有任何GUI元素发生变化，重新绘制窗口
        if (GUI.changed)
            Repaint();
    }

    // 处理事件方法
    private void ProcessEvents(Event currentEvent)
    {
        ProcessRoomNodeGraphEvent(currentEvent); // 处理房间节点图的事件
    }

    // 处理房间节点图的事件
    private void ProcessRoomNodeGraphEvent(Event currentEvent)
    {
        // 监听鼠标按下事件
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent); // 处理鼠标按下事件
                break;
        }
    }

    // 处理鼠标按下事件
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // 如果按下的是右键（button == 1）
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition); // 显示右键菜单
        }
    }

    // 显示右键菜单
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu(); // 创建一个菜单

        // 向菜单中添加创建房间节点的选项
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);

        // 显示菜单
        menu.ShowAsContext();
    }

    // 创建房间节点（调用此方法时不指定节点类型，默认为 none）
    private void CreateRoomNode(object mousePositionObj)
    {
        // 使用默认的房间节点类型（类型为 none）
        CreateRoomNode(mousePositionObj, roomNodeTypeList.list.Find(x => x.isNone));
    }

    // 创建房间节点，并设置节点类型
    private void CreateRoomNode(object mousePositionObj, RoomNodeTypeSO roomNodeType)
    {
        // 将传入的鼠标位置对象转换为 Vector2（假设鼠标位置是一个 2D 坐标）
        Vector2 mousePosition = (Vector2)mousePositionObj;

        // 创建一个新的 RoomNodeSO 实例（这是一个 ScriptableObject 类型的对象）
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // 将新创建的 roomNode 添加到当前房间节点图（currentRoomNodeGraph）的节点列表中
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // 初始化 roomNode，设置其矩形位置和大小，并传入当前的房间节点图和房间节点类型
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph,
            roomNodeType);

        // 将新创建的 roomNode 作为 Asset 添加到当前房间节点图中
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        // 保存所有资产，确保刚才创建的 Asset 被写入磁盘
        AssetDatabase.SaveAssets();
    }

    // 绘制房间节点
    private void DrawRoomNodes()
    {
        // 遍历当前房间节点图中的所有房间节点，并绘制它们
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(roomNodeStyle); // 调用 RoomNodeSO 的 Draw 方法来绘制每个节点
        }

        // 标记 GUI 发生了变化
        GUI.changed = true;
    }
}