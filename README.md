# 💎 Match-3 Puzzle

A 2D Match-3 puzzle game developed with **Unity and C#**, focusing on grid-based gameplay, tile swapping, matching logic, cascading effects, and game state management.

## 🎮 Game Overview

**Match-3 Puzzle** is a classic tile-matching game where players swap adjacent gems to create matches of three or more identical tiles.

The project was developed to practice and demonstrate gameplay programming, grid systems, object interaction, and reusable game architecture in Unity.

## ✨ Features

* 💎 Match-3 Puzzle Gameplay
* 🔄 Tile Swapping System
* 🧩 Grid-Based Board
* 💥 Match Detection
* ⬇️ Tile Falling / Cascading
* 🎲 Automatic Tile Generation
* 🎯 Score System
* ❤️ Game State Management
* 🖱️ Mouse / Touch Interaction
* 📱 Android Support

## 🛠️ Technologies

* **Unity**
* **C#**
* 2D Physics
* SpriteRenderer
* Unity UI
* Coroutines
* ScriptableObject
* Git / GitHub

## 🧠 Gameplay Systems

### Grid System

The game board is represented using a 2D grid system. Each tile stores its current grid position and configuration.

### Tile Swapping

Players can select and swap adjacent tiles.

The system validates the swap before updating the board.

### Match Detection

The game checks horizontal and vertical directions to detect matching tiles.

Example:

```text
💎 💎 💎
```

When three or more identical tiles are connected, they are removed from the board.

### Cascading System

After matched tiles are removed:

1. Empty spaces are detected.
2. Tiles above fall down.
3. New tiles are generated.
4. The board checks for additional matches.

This creates the classic Match-3 cascade effect.

### ScriptableObject Configuration

Gem properties are separated into configuration data using ScriptableObjects, making it easier to create and modify different gem types without changing gameplay code.

## 📁 Project Structure

```text
Assets/
├── Scripts/
│   ├── Gameplay/
│   ├── Board/
│   ├── Swap/
│   ├── UI/
│   └── Data/
│
├── ScriptableObjects/
├── Prefabs/
├── Scenes/
├── Sprites/
└── Audio/
```

## 🎯 Technical Highlights

* Implemented a reusable **Grid / Board System**
* Created a **Tile Swapping System**
* Implemented **Match Detection**
* Implemented **Tile Falling and Cascading**
* Used **ScriptableObject** for gem configuration
* Used **Coroutines** to control gameplay sequences
* Separated gameplay logic into reusable C# components

## 📱 Platform

* Windows
* Android

## 🎥 Demo

> Add your gameplay video here.

**YouTube:**https://youtu.be/ea6OPHWEZjU

## 📸 Screenshots

Add screenshots of:

* Main Menu
* Match-3 Board
* Tile Swapping
* Match Effect
* Game UI

## 🚀 How to Run

1. Clone this repository.
2. Open the project with the recommended Unity Editor version.
3. Open the main gameplay scene.
4. Press **Play**.

## 👨‍💻 Developer

**Hoàng Hữu Hậu**

Unity Developer Intern Candidate

**Skills demonstrated:**

* C#
* Unity 2D
* Gameplay Programming
* Grid Systems
* Match-3 Mechanics
* UI Programming
* ScriptableObject
* Git / GitHub
