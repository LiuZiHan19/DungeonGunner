using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Store a list of room node types
/// </summary>
[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Objects/Dungeon/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{
    public List<RoomNodeTypeSO> list;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif

    #endregion
}