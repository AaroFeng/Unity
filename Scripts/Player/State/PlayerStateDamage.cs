using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDamage : PlayerStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.Damage; //记录状态
        aniName = "Damage"; //记录动画名
    }

    public override void OnEnter()
    {
        if (player.StateActionCheck(PlayerState.Death))
        {
            //切换到 死亡状态
            manager.ChangeState<PlayerStateDeath>();
            return;
        }

        //进入状态 播放对应状态动画
        animator.SetInteger("Damage", 1);

        //冲击力 赋值
        HoriMove.Set(player.DamageImpact.x, 0, player.DamageImpact.z); //水平方向移动
        vertiMove.Set(0, player.DamageImpact.y, 0); //垂直方向移动 重力

        //转向冲击力反方向
        transform.rotation = Quaternion.LookRotation(-HoriMove);

        //播放粒子效果组
        particle.Play(playerState);
    }

    public override void OnControl()
    {
        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //重置再次受伤
        if (animator.GetBool("DamageAgain"))
            animator.SetBool("DamageAgain", false);

        //是否连续受伤
        if (player.StateActionCheck(PlayerState.Damage))
        {
            //连续受伤
            animator.SetBool("DamageAgain", true);

            //切换到受伤状态
            manager.ChangeState<PlayerStateDamage>();
            return;
        }

        //受伤动画30%后
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (player.StateActionCheck(PlayerState.Dodge))
            {
                //切换到闪避状态
                manager.ChangeState<PlayerStateDodge>();
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && onGround) //键盘R键按下
        {
            if (player.StateActionCheck(PlayerState.Skill1))
            {
                //切换到技能1状态
                manager.ChangeState<PlayerStateSkill1>();
                return;
            }
        }

        //受伤动画结束后 在地面
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f && onGround)
        {
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

        //水平移动速度随时间衰减
        if (HoriMove.magnitude > 0.05f)
        {
            if (onGround)
                HoriMove += -HoriMove * Time.deltaTime * 10.0f;
            else
                HoriMove += -HoriMove * Time.deltaTime * 2.8f;
        }
        else
            HoriMove = Vector3.zero;

        //受伤动画转换
        if (onGround)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.2f)
                animator.SetFloat("BlendNum", 0f);
        }
        else
        {
            if (animator.GetFloat("BlendNum") < 1)
                animator.SetFloat("BlendNum", animator.GetFloat("BlendNum") + 0.1f);
        }

        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
        {
            cc.Move(HoriMove * Time.deltaTime); //无间隙 直接进行冲击力移动
            return;
        }

        //动态设定水平速度
        cc.Move(HoriMove * Time.deltaTime);
    }

    public override void OnExit()
    {
        //离开状态 离开对应状态动画
        animator.SetInteger("Damage", 0);

        //停止粒子效果组
        particle.Stop(playerState);
    }
}
