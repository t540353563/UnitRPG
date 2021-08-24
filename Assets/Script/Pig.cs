using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    public float maxSpeed = 8;
    public float minSpeed = 5;

    public Sprite hurt;
    public GameObject boom;
    public GameObject score;

    public AudioClip collisionAudio;
    public AudioClip dieAudio;
    public AudioClip birdCollisonAudio;

    private SpriteRenderer render;

    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //小鸟撞击发出声音
        if (collision.gameObject.tag == "Player")
        {
            AudioPlay(birdCollisonAudio);
        }

        //相撞物体的相对速度大于最大速度 ，判定猪死亡
        if (collision.relativeVelocity.magnitude > maxSpeed)
        {
            Dead();
        }
        else if (collision.relativeVelocity.magnitude < maxSpeed && collision.relativeVelocity.magnitude > minSpeed) //受伤
        {
            AudioPlay(collisionAudio);
            render.sprite = hurt;
        }
        else
        {

        }

    }

    public void Dead()
    {
        //播放音乐
        AudioPlay(dieAudio);

        GameManager._instance.pigs.Remove(this);

        Destroy(gameObject);

        GameObject go = Instantiate(score, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        Destroy(go, 1.5f);

        Instantiate(boom, transform.position, Quaternion.identity);

    }

    public void AudioPlay(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }


}
