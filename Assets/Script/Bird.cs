using Assets.Script;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private bool isClick = false;

    public TestMyTrail myTrail;

    private bool canMove = true;
    private float smooth = 5;

    public bool isFly = false;

    [HideInInspector]
    public SpringJoint2D Sp;
    public Rigidbody2D Rg;
    public SpriteRenderer render;

    public float MaxDis = 1.3f;
    public Transform RightPods;
    public Transform LeftPods;

    public GameObject boom;

    public LineRenderer left;
    public LineRenderer right;

    public AudioClip selectAudio;
    public AudioClip flyAudio;


    #region 基础功能
    private void OnMouseDown()
    {
        if (canMove)
        {
            AudioPlay(selectAudio);
            isClick = true;
            Rg.isKinematic = true;
        }
    }

    private void OnMouseUp()
    {
        if (canMove)
        {
            AudioPlay(flyAudio);

            isClick = false;
            Rg.isKinematic = false;
            Invoke("Fly", 0.1f);

            //禁用画线
            left.enabled = false;
            right.enabled = false;

            canMove = false;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isFly = false;
        myTrail.TrailClear();
    }

    private void Awake()
    {
        Sp = GetComponent<SpringJoint2D>();
        Rg = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();

        myTrail = GetComponent<TestMyTrail>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isClick)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position -= new Vector3(0, 0, Camera.main.transform.position.z);

            if (Vector3.Distance(transform.position, RightPods.position) > MaxDis)
            {
                Vector3 pos = (transform.position - RightPods.position).normalized;//单位化向量
                pos *= MaxDis;//最大长度向量
                transform.position = pos + RightPods.position;
            }

            Line();
        }

        //相机跟随 镜头水平移动因此y不变
        Vector3 clampPos = new Vector3(Mathf.Clamp(transform.position.x, 0, 15), Camera.main.transform.position.y, Camera.main.transform.position.z);
        //差值平滑运动
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, clampPos, smooth);

        if (isFly)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShowSkill();
            }
        }

    }

    /// <summary>
    /// 飞出接触绑定关系
    /// </summary>
    private void Fly()
    {
        isFly = true;

        Sp.enabled = false;

        myTrail.TrailStart();

        Invoke("Next", 3);
    }

    /// <summary>
    /// 划线
    /// </summary>
    private void Line()
    {
        //启用画线
        left.enabled = true;
        right.enabled = true;

        left.SetPosition(0, LeftPods.position);
        left.SetPosition(1, transform.position);

        right.SetPosition(0, RightPods.position);
        right.SetPosition(1, transform.position);
    }

    /// <summary>
    /// 下一只小鸟飞出
    /// </summary>
    public virtual void Next()
    {
        GameManager._instance.birds.Remove(this);

        Destroy(gameObject);
        Instantiate(boom, transform.position, Quaternion.identity);

        GameManager._instance.NextBird();
    }

    public void AudioPlay(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    public virtual void ShowSkill()
    {
        isFly = false;
    }
    #endregion

}
