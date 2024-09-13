using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;
using Vector2 = UnityEngine.Vector2;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    
    //node layout values
    private float nodeWidth = 160f;
    private float nodeHeight = 75f;
    private int nodePadding = 25;
    private int nodeBorder = 12;
    
    //connecting line values
    private float connectingLineWidth = 3f;
    
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
            //draw line if being dragged
            DrawDraggedLine();                 //this method is called first so the track line gets draw first and will appear behind to the room node
            
            
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

    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            //draw line from node to line position
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }


    private void ProcessEvents(Event currentEvent)
    {
        //get room node that mouse is over if its is null or not corrently been dragged
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        //if mouse isn't over a room node or we are currently dragging a line from the room node then process graph events
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvent(currentEvent);
        }
        //else process room node events
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    
    //check to see to mouse is over a room node - then return the room node 
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
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
            
            // process mouse up events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            
            //process mouse drag event
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
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
    
    //process mouse up events
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        //if releasing the right mouse button and currently dragging a line
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            //check if over a room node
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                //if so set is as a child of the parent room node if it can be added
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    //set parent ID in child room node
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }
            ClearLineDrag();
        }
    }
    
    //process mouse drag event
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        //process right click drag event = draw line
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
    }

    //process right mouse drag event = draw line
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    //drag connecting line from room node
    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }
    
    //clear line drag from a room node
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
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
