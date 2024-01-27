using System.Collections;
using UnityEngine;
using TMPro;

//tosaaa_3

public class SimpleNPC : MonoBehaviour
{
    public string npcName = "NPC";
    public int health = 100;
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;
    public float jumpProbability = 0.1f; // Probability of jumping during wandering
    public float jumpForce = 5f; // Force applied when jumping
    public float detectionRange = 10f;
    public float speed = 15f;
    public AudioClip interactionSound;
    public TextMeshProUGUI interactionText;
    public string initialInteractionText = "Hello, adventurer! How can I help you?";

    private Transform player;
    private Vector3 wanderPoint;
    private bool isWandering;

    void Start()
    {
        Debug.Log($"Hello, I am {npcName}. Let's get started!");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        isWandering = false;

        // Start wandering coroutine
        StartCoroutine(Wander());
    }

    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.CompareTag("NPC"))
                {
                    Interact();
                }
            }
        }
    }

    void Move()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If player is in detection range, stop wandering and face the player
        if (distanceToPlayer <= detectionRange)
        {
            StopWandering();
            FacePlayer();
        }
        else if (!isWandering)
        {
            // Resume wandering if not already wandering
            StartCoroutine(Wander());
        }
    }

    IEnumerator Wander()
    {
        isWandering = true;

        while (true)
        {
            // Check if the NPC should jump
            if (Random.value < jumpProbability)
            {
                Jump();
            }

            // Generate a random point within the specified wander radius
            wanderPoint = transform.position + Random.insideUnitSphere * wanderRadius;
            wanderPoint.y = transform.position.y;

            // Move towards the random point
            while (Vector3.Distance(transform.position, wanderPoint) > 0.2f)
            {
                transform.LookAt(wanderPoint);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                yield return null;
            }

            // Wait for a specified interval before choosing the next random point
            yield return new WaitForSeconds(wanderInterval);
        }
    }

    void Jump()
    {
        // Apply a vertical force for jumping
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void StopWandering()
    {
        isWandering = false;
    }

    void FacePlayer()
    {
        transform.LookAt(player);
    }

    void Interact()
    {
        Debug.Log($"{npcName} says: {initialInteractionText}");

        if (interactionSound != null)
        {
            AudioSource.PlayClipAtPoint(interactionSound, transform.position);
        }

        if (interactionText != null)
        {
            interactionText.text = initialInteractionText;
        }
    }

    void HideInteractionText()
    {
        // Hide interaction text
        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log($"{npcName} took {damageAmount} damage. Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{npcName} has died.");
        // Implement any death-related logic here
        Destroy(gameObject); // Remove the NPC from the scene
    }
}
