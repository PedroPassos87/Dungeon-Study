# Dungeon Study in Unity

The main goal of this project is to develop a **dungeon** using Unity, where the rooms are procedurally generated. I'm using a **Node Graph** integrated with a custom **Editor** and a node **overview** for management. The focus is to maintain room generation **randomized**, yet **well-structured**, ensuring a unique experience for the player.

## What is a Node Graph?

A **Node Graph** is a visual tool used to organize and connect different components or entities in a modular way. In the context of Unity, Node Graphs are often used for:
- **Organizing the logic** of complex systems, such as dungeon generation, animations, or artificial intelligence (AI).
- **Visualizing the flow** of how entities (nodes) interact with each other by connecting data inputs and outputs.
- **Simplifying content creation** through a visual interface, where each node represents a functionality or execution block, and the connections between them define how data or actions flow between parts.

## Room Node Graph Editor Window

The first step is creating a custom **Room Node Graph Editor** window within Unity. This window serves as the interface for managing procedural dungeon generation. It provides a visual representation of the nodes (rooms) and their connections (paths).

The editor window is accessible via Unity's menu at `Window > Dungeon Editor > Room Node Graph Editor`.

### Key Features:

- **Node Layout and Appearance:** 
  Define the layout properties for nodes (e.g., width, height, padding, border) for visual consistency in the editor.
  
- **Connecting Nodes:** 
  Draw connecting lines between nodes to represent room-to-room connections within the dungeon.

- **User Interaction:**
  Handle mouse inputs for actions like selecting nodes, dragging them around, and creating connections. A context menu allows the creation of new nodes via a right-click.

- **Visual Feedback:**
  The editor visually updates to show real-time connections between rooms, helping manage the dungeon structure.

### Workflow:

1. **Node Creation:** 
   Right-click to create new room nodes in the graph editor.

2. **Node Connections:** 
   Drag lines from one node to another to form parent-child relationships between rooms, defining dungeon paths.

3. **Node Manipulation:** 
   Drag and arrange nodes to create a logical dungeon layout. Each node maintains information about its connections, ensuring a structured graph.

---

## Creating and Managing Room Nodes

The **Room Node ScriptableObject (RoomNodeSO)** is the core representation of each room within the graph. This script defines properties, event handling, and relationships between room nodes.

### Key Concepts:

- **Room Node Properties:**
  Each node has a unique ID, along with references to parent and child nodes (via ID lists) and a room type. These properties allow the node to track its connections and manage relationships in the dungeon layout.

- **Event Processing:**
  The script handles user interactions such as:
  - **Mouse Down:** Select nodes or initiate connections by right-clicking.
  - **Mouse Drag:** Drag nodes around the editor interface to rearrange them.
  
- **Node Connections:**
  Nodes can connect to each other by establishing parent-child relationships through ID references, which helps define the dungeon's layout and paths.

- **Node Types:**
  The `RoomNodeSO` includes a dropdown menu that lets users select the room type from a predefined list. This allows for flexible customization of each room's behavior or properties.

### Workflow:

1. **Creating a Room Node:**
   A new room node is created in the editor with a unique ID. The node type can be selected from a dropdown menu populated from the available room types.

2. **Managing Connections:**
   Nodes track connections to parent and child nodes, enabling easy visualization of room relationships in the dungeon.

3. **Visual Manipulation:**
   Users can select, drag, and arrange nodes within the graph editor to map out the dungeon structure.

4. **Event Handling:**
   The system processes mouse input for selecting, dragging, and connecting nodes, ensuring smooth interaction with the editor.

For detailed logic and implementation, refer to the **RoomNodeSO** script in `Scripts > NodeGraph > RoomNodeSO`.





