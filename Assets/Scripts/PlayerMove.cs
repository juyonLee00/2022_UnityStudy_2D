using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public float maxSpeed;
    public float jumpPower;
    SpriteRenderer spriteRenderer;
    Animator anim;
    public GameManager gameManager;

    void Awake()
    {
      rigid = GetComponent<Rigidbody2D>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      anim = GetComponent<Animator>();
    }

    private void Update()
    {
      //Jump
      if(Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
      {
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        anim.SetBool("isJumping", true);
      }

      //Stop Speed
      if(Input.GetButtonUp("Horizontal"))
      {
        rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
      }

      //움직임에 따른 방향 전환
      if(Input.GetButton("Horizontal"))
        spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

      //애니메이션 추가
      if(Mathf.Abs(rigid.velocity.x) < 0.3)
        anim.SetBool("isWalking", false);
      else
        anim.SetBool("isWalking", true);

    }

    void FixedUpdate()
    {
        //키보드로 움직임 조작
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxSpeed)
          rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if(rigid.velocity.x < maxSpeed*(-1))
          rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        //Landing Platform-Ray
        if(rigid.velocity.y < 0)
        {
          Debug.DrawRay(rigid.position, Vector3.down, new Color(0,1,0));

          RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

          //rayHit 관통안됨
          if(rayHit.collider != null)
          {
            if(rayHit.distance < 0.5f)
              anim.SetBool("isJumping", false);
          }

        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
      if(collision.gameObject.tag == "Enemy")
      {
        //Attack
        if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
        {
          OnAttack(collision.transform);
        }
        else
          OnDamaged(collision.transform.position);
      }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
      if(collision.gameObject.tag == "Item")
      {
        //Point
        gameManager.stagePoint += 100;

        //Deactive Item
        collision.gameObject.SetActive(false);
      }

      else if(collision.gameObject.tag == "Finish")
      {
        //Next Stage
        gameManager.NextStage();
      }
    }

    void OnAttack(Transform enemy)
    {
      //Point
      gameManager.stagePoint += 100;

      //반발력 추가
      rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

      //Enemy die
      EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
      enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
      //HealthDown
      gameManager.HealthDown();
      //Change Layer(Immortal Active)
      gameObject.layer = 11;

      spriteRenderer.color = new Color(1, 1, 1, 0.4f);

      //Reaction Force
      int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
      rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

      Invoke("OffDamaged", 3);
      OffDamaged();
    }

    void OffDamaged()
    {
      gameObject.layer = 10;
      spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
      //Sprite Alpha
      spriteRenderer.color = new Color(1, 1, 1, 0.4f);

      //Sprite Flip Y
      spriteRenderer.flipY = true;

      //collider Disable
      //boxcollider.enabled = false;

      //Die Effect jump
      rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
      rigid.velocity = Vector2.zero;
    }
}
