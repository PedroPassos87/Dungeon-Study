# Dungeon Study in Unity

The main idea of this project is to develop a **dungeon** using Unity, where the rooms are procedurally generated. I'm using a **Node Graph** integrated with a custom **Editor** and a node **overview** for management. The goal is to keep the room generation **randomized**, yet **well-structured**, ensuring a unique experience for the player.

## What is a Node Graph?

A **Node Graph** is a visual tool used to organize and connect different components or entities in a modular way. In the context of Unity, Node Graphs are often used for:
- **Organizing the logic** of complex systems, such as dungeon generation, animations, or artificial intelligence (AI).
- **Visualizing the flow** of how entities (nodes) interact with each other by connecting data inputs and outputs.
- **Simplifying content creation** through a visual interface, where each node represents a functionality or execution block, and the connections between them define how data or actions flow between parts.

## Creating the Room Node Graph Editor Window

The first step in the project is to create a custom editor window within Unity. This window, named **Room Node Graph Editor**, serves as the interface for managing procedural dungeon generation. The purpose of this editor is to provide a visual representation of the nodes and their connections.

By implementing this editor window, we create a dedicated space where we can visualize, manage, and interact with the nodes that represent the various rooms of the dungeon. This interface allows us to add, remove, and connect rooms, ensuring a clear and well-structured dungeon layout.

The window is accessed via the Unity menu under `Window > Dungeon Editor > Room Node Graph Editor`.

## Adding Functionality to the Room Node Graph Editor

Here, we extend the **Room Node Graph Editor** by introducing key functionalities for managing and visualizing the room nodes and their connections.

### Key Concepts:

- **Node Layout and Appearance:** 
  We define the layout properties of each node, such as width, height, padding, and border, to ensure the nodes are visually distinct and consistently styled within the editor.

- **Connecting Lines Between Nodes:**
  To represent the relationships between rooms, we include functionality to draw connecting lines between nodes. This helps visualize how rooms are linked in the dungeon.

- **Event Handling:**
  The editor window listens for various user interactions, such as mouse clicks and drags. These interactions allow us to:
  - Create new nodes at the clicked position.
  - Draw connection lines between nodes.
  - Manage interactions between nodes, like dragging or selecting.

- **Context Menu for Node Creation:**
  When right-clicking within the graph editor, a context menu appears that allows the creation of new room nodes at the mouse position.
  
### Workflow Overview:

1. **Node Creation:**
   Users can create new room nodes by right-clicking in the graph editor and selecting an option from the context menu. These nodes are then placed at the mouse position.

2. **Node Management:**
   Each room node is stored within a list and can be manipulated via user interactions. Nodes can be connected to each other, forming a visual graph of the dungeon layout.

3. **Dragging Lines to Connect Nodes:**
   Users can drag lines from one node to another, establishing a connection between parent and child nodes. This connection represents the paths between rooms in the dungeon.

4. **Visual Representation:**
   The editor window provides a visual representation of the dungeon's structure, allowing us to see the nodes and their connections in real-time.

You can find more details on the script's implementation in `Scripts > NodeGraph > Editor`.


