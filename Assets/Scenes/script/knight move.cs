  using UnityEngine;

  public class PlayerMove2D : MonoBehaviour
  {
      [SerializeField] private float moveSpeed = 5f;

      private Rigidbody2D rb;
      private SpriteRenderer spriteRenderer;
      private Vector2 moveInput;

      private void Awake()
      {
          rb = GetComponent<Rigidbody2D>();
          spriteRenderer = GetComponent<SpriteRenderer>();
      }

      private void Update()
      {
          float x = Input.GetAxisRaw("Horizontal"); // 좌우 방향키
          float y = Input.GetAxisRaw("Vertical");   // 상하 방향키

          moveInput = new Vector2(x, y).normalized;

          // 좌우 이동할 때만 바라보는 방향 변경
          if (x > 0)
          {
              spriteRenderer.flipX = false; // 오른쪽 보기
          }
          else if (x < 0)
          {
              spriteRenderer.flipX = true;  // 왼쪽 보기
          }
      }

      private void FixedUpdate()
      {
          rb.linearVelocity = moveInput * moveSpeed;
      }
  }
