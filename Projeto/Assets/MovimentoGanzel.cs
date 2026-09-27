using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovimentoGanzel : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float velocidade = 5f;

    private Rigidbody2D rb2d;
    private Vector2 direcao;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0f;
        rb2d.freezeRotation = true;
    }

    private void Update()
    {
        float movimentoHorizontal = Input.GetAxisRaw("Horizontal");
        float movimentoVertical = Input.GetAxisRaw("Vertical");

        direcao = new Vector2(movimentoHorizontal, movimentoVertical).normalized;
    }

    private void FixedUpdate()
    {
        rb2d.velocity = direcao * velocidade;
    }

    private void OnDisable()
    {
        direcao = Vector2.zero;
        if (rb2d != null)
        {
            rb2d.velocity = Vector2.zero;
        }
    }
}

