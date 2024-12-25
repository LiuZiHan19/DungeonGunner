using UnityEngine;

[CreateAssetMenu(fileName = "DialogNodeType_", menuName = "Scriptable Objects/Dialog/Dialog Node Type", order = 2)]
public class DialogNodeTypeSO : ScriptableObject
{
    public string dialogNodeTypeName;

    public bool displayInNodeGraphEditor = true;

    public bool isStartNode; // 是否是对话的起始节点

    public bool isEndNode; // 是否是对话的结束节点

    public bool isNormalNode; // 是否是普通对话节点

    public bool isChoiceNode; // 是否是玩家选择节点

    public bool isNarrationNode; // 是否是旁白节点

    public bool isEventNode; // 是否是事件触发节点

    public bool isNone;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(dialogNodeTypeName), dialogNodeTypeName);
    }
#endif

    #endregion
}