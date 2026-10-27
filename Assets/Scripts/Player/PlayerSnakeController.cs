/*
 * GAME 220: Slither Template -- PlayerSnakeController
 * Steers the head toward the pointer and keeps the body in a List (HEAD -> Seg0 -> Seg1 -> ...).
 * Start here: almost every loop and List pattern in the project shows up in this file first.
 *
 * HOW THE PROJECT FITS TOGETHER
 *   PlayerSnakeController  you: reads the pointer, moves the head, owns List<SnakeSegment>
 *        | each body piece is a
 *   SnakeSegment           follows the Transform in front of it
 *   FoodSpawner            keeps a List<FoodPellet> stocked (for, while, Add, Remove)
 *        | spawns
 *   FoodPellet             when a snake touches it: Grow(), ScoreManager.AddScore()
 *   AISnakeController      the same body List and loops as this file, steered by a timer
 *   GameManager            List<AISnakeController>, game over, victory, restart
 *   BoundsHelper           static helpers every script shares: wrap, clamp, random spot
 *
 * [LOOP] at the end of a line marks a loop. Every for, foreach, and while in the project has one.
 *
 * Read alongside: LOOPS_AND_LISTS_CONCEPTS.md section 1
 * Session 1: Update()/MoveHead()   Sessions 2-3: AddSegment()/Grow()   Session 5: CheckBoundaries()/Die()
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSnakeController : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────────────
    // INPUT - Input System actions (not the old Input class)
    // ─────────────────────────────────────────────────────────────────────────
    // An InputAction names what the player DOES ("Steer"), not which device does it.
    // The code asks the action, so rebinding it in the Inspector needs no code change.
    // [SerializeField] private = you can edit it in the Inspector, other scripts can't touch it.

    [Header("Input")]
    [SerializeField] private InputAction steerAction =
        new InputAction("Steer", InputActionType.Value, "<Pointer>/position", expectedControlType: "Vector2");

    // ─────────────────────────────────────────────────────────────────────────
    // SETTINGS - Tweak these in the Inspector
    // ─────────────────────────────────────────────────────────────────────────

    [Header("Movement")]
    public float moveSpeed = 5f;                 // World units per second
    public float rotationSpeed = 200f;           // Degrees per second the head can turn

    [Header("Body")]
    public int startingSegments = 3;             // How many times CreateStartingBody() loops
    public GameObject segmentPrefab;             // Drag SnakeSegment.prefab here
    public float segmentSpacing = 0.5f;          // Gap each segment keeps from the one ahead

    // ─────────────────────────────────────────────────────────────────────────
    // STATE - Tracked while playing
    // ─────────────────────────────────────────────────────────────────────────
    // LISTS: a List grows and shrinks while the game runs. An array's size is fixed.
    //   bodySegments.Add(seg)                    put one on the end
    //   bodySegments.Count                       how many are in it right now
    //   bodySegments[0]                          the first one (indexes start at 0)
    //   bodySegments[bodySegments.Count - 1]     the last one
    //   foreach (SnakeSegment s in bodySegments) { ... }   visit each one in turn

    // Why a List here and not an array like SnakeSegment[3]?
    private List<SnakeSegment> bodySegments = new List<SnakeSegment>();

    private Vector3 moveDirection;
    private bool isAlive = true;

    // ─────────────────────────────────────────────────────────────────────────
    // UNITY MESSAGES - Unity calls these automatically
    // ─────────────────────────────────────────────────────────────────────────
    // Update() runs once per drawn frame (30, 60, 144 times a second: it depends on the PC).
    // FixedUpdate() runs on a steady clock, 50 times a second by default.
    // This script READS input in Update() and MOVES in FixedUpdate().

    void Start()
    {
        moveDirection = transform.up;
        CreateStartingBody();
    }

    // An action does nothing until it is enabled, and it shows no error.
    // Why enable in OnEnable() instead of Start()?
    void OnEnable()
    {
        steerAction.Enable();
    }

    void OnDisable()
    {
        steerAction.Disable();
    }

    void Update()
    {
        if (!isAlive) return;

        Vector2 pointerScreenPos = steerAction.ReadValue<Vector2>();        // Screen pixels
        Vector3 pointerWorldPos = Camera.main.ScreenToWorldPoint(pointerScreenPos);

        // Why set z to 0 before working out the direction?
        pointerWorldPos.z = 0;
        moveDirection = (pointerWorldPos - transform.position).normalized;
    }

    // Why move the head here, and not in Update() where the input is read?
    void FixedUpdate()
    {
        if (!isAlive) return;
        MoveHead();
        CheckBoundaries();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MOVEMENT - Session 1
    // ─────────────────────────────────────────────────────────────────────────

    void MoveHead()
    {
        transform.position += moveDirection * moveSpeed * Time.fixedDeltaTime;

        if (moveDirection != Vector3.zero)
        {
            // Atan2 measures angles from the RIGHT (+x). Why subtract 90 for a head that faces up?
            float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BODY LIST - Sessions 2-3
    // ─────────────────────────────────────────────────────────────────────────
    // FOR LOOPS: use one when you know how many times to repeat.
    //   for (int i = 0; i < startingSegments; i++)
    //        start     keep going while     add 1 after each pass
    // With startingSegments = 3, i is 0, then 1, then 2, and the loop stops at 3.

    void CreateStartingBody()
    {
        for (int i = 0; i < startingSegments; i++)                    // [LOOP]
        {
            AddSegment();
        }
        Debug.Log($"Created snake with {bodySegments.Count} segments");
    }

    // Adds one segment to the end of the snake
    void AddSegment()
    {
        Vector3 spawnPosition;
        Transform targetToFollow;

        if (bodySegments.Count == 0)
        {
            spawnPosition = transform.position - transform.up * segmentSpacing; // First one follows the head
            targetToFollow = transform;
        }
        else
        {
            // Why Count - 1 and not Count?
            SnakeSegment lastSegment = bodySegments[bodySegments.Count - 1];
            spawnPosition = lastSegment.transform.position;               // Starts on top of the last one
            targetToFollow = lastSegment.transform;
        }

        GameObject newSegmentObj = Instantiate(segmentPrefab, spawnPosition, Quaternion.identity);
        SnakeSegment newSegment = newSegmentObj.GetComponent<SnakeSegment>(); // null if the prefab has no SnakeSegment

        if (newSegment != null)
        {
            newSegment.SetTarget(targetToFollow, segmentSpacing);

            // Copy the head's color to the segment
            SpriteRenderer headSprite = GetComponent<SpriteRenderer>();
            SpriteRenderer segmentSprite = newSegmentObj.GetComponent<SpriteRenderer>();
            if (headSprite != null && segmentSprite != null)
            {
                segmentSprite.color = headSprite.color;
            }

            // The tag and collider let an AI snake hit our body. Look at AISnakeController.OnTriggerEnter2D().
            newSegmentObj.tag = "Player";
            CircleCollider2D collider = newSegmentObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
            collider.isTrigger = true;

            bodySegments.Add(newSegment);                                 // Onto the end of the List
        }
    }

    // Called by FoodPellet. The same for loop as CreateStartingBody(), run `amount` times.
    public void Grow(int amount)
    {
        for (int i = 0; i < amount; i++)                              // [LOOP]
        {
            AddSegment();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // COLLISION AND EDGES - Session 5
    // ─────────────────────────────────────────────────────────────────────────

    // Only the head has this script. Body segments have a collider but no OnTriggerEnter2D of their own.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AISnake"))
        {
            Die();
        }
    }

    // FOREACH LOOPS: use one to visit every item in a collection, first to last.
    //   foreach (SnakeSegment segment in bodySegments) { ... }
    // There's no index to manage. `segment` is each item in turn.
    void CheckBoundaries()
    {
        if (!BoundsHelper.IsInBounds(transform.position))
        {
            Vector3 oldPosition = transform.position;
            transform.position = BoundsHelper.WrapPosition(transform.position);
            Vector3 teleportOffset = transform.position - oldPosition;

            // Comment out this foreach and steer off an edge. What does the body do, and why?
            foreach (SnakeSegment segment in bodySegments)                // [LOOP]
            {
                if (segment != null)
                {
                    segment.Teleport(teleportOffset);
                }
            }
        }
    }

    void Die()
    {
        isAlive = false;                         // Update() and FixedUpdate() stop at their first line now
        Debug.Log("Player died! Segments: " + bodySegments.Count);

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }

    // Unity calls this when the head is destroyed (for example when the scene reloads)
    void OnDestroy()
    {
        foreach (SnakeSegment segment in bodySegments)                    // [LOOP]
        {
            if (segment != null)
            {
                Destroy(segment.gameObject);
            }
        }
    }
}
