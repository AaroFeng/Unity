using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateThump : PlayerStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.Thump; //记录状态
        aniName = "Thump"; //记录动画名
    }

    public override void OnEnter()
    {
        //进入状态 播放对应状态动画
        animator.SetInteger("Thump", 2);

        //播放粒子效果组
        particle.Play(playerState);
    }

    public override void OnControl()
    {
        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //检查是否可进行状态行为
        if (player.StateActionCheck(PlayerState.Damage))
        {
            //不切换到受伤状态
        }

        //重击动画结束后
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f)
        {
            //检查是否可进行状态行为
            if (player.StateActionCheck(PlayerState.Move))
            {
                //切换到移动状态
                manager.ChangeState<PlayerStateMove>();
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

        //应用动画位移

        //判断重击动画
        if(animator.GetFloat("BlendNum") == 0)
        {
            //相机震动
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.17f &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.25f)
                camera.Shake(0.06f, 5.5f, 0.003f, 0.25f);
        }
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.12f &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.27f)
                camera.Shake(0.08f, 4.5f, 0.004f, 0.3f);
        }
        
    }

    public override void OnExit()
    {
        //离开状态 离开对应状态动画
        animator.SetInteger("Thump", 0);

        //停止粒子效果组
        particle.Stop(playerState);
    }
}
