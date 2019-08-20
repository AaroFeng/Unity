using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateGuardBreak : PlayerStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.GuardBreak; //记录状态
        aniName = "GuardBreak"; //记录动画名
    }

    public override void OnEnter()
    {
        //进入状态 播放对应状态动画
        animator.SetInteger("Damage", 3);

        //冲击力 赋值
        HoriMove.Set(player.DamageImpact.x, 0, player.DamageImpact.z); //水平方向移动
        vertiMove.Set(0, player.DamageImpact.y, 0); //垂直方向移动 重力
    }

    public override void OnControl()
    {
        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //受伤动画结束后
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f)
        {
            if (player.StateActionCheck(PlayerState.Death))
            {
                //切换到 死亡状态
                manager.ChangeState<PlayerStateDeath>();
                return;
            }

            if (player.StateActionCheck(PlayerState.Move))
            {
                //切换到移动状态
                manager.ChangeState<PlayerStateMove>();
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) //键盘R键按下
        {
            if (player.StateActionCheck(PlayerState.Skill1))
            {
                //切换到技能1状态
                manager.ChangeState<PlayerStateSkill1>();
                return;
            }
        }
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力

        //水平移动速度衰减
        HoriMove.x *= 0.6f;
        HoriMove.z *= 0.6f;

        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
        {
            cc.Move(HoriMove * Time.deltaTime); //无间隙 直接进行冲击力移动
            return;
        }
        
        //动态设定水平速度
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.7f)
            cc.Move(HoriMove * Time.deltaTime);
    }

    public override void OnExit()
    {
        //离开状态 离开对应状态动画
        animator.SetInteger("Damage", 0);
    }
}
