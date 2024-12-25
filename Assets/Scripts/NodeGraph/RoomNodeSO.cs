using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    // 以下字段是为编辑器使用的隐藏字段，不会在 Inspector 中显示
    [HideInInspector] public string id; // 唯一标识符 (GUID)，由 C# 生成
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>(); // 记录父节点 ID 的列表
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>(); // 记录子节点 ID 的列表
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph; // 当前房间节点所属的房间节点图
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList; // 所有房间节点类型的列表
    public RoomNodeTypeSO roomNodeType; // 当前节点的类型

    #region Editor Code

#if UNITY_EDITOR

    // 以下字段和方法仅在编辑器模式下可用
    [HideInInspector] public Rect rect; // 界面上绘制节点的位置和大小

    // 初始化房间节点对象的相关属性
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect; // 设置节点的矩形区域（位置和大小）
        this.id = Guid.NewGuid().ToString(); // 生成一个新的唯一 ID
        this.name = "RoomNode"; // 设置节点的名称
        this.roomNodeGraph = nodeGraph; // 设置房间节点所属的图
        this.roomNodeType = roomNodeType; // 设置节点类型

        // 获取资源管理器中所有房间节点类型列表
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    // 绘制房间节点的 GUI 界面
    public void Draw(GUIStyle style)
    {
        // 开始绘制指定位置的 GUI 区域
        GUILayout.BeginArea(rect, style);

        // 检查是否有任何变化（例如节点类型选择发生了变化）
        EditorGUI.BeginChangeCheck();

        // 获取当前节点类型在房间节点类型列表中的索引
        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

        // 显示一个下拉框，供用户选择不同的房间节点类型
        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

        // 根据用户选择的索引，更新当前节点的类型
        roomNodeType = roomNodeTypeList.list[selection];

        // 如果节点类型发生变化，标记该对象为脏，提示需要保存
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        // 结束当前的 GUI 绘制区域
        GUILayout.EndArea();
    }
    
    // 获取所有可在节点图编辑器中显示的房间节点类型
    public string[] GetRoomNodeTypesToDisplay()
    {
        // 创建一个字符串数组来存储显示的房间节点类型名称
        string[] roomNodeTypes = new string[roomNodeTypeList.list.Count];

        // 遍历所有房间节点类型列表，将可以显示在编辑器中的类型名称添加到数组
        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor) // 判断该类型是否可以在节点图中显示
            {
                roomNodeTypes[i] = roomNodeTypeList.list[i].roomNodeTypeName; // 添加类型名称
            }
        }

        // 返回可以显示的房间节点类型名称数组
        return roomNodeTypes;
    }

#endif

    #endregion
}
