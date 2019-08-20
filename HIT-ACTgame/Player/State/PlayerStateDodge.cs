using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDodge : PlayerStateBase
{
    bool nextCombo; //鼠标连击是否按下
    float jumpSpeed = 8.0f; //向上跳跃力度

    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.Dodge; //记录状态
        aniName = "Dodge"; //记录动画名

        speed = 13.0f; //设定水平移动速度
    }

    public override void OnEnter()
    {
        //进入状态 播放对应状态动画
        animator.SetBool(aniName, true);

        //开始跳跃时 获取一次输入
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        //判断动画 根据摄像机方向转换direction 玩家转向
        if (move != Vector3.zero) //有输入
        {
            //根据主摄像机 将direction转换为相对于相机的世界坐标
            move = Camera.main.transform.TransformDirection(move);
            move.y = 0; //y轴值清零
            move.Normalize();  //单位化向量大小 修正因摄像机Rotation导致的数值波动

            float angle = Vector3.Angle(move, transform.forward); //计算闪避方向 与 玩家当前朝向 的角度

            if (angle < 140) //非后方闪避
            {
                transform.rotation = Quaternion.LookRotation(move); //转向移动方向
                animator.SetFloat("BlendNum", 1);//设定 闪避动画 混合树数值
            }
            else //后方闪避
            {
                transform.rotation = Quaternion.LookRotation(-move); //转向移动方向的反方向
                animator.SetFloat("BlendNum", -1);//设定 闪避动画 混合树数值
            }
            
        }
        else
        {
            move = transform.forward;//向当前Player前方闪避
            animator.SetFloat("BlendNum", 1); //设定动画混合树数值 播放跳跃动画
        }
        //水平移动 赋值给 最终移动方向
        move *= speed;
        vertiMove.y = jumpSpeed; //跳跃向上速度
        HoriMove.Set(move.x, 0, move.z); //水平移动速度

        //默认鼠标连击未按下
        nextCombo = false;

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
            //不切换到受伤状态 是否死亡
            if (player.StateActionCheck(PlayerState.Move))
            {
                //切换到移动状态
                manager.ChangeState<PlayerStateMove>();
                return;
            }
        }

        //有效连击时间内
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                nextCombo = true; //记录按下
            }
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && nextCombo)
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

        //闪避动画结束后
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

        if(animator.GetFloat("BlendNum") == 1) //非后方闪避
        {
            //动态设定水平速度
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.1f)
                cc.Move(HoriMove * 0.3f * Time.deltaTime);
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.6f)
                cc.Move(HoriMove * Time.deltaTime);
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
                cc.Move(HoriMove * 0.3f * Time.deltaTime);
        }
        else //后方闪避
        {
            cc.Move(new Vector3(0, gravity, 0) * Time.deltaTime); //重力速度
            //应用动画位移
        }
    }

    public override void OnExit()
    {
        //离开状态 离开对应状态动画
        animator.SetBool(aniName, false);

        //停止粒子效果组
        particle.Stop(playerState);
    }
}
