using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;  // 移動速度
    public float rotateSpeed = 5f;  // 旋轉速度

    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        MoveInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MoveInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 根據輸入設置動畫參數
        float moveMagnitude = Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput);
        anim.SetFloat("Move", moveMagnitude);

        // 根據輸入設置角色朝向
        if (moveMagnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(horizontalInput, 0f, verticalInput));
            rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    private void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
        movement.Normalize();

        // 使用rb.MovePosition()移動角色
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
    }
}
