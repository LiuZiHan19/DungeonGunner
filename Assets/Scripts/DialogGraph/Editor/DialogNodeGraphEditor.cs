using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class DialogNodeGraphEditor : EditorWindow
{
    private GUIStyle dialogNodeStyle;
    private static DialogNodeGraphSO currentDialogNodeGraph;
    private static DialogNodeTypeListSO dialogNodeTypeList;
    private DialogNodeSO currentDialogNode;

    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    private void OnEnable()
    {
        dialogNodeStyle = new GUIStyle
        {
            normal =
            {
                background = EditorGUIUtility.Load("Node1") as Texture2D,
                textColor = Color.white
            },
            padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding),
            border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder)
        };

        dialogNodeTypeList = GameResources.Instance.dialogNodeTypeList;
    }

    [MenuItem("Dialog Graph Editor", menuItem = "Window/Dungeon Editor/Dialog Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<DialogNodeGraphEditor>("Dialog Graph Editor");
    }

    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        DialogNodeGraphSO dialogNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as DialogNodeGraphSO;

        if (dialogNodeGraph != null)
        {
            OpenWindow();
            currentDialogNodeGraph = dialogNodeGraph; // 设置当前房间节点图
            return true;
        }

        return false;
    }
    
    private void OnGUI()
    {
        if (currentDialogNodeGraph != null)
        {
            ProcessEvents(Event.current);

            DrawRoomNodes();
        }

        if (GUI.changed)
            Repaint();
    }

    #region Process Events

    private void ProcessEvents(Event currentEvent)
    {
        if (currentDialogNode == null || currentDialogNode.isLeftClickDragging == false)
        {
            currentDialogNode = IsMouseOverDialogNode(currentEvent);
        }

        if (currentDialogNode == null)
        {
            ProcessDialogNodeGraphEvent(currentEvent);
        }
        else
        {
            currentDialogNode.ProcessEvent();
        }
    }

    private DialogNodeSO IsMouseOverDialogNode(Event currentEvent)
    {
        for (int i = currentDialogNodeGraph.dialogNodeList.Count - 1; i > -1; i--)
        {
            if (currentDialogNodeGraph.dialogNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentDialogNodeGraph.dialogNodeList[i];
            }
        }

        return null;
    }

    private void ProcessDialogNodeGraphEvent(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    #endregion

    #region Context Menu

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Dialog Node"), false, CreateDialogNode, mousePosition);

        menu.ShowAsContext();
    }

    private void CreateDialogNode(object mousePositionObj)
    {
        CreateDialogNode(mousePositionObj, dialogNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateDialogNode(object mousePositionObj, DialogNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObj;

        DialogNodeSO roomNode = ScriptableObject.CreateInstance<DialogNodeSO>();

        currentDialogNodeGraph.dialogNodeList.Add(roomNode);

        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentDialogNodeGraph,
            roomNodeType);

        AssetDatabase.AddObjectToAsset(roomNode, currentDialogNodeGraph);

        AssetDatabase.SaveAssets();
    }

    #endregion

    private void DrawRoomNodes()
    {
        foreach (var roomNode in currentDialogNodeGraph.dialogNodeList)
        {
            roomNode.Draw(dialogNodeStyle);
        }

        GUI.changed = true;
    }
}