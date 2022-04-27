using UnityEngine;
using UnityEngine.SceneManagement;

public class Invaders : MonoBehaviour
{
    public Invader[] prefabs = new Invader[5];
    public AnimationCurve speed = new AnimationCurve();
    public Vector3 direction { get; private set; } = Vector3.right;
    public Vector3 initialPosition { get; private set; }
    public System.Action<Invader> killed;

    public int AmountKilled { get; private set; }
    public int AmountAlive => TotalAmount - AmountKilled;
    public int TotalAmount => rows * columns;
    public float PercentKilled => (float)AmountKilled / (float)TotalAmount;

    [Header("Grid")]
    public int rows = 6;
    public int columns = 10;

    [Header("Missiles")]
    public Projectile missilePrefab;
    public float missileSpawnRate = 1f;


    private void Awake()
    {
        initialPosition = transform.position;

        // Form the grid of invaders

        for (int row = 0; row < rows; row++)
        {
            float width = 2.0f * (columns - 1);
            float heigh = 2.0f * (rows - 1);
            Vector2 centerOffset = new Vector2(-width * 0.5f, -heigh * 0.5f);
            Vector3 rowPosition = new Vector3(centerOffset.x, (2f * row) + centerOffset.y, 0.0f);

            for (int col = 0; col < columns; col++)
            {
                // Create an invader and parent it to this transform
                Invader invader = Instantiate(prefabs[row], transform);
                invader.killed += OnInvaderKilled;

                // Calculate and set the position of the invader in the row
                Vector3 position = rowPosition;
                position.x += col * 2.0f;
                invader.transform.localPosition = position;
            }
        }

    }
    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), missileSpawnRate, missileSpawnRate);
    }


    private void Update()
    {
        // Evaluate the speed of the invaders based on how many have been killed
        float speed = this.speed.Evaluate(PercentKilled);
        transform.position += direction * speed * Time.deltaTime;

        //Check when the invaders reach the edge of the screen
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        //The invaders will advance to the next row after reaching the edge of the screen
        foreach (Transform invader in transform)
        {
            // Skip dead invaders
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            // Check the left edge or right edge based on the current direction
            if (direction == Vector3.right && invader.position.x >= (rightEdge.x - 1.0f))
            {
                AdvanceRow();
                break;
            }
            else if (direction == Vector3.left && invader.position.x <= (leftEdge.x + 1.0f))
            {
                AdvanceRow();
                break;
            }
        }
    }
    private void AdvanceRow()
    {
        // Flip the direction the invaders are moving
        direction = new Vector3(-direction.x, 0f, 0f);

        // Move the entire grid of invaders down a row
        Vector3 position = transform.position;
        position.y -= 1.0f;
        transform.position = position;
    }
    private void MissileAttack()
    {
        int amountAlive = AmountAlive;

        // No missiles should spawn when no invaders are alive
        if (amountAlive == 0)
        {
            return;
        }

        foreach (Transform invader in transform)
        {
            // Killed invaders cant shoot missiles
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }
            // Random chance to spawn a missile based upon how many invaders are alive
            if (Random.value < (1.0f / (float)amountAlive))
            {
                Instantiate(missilePrefab, invader.position, Quaternion.identity);
                break;
            }

        }
    }
    private void OnInvaderKilled(Invader invader)
    {
        invader.gameObject.SetActive(false);
        AmountKilled++;
        killed(invader);
    }

    public void ResetInvaders()
    {
        AmountKilled = 0;
        direction = Vector3.right;
        transform.position = initialPosition;

        foreach (Transform invader in transform)
        {
            invader.gameObject.SetActive(true);
        }
    }
    
}