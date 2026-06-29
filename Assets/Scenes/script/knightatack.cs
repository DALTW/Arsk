using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class knightatack : MonoBehaviour
{
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private Sprite[] slashFrames;
    [SerializeField] private float slashFrameTime = 0.06f;
    [SerializeField] private float slashOffset = 0.75f;
    [SerializeField] private float slashScale = 1f;
    [SerializeField] private Vector2 hitboxSize = new Vector2(1.2f, 0.5f);
    [SerializeField] private int slashSortingOrder = 5;

    private SpriteRenderer playerSpriteRenderer;
    private Vector2 lastFacingDirection = Vector2.right;
    private float nextAttackTime;

    private void Awake()
    {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();

        if (playerSpriteRenderer != null && playerSpriteRenderer.flipX)
        {
            lastFacingDirection = Vector2.left;
        }
    }

    private void OnEnable()
    {
        nextAttackTime = Time.time + Mathf.Max(0.01f, attackInterval);
    }

    private void Update()
    {
        UpdateFacingDirection();

        if (Time.time < nextAttackTime)
        {
            return;
        }

        nextAttackTime = Time.time + Mathf.Max(0.01f, attackInterval);
        StartCoroutine(PlaySlash(lastFacingDirection));
    }

    private void UpdateFacingDirection()
    {
        Vector2 inputDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (inputDirection.sqrMagnitude > 0.01f)
        {
            lastFacingDirection = inputDirection.normalized;
        }
    }

    private IEnumerator PlaySlash(Vector2 direction)
    {
        if (slashFrames == null || slashFrames.Length == 0)
        {
            yield break;
        }

        direction = direction.sqrMagnitude > 0.01f ? direction.normalized : Vector2.right;

        GameObject slash = new GameObject("Knight Slash");
        slash.layer = gameObject.layer;
        slash.transform.position = transform.position + (Vector3)(direction * slashOffset);
        slash.transform.rotation = Quaternion.Euler(0f, 0f, GetSlashAngle(direction));
        slash.transform.localScale = new Vector3(slashScale, slashScale, 1f);
        slash.AddComponent<PlayerAttackHitbox>();

        SpriteRenderer slashRenderer = slash.AddComponent<SpriteRenderer>();
        slashRenderer.sortingOrder = slashSortingOrder;
        slashRenderer.flipX = ShouldFlipSlash(direction);

        if (playerSpriteRenderer != null)
        {
            slashRenderer.sortingLayerID = playerSpriteRenderer.sortingLayerID;
        }

        BoxCollider2D hitbox = slash.AddComponent<BoxCollider2D>();
        hitbox.isTrigger = true;
        hitbox.size = hitboxSize;

        Rigidbody2D slashBody = slash.AddComponent<Rigidbody2D>();
        slashBody.bodyType = RigidbodyType2D.Kinematic;
        slashBody.gravityScale = 0f;
        slashBody.simulated = true;

        WaitForSeconds frameDelay = new WaitForSeconds(Mathf.Max(0.01f, slashFrameTime));

        foreach (Sprite frame in slashFrames)
        {
            if (frame != null)
            {
                slashRenderer.sprite = frame;
            }

            yield return frameDelay;
        }

        Destroy(slash);
    }

    private static float GetSlashAngle(Vector2 direction)
    {
        if (ShouldFlipSlash(direction))
        {
            return 0f;
        }

        return Vector2.SignedAngle(Vector2.right, direction);
    }

    private static bool ShouldFlipSlash(Vector2 direction)
    {
        return direction.x < -0.5f && Mathf.Abs(direction.y) < 0.5f;
    }
}
