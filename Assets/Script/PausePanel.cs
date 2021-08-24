using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    private Animator anim;

    public GameObject button;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }

    public void Pause()
    {
        anim.SetBool("isPause", true);
        button.SetActive(false);
    }

    public void Home()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        anim.SetBool("isPause", false);
    }

    /// <summary>
    /// 暂停动画播放完
    /// </summary>
    public void pauseAnimEnd()
    {
        Time.timeScale = 0;
    }

    /// <summary>
    /// 返回动画播放完
    /// </summary>
    public void ResumeAnimEnd()
    {
        button.SetActive(true);
    }

}
