using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    // guid generated by c#
    [HideInInspector] public string guid;
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeSO  roomNodeGraph;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    public RoomNodeTypeSO roomNodeType;
}