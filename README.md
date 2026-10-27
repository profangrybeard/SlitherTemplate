# 🐍 Slither.io Unity Template

**GAME 220: Core Programming - Loops and Lists**

A comprehensive Unity 2D educational template teaching programming fundamentals through game development. Students build a fully-functional Slither.io clone over 3 weeks while learning loops, lists, and proper game architecture.

---

## 🎯 Project Overview

**Duration:** 3 weeks (6 class sessions)
**Dates:** February 4 - February 16
**Unity Version:** 6000.6.0f1
**Renderer:** Built-in 2D
**Skill Level:** First-time programmers learning core concepts

### What Students Build:
- Mouse-controlled snake that grows by eating food
- Body segments that follow the head in a chain (each one follows the segment ahead)
- AI opponents that wander and compete for food
- Collision detection (AI snakes, food) and boundary wrapping
- Score tracking and game over/restart flow

---

## 📚 Learning Objectives

### Primary Concepts:
- **Lists (`List<T>`)** - Dynamic collections for segments, food, AI
- **for loops** - Counted iteration (spawning, initialization)
- **foreach loops** - Collection iteration (updating segments)
- **while loops** - Conditional iteration (validation, limits)

### Secondary Concepts:
- **Update vs FixedUpdate** - Input responsiveness vs physics consistency
- **Component communication** - Scripts working together
- **Collision detection** - Unity's OnTriggerEnter2D system
- **Code reuse** - AI using same patterns as player

---

## 🗓️ 6-Session Progression

| Session | Date | System | Key Concepts | Status |
|---------|------|--------|--------------|--------|
| 1 | Feb 4 | Snake Movement & Input | Update/FixedUpdate, Vector math | ✅ |
| 2 | Feb 6 | Body Segments Part 1 | List<T>, for loops | ✅ |
| 3 | Feb 9 | Body Segments Part 2 & Growth | List indexing, for loops | ✅ |
| 4 | Feb 11 | Food System | List management, while loops | ✅ |
| 5 | Feb 13 | Collision Detection | Loop optimization, boundaries | ✅ |
| 6 | Feb 16 | AI Snakes & Polish | Code reuse, multiple Lists | ✅ |

---

## 🚀 Quick Start

### 1. Setup (One-time)
```bash
# Clone and open in Unity 6000.6.0f1
git clone [your-repo-url]
cd SlitherTemplate
# Open project in Unity Hub
```

### 2. Configure Scene
Follow detailed instructions in: **[SETUP_INSTRUCTIONS.md](SETUP_INSTRUCTIONS.md)**

Key steps:
- Create 3 tags: Player, Food, AISnake
- Build 3 prefabs: SnakeSegment, FoodPellet, AISnake
- Setup scene with GameManager, FoodSpawner, ScoreManager, PlayerSnake
- Create UI (score display, game over text)

### 3. Test
Press **Play** in Unity. You should see:
- Player snake following mouse
- 20 food pellets spawning
- 3 AI snakes wandering
- Score increasing on food collection

---

## 📁 Project Structure

```
Assets/
├── Scenes/
│   └── SlitherTemplate_Base.unity  # Main game scene
├── Scripts/
│   ├── Player/
│   │   ├── PlayerSnakeController.cs   # Mouse input, movement, collision
│   │   └── SnakeSegment.cs            # Individual segment following
│   ├── Food/
│   │   ├── FoodSpawner.cs             # Spawn/manage food with Lists
│   │   └── FoodPellet.cs              # Collision detection
│   ├── AI/
│   │   └── AISnakeController.cs       # Random wandering AI
│   ├── Managers/
│   │   ├── GameManager.cs             # Game initialization, AI spawning
│   │   └── ScoreManager.cs            # Score tracking, UI updates
│   ├── Utils/
│   │   └── BoundsHelper.cs            # Static boundary utilities
│   └── README.md                       # Full teaching guide
├── Prefabs/
│   ├── SnakeSegment.prefab
│   ├── FoodPellet.prefab
│   └── AISnake.prefab
└── Materials/
    ├── PlayerSnakeMaterial.mat        # Green (materials aren't assigned;
    ├── AISnakeMaterial.mat            # Red    prefabs set Sprite Renderer
    └── FoodMaterial.mat               # Yellow Color instead)
```

---

## 🤖 AI Tutor (Google Antigravity)

VS Code recommends the **Google Antigravity** extension when students open the project
(`.vscode/extensions.json`). It runs in **tutor mode**:
[`.agents/rules/tutor-mode.md`](.agents/rules/tutor-mode.md) (with [`AGENTS.md`](AGENTS.md) as a
pointer for other AI tools) tells it to guide with hints and questions rather than write code.
It won't write students' new food types or power-ups, their AI behaviors (boundary avoidance,
a second AI type), or the extension features below, and it won't answer the `// Why...?`
questions in the scripts. Copilot and VS Code's inline suggestions are turned off in
`.vscode/settings.json`. Students sign in by following [AI_TUTOR.md](AI_TUTOR.md); in-panel
sign-in is currently broken, so it uses PowerShell instead.

**Check it before class:** open VS Code, sign in, and ask the tutor to "write the code for a new
food type worth 50 points." It should decline and offer a hint.

---

## 🎓 For Teachers

### Session-by-Session Guide

**Session 1: Movement Basics**
- Focus: Why Update for input, FixedUpdate for physics
- Demo: Add Debug.Log to both, show different call rates
- File: `PlayerSnakeController.cs` (`Update()`, `MoveHead()`)

**Session 2: Introduction to Lists**
- Focus: Dynamic collections vs fixed arrays
- Demo: Try adding to array (can't!), then List.Add()
- Files: `PlayerSnakeController.cs` (`CreateStartingBody()`, `AddSegment()`), `SnakeSegment.cs`

**Session 3: Growth and the Follow Chain**
- Focus: `bodySegments[bodySegments.Count - 1]` (the last item) and growing with a `for` loop
- Demo: Raise the `Grow()` amount and watch `bodySegments.Count` in the Console
- File: `PlayerSnakeController.cs` (`AddSegment()`, `Grow()`)

**Session 4: List Management**
- Focus: Add/Remove operations, validation loops
- Demo: Change food count, watch List size
- Files: `FoodSpawner.cs`, `FoodPellet.cs`

**Session 5: Loop Optimization**
- Focus: boundary wrapping, `foreach` to move every segment, game over flow
- Demo: Swap `WrapPosition` for `ClampPosition` and compare
- Files: `PlayerSnakeController.cs` (`CheckBoundaries()`, `Die()`), `BoundsHelper.cs`

**Session 6: Code Reuse**
- Focus: Same patterns work for player and AI
- Demo: Side-by-side comparison of PlayerSnake vs AISnake
- Files: `AISnakeController.cs`, `GameManager.cs`

### Assessment Ideas

**Formative:**
- Can students explain Update vs FixedUpdate?
- Can they describe List.Add() vs array indexing?
- Can they explain why the body moves with a `foreach` when the head wraps?

**Summative:**
- Add a second food type worth 50 points
- Make AI avoid boundaries proactively
- Create speed boost power-up
- Build a second AI personality type

---

## 🎮 For Students

### Getting Started
1. Read `SETUP_INSTRUCTIONS.md` first
2. Once scene works, explore scripts in session order
3. **Read each script's header**, then the matching section of `LOOPS_AND_LISTS_CONCEPTS.md` for the WHY and HOW
4. Answer the `// Why...?` questions in the scripts for yourself
5. Experiment with Inspector values
6. Try the extension ideas below
7. **Set up your AI tutor:** follow [AI_TUTOR.md](AI_TUTOR.md) to sign in to Google Antigravity in VS Code. It gives hints and asks questions; it won't write your features for you.

### Understanding the Code

**Key Pattern:** Input in Update, Physics in FixedUpdate
```csharp
void Update() {
    // Read mouse input - needs immediate responsiveness
}

void FixedUpdate() {
    // Move snake - needs consistent physics timing (50Hz)
}
```

**Key Pattern:** Lists for dynamic collections
```csharp
List<SnakeSegment> bodySegments = new List<SnakeSegment>();
bodySegments.Add(newSegment);  // Grows dynamically!
```

**Key Pattern:** Following the last item in a List
```csharp
SnakeSegment lastSegment = bodySegments[bodySegments.Count - 1];  // Last item
newSegment.SetTarget(lastSegment.transform, segmentSpacing);       // Follow it
```

### Extension Ideas

**Easy:**
- Different colored food worth different points
- Speed boost power-up
- Trail effect behind snake
- Sound effects

**Medium:**
- Minimap showing full arena
- Camera smoothly follows player
- AI with personalities (aggressive, defensive)
- Obstacles that kill snakes

**Hard:**
- AI pathfinding to nearest food
- Local 2-player (WASD + Arrow keys)
- Boss AI with special abilities
- Procedural map generation
- Re-add a `Queue<Vector3>` position history so segments follow the head's exact path (Enqueue/Dequeue, `while` loop to cap its size)
- Self-collision: a `for` loop over `bodySegments` starting at index 3 (the first few are always touching the head), with `break` on the first hit

---

## 🔧 Technical Details

### Loop Usage Summary
| Loop Type | Use Cases | Count |
|-----------|-----------|-------|
| `for` | Build starting body, grow, spawn food, spawn AI | 6 |
| `foreach` | Move segments on wrap, cleanup, check AI distances | 7 |
| `while` | Food spawn position retry | 1 |

### List/Collection Usage
| Collection | Purpose | Modified In |
|------------|---------|-------------|
| `List<SnakeSegment>` | Track body segments | Player, AI |
| `List<FoodPellet>` | Track active food | FoodSpawner |
| `List<AISnakeController>` | Track AI opponents | GameManager |

### Update Timing Strategy
| Method | Used For | Frequency |
|--------|----------|-----------|
| `Update()` | Input, UI, timers, decisions | Variable (~60-144fps) |
| `FixedUpdate()` | Movement, physics, collisions | Fixed 50Hz (0.02s) |

---

## 🐛 Common Issues

### Snake doesn't move
- Check moveSpeed > 0
- Verify Main Camera tagged "MainCamera"

### No collisions
- Colliders need "Is Trigger" checked
- Objects need correct tags
- Need Rigidbody2D on at least one object

### Food doesn't spawn
- FoodPrefab assigned in FoodSpawner?
- initialFoodCount > 0?
- Spawn area reasonable size?

### AI doesn't spawn
- AISnakePrefab assigned in GameManager?
- numberOfAISnakes > 0?

See **[SETUP_INSTRUCTIONS.md](SETUP_INSTRUCTIONS.md)** for detailed troubleshooting.

---

## 📖 Resources

### Documentation
- **Full Teaching Guide:** `Assets/Scripts/README.md`
- **Setup Instructions:** `SETUP_INSTRUCTIONS.md`
- **Concepts Reference:** `LOOPS_AND_LISTS_CONCEPTS.md` (the long explanations, diagrams, and TRY IT experiments)
- **Inline Comments:** labeled sections, short concept reviews, and `// Why...?` questions to think about; each script's header names its concepts section
- **AI Tutor Setup:** `AI_TUTOR.md` (students sign in to the VS Code tutor here)

### Unity Concepts Used
- 2D Sprite Rendering
- Physics2D (Collider2D, Rigidbody2D, triggers)
- Input System (mouse position)
- UI (Canvas, Text)
- Prefab instantiation
- SceneManagement

---

## 🤝 Contributing

This is an educational template for GAME 220. Suggestions welcome:
- Open issue for bugs or improvements
- Propose enhancements that maintain "bare minimum" simplicity
- Share student extensions and modifications

---

## 📄 License

See [LICENSE](LICENSE) file.

---

## 🎯 Design Philosophy

**"Last Known Good"**
Students should always have working code. Each session adds functionality without breaking previous sessions.

**"Bare Minimum"**
Implement the simplest version that works. No over-engineering. Focus on core concepts.

**"Read, Repeat, Understand"**
Scripts keep comments to about a quarter of the lines, so students can find the code and still have something to think about: short reviews where a concept first appears and `// Why...?` questions at the line where a decision is made. `LOOPS_AND_LISTS_CONCEPTS.md` explains WHY and HOW at length. Students read first, then implement similar patterns.

**"Loops and Lists First"**
Every system demonstrates practical loop usage and list management. This is the lens through which we teach.

---

**Built with ❤️ for GAME 220 students**
**Questions?** Check `Assets/Scripts/README.md` or ask your instructor!
