using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;


    #region Editor Code
    //the following code should only be run in the Unity Editor
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;
    
    //initialise node

    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;
        
        //load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
    
    //draw node with the nodestyle
    public void Draw(GUIStyle nodeStyle)
    {
        // draw node box using begin area
        GUILayout.BeginArea(rect, nodeStyle);
        
        //start region to detect popup selection changes
        EditorGUI.BeginChangeCheck();
        
        //if room node has a parent or is of type entrance then display a label else display a popup
        if (parentRoomNodeIDList.Count>0 || roomNodeType.isEntrance)
        {
            //display a label that can't be changed
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            //display a popup using the roomnodetype name values that can be selected from 
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
            
            roomNodeType = roomNodeTypeList.list[selection];
            
            //if the room type selection has changed making child connections potentially invalid
            if (roomNodeTypeList.list[selection].isCorridor && !roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selection].isCorridor && roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selection].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i =childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        //get child room node
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);
                    
                        //if the child room node is not null
                        if (childRoomNode!= null)
                        {
                            //remove childID from parent room node
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                        
                            //remove parentID from child room node
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
        }
        
        if(EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    //populate a string array with the room node types to display that can be selected
    private string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i<roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }
    
    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //mouse down events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            //mouse up events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            //mouse drag events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            
            default:
                break;
        }
    }

    //process mouse down event
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //left click down
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        //right click down
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    //press left click down event
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;
        
        //toggle node selection
        isSelected = !isSelected;
    }
    
    //process right click down
    public void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }
    
    //process mouse up event
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        //left click down
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }
    
    //press left click up event
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }
    
    //process mouse drag event
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    //process left mouse drag event
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    //drag node
    private void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }
    
    //add childID to the node (returns true if the node has been added, false otherwise)
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        //check child node can be added validly to parent
        if (IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }
        
        return false;
    }

    //check the child node can be validly added to the parent node - return true if it can be otherwise retur false
    private bool IsChildRoomValid(string childID)
    {
        bool isConnectBossNodeAlredy = false;
        //check it there is already a connected boss room in the node graph
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count>0)
            {
                isConnectBossNodeAlredy = true;
            }
        }
        
        //if the child node has a type of boss room and there is aleready connected boss room node then return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectBossNodeAlredy)
        {
            return false;
        }
        
        //if the child node has a type of none  then return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
        {
            return false;
        }
        
        //if the node already has a child with this ID return false
        if (childRoomNodeIDList.Contains(childID))
        {
            return false;
        }
        
        //if this node ID and the child ID are the same then return false
        if (id == childID)
        {
            return false;
        }

        //if this childID is already in the parentID return false
        if (parentRoomNodeIDList.Contains(childID))
        {
            return false;
        }
        
        //
        //
        //---------ESSE DE BAIXO É O QUE DEVE SER MUDADO CASO QUEIRA MAIS FILHOS EM UM PAI---------
        //
        //
        
        //if the child node already has a parent return false
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
        {
            return false;
        }
        
        //if child is a corridor and this node a corridor return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
        {
            return false;
        }
        
        //if child is not a corridor and this node is not a corridor return false
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
        {
            return false;
        }
        
        //MAXIMO DE CORREDORES PODE SER MUDADO TAMBÉM
        //if adding a corridor check that this node has < the maximum permitted child corridors
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
        {
            return false;
        }
        
        //if the child room is an entrance return false - the entrance must always be the top level parent node
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
        {
            return false;
        }
        
        //if adding a room to a corridor check that this corridor node doesn't ahve a room added
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
        {
            return false;
        }

        return true;

    }

    //add parentID to the node (returns true if the node has been added, false otherwise)
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }
    
    //remove childID from the node (returns true if the node has been removed, false otherwise)
    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        //if the node contains the child ID then remove it
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }

        return false;
    }

    //remove parentID from the node (returns true if the node has been removed, false otherwise)
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        //if the node contains the parent ID then remove it
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }

        return false;
    }

#endif

    #endregion
}
