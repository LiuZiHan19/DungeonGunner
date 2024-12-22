using UnityEditor;
using UnityEditor.Callbacks;

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
}