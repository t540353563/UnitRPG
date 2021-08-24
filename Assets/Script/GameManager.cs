using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //定义待执行的子线程事件队列
    private static bool NoUpdate = true;
    private static List<Action> UpdateQueue = new List<Action>();
    private static List<Action> UpdateRunQueue = new List<Action>();

    public List<Bird> birds;
    public List<Pig> pigs;
    public static GameManager _instance;

    public GameObject win, lose;

    public GameObject[] starts;

    //小鸟初始位置
    private Vector3 originPod;

    //挂载socket脚本
    const int portNo = 500;
    private TcpClient _client;
    byte[] data;
    string Error_Message;

    void Start()
    {
        try
        {
            this._client = new TcpClient();
            this._client.Connect("127.0.0.1", portNo);
            data = new byte[this._client.ReceiveBufferSize];
            //SendMessage(txtNick.Text);
            SendMessage("Unity Demo Client is Ready!");
            this._client.GetStream().BeginRead(data, 0, Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage, null);
        }
        catch (Exception ex)
        {
        }

        Initialized();
    }

    private void Awake()
    {
        _instance = this;

        if (birds.Count > 0)
            originPod = birds[0].transform.position;
    }

    private void Update()
    {
        lock (UpdateQueue)
        {
            if (NoUpdate) return;
            UpdateRunQueue.AddRange(UpdateQueue);
            UpdateQueue.Clear();
            NoUpdate = true;
            for (var i = 0; i < UpdateRunQueue.Count; i++)
            {
                var action = UpdateRunQueue[i];
                if (action == null) continue;
                action();
            }
            UpdateRunQueue.Clear();
        }
    }

    #region 基础功能
    private void Initialized()
    {
        for (int i = 0; i < birds.Count; i++)
        {
            if (i == 0)
            {
                birds[i].transform.position = originPod;
                birds[i].enabled = true;
                birds[i].Sp.enabled = true;
            }
            else
            {
                birds[i].enabled = false;
                birds[i].Sp.enabled = false;
            }
        }
    }

    /// <summary>
    /// 游戏判定逻辑
    /// </summary>
    public void NextBird()
    {
        if (pigs.Count > 0)
        {
            if (birds.Count > 0)
            {
                //飞下一只
                Initialized();
            }
            else
            {
                //输了
                lose.SetActive(true);
            }
        }
        else
        {
            //赢了
            win.SetActive(true);
        }
    }


    public void ShowStarts()
    {
        StartCoroutine("show");
    }

    IEnumerator show()
    {
        //根据剩余的小鸟显示星星
        for (int i = 0; i <= birds.Count; i++)
        {
            yield return new WaitForSeconds(0.5f);
            starts[i].SetActive(true);
        }
    }

    public void Replay()
    {
        SceneManager.LoadScene(2);
    }

    public void Home()
    {
        SceneManager.LoadScene(1);
    }
    #endregion

    #region 外部通信
    public new void SendMessage(string message)
    {
        try
        {
            NetworkStream ns = this._client.GetStream();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            ns.Write(data, 0, data.Length);
            ns.Flush();
        }
        catch (Exception ex)
        {
            Error_Message = ex.Message;
            //MessageBox.Show(ex.ToString());
        }
    }

    public void ReceiveMessage(IAsyncResult ar)
    {
        try
        {
            //清空errormessage
            Error_Message = "";
            int bytesRead;
            bytesRead = this._client.GetStream().EndRead(ar);
            if (bytesRead < 1)
            {
                return;
            }
            else
            {
                Debug.Log(System.Text.Encoding.ASCII.GetString(data, 0, bytesRead));
                string message = System.Text.Encoding.ASCII.GetString(data, 0, bytesRead);
                //ReceiveMsgDo(message);
                switch (message)
                {
                    case "1":
                        ExecuteUpdate(new Action(() => { Replay(); }));
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Error_Message = ex.Message;
        }
        finally
        {
            this._client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage, null);
        }
    }

    /// <summary>
    /// 接收Unity消息进行操作
    /// </summary>
    /// <param name="msg"></param>
    public void ReceiveMsgDo(string msg)
    {
        //List<MessageData> datas = JsonConvert.DeserializeObject<List<MessageData>>(msg);
        //foreach (MessageData data in datas)
        //{
        //    GameMassageReciver.Instance.SetCurrentOperateCommand(data.OperateType, data.OperateData.ToString(), data.OperateTarget);
        //}

        switch (msg)
        {
            case "1":
                ExecuteUpdate(new Action(() => { Replay(); }));
                break;
        }
    }


    void OnDestroy()
    {
        this._client.Close();
    }
    #endregion

    #region 线程调用
    //添加待执行子线程事件的接口
    public void ExecuteUpdate(Action action)
    {
        lock (UpdateQueue)
        {
            UpdateQueue.Add(action);
            NoUpdate = false;
        }
    }
    #endregion
}
