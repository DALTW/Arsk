using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class monstermove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Sprite[] moveFrames;
    [SerializeField] private float animationFrameTime = 0.12f;

    private Transform target;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private int frameIndex;
    private float frameTimer;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            target = player != null ? player.transform : null;
        }

        if (spriteRenderer != null && moveFrames != null && moveFrames.Length > 0)
        {
            spriteRenderer.sprite = moveFrames[0];
        }
    }

    private void Update()
    {
        AnimateMovement();
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 currentPosition = body.position;
        Vector2 targetPosition = target.position;
        Vector2 direction = targetPosition - currentPosition;

        if (direction.sqrMagnitude < 0.001f)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        direction.Normalize();
        body.linearVelocity = direction * moveSpeed;

        if (spriteRenderer != null && Mathf.Abs(direction.x) > 0.01f)
        {
            spriteRenderer.flipX = direction.x < 0f;
        }
    }

    private void AnimateMovement()
    {
        if (spriteRenderer == null || moveFrames == null || moveFrames.Length == 0)
        {
            return;
        }

        frameTimer += Time.deltaTime;
        if (frameTimer < animationFrameTime)
        {
            return;
        }

        frameTimer = 0f;
        frameIndex = (frameIndex + 1) % moveFrames.Length;
        spriteRenderer.sprite = moveFrames[frameIndex];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerAttackHitbox>() != null)
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }

    public void SetMoveFrames(Sprite[] newMoveFrames)
    {
        moveFrames = newMoveFrames;
        frameIndex = 0;
        frameTimer = 0f;

        if (spriteRenderer != null && moveFrames != null && moveFrames.Length > 0)
        {
            spriteRenderer.sprite = moveFrames[0];
        }
    }

    public void SetAnimationFrameTime(float newAnimationFrameTime)
    {
        animationFrameTime = Mathf.Max(0.01f, newAnimationFrameTime);
    }
}
