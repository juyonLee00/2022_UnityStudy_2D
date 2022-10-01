using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Invoke("Think", 5);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0,1,0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if(rayHit.collider == null)
        {
          Turn();
        }
    }

    void Think()
    {
      //Set Next Active
      nextMove = Random.Range(-1, 2);

      //Sprite Animation
      anim.SetInteger("WalkSpeed", nextMove);

      //Flip Sprite
      if(nextMove != 0)
        spriteRenderer.flipX = nextMove == 1;

      //Recursive
      float nextThinkTime = Random.Range(2, 5);
      Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
      nextMove *= -1;
      spriteRenderer.flipX = nextMove == 1;

      CancelInvoke();
      Invoke("Think", 5);
    }
}
