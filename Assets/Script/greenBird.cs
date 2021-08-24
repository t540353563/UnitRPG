using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class greenBird : Bird
{
    /// <summary>
    /// 重写父类方法
    /// </summary>
    public override void ShowSkill()
    {
        base.ShowSkill();
        Vector3 speed = Rg.velocity;
        speed.x *= -1;
        Rg.velocity = speed;
    }
}
