# Loops and Lists Concepts Reference

**GAME 220: Core Programming -- Slither Game**

The scripts in `Assets/Scripts/` keep their comments brief so you can find the code: a short
review where a concept first appears, and `// Why...?` questions for you to work out.
This document holds the longer explanations. Each script's header names the section here
that goes with it. Search a script for the method name given here to find the code.

---

## Contents

1. [PlayerSnakeController: Input, Lists, and Loops](#1-playersnakecontroller-input-lists-and-loops)
2. [SnakeSegment: Following Another Object](#2-snakesegment-following-another-object)
3. [FoodSpawner: The List Add/Remove Lifecycle](#3-foodspawner-the-list-addremove-lifecycle)
4. [FoodPellet: Triggers and Component Communication](#4-foodpellet-triggers-and-component-communication)
5. [ScoreManager: UI, Encapsulation, and PlayerPrefs](#5-scoremanager-ui-encapsulation-and-playerprefs)
6. [BoundsHelper: Static Utility Classes](#6-boundshelper-static-utility-classes)
7. [GameManager: Spawning and Cleaning Up a List](#7-gamemanager-spawning-and-cleaning-up-a-list)
8. [AISnakeController: Code Reuse](#8-aisnakecontroller-code-reuse)
9. [TRY IT Experiments](#9-try-it-experiments)

---

## 1. PlayerSnakeController: Input, Lists, and Loops

### Input actions (`steerAction`)

An `InputAction` is a **name** for something the player can do ("Steer"). It is kept apart
from what triggers it (the mouse, a touchscreen, a gamepad). Those triggers are called
**bindings**. Expand the action in the Inspector to see its bindings.

An action does nothing until it is **enabled**. The script enables it in `OnEnable()`, not
`Start()`. `OnEnable()` runs every time the object is switched on, so input always comes back
after the object is turned off and on again. `OnDisable()` turns it off. `GameManager` does the
same for its `restartAction`.

### Update vs FixedUpdate

- `Update()` reads input every frame, so steering responds right away.
- `FixedUpdate()` moves the head on a fixed timestep (50 Hz), so speed stays the same at any frame rate.

### The body list

The body uses a "follow the leader" pattern:

```
HEAD -> Seg0 -> Seg1 -> Seg2 -> ...
```

`List<SnakeSegment> bodySegments` holds every segment. A `List` can grow and shrink, but an
array has a fixed size. That is why a snake that grows uses a list.

- `CreateStartingBody()`: a `for` loop runs `AddSegment()` exactly `startingSegments` times.
- `AddSegment()`: the first segment follows the head. Each later one follows
  `bodySegments[bodySegments.Count - 1]`, the current last segment. Then `List.Add()` puts it on the end.
- `Grow(amount)`: the same `for` loop, run `amount` times.
- `CheckBoundaries()`: when the head wraps, a `foreach` loop moves every segment by the same offset.
- `OnDestroy()`: a `foreach` loop destroys every segment.

---

## 2. SnakeSegment: Following Another Object

Each segment stores a `Transform target` for the object in front of it. In `FixedUpdate()` it:

1. Finds the direction to the target.
2. Works out a point `followDistance` behind the target.
3. Moves there at a constant speed with `Vector3.MoveTowards`.

`Teleport(offset)` moves the segment by the same amount the head wrapped, so the body stays
in one piece at the screen edge.

---

## 3. FoodSpawner: The List Add/Remove Lifecycle

- `Start()`: a `for` loop spawns `initialFoodCount` pellets.
- `SpawnFood()`: `List.Add()` starts tracking the new pellet, and `SetSpawner(this)` gives the
  pellet a way to call back.
- `OnFoodCollected()`: `List.Remove()` stops tracking it, then a replacement spawns so the count stays the same.
- `GetRandomValidPosition()`: a `while` loop. Use `while` when you don't know how many tries you
  need but you do know when to stop. It keeps picking random spots while they are too close to a
  snake. `maxSpawnAttempts` stops it from looping forever.
- `IsPositionValid()`: a `foreach` loop checks the distance to every AI snake.

---

## 4. FoodPellet: Triggers and Component Communication

### What the pellet does

When a snake (player or AI) touches the food:
1. The snake grows longer by `growthAmount` (1 by default).
2. The score goes up (player only).
3. The pellet is destroyed and a new one spawns somewhere else.

```
   [Snake Head]  ---touches--->  [Food Pellet]
                                       |
                             OnTriggerEnter2D fires
                                       |
                        Tag is "Player" or "AISnake"?
                               /              \
                             YES               NO
                              |                 |
                   GetComponent<...>()      do nothing
                              |
                   snake grows (player: +10 score)
                              |
                   spawner.OnFoodCollected(this)
                              |
                   pellet destroyed, new one spawns
```

### Trigger vs collider

| | Collider (Is Trigger OFF) | Trigger (Is Trigger ON) |
|---|---|---|
| Objects | Bounce off each other | Pass through each other |
| Callback | `OnCollisionEnter2D` | `OnTriggerEnter2D` |
| Good for | Walls, platforms, physical objects | Pickups, detection zones, power-ups |

Food uses a **trigger**. The snake should not bounce off food. We only want to know when they overlap.

### Requirements for `OnTriggerEnter2D`

If any of these is missing, Unity never calls it. This causes a lot of bugs:

1. **Both** objects have a `Collider2D` (snake head and pellet both use `CircleCollider2D`).
2. **At least one** has **Is Trigger** checked (the pellet).
3. **At least one** has a `Rigidbody2D` (the snake head).

The `other` parameter is the collider that entered. Use it to find out what touched the pellet.

### `CompareTag` instead of `==`

- `other.gameObject.tag == "Player"` allocates memory (garbage) and is slower.
- `other.CompareTag("Player")` creates no garbage. This runs on every collision, so the difference adds up.

### `GetComponent<T>()`

`other.GetComponent<PlayerSnakeController>()` finds that script on the object that touched us.
It returns `null` if the script isn't there, so the code checks before using it.

### Component communication (`SetSpawner` / `OnFoodCollected`)

```
   FoodSpawner <--------------> FoodPellet
        |  "Here's my reference"     |
        |  (SetSpawner)              |
        |--------------------------->|
        |  "I was collected!"        |
        |  (OnFoodCollected)         |
        |<---------------------------|
```

1. `FoodSpawner` creates the pellet with `Instantiate()`.
2. It calls `pellet.SetSpawner(this)` so the pellet has a reference back.
3. When collected, the pellet calls `spawner.OnFoodCollected(this)`. The spawner removes it from
   its list, destroys it, and spawns a replacement.

If a pellet has no spawner (it was placed in the scene by hand), it destroys itself.

The score lookup uses `FindAnyObjectByType<ScoreManager>()`. That is slower than caching a
reference, but it is easier to read while you are learning. AI snakes grow but don't score.

### The bobbing effect (`Update`)

This is only a visual effect, so it runs in `Update()` for the smoothest motion. Physics would go in `FixedUpdate()`.
`Mathf.Sin(Time.time * bobSpeed)` swings smoothly between -1 and 1 as time goes on. `bobAmount` sets how
far the pellet moves, and `Time.deltaTime` keeps it the same at any frame rate.

---

## 5. ScoreManager: UI, Encapsulation, and PlayerPrefs

### One job per script (separation of concerns)

ScoreManager is the only place the score is kept. It tracks the current score, updates the
UI, saves and loads the high score, and shows the game over and victory messages.

```
  PlayerSnakeController   Job: move the snake
           |
           |  "I touched food"
           v
       FoodPellet         Job: detect collection, report the score
           |
           |  scoreManager.AddScore(10)
           v
      ScoreManager        Job: track the score, update the UI
           |
           v
         UI Text          Job: show text to the player
```

If score logic lived in `PlayerSnakeController`, that file would get longer and mix movement
with UI, and score code would be harder to find. This idea is called the **Single Responsibility Principle**.

### Updating on events

The score does not update every frame. It updates **when something happens**:
food collected -> `AddScore(10)` -> `UpdateScoreDisplay()`. That is cheaper than checking "did
the score change?" every frame.

### Connecting UI in the Inspector

1. Create **UI > Text - TextMeshPro** in the scene (for example "ScoreText").
2. Select the GameObject with `ScoreManager`.
3. Drag ScoreText from the Hierarchy into the **Score Text** field.

TextMeshPro (`TMP_Text`) is Unity's current text system. It renders better and has more styling
options than the old `UI.Text`.

### Encapsulation

`currentScore` and `highScore` are `private`. If they were public, any script could write
`scoreManager.currentScore = 9999999;`. Other scripts call `AddScore()` instead, so the score
always changes the same way. The UI updates, and the high score gets checked.
`GetCurrentScore()` and `GetHighScore()` let other scripts **read** the score but not **change** it.

`UpdateScoreDisplay()` is private. It is a helper only this script calls.
`ResetScore()` is there for restarting without reloading the scene, for challenge modes, or for testing.

### PlayerPrefs

PlayerPrefs saves key-value pairs that stay after the game closes.

- `PlayerPrefs.GetInt("HighScore", 0)`: `"HighScore"` is the key. `0` is the default if nothing is saved yet.
- `PlayerPrefs.SetInt("HighScore", highScore)` stores the value, and `PlayerPrefs.Save()` writes it to disk now.
  Without `Save()`, the value can be lost if the game crashes.
- Use it for high scores and settings. For bigger save data, use files.

### Building strings

- `"Score: " + currentScore.ToString()` joins text and a number. `ToString()` turns `100` into `"100"`.
- `$"Score: {currentScore}"` does the same thing with string interpolation. `{name}` is replaced by
  the value, and `\n` starts a new line (see `ShowGameOver()` and `ShowVictory()`).

---

## 6. BoundsHelper: Static Utility Classes

### What `static` means

A `public static class`:
- is never created with `new BoundsHelper()`,
- is never attached to a GameObject,
- is used by calling methods on the class name.

```csharp
// Regular class: you need an instance
PlayerSnakeController player = FindAnyObjectByType<PlayerSnakeController>();
player.Grow(1);

// Static class: call it on the class name
bool isInside = BoundsHelper.IsInBounds(position);
Vector3 wrapped = BoundsHelper.WrapPosition(position);
```

Use static for utility functions that don't need to remember anything, such as math helpers and
shared constants. BoundsHelper fits because every script uses the same bounds, and its methods
are only calculations.

**Static fields** are shared. When `minBounds` changes, every script sees the new value.
A regular field is like everyone having their own notepad. A static field is like one shared whiteboard.

### The play area

`minBounds = (-17, -10)` and `maxBounds = (17, 10)`. That fits a camera with orthographic size 10 at 16:9,
so all the action stays on screen. The area is 34 units wide and 20 tall, centered on (0, 0).

```
                maxBounds.y = 10 (top)
          +-------------------------+
          |                         |
  min.x = |        PLAY AREA        | max.x = 17
    -17   |                         |
          +-------------------------+
                minBounds.y = -10 (bottom)
```

### Wrapping vs clamping

- **Wrapping** (`WrapPosition`, used by both snakes): leave one side and come back on the opposite side, like Pac-Man.
- **Clamping** (`ClampPosition`): stop at the edge, like hitting a wall.

```
 WRAP:   |  o-------------------->|   exits right ...
        >o                        |   ... appears on the left

 CLAMP:  |  o-------------------->o|  stops at the edge
```

### The methods

| Method | What it does | Example |
|---|---|---|
| `IsInBounds(pos)` | `true` if inside. Uses `&&`, so all four edge checks must pass. | `if (!BoundsHelper.IsInBounds(transform.position))` |
| `WrapPosition(pos)` | Moves an out-of-bounds position to the opposite edge | `transform.position = BoundsHelper.WrapPosition(transform.position);` |
| `ClampPosition(pos)` | `Mathf.Clamp` each axis into the bounds. Z stays 0 for 2D. | `transform.position = BoundsHelper.ClampPosition(transform.position);` |
| `GetRandomPosition()` | Random point in the play area | `Instantiate(prefab, BoundsHelper.GetRandomPosition(), Quaternion.identity);` |
| `GetRandomPositionInArea(min, max)` | Random point in a smaller area, trimmed to the bounds with `Mathf.Max`/`Mathf.Min` | Left half: `GetRandomPositionInArea(new Vector2(-17, -10), new Vector2(0, 10))` |
| `GetDistanceToBounds(pos)` | Distance to the nearest edge: `Mathf.Min` of all four distances | AI edge avoidance, warnings near the edge |
| `GetBoundsSize()` | Width and height (34 x 20) | Gizmos |
| `GetBoundsCenter()` | Center point, (0, 0) | Spawning the player in the middle |

### Math helpers

- `Mathf.Clamp(value, min, max)`: returns `min` if the value is too low, `max` if too high, or the value
  itself. `Clamp(50, 0, 100)` is 50, `Clamp(-10, 0, 100)` is 0, `Clamp(200, 0, 100)` is 100.
- `Mathf.Max(a, b)` / `Mathf.Min(a, b)` return the larger or smaller value. `Mathf.Min` also takes more than two values.
- `Random.Range(min, max)`: for **floats** it can return any value in the range. For **ints** the max
  is **left out**, so `Random.Range(0, 10)` returns 0 to 9.

---

## 7. GameManager: Spawning and Cleaning Up a List

### Why track AI snakes in a list

`List<AISnakeController> aiSnakes` lets the game clean them all up at game over, count them with
`aiSnakes.Count`, and check for a win. Without it, the code would call
`FindObjectsByType<AISnakeController>()` every time, and that is slow.

`gameOver` stops game logic after the game ends and turns on restart input. `GameOver()` returns
early if it was already called, for example when the player hits two things in one frame.

### `for` loop: spawning (`SpawnAISnakes`)

```
for (int i = 0; i < numberOfAISnakes; i++)
         -----   --------------------  ---
           |              |             +-- add 1 each time
           |              +---------------- keep going while i < 3
           +------------------------------- start at 0

  i=0: SpawnSingleAISnake() -> aiSnakes = [AI1]            Count = 1
  i=1: SpawnSingleAISnake() -> aiSnakes = [AI1][AI2]       Count = 2
  i=2: SpawnSingleAISnake() -> aiSnakes = [AI1][AI2][AI3]  Count = 3
  i=3: 3 < 3? FALSE -> exit the loop
```

`SpawnSingleAISnake()` uses the same `List.Add()` pattern as `FoodSpawner` and `PlayerSnakeController`.
`GetRandomSpawnPosition()` calls `BoundsHelper.GetRandomPosition()` so AI always spawn inside the play area.

### Cleanup at game over (`GameOver`): Destroy **and** Clear

**Step 1: a `foreach` loop destroys every AI GameObject.** `foreach` fits because every item gets
the same treatment and the index isn't needed. The loop checks for `null` because some AI may already
be destroyed (for example, if one died at the same moment as the player).

```
Before:          aiSnakes = [AI1][AI2][AI3][AI4][AI5]    5 snakes in the world
After Destroy:   aiSnakes = [null][null][null][null][null]   Count is still 5!
```

The list still has 5 entries, but they point to nothing. Using them would throw a
`NullReferenceException`.

**Step 2: `aiSnakes.Clear()` empties the list.**

```
After Clear:     aiSnakes = [ ]                          Count = 0
```

`Clear()` removes everything at once. It is faster than calling `Remove()` for each item.

| `Destroy(gameObject)` | `List.Clear()` |
|---|---|
| Removes the object from the **game world** | Removes the references from the **list** |
| Only Destroy: snakes vanish, but the list is full of nulls -> `NullReferenceException` | Only Clear: the list is empty, but the snakes are still in the game -> ghost snakes |

**You need both.**

### `List.Remove()`: one snake dies (`OnAISnakeDied`)

```
Before: [AI1][AI2][AI3]   Count = 3
Remove(AI2)
After:  [AI1][AI3]        Count = 2    (the items after it move down to fill the gap)
```

When `Count` reaches 0, `Victory()` runs. `RemoveAISnake()` is an older name that just calls `OnAISnakeDied()`.

### Restarting (`RestartGame`)

`SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex)` reloads the scene. That resets
everything: the player respawns, every list is empty (the scripts are new instances), the score resets,
and every GameObject is created again.

### Summary of list operations

- `List.Add(item)`: add one item
- `List.Remove(item)`: remove one specific item
- `List.Clear()`: remove every item
- `List.Count`: how many items there are now
- Always check for `null` while looping, because some objects may already be destroyed.

---

## 8. AISnakeController: Code Reuse

AISnakeController uses the **same** patterns as `PlayerSnakeController`: the same
`List<SnakeSegment>`, the same `for` loop to build the body, and the same `foreach` loops for
wrapping and cleanup. The only real difference is where the direction comes from.
A timer (`directionChangeTimer -= Time.deltaTime`) picks a new `Random.insideUnitCircle`
direction every `directionChangeInterval` seconds, instead of reading the pointer.
That shows how one pattern can be reused in a different situation.

---

## 9. TRY IT Experiments

These match the one-line `// TRY IT` comments in the scripts.

**FoodPellet: `growthAmount`**

`growthAmount` sets how much any snake grows from one pellet.
1. Set `growthAmount = 5` on the FoodPellet prefab. Eat a few. How fast does `bodySegments.Count` climb?
2. Set `growthAmount = 0`. The pellet still disappears and the score still goes up. Why?
3. Challenge: every pellet is the same right now. What would one pellet need to know so it could
   behave differently from another? (This is where the summative's 50-point food starts.)

**GameManager: `numberOfAISnakes`**
1. `0`: no opponents.
2. `10`: a crowded arena.
3. `20`: chaos mode. Watch the Console to see how many spawn.

**BoundsHelper: `WrapPosition`**
1. Steer your snake off the right edge and watch where it appears.
2. Try the top, bottom, and left edges.
3. Challenge: change `PlayerSnakeController.CheckBoundaries()` to use `ClampPosition`. What happens at an edge now?
