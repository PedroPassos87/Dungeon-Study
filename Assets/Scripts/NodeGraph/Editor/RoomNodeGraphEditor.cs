using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    
    //node layout 
    private float nodeWidth = 160f;
    private float nodeHeight = 75f;
    private int nodePadding = 25;
    private int nodeBorder = 12;
    
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;  //loading inbuilt texture
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding,nodePadding,nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder,nodeBorder,nodeBorder,nodeBorder);

    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(new Vector2(100f,100f), new Vector2(nodeWidth,nodeHeight)),roomNodeStyle);
        EditorGUILayout.LabelField("Node 1");
        GUILayout.EndArea();
    }
    
    
}
