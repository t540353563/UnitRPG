using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blackBird : Bird
{
    public List<Pig> pigs = new List<Pig>();
    public List<block> blocks = new List<block>();

    /// <summary>
    /// 重写父类方法
    /// </summary>
    public override void ShowSkill()
    {
        base.ShowSkill();
        for (int i = 0; i < pigs.Count; i++)
        {
            pigs[i].Dead();
        }

        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].Dead();
        }

        Onclear();
    }

    public override void Next()
    {
        GameManager._instance.birds.Remove(this);
        Destroy(gameObject);
        GameManager._instance.NextBird();
    }

    /// <summary>
    ///  碰撞检测
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.GetComponent<Pig>() != null)
            {
                pigs.Add(collision.gameObject.GetComponent<Pig>());
            }
            else if (collision.gameObject.GetComponent<block>() != null)
            {
                blocks.Add(collision.gameObject.GetComponent<block>());
            }
        }
    }

    /// <summary>
    /// 脱离碰撞范围
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.GetComponent<Pig>() != null)
            {
                pigs.Remove(collision.gameObject.GetComponent<Pig>());
            }
            else if (collision.gameObject.GetComponent<block>() != null)
            {
                blocks.Remove(collision.gameObject.GetComponent<block>());
            }
        }
    }


    public void Onclear()
    {
        Rg.velocity = Vector3.zero;
        Instantiate(boom, transform.position, Quaternion.identity);

        render.enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        myTrail.TrailClear();
    }

}
