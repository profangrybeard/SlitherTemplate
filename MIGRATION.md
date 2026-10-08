# Migration: Unity 6.5 · URP · Input System

Companion doc for bringing this template in line with PeggleTemplate, which has already
been through this migration and tested in-editor.

Full playbook (both templates, side by side):
https://claude.ai/code/artifact/3480539e-c224-4a43-a2ff-c683f936ab6b

> **Status (8 Oct 2026): done, on Unity 6000.6.0f1.** Input is New-only with
> `steerAction` / `restartAction`, TMP `Examples & Extras` and the empty `SampleScene` are
> removed, Build Settings points at `SlitherTemplate_Base.unity`, and the project opens
> with zero console warnings. Stayed on Built-In rendering. The notes below are the
> original plan, kept for reference.

---

## Where this project stood (before the migration)

Read from disk, 25 Aug 2026.

| | |
|---|---|
| Unity | `6000.0.63f1` — needs upgrade |
| Active Input Handling | `2` (Both) — needs to become `1` (New only) |
| Render pipeline | Built-In (`m_CustomRenderPipeline: {fileID: 0}`) |
| Input System package | `1.16.0` |
| URP package | not installed |
| TMP `Examples & Extras` | **present — 284 files, 41 legacy `Input.` calls** |
| EventSystem | already `InputSystemUIInputModule` ✅ |
| Legacy input call sites | 2 |
| Build Settings scene | **wrong — points at `SampleScene.unity`** |

Two things are already broken independent of the migration: the build scene, and the TMP
sample folder that will block New-only input.

---

## Legacy input in this project

| File · line | Call | Becomes |
|---|---|---|
| `Assets/Scripts/Managers/GameManager.cs:182` | `Input.GetKeyDown(KeyCode.R)` | `restartAction.WasPressedThisFrame()` |
| `Assets/Scripts/Player/PlayerSnakeController.cs:48` | `Input.mousePosition` | `steerAction.ReadValue<Vector2>()` |

The snake steers by reading the pointer every frame, so `steerAction` is a **Value** action
bound to `<Mouse>/position`. Restart is a **Button** bound to `<Keyboard>/r`.

Peggle had no restart action — this is the one pattern that is new here.

---

## The migration, in order

**The order matters.** Step 4 must finish before step 5, or the project stops compiling and
you cannot open the editor to fix it.

### 1. Close Unity first

Scenes, prefabs and `ProjectSettings/*.asset` are YAML. Editing them while the editor holds
them open means the editor wins on next save. Script and Markdown edits are safe with Unity
running — it just recompiles.

```bash
tasklist //FI "IMAGENAME eq Unity.exe" | grep Unity.exe
```

```bash
ls Temp/UnityLockfile
```

Empty output from the first and a missing lockfile from the second both mean it is safe.
Commit before starting, so every step below is revertible.

### 2. Upgrade to Unity 6000.5.9f1

Open in 6.5 and let it convert. All of this is expected in `git status`:

- Package version bumps
- `com.unity.modules.physicscore2d` added
- `com.unity.modules.vr` removed
- `com.unity.render-pipelines.universal` added *as a dependency of the 2D feature set*
- New files: `DefaultVolumeProfile.asset`, `UniversalRenderPipelineGlobalSettings.asset`,
  `PhysicsCoreProjectSettings2D.asset`, `ProjectAuditorSettings.asset`

Installing the URP package does **not** switch the renderer. Check what is actually active:

```bash
grep -n "m_CustomRenderPipeline" ProjectSettings/GraphicsSettings.asset
```

`{fileID: 0}` means Built-In. A real guid means URP is live.

### 3. Decide on URP deliberately

Two traps hit Peggle:

- **Unity's default URP asset is the 3D renderer** (`UniversalRendererData`). A 2D project
  wants `Renderer2DData`, or there is no 2D lighting path at all.
- **Built-In particle materials go magenta.** `Default-Particle` is fileID `10301`.
  `Sprites-Default` (fileID `10754`) still resolves fine under URP.

```bash
grep -rn "fileID: 10301" Assets --include="*.prefab" --include="*.unity"
```

```bash
grep -rn "fileID: 10754" Assets --include="*.prefab" --include="*.unity"
```

The first finds particle materials that will break. The second finds sprites, which are
fine.

Staying on Built-In is a legitimate answer. Only take URP if you want 2D lights — and if
you do, use the 2D renderer.

### 4. Delete TextMesh Pro's sample content — BLOCKER

`Assets/TextMesh Pro/Examples & Extras/` has **41 legacy `Input.` calls across 284 files**.
Set Active Input Handling to New-only before removing it and the whole project stops
compiling, including the editor scripts you would need to change it back.

Confirm nothing real depends on it first:

```bash
find "Assets/TextMesh Pro/Examples & Extras" -name "*.meta" -exec grep -h "^guid:" {} \; | sed "s/guid: //" > /tmp/ex_guids.txt
```

```bash
while read g; do grep -rqs "$g" Assets --include="*.unity" --include="*.prefab" && echo "REFERENCED: $g"; done < /tmp/ex_guids.txt
```

Silence from the second command means nothing references the folder. Then remove it:

```bash
rm -rf "Assets/TextMesh Pro/Examples & Extras" "Assets/TextMesh Pro/Examples & Extras.meta"
```

TMP core is untouched — `Fonts`, `Resources`, `Shaders`, `Sprites` all remain, including
`LiberationSans SDF`, which the scene text uses.

This also removes 41 worked examples of the dying API from the folder a curious student
would browse.

### 5. Rewrite the input code

The pattern Peggle settled on: one serialized `InputAction` field per verb. Teaches actions
and bindings, stays readable, needs no `.inputactions` asset or `PlayerInput` component.

```csharp
using UnityEngine.InputSystem;

[SerializeField] private InputAction steerAction;    // Value  · Vector2
[SerializeField] private InputAction restartAction;  // Button

private void OnEnable()  { steerAction.Enable();  restartAction.Enable(); }
private void OnDisable() { steerAction.Disable(); restartAction.Disable(); }

Vector2 pointer = steerAction.ReadValue<Vector2>();
bool restarted  = restartAction.WasPressedThisFrame();
```

> **An action that is never enabled fails silently.** No exception, no console warning —
> input just does nothing. Forgetting `OnEnable()` is the most common way this migration
> looks broken.

Use `OnEnable`/`OnDisable`, not `Start`. `Start` runs once ever; an object disabled and
re-enabled would come back deaf.

Bindings live in the prefab or scene YAML. If hand-authoring rather than clicking through
the Inspector, the shape is:

```yaml
  restartAction:
    m_Name: Restart
    m_Type: 1
    m_ExpectedControlType:
    m_Id: <guid>
    m_Processors:
    m_Interactions:
    m_SingletonActionBindings:
    - m_Name:
      m_Id: <guid>
      m_Path: <Keyboard>/r
      m_Interactions:
      m_Processors:
      m_Groups:
      m_Action: Restart
      m_Flags: 0
    m_Flags: 0
    m_Priority: 0
```

`m_Type` is `0` for Value, `1` for Button, `2` for PassThrough. Add another `- m_Name:`
block under `m_SingletonActionBindings` for a second device — one action with two bindings
is what lets a single line of code serve both mouse and keyboard.

### 6. Switch Active Input Handling to New

Verify nothing legacy survives **first**:

```bash
grep -rn "Input\.\|KeyCode\." Assets --include="*.cs" | grep -v "InputSystem\|InputAction"
```

That must return nothing. Then:

```bash
sed -i "s/^  activeInputHandler: 2$/  activeInputHandler: 1/" ProjectSettings/ProjectSettings.asset
```

The values are `0` Old, `1` New only, `2` Both.

New-only is the point: pasting `Input.GetKeyDown` from a tutorial now fails to compile
instead of quietly working.

### 7. EventSystem — already done here

`SlitherTemplate_Base.unity` already uses `InputSystemUIInputModule`. Nothing to do, but
verify after the upgrade:

```bash
grep -c "4f231c4fb786f3946a6b90b886c48677" Assets/Scenes/SlitherTemplate_Base.unity
```

```bash
grep -c "01614664b831546d2ae94a42149d80ac" Assets/Scenes/SlitherTemplate_Base.unity
```

The first is the old `StandaloneInputModule` and should be `0`. The second is
`InputSystemUIInputModule` and should be `1`.

---

## Audit checklist

Independent of the migration. Every item below was actually wrong in Peggle.

- [ ] **Build Settings points at the wrong scene — CONFIRMED BROKEN HERE.**
      The list names `Assets/Scenes/SampleScene.unity`, not `SlitherTemplate_Base.unity`.
      A fresh clone opens an empty scene.
      Check with `grep "path:" ProjectSettings/EditorBuildSettings.asset`
- [ ] **Debug values left in the scene.** Scene overrides beat script defaults silently.
      Read the serialized values in the scene, not the field initialisers in the `.cs`.
- [ ] **Components on the instance instead of the prefab.** Peggle's bucket collider lived
      in the scene, so the prefab was non-functional on its own. Look for non-empty
      `m_AddedComponents:` in scene YAML.
- [ ] **Orphaned property overrides from deleted script fields.** Peggle's scene carried 50
      overrides for a field the script no longer had — that is how the win condition became
      unreachable.
- [ ] **Public methods nothing calls.** `grep` each public method name across `Assets/`.
      One hit means only the definition exists, and nothing ever calls it.
- [ ] **Docs describing files that do not exist.** This repo has four root docs — `README`,
      `SETUP_INSTRUCTIONS`, `QUICKSTART_CHECKLIST`, `IMPLEMENTATION_SUMMARY` — worth
      checking against reality after the migration.
- [ ] **Meta files committed**, new Markdown files included.
- [ ] **Git LFS installed before cloning**, not after.

---

## Reusable from PeggleTemplate

| File | Reuse |
|---|---|
| `Assets/Scripts/INPUT_SYSTEM.md` | Copy as-is; swap the action names |
| `README.md` troubleshooting | Two entries: legacy `Input` not compiling, and silent dead input |
| `STUDENT_GUIDE.md` Week 1 | Input moved to Week 1; the rebind experiment works anywhere |
| `CLAUDE.md` tech stack | Record New-only **and why**, so nobody sets it back to Both |

---

## Status

The Peggle migration these steps describe was **tested in the editor** — it compiled, ran,
and played, hand-authored YAML included. This is a path that has been walked end to end.

Nothing about this project's actual gameplay has been reviewed — only versions, packages,
render pipeline, input call sites, scenes and build settings.
