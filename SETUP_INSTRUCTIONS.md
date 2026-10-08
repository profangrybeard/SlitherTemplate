# Slither.io Template - Setup Instructions

This guide walks you through setting up the Unity scene and prefabs for the Slither.io template.

---

## 📋 Prerequisites

1. Unity 6000.6.0f1
2. Project opened in Unity Editor
3. All scripts are in `Assets/Scripts/` folders

---

## 🎨 Step 1: Create Required Tags

Unity uses tags to identify different types of GameObjects. We need three tags:

1. Open **Edit → Project Settings → Tags and Layers**
2. Under **Tags**, click the **+** button to add new tags:
   - `Player`
   - `Food`
   - `AISnake`

---

## 🎯 Step 2: Create Sprites

Since this is a minimal template, we'll use Unity's built-in circle sprite:

1. In the **Hierarchy**, right-click → **2D Object → Sprites → Circle**
2. This creates a white circle sprite we can use
3. Delete this GameObject (we just needed Unity to recognize the sprite type)

**Note:** Unity has a built-in circle sprite we'll reference in prefabs.

---

## 📦 Step 3: Create Prefabs

### **A. SnakeSegment Prefab**

1. **Create GameObject:**
   - Hierarchy → Right-click → **Create Empty**
   - Name it: `SnakeSegment`

2. **Add Components:**
   - **Add Component → Rendering → Sprite Renderer**
     - Sprite: Select "Circle" (Unity built-in)
     - Material: Select `Assets/Materials/PlayerSnakeMaterial`
   - **Add Component → Physics 2D → Circle Collider 2D**
     - Is Trigger: ✅ **Checked**
     - Radius: `0.4`
   - **Add Component → Scripts → Snake Segment**

3. **Transform Settings:**
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (0.8, 0.8, 1)

4. **Create Prefab:**
   - Drag `SnakeSegment` from Hierarchy into `Assets/Prefabs/` folder
   - Delete the GameObject from the Hierarchy

---

### **B. FoodPellet Prefab**

1. **Create GameObject:**
   - Hierarchy → Right-click → **Create Empty**
   - Name it: `FoodPellet`

2. **Add Components:**
   - **Add Component → Rendering → Sprite Renderer**
     - Sprite: Circle (Unity built-in)
     - Material: Select `Assets/Materials/FoodMaterial`
   - **Add Component → Physics 2D → Circle Collider 2D**
     - Is Trigger: ✅ **Checked**
     - Radius: `0.15`
   - **Add Component → Scripts → Food Pellet**

3. **Transform Settings:**
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (0.3, 0.3, 1)

4. **Set Tag:**
   - At top of Inspector: **Tag → Food**

5. **Create Prefab:**
   - Drag `FoodPellet` into `Assets/Prefabs/` folder
   - Delete from Hierarchy

---

### **C. AISnake Prefab**

1. **Create GameObject:**
   - Hierarchy → Right-click → **Create Empty**
   - Name it: `AISnake`

2. **Add Components:**
   - **Add Component → Rendering → Sprite Renderer**
     - Sprite: Circle (Unity built-in)
     - Material: Select `Assets/Materials/AISnakeMaterial`
   - **Add Component → Physics 2D → Circle Collider 2D**
     - Is Trigger: ✅ **Checked**
     - Radius: `0.5`
   - **Add Component → Physics 2D → Rigidbody 2D**
     - Body Type: **Kinematic**
     - Simulated: ✅ **Checked**
   - **Add Component → Scripts → AI Snake Controller**

3. **Transform Settings:**
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (1, 1, 1)

4. **Set Tag:**
   - Tag → **AISnake**

5. **Script Configuration:**
   - Segment Prefab: **Drag `SnakeSegment` prefab here**
   - Starting Segments: `3`
   - Move Speed: `4`

6. **Create Prefab:**
   - Drag `AISnake` into `Assets/Prefabs/` folder
   - Delete from Hierarchy

---

## 🎬 Step 4: Setup Main Scene

### **A. Configure Camera**

1. Select **Main Camera** in Hierarchy
2. In Inspector:
   - Projection: **Orthographic**
   - Size: `10`
   - Background: Choose a dark color (e.g., RGB: 20, 20, 30)

---

### **B. Create Manager Objects**

#### **GameManager**

1. Hierarchy → Right-click → **Create Empty**
2. Name: `GameManager`
3. Add Component → **Game Manager** script
4. Configure:
   - AI Snake Prefab: **Drag AISnake prefab here**
   - Number Of AI Snakes: `3`
   - Spawn Area Min: `(-20, -15)`
   - Spawn Area Max: `(20, 15)`

---

#### **FoodSpawner**

1. Create Empty → Name: `FoodSpawner`
2. Add Component → **Food Spawner** script
3. Configure:
   - Food Prefab: **Drag FoodPellet prefab here**
   - Initial Food Count: `20`
   - Spawn Area Min: `(-20, -15)`
   - Spawn Area Max: `(20, 15)`
   - Min Distance From Snakes: `2`

---

#### **ScoreManager**

1. Create Empty → Name: `ScoreManager`
2. Add Component → **Score Manager** script
3. We'll connect UI next

---

### **C. Create UI**

#### **Canvas Setup**

1. Hierarchy → Right-click → **UI → Canvas**
2. Canvas settings:
   - Render Mode: **Screen Space - Overlay**
   - UI Scale Mode: **Scale with Screen Size**
   - Reference Resolution: `1920 x 1080`

---

#### **Score Text**

1. Right-click Canvas → **UI → Legacy → Text**
2. Name: `ScoreText`
3. Configure:
   - Text: "Score: 0"
   - Font Size: `36`
   - Color: White
   - Alignment: Top-Left
   - Rect Transform:
     - Anchor: Top-Left
     - Position: (20, -20) from top-left
     - Width: `300`, Height: `50`

4. Connect to ScoreManager:
   - Select `ScoreManager` in Hierarchy
   - In Score Manager script component:
     - Score Text: **Drag ScoreText here**

---

#### **Game Over Text**

1. Right-click Canvas → **UI → Legacy → Text**
2. Name: `GameOverText`
3. Configure:
   - Text: "GAME OVER"
   - Font Size: `60`
   - Font Style: **Bold**
   - Color: Red
   - Alignment: Center (both horizontal and vertical)
   - Rect Transform:
     - Anchor: **Center**
     - Position: (0, 0)
     - Width: `800`, Height: `400`

4. **Disable by default:**
   - In Inspector, **uncheck the checkbox** next to GameObject name

5. Connect to ScoreManager:
   - Select `ScoreManager`
   - Game Over Text: **Drag GameOverText here**

---

### **D. Create Player Snake**

1. Create Empty → Name: `PlayerSnake`
2. Add Components:
   - **Sprite Renderer**
     - Sprite: Circle
     - Material: `PlayerSnakeMaterial`
   - **Circle Collider 2D**
     - Is Trigger: ✅ **Checked**
     - Radius: `0.5`
   - **Rigidbody 2D**
     - Body Type: **Kinematic**
     - Simulated: ✅ **Checked**
   - **Player Snake Controller** script

3. Transform:
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (1, 1, 1)

4. Set Tag: **Player**

5. Configure Script:
   - Segment Prefab: **Drag SnakeSegment prefab here**
   - Starting Segments: `3`
   - Move Speed: `5`
   - Rotation Speed: `200`
   - Collision Distance: `0.4`

---

## ✅ Step 5: Final Checks

### **Hierarchy Should Look Like:**

```
SlitherTemplate_Base
├── Main Camera
├── GameManager
├── FoodSpawner
├── ScoreManager
├── Canvas
│   ├── ScoreText
│   └── GameOverText
└── PlayerSnake
```

### **Assets Folder Should Have:**

```
Assets/
├── Scenes/
│   └── SlitherTemplate_Base.unity
├── Scripts/
│   ├── Player/
│   │   ├── PlayerSnakeController.cs
│   │   └── SnakeSegment.cs
│   ├── Food/
│   │   ├── FoodSpawner.cs
│   │   └── FoodPellet.cs
│   ├── AI/
│   │   └── AISnakeController.cs
│   ├── Managers/
│   │   ├── GameManager.cs
│   │   └── ScoreManager.cs
│   ├── Utils/
│   │   └── BoundsHelper.cs
│   └── README.md
├── Prefabs/
│   ├── SnakeSegment.prefab
│   ├── FoodPellet.prefab
│   └── AISnake.prefab
└── Materials/
    ├── PlayerSnakeMaterial.mat
    ├── AISnakeMaterial.mat
    └── FoodMaterial.mat
```

---

## 🎮 Step 6: Test the Game!

### **Play Mode Test Checklist:**

1. Press **Play** in Unity Editor
2. ✅ Mouse cursor visible
3. ✅ Player snake head follows mouse
4. ✅ 3 body segments trail behind head
5. ✅ 20 food pellets spawn (yellow circles)
6. ✅ 3 AI snakes spawn (red circles) and wander
7. ✅ Collecting food grows snake
8. ✅ Score increases when eating food
9. ✅ Snake dies on self-collision
10. ✅ Snake wraps at boundaries
11. ✅ Press **R** to restart after game over

---

## 🐛 Troubleshooting

### **Problem: "Missing Reference" errors**
**Solution:** Make sure all prefab fields are assigned:
- PlayerSnakeController needs SnakeSegment prefab
- AISnakeController needs SnakeSegment prefab
- GameManager needs AISnake prefab
- FoodSpawner needs FoodPellet prefab

### **Problem: Snake doesn't move**
**Solution:** Check PlayerSnakeController:
- Move Speed should be > 0
- Camera must be tagged "MainCamera"

### **Problem: No collisions happening**
**Solution:** Check:
- Colliders have "Is Trigger" checked
- Objects have correct tags (Player, Food, AISnake)
- At least one object in collision has Rigidbody2D

### **Problem: Food doesn't spawn**
**Solution:** Check FoodSpawner:
- Food Prefab is assigned
- Initial Food Count > 0
- Spawn area is reasonable (not too small)

### **Problem: AI doesn't spawn**
**Solution:** Check GameManager:
- AI Snake Prefab is assigned
- Number Of AI Snakes > 0

### **Problem: Score doesn't show**
**Solution:** Check ScoreManager:
- Score Text field is assigned
- Canvas is set to Screen Space - Overlay

---

## 🎓 For Students

Once everything is working, explore these files in order:

1. **Session 1**: `PlayerSnakeController.cs` (lines 80-115)
2. **Session 2**: `PlayerSnakeController.cs` (lines 117-144), `SnakeSegment.cs`
3. **Session 3**: `PlayerSnakeController.cs` (lines 146-184)
4. **Session 4**: `FoodSpawner.cs`, `FoodPellet.cs`
5. **Session 5**: `PlayerSnakeController.cs` (lines 211-240), `BoundsHelper.cs`
6. **Session 6**: `AISnakeController.cs`, `GameManager.cs`

Read the comments - they explain WHY and HOW!

---

## 🚀 Next Steps

After setup is complete:
1. Read `Assets/Scripts/README.md` for full teaching guide
2. Review each script's teaching comments
3. Experiment with Inspector values
4. Try the extension ideas in the README

---

**Happy coding! 🐍**
