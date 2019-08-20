using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateGuard : PlayerStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.Guard; //记录状态
        aniName = "Guard"; //记录动画名
    }

    public override void OnEnter()
    {
        //进入状态 播放对应状态动画
        animator.SetBool(aniName, true);

        //播放粒子效果组
        particle.Play(playerState);
    }

    public override void OnControl()
    {
        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //检查是否可进行状态行为
        if (player.StateActionCheck(PlayerState.GuardDamage))
        {
            //切换到防卫受伤状态
            manager.ChangeState<PlayerStateGuardDamage>();
            return;
        }

        //检查耐力值是否耗尽
        if (!player.StateActionCheck(PlayerState.Guard))
        {
            if (player.StateActionCheck(PlayerState.Move))
            {
                //切换到移动状态
                manager.ChangeState<PlayerStateMove>();
                return;
            }
        }

        if (!Input.GetKey(KeyCode.LeftShift)) //放开LeftShift
        {
            //检查是否可进行状态行为
            if (player.StateActionCheck(PlayerState.Move))
            {
                //切换到移动状态
                manager.ChangeState<PlayerStateMove>();
                return;
            }    
        }

        if (Input.GetMouseButtonDown(0)) //鼠标左键按下
        {
            //检查是否可进行状态行为
            if (player.StateActionCheck(PlayerState.Combo1))
            {
                //切换到连击1状态
                manager.ChangeState<PlayerStateCombo1>();
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

        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;
    }

    public override void OnExit()
    {
        //离开状态 离开对应状态动画
        animator.SetBool(aniName, false);

        //停止粒子效果组
        particle.Stop(playerState);
    }
}
