# Slither.io Template - Quick Start Checklist

Use this checklist to set up the template in Unity. Estimated time: 40 minutes.

---

## ✅ Pre-Setup Verification

- [ ] Unity 6000.6.0f1 installed
- [ ] Project opened in Unity Editor
- [ ] All Assets/Scripts folders visible in Project window
- [ ] 8 C# scripts compile without errors

---

## 📝 Part 1: Tags (5 minutes)

- [ ] Open **Edit → Project Settings → Tags and Layers**
- [ ] Click **+** button under Tags section
- [ ] Add tag: `Player`
- [ ] Add tag: `Food`
- [ ] Add tag: `AISnake`
- [ ] Close Project Settings

---

## 🎨 Part 2: Prefabs (15 minutes)

### SnakeSegment Prefab
- [ ] Hierarchy → Create Empty → Name: `SnakeSegment`
- [ ] Add Component → Sprite Renderer
  - [ ] Sprite: Circle (Unity built-in)
  - [ ] Material: `Assets/Materials/PlayerSnakeMaterial`
- [ ] Add Component → Circle Collider 2D
  - [ ] Is Trigger: ✅ Checked
  - [ ] Radius: `0.4`
- [ ] Add Component → Snake Segment (script)
- [ ] Transform Scale: `(0.8, 0.8, 1)`
- [ ] Drag to `Assets/Prefabs/` folder
- [ ] Delete from Hierarchy

### FoodPellet Prefab
- [ ] Hierarchy → Create Empty → Name: `FoodPellet`
- [ ] Add Component → Sprite Renderer
  - [ ] Sprite: Circle
  - [ ] Material: `Assets/Materials/FoodMaterial`
- [ ] Add Component → Circle Collider 2D
  - [ ] Is Trigger: ✅ Checked
  - [ ] Radius: `0.15`
- [ ] Add Component → Food Pellet (script)
- [ ] Transform Scale: `(0.3, 0.3, 1)`
- [ ] Tag: `Food`
- [ ] Drag to `Assets/Prefabs/` folder
- [ ] Delete from Hierarchy

### AISnake Prefab
- [ ] Hierarchy → Create Empty → Name: `AISnake`
- [ ] Add Component → Sprite Renderer
  - [ ] Sprite: Circle
  - [ ] Material: `Assets/Materials/AISnakeMaterial`
- [ ] Add Component → Circle Collider 2D
  - [ ] Is Trigger: ✅ Checked
  - [ ] Radius: `0.5`
- [ ] Add Component → Rigidbody 2D
  - [ ] Body Type: `Kinematic`
  - [ ] Simulated: ✅ Checked
- [ ] Add Component → AI Snake Controller (script)
  - [ ] Segment Prefab: Drag `SnakeSegment` prefab here
  - [ ] Starting Segments: `3`
  - [ ] Move Speed: `4`
- [ ] Transform Scale: `(1, 1, 1)`
- [ ] Tag: `AISnake`
- [ ] Drag to `Assets/Prefabs/` folder
- [ ] Delete from Hierarchy

---

## 🎬 Part 3: Scene Setup (20 minutes)

### Camera
- [ ] Select Main Camera
- [ ] Projection: `Orthographic`
- [ ] Size: `10`
- [ ] Background: Dark color (RGB: 20, 20, 30)

### GameManager
- [ ] Hierarchy → Create Empty → Name: `GameManager`
- [ ] Add Component → Game Manager (script)
- [ ] AI Snake Prefab: Drag `AISnake` prefab here
- [ ] Number Of AI Snakes: `3`
- [ ] Spawn Area Min: `(-20, -15)`
- [ ] Spawn Area Max: `(20, 15)`

### FoodSpawner
- [ ] Hierarchy → Create Empty → Name: `FoodSpawner`
- [ ] Add Component → Food Spawner (script)
- [ ] Food Prefab: Drag `FoodPellet` prefab here
- [ ] Initial Food Count: `20`
- [ ] Spawn Area Min: `(-20, -15)`
- [ ] Spawn Area Max: `(20, 15)`
- [ ] Min Distance From Snakes: `2`

### ScoreManager
- [ ] Hierarchy → Create Empty → Name: `ScoreManager`
- [ ] Add Component → Score Manager (script)
- [ ] (We'll connect UI next)

### UI Canvas
- [ ] Hierarchy → UI → Canvas
- [ ] Canvas Scaler:
  - [ ] UI Scale Mode: `Scale with Screen Size`
  - [ ] Reference Resolution: `1920 x 1080`

### ScoreText
- [ ] Right-click Canvas → UI → Legacy → Text
- [ ] Name: `ScoreText`
- [ ] Text: `"Score: 0"`
- [ ] Font Size: `36`
- [ ] Color: White
- [ ] Alignment: Top-Left
- [ ] Rect Transform:
  - [ ] Anchor: Top-Left
  - [ ] Position X: `100`, Y: `-30`
  - [ ] Width: `300`, Height: `50`

### GameOverText
- [ ] Right-click Canvas → UI → Legacy → Text
- [ ] Name: `GameOverText`
- [ ] Text: `"GAME OVER"`
- [ ] Font Size: `60`
- [ ] Font Style: Bold
- [ ] Color: Red
- [ ] Alignment: Center (both)
- [ ] Rect Transform:
  - [ ] Anchor: Center
  - [ ] Position: `(0, 0)`
  - [ ] Width: `800`, Height: `400`
- [ ] **Uncheck GameObject active** (disable it)

### Connect UI to ScoreManager
- [ ] Select `ScoreManager` in Hierarchy
- [ ] Score Text field: Drag `ScoreText` here
- [ ] Game Over Text field: Drag `GameOverText` here

### PlayerSnake
- [ ] Hierarchy → Create Empty → Name: `PlayerSnake`
- [ ] Add Component → Sprite Renderer
  - [ ] Sprite: Circle
  - [ ] Material: `PlayerSnakeMaterial`
- [ ] Add Component → Circle Collider 2D
  - [ ] Is Trigger: ✅ Checked
  - [ ] Radius: `0.5`
- [ ] Add Component → Rigidbody 2D
  - [ ] Body Type: `Kinematic`
  - [ ] Simulated: ✅ Checked
- [ ] Add Component → Player Snake Controller (script)
  - [ ] Segment Prefab: Drag `SnakeSegment` prefab here
  - [ ] Starting Segments: `3`
  - [ ] Move Speed: `5`
  - [ ] Rotation Speed: `200`
  - [ ] Collision Distance: `0.4`
- [ ] Tag: `Player`
- [ ] Position: `(0, 0, 0)`
- [ ] Scale: `(1, 1, 1)`

---

## 🧪 Part 4: Testing (5 minutes)

- [ ] Save scene (Ctrl+S / Cmd+S)
- [ ] Press Play button
- [ ] **Mouse cursor visible and moves**
- [ ] **Player snake head follows cursor**
- [ ] **3 body segments trail behind head**
- [ ] **20 food pellets spawn (yellow circles)**
- [ ] **3 AI snakes spawn (red circles)**
- [ ] **AI snakes wander randomly**
- [ ] Move snake to collect food
- [ ] **Snake grows when eating food**
- [ ] **Score increases (+10 per food)**
- [ ] **Food respawns after collection**
- [ ] Try to hit your own body
- [ ] **Game over on self-collision**
- [ ] **"GAME OVER" text appears**
- [ ] Press **R** key
- [ ] **Game restarts successfully**

---

## 🎉 Success!

If all tests pass, the template is ready!

### Next Steps:
1. Read `Assets/Scripts/README.md` for full teaching guide
2. Review session-by-session breakdown
3. Prepare for Session 1 (Feb 4): Update vs FixedUpdate
4. Bookmark `SETUP_INSTRUCTIONS.md` for detailed troubleshooting

---

## 🐛 Quick Troubleshooting

### No movement?
- Check PlayerSnakeController has Move Speed > 0
- Verify Main Camera is tagged "MainCamera"

### No collisions?
- Check "Is Trigger" is checked on all Collider2Ds
- Verify tags are correctly assigned

### No food spawning?
- Check Food Prefab is assigned in FoodSpawner
- Verify Initial Food Count > 0

### No AI spawning?
- Check AI Snake Prefab is assigned in GameManager
- Verify Number Of AI Snakes > 0

### Score not showing?
- Check Score Text is assigned in ScoreManager
- Verify Canvas is in Screen Space - Overlay mode

---

**Total Time:** ~40 minutes
**Status:** Ready for classroom use February 4! 🎓🐍
