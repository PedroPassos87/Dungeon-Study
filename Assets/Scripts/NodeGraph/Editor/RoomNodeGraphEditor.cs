using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeTypeListSO roomNodeTypeList;
    
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
        //layout style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;  //loading inbuilt texture
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding,nodePadding,nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder,nodeBorder,nodeBorder,nodeBorder);
        
        //load room node types
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// opens the room node graph editor window if a room node graph scriptable object asset is double clicked in the inspector
    /// </summary>
    [OnOpenAsset(0)] //Need the namespace UnityEditor.Callback
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }

        return false;
    }


    //draw Editor GUI
    private void OnGUI()
    {
        //if a scriptable object of type RoomNodeGraphSO has been selected the process
        if (currentRoomNodeGraph != null)
        {
            //process events
            ProcessEvents(Event.current);
            
            //draw room nodes
            DrawRoomNodes();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }
    

    private void ProcessEvents(Event currentEvent)
    {
        ProcessRoomNodeGraphEvent(currentEvent);
    }

    //Process Room Node Graph Events
    private void ProcessRoomNodeGraphEvent(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            default:
                break;
        }
        
    }

    //process mouse down events on the room node graph (not over a node)
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //process right click mouse down on graph event
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        
        menu.ShowAsContext();
    }

    //create room node at the mouse position
    private void CreateRoomNode(object mousePositionObject)
    {
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    //create a room node at mouse position - overloaded to also pass in RoomNodeType
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;
        
        //create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        
        // add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        
        // set room node values
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)),currentRoomNodeGraph,roomNodeType);
        
        // add room node to room node graph scrpitable object asset database
        AssetDatabase.AddObjectToAsset(roomNode,currentRoomNodeGraph);
        
        AssetDatabase.SaveAssets();
        
    }
    
    //draw room nodes in the graph window
    private void DrawRoomNodes()
    {
        //loop through all room nodes and draw them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(roomNodeStyle);
        }

        GUI.changed = true;
    }
}
