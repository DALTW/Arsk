using UnityEngine;

[DisallowMultipleComponent]
public class MonsterSpawner : MonoBehaviour
{
    [System.Serializable]
    private class MonsterData
    {
        public string monsterName = "";
        public Sprite[] moveFrames = new Sprite[0];
        public float moveSpeed = 2f;
        public float scale = 3f;
        public float colliderRadius = 0.25f;
        public float animationFrameTime = 0.12f;
    }

    [SerializeField] private MonsterData[] monsters;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private int spawnCount = 3;
    [SerializeField] private float minSpawnDistance = 8f;
    [SerializeField] private float maxSpawnDistance = 12f;
    [SerializeField] private int monsterSortingOrder = 1;
    [SerializeField] private float movementThreshold = 0.01f;

    private Transform player;
    private Vector3 lastPlayerPosition;
    private float spawnTimer;

    private void Start()
    {
        FindPlayer();

        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        bool playerMoved = (player.position - lastPlayerPosition).sqrMagnitude >
            movementThreshold * movementThreshold;
        lastPlayerPosition = player.position;

        if (!playerMoved)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer < spawnInterval)
        {
            return;
        }

        spawnTimer = 0f;
        SpawnMonsters();
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }

    private void SpawnMonsters()
    {
        if (monsters == null || monsters.Length == 0)
        {
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnMonster();
        }
    }

    private void SpawnMonster()
    {
        MonsterData monsterData = monsters[Random.Range(0, monsters.Length)];

        if (monsterData == null || monsterData.moveFrames == null || monsterData.moveFrames.Length == 0)
        {
            return;
        }

        Sprite sprite = monsterData.moveFrames[0];

        Vector2 direction = Random.insideUnitCircle.normalized;
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.right;
        }

        float spawnDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 spawnPosition = player.position + (Vector3)(direction * spawnDistance);

        GameObject monster = new GameObject($"{monsterData.monsterName} Monster");
        monster.transform.position = spawnPosition;
        monster.transform.localScale = new Vector3(monsterData.scale, monsterData.scale, 1f);
        monster.tag = "Monster";

        SpriteRenderer spriteRenderer = monster.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = monsterSortingOrder;

        CircleCollider2D collider = monster.AddComponent<CircleCollider2D>();
        collider.radius = monsterData.colliderRadius;

        Rigidbody2D body = monster.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        monstermove move = monster.AddComponent<monstermove>();
        move.SetTarget(player);
        move.SetMoveSpeed(monsterData.moveSpeed);
        move.SetMoveFrames(monsterData.moveFrames);
        move.SetAnimationFrameTime(monsterData.animationFrameTime);
    }
}
