using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DialogNodeGraphSO", menuName = "Scriptable Objects/Dialog/Dialog Node Graph", order = 1)]
public class DialogNodeGraphSO : ScriptableObject
{
    [HideInInspector] public DialogNodeTypeListSO dialogNodeTypeList;
    [HideInInspector] public List<DialogNodeSO> dialogNodeList = new List<DialogNodeSO>();
    [HideInInspector] public Dictionary<string, DialogNodeSO> dialogNodeDictionary = new Dictionary<string, DialogNodeSO>();
}