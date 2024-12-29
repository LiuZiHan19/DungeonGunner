using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogTypeListSO", menuName = "Scriptable Objects/Dialog/Dialog Node Type List")]
public class DialogNodeTypeListSO : ScriptableObject
{
    public List<DialogNodeTypeSO> list;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif

    #endregion
}