using UnityEngine;

[CreateAssetMenu(fileName = "DialogNodeType_", menuName = "Scriptable Objects/Dialog/Dialog Node Type", order = 2)]
public class DialogNodeTypeSO : ScriptableObject
{
    public string dialogNodeTypeName;

    public bool displayInNodeGraphEditor = true;

    public bool isStartConversation; // 是否是对话的起始节点

    public bool isEndConversation; // 是否是对话的结束节点

    public bool isNormalConversation; // 是否是普通对话节点

    public bool isChoiceConversation; // 是否是玩家选择节点

    public bool isNarrationConversation; // 是否是旁白节点

    public bool isEventConversation; // 是否是事件触发节点

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