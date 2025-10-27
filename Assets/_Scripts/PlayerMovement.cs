using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 4f;
    public Animator animator;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Vector2 input;
    Vector2 lastMoveDir = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (!animator) animator = GetComponent<Animator>();
    }

    void Update() {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        if (input.sqrMagnitude > 1f) input.Normalize();

        animator.SetFloat("MoveX", input.x);
        animator.SetFloat("MoveY", input.y);
        animator.SetFloat("Speed", input.sqrMagnitude);

        if (input.sqrMagnitude > 0.0001f) lastMoveDir = input;
        animator.SetFloat("LastX", lastMoveDir.x);
        animator.SetFloat("LastY", lastMoveDir.y);

        // flip só quando de fato é movimento lateral
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y) && Mathf.Abs(input.x) > 0.01f) {
            sr.flipX = input.x > 0f;        // sprite base olha para ESQUERDA → direita = flipX true
        } else {
            sr.flipX = lastMoveDir.x > 0f;  // parado: mantém o último lado
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
    }
}