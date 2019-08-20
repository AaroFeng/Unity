using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcKiStateDamage : OrcKiStateBase
{
    //脚步球形检测 是否落地
    Vector3 checkPoint; //球形检测 圆心
    float radius = 0.3f; //球形检测 半径
    bool onGround; //是否在地面

    public override void OnInit()
    {
        base.OnInit();
        orcKiState = OrcKiState.Damage;
        aniName = "Damage";
    }

    public override void OnEnter()
    {
        //播放对应动画
        animator.SetBool(aniName, true);
        //重置受伤触发
        orcKing.damage_ = false;
        //是否死亡
        if (orcKing.Death)
        {
            //切换到 死亡状态
            if (manager.ChangeState<OrcKiStateDeath>())
                return;
        }

        //冲击力 赋值
        HoriMove.Set(orcKing.DamageImpact.x, 0, orcKing.DamageImpact.z); //水平方向移动
        vertiMove.Set(0, orcKing.DamageImpact.y, 0); //垂直方向移动 重力

        //开始时在地面
        onGround = true;

        //播放粒子效果组
        particle.Play(orcKiState);

        //兽人首领 冲击力过小 直接回到追逐状态
        if (orcKing.DamageImpact.magnitude < 20.0f)
        {
            //切换到追逐状态
            if (manager.ChangeState<OrcKiStateChase>())
                return;
        }
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力
        RefreshSkillCD(); //刷新技能CD

        //状态保护 进入动画之后才执行
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //重置再次受伤
        if (animator.GetBool("DamageAgain"))
            animator.SetBool("DamageAgain", false);
        if (orcKing.damage_)
        {
            //连续受伤
            animator.SetBool("DamageAgain", true);
            //切换到 受伤状态
            if (manager.ChangeState<OrcKiStateDamage>())
                return;
        }

        if(onGround)
        {
            //受伤动画结束后
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f)
            {
                //切换到追逐状态
                if (manager.ChangeState<OrcKiStateChase>())
                    return;
            }
        }

        //----------

        //水平移动速度随时间衰减
        if (HoriMove.magnitude > 0.05f)
        {
            if (onGround)
                HoriMove += -HoriMove * Time.deltaTime * 10.0f;
            else
                HoriMove += -HoriMove * Time.deltaTime * 3.0f;
        }
        else
            HoriMove = Vector3.zero;

        //动态设定水平速度
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.70f)
            cc.Move(HoriMove * Time.deltaTime);
    }

    public override void OnExit()
    {
        //退出对应动画
        animator.SetBool(aniName, false);

        //停止粒子效果组
        //particle.Stop(orcKiState);
    }
}
