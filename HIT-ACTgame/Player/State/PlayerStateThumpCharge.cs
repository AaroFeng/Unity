using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateThumpCharge : PlayerStateBase
{
    float chargeTime; //蓄力时间

    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.ThumpCharge; //记录状态
        aniName = "ThumpCharge"; //记录动画名
    }

    public override void OnEnter()
    {
        //进入状态 播放对应状态动画
        animator.SetInteger("Thump", 1);

        //重置蓄力时间
        chargeTime = 0.0f;

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

        if (Input.GetKeyDown(KeyCode.R)) //键盘R键按下
        {
            if (player.StateActionCheck(PlayerState.Skill1))
            {
                //切换到技能1状态
                manager.ChangeState<PlayerStateSkill1>();
                return;
            }
        }

        //右键持续按住
        if (Input.GetMouseButton(1))
        {
            chargeTime += Time.deltaTime; //累计蓄力时间
        }
        else
        {
            //根据蓄力时间判断重击等级
            if (chargeTime < 2.0f)
            {
                animator.SetFloat("BlendNum", 0); //低重击
                //切换到重击状态
                if (player.StateActionCheck(PlayerState.Thump))
                {
                    manager.ChangeState<PlayerStateThump>();
                    return;
                }
            }
            else
            {
                animator.SetFloat("BlendNum", 1); //高重击
                //切换到重击状态
                if (player.StateActionCheck(PlayerState.Thump))
                {
                    manager.ChangeState<PlayerStateThump>();
                    return;
                }
            }
        }
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力

        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //相机震动
        camera.Shake(0.01f, 6.0f, 0.002f, 0.2f);
    }

    public override void OnExit()
    {
        //停止粒子效果组
        particle.Stop(playerState);
    }
}
