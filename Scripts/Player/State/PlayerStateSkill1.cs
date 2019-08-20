using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateSkill1 : PlayerStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.Skill1; //记录状态
        aniName = "Skill1a"; //记录动画名
    }

    public override void OnEnter()
    {
        //进入状态 播放对应状态动画
        animator.SetInteger("Skill", 1);

        //播放粒子效果组
        particle.Play(playerState);
    }

    public override void OnControl()
    {
        //状态保护 进入动画后才执行方法
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill1a")) //技能1动画第一部分
        {
            //技能1动画第一部分 结束后
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
            {
                //离开 技能1动画第一部分
                animator.SetInteger("Skill", 0);
            }

            //相机震动
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.14f &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
                camera.Shake(0.02f, 5.0f, 0.002f, 0.3f);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill1b")) //技能1动画第二部分
        {
            //技能1动画第二部分 结束后
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
            {
                if (player.StateActionCheck(PlayerState.Move))
                {
                    //切换到移动状态
                    manager.ChangeState<PlayerStateMove>();
                    return;
                }
            }

            //相机震动
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.10f &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.15f)
                camera.Shake(0.05f, 5.0f, 0.002f, 0.3f);
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.50f &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.60f)
                camera.Shake(0.08f, 3.0f, 0.04f, 0.6f);
        }  
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力

        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //应用动画位移
    }

    public override void OnExit()
    {
        //离开状态 离开对应状态动画
        animator.SetInteger("Skill", 0);

        //停止粒子效果组
        particle.Stop(playerState);
    }
}
