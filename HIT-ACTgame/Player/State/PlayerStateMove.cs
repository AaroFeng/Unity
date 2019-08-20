using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMove : PlayerStateBase
{
    bool inWalk = false; //是否走路模式

    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.Move; //记录状态
        aniName = "Move"; //记录动画名

        speed = 6.0f; //设定水平移动速度
    }

    public override void OnEnter()
    {
        //进入状态 播放对应状态动画
        //默认动画状态
        inWalk = false; //默认奔跑模式
    }

    public override void OnControl()
    {
        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //检查是否可进行状态行为
        if (player.StateActionCheck(PlayerState.Damage))
        {
            //切换到受伤状态
            manager.ChangeState<PlayerStateDamage>();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) //键盘空格按下
        {
            if (player.StateActionCheck(PlayerState.Jump))
            {
                //切换到跳跃状态
                manager.ChangeState<PlayerStateJump>();
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt)) //键盘左Alt按下
        {
            if (player.StateActionCheck(PlayerState.Dodge))
            {
                //切换到闪避状态
                manager.ChangeState<PlayerStateDodge>();
                return;
            }
        }

        if(Input.GetKey(KeyCode.LeftShift)) //键盘左Shift按下
        {
            if (player.StateActionCheck(PlayerState.Guard))
            {
                //切换到防卫状态
                manager.ChangeState<PlayerStateGuard>();
                return;
            }
        }

        if (Input.GetMouseButtonDown(0)) //鼠标左键按下
        {
            if (player.StateActionCheck(PlayerState.Combo1))
            {
                //切换到连击1状态
                manager.ChangeState<PlayerStateCombo1>();
                return;
            }
        }

        if (Input.GetMouseButton(1)) //鼠标右键按住
        {
            if (player.StateActionCheck(PlayerState.ThumpCharge))
            {
                //切换到重击蓄力状态
                manager.ChangeState<PlayerStateThumpCharge>();
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

        if (Input.GetKeyDown(KeyCode.LeftControl)) //切换慢速移动状态
        {
            inWalk = !inWalk;
        }
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力

        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //获取键盘输入
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);

        if (move != Vector3.zero) //有输入
        {
            float walkSpeed; //走路模式速度

            if (inWalk) //走路模式
            {
                //设定动画混合树数值 播放走路动画
                animator.SetFloat("BlendNum", move.magnitude * -2.0f);
                walkSpeed = 0.3f; //设置移动速度
            }
            else
            {
                //设定动画混合树数值 播放奔跑动画
                animator.SetFloat("BlendNum", move.magnitude * 2.0f);
                walkSpeed = 1.0f;
            }

            //根据主摄像机 将direction转换为相对于相机的世界坐标
            move = Camera.main.transform.TransformDirection(move);
            move.y = 0; //y轴值清零
            move.Normalize();  //单位化向量大小 修正因摄像机Rotation导致的数值波动
            transform.rotation = Quaternion.LookRotation(move); //转向移动方向
            //水平移动
            move *= speed * walkSpeed;
            HoriMove.Set(move.x, 0, move.z); 
            cc.Move(HoriMove * Time.deltaTime);
        }
        else //无输入时清零水平移动
        {
            //设定动画混合树数值 播放Idle动画
            animator.SetFloat("BlendNum", move.magnitude);
        }
    }

    public override void OnExit()
    {
        
    }
}
