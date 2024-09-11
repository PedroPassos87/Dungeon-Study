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

You can find how it is created under `Scripts > NodeGraph > Editor`.

