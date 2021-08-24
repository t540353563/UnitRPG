using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yellowBird : Bird
{
    /// <summary>
    /// 重写父类方法
    /// </summary>
    public override void ShowSkill()
    {
        base.ShowSkill();
        Rg.velocity *= 2;
    }
}
