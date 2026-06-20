using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f; // Velocidade do passo
    [SerializeField] private float gridSize = 1f;  // Tamanho de cada quadrado no mapa

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private bool isMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Garante que o Rigidbody não mude a física sozinho e ativa a interpolação para ficar suave
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        // Define a posição inicial como o primeiro destino
        targetPosition = rb.position;
    }

    private void Update()
    {
        // Se já estiver se movendo para um bloco, não aceita novos comandos
        if (isMoving) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Impede movimento diagonal (estilo Pokémon clássico: ou anda no X ou no Y)
        if (moveX != 0) moveY = 0;

        // Se o jogador apertou alguma direção
        if (moveX != 0 || moveY != 0)
        {
            Vector2 direction = new Vector2(moveX, moveY);
            
            // Calcula a posição do próximo bloco no mapa
            targetPosition = (Vector2)transform.position + direction * gridSize;
            
            // Inicia a caminhada até o bloco
            isMoving = true;
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            // Move suavemente o Rigidbody em direção ao destino final do bloco
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            // Se chegou muito perto ou exatamente no destino, para de andar e se alinha perfeitamente
            if (Vector2.Distance(rb.position, targetPosition) < 0.01f)
            {
                rb.MovePosition(targetPosition);
                isMoving = false;
            }
        }
        else
        {
            // Garante que a velocidade física seja zero quando parado para evitar deslizar
            rb.linearVelocity = Vector2.zero;
        }
    }
}