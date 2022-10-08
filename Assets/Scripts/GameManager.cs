using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public gameObject[] stages;

    // Start is called before the first frame update
    public void NextStage()
    {
        if(stageIndex < Stages.Length)
        {
          Stages[stageIndex].SetActive(false);
          stageIndex++;
          Stages[stageIndex].SetActive(true);
          PlayerReposition();
        }

        else
        {
          Time.timeScale = 0;
        }

        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
      if(health > 0)
        HealthDown();

      else
      {
        player.OnDie();

      }
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
      if(collision.gameObject.tag == "Player")
      {
        if(health > 1)
        {
          PlayerReposition();
        }

        HealthDown();

      }

    }

    void PlayerReposition()
    {
      player.transform.position = new Vector3(0, 0, -1);
      player.VelocityZero();
    }
}
