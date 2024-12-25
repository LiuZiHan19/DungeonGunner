using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class DialogNodeGraphEditor : EditorWindow
{
    private static DialogNodeGraphSO currentDialogNodeGraph;

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
        }
    }

    private void ProcessEvents(Event currentEvent)
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

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu(); // 创建一个菜单

        // 向菜单中添加创建房间节点的选项
        menu.AddItem(new GUIContent("Create Dialog Node"), false, null, mousePosition);

        // 显示菜单
        menu.ShowAsContext();
    }
}