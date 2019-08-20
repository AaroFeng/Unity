using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    PlayerStateManager manager;
    PlayerParticle particle;

    //人物属性值
    float atk = 5; //攻击力
    public float Atk { get { return atk; }set { atk = value; } }
    float hp; //体力值
    float hpMax = 800; //最大体力值
    public float Hp { get { return hp; } set { hp = value; } }
    public float HpMax { get { return hpMax; } set { hpMax = value; } }
    float sp; //耐力值
    float spMax = 100; //最大耐力值
    public float Sp { get { return sp; } set { sp = value; } }
    public float SpMax { get { return spMax; } set { spMax = value; } }
    float ep; //能量值 
    float epMax = 100; //最大能量值
    public float Ep { get { return ep; } set { ep = value; } }
    public float EpMax { get { return epMax; } set { epMax = value; } }
    int epPoint; //能量点
    int epPointMax = 3; //最大能量点
    public int EpPoint { get { return epPoint; } set { epPoint = value; } }
    public int EpPointMax { get { return epPointMax; }set { epPointMax = value; } }

    bool spUseup = false; //耐力耗尽
    public bool SpUseup { get { return spUseup; } set { spUseup = value; } }

    bool damage = false; //受伤触发
    bool death = false; //是否死亡
    public bool Death { get { return death; } }

    float spCost; //耐力值消耗量
    float spGain; //耐力值获得量
    float epGain; //能量值获得量
    int epPointCost; //能量点消耗量

    //攻击范围检测
    Vector3 checkPoint; //球形攻击范围 球心 
    float atkRadius; //球形攻击范围 半径
    float atkAngle; //球形攻击范围 有效角度
    float realAtk; //真实伤害
    Vector3 atkDirection; //攻击方向
    //水平圆形16方向向量 动态动态圆形攻击向量组
    List<Vector3> circleDirection = new List<Vector3>();
    int nowDirec;
    //攻击动作冲击力
    float impactHori; //水平冲击力
    float impactVeti; //垂直冲击力
    Vector3 AtkImpact = new Vector3(); //最终冲击力向量

    //受伤承受冲击力向量
    Vector3 damageImpact = new Vector3();
    public Vector3 DamageImpact
    {
        get { return damageImpact; }
    }

    void OnEnable() //死亡时 重新激活
    {
        if(death)
        {
            death = false;
            //初始化属性值
            hp = hpMax;
            sp = spMax;
            ep = 0;
            epPoint = 3;
            spGain = 20.0f; //初始状态为 Move 每秒获得耐力值
        }
    }

    void Start()
    {
        manager = GetComponent<PlayerStateManager>();
        particle = GetComponent<PlayerParticle>();

        //初始化属性值
        hp = hpMax;
        sp = spMax;
        ep = 0;
        epPoint = 3;
        spGain = 20.0f; //初始状态为 Move 每秒获得耐力值
    }

    public void Damage(float enemyAtk,Vector3 AtkImpact)
    {
        //机能状态免伤
        if (manager.CurrentState.PlayerState.Equals(PlayerState.Skill1))
            return;

        //闪避状态减伤
        if (manager.CurrentState.PlayerState.Equals(PlayerState.Dodge))
            enemyAtk *= 0.2f; //减伤80%

        //防御状态减伤
        if (manager.CurrentState.PlayerState.Equals(PlayerState.Guard) ||
            manager.CurrentState.PlayerState.Equals(PlayerState.GuardDamage)) 
            enemyAtk *= 0.5f; //减伤50%

        damage = true; //触发受伤状态

        //受到伤害 但不进入 受伤状态动画 的状态
        if (manager.CurrentState.PlayerState.Equals(PlayerState.Jump) ||
            manager.CurrentState.PlayerState.Equals(PlayerState.Thump) ||
            manager.CurrentState.PlayerState.Equals(PlayerState.Thump1) ||
            manager.CurrentState.PlayerState.Equals(PlayerState.Thump2) ||
            manager.CurrentState.PlayerState.Equals(PlayerState.ThumpCharge) ||
            manager.CurrentState.PlayerState.Equals(PlayerState.Skill1)) 
            damage = false;

        damageImpact = AtkImpact; //记录受到的冲击力

        //减少hp
        hp -= enemyAtk;
        if (hp <= 0) //是否死亡
        {
            hp = 0;
            death = true;
        }
    }

    public void RefreshAttribute()
    {
        //耐力耗尽
        if (spUseup)
        {
            if (sp >= spMax) //耐力值回满时 
                spUseup = false;
        }
        else if (sp < 1) //耐力值耗尽时
            spUseup = true;
            
        //空闲 移动 状态时 持续恢复耐力值
        if (manager.CurrentState.PlayerState.Equals(PlayerState.Idle) ||
            manager.CurrentState.PlayerState.Equals(PlayerState.Move) ||
            manager.CurrentState.PlayerState.Equals(PlayerState.Damage))
        {
            sp += spGain * Time.deltaTime; //每秒获得耐力值
            if (sp > spMax)
                sp = spMax;
        }
        //防御 重击蓄力 状态时 持续消耗耐力值
        else if (manager.CurrentState.PlayerState.Equals(PlayerState.Guard) ||
            manager.CurrentState.PlayerState.Equals(PlayerState.ThumpCharge))
        {
            sp -= spCost * Time.deltaTime; //每秒消耗耐力值
            if (sp < 0)
                sp = 0;
        }

        //能量值 蓄满时 获得一点能量点
        if (ep >= epMax)
        {
            //能量点未满时
            if (epPoint < epPointMax)
            {
                ep = 0; //清空能量值
                epPoint += 1; //获得能量点
            }
            else
                ep = epMax;

            //播放粒子效果 武器材质
            particle.Play(PlayerState.EpPoint);                                                
        }
    }

    void GetCircleDirection() //根据玩家当前方向 获取动态圆形攻击向量组
    {
        //清空重置
        circleDirection.Clear();
        nowDirec = -1;

        //玩家位置 前 左 后 右 四个方向向量
        List<Vector3> fourDirections = new List<Vector3>();
        fourDirections.Add(transform.forward);
        fourDirections.Add(-transform.right);
        fourDirections.Add(-transform.forward);
        fourDirections.Add(transform.right);

        for (int i = 0; i < fourDirections.Count; i++)
        {
            int next = i + 1; //下个向量
            if (next >= fourDirections.Count) //防止越界
                next = 0;

            //两个向量中间的3个向量
            Vector3 vec2 = Vector3.Lerp(fourDirections[i], fourDirections[next], 0.5f);
            vec2.Normalize(); //单位化长度
            Vector3 vec1 = Vector3.Lerp(fourDirections[i], vec2, 0.5f);
            vec1.Normalize();
            Vector3 vec3 = Vector3.Lerp(vec2, fourDirections[next], 0.5f);
            vec3.Normalize();

            //保存进 动态圆形攻击向量组
            circleDirection.Add(fourDirections[i]);
            circleDirection.Add(vec1);
            circleDirection.Add(vec2);
            circleDirection.Add(vec3);
        }
    }

    //状态动作检查 能否进行
    public bool StateActionCheck(PlayerState state)
    {
        if(spUseup) //耐力耗尽时 不能进行的动作
        {
            if (state.Equals(PlayerState.Jump) || state.Equals(PlayerState.Dodge) || state.Equals(PlayerState.Guard) ||
                state.Equals(PlayerState.Combo1) || state.Equals(PlayerState.ThumpCharge))
                return false;
        }

        switch (state)
        {
            case PlayerState.Idle:
                spGain = 50.0f; //耐力值获得量
                if (!death)
                {
                    return true;
                }
                break;
            case PlayerState.Move:
                spGain = 20.0f;
                if (!death)
                {
                    return true;
                }
                break;
            case PlayerState.Jump:
                spCost = 10.0f; //耐力值消耗量
                if (sp > 0)
                {
                    sp -= spCost; //单次消耗
                    return true;
                }
                break;
            case PlayerState.Dodge:
                spCost = 15.0f;
                if (sp > 0)
                {
                    sp -= spCost;
                    return true;
                }
                break;
            case PlayerState.Guard:
                spCost = 5.0f;
                if (sp > 0)
                {
                    //刷新属性值方法中 每秒消耗spCost
                    return true;
                }
                break;
            case PlayerState.GuardDamage:
                if (damage) //如果受伤
                {
                    damage = false; //重置受伤触发

                    epGain = 5.0f; //能量值获得量
                    ep += epGain;
                    if (ep > epMax) //能量值上限
                        ep = epMax;

                    spCost = 5.0f; //耐力值消耗量
                    sp -= spCost;

                    return true;
                }
                break;
            case PlayerState.GuardBreak:
                if (sp <= 10) //耐力低于10
                {
                    sp = 0;
                    return true;
                }    
                break;
            case PlayerState.Combo1:
                spCost = 5.0f;
                if (sp > 0)
                {
                    sp -= spCost;
                    return true;
                }
                break;
            case PlayerState.Combo2:
                spCost = 5.0f;
                if (sp > 0)
                {
                    sp -= spCost;
                    return true;
                }
                break;
            case PlayerState.Combo3:
                spCost = 10.0f;
                if (sp > 0)
                {
                    sp -= spCost;
                    return true;
                }
                break;
            case PlayerState.Combo4:
                spCost = 10.0f;
                if (sp > 0)
                {
                    sp -= spCost;
                    return true;
                }
                break;
            case PlayerState.ThumpCharge:
                spCost = 10.0f;
                if (sp > 0)
                {
                    //刷新属性值方法中 每秒消耗spCost
                    return true;
                }
                break;
            case PlayerState.Thump:
                spCost = 5.0f;
                if (sp >= 0) //蓄力至空体力
                {
                    sp -= spCost;
                    if (sp < 0) //限制耐力值下限
                        sp = 0;
                    return true;
                }
                break;
            case PlayerState.Thump1:
                spCost = 15.0f;
                if (sp > 0)
                {
                    sp -= spCost;
                    return true;
                }
                break;
            case PlayerState.Thump2:
                spCost = 10.0f;
                if (sp > 0)
                {
                    sp -= spCost;
                    return true;
                }
                break;
            case PlayerState.Skill1:
                epPointCost = 1;
                if (epPoint >= epPointCost)
                {
                    epPoint -= epPointCost;

                    GetCircleDirection(); //根据玩家位置 获取新的 圆形方向向量组

                    if (epPoint <= 0)
                    {
                        //停止粒子效果 武器材质
                        particle.Stop(PlayerState.EpPoint);
                    }

                    return true;
                }
                break;
            case PlayerState.Damage:
                if (damage) //如果受伤
                {
                    damage = false; //重置受伤触发
                    spGain = 5.0f; //耐力值获得量
                    epGain = 3.0f; //能量值获得量

                    ep += epGain;
                    if (ep > epMax) //能量值上限
                        ep = epMax;

                    return true;
                }
                break;
            case PlayerState.Death:
                if (death) //如果死亡
                {
                    //清空属性值
                    hp = 0;
                    sp = 0;
                    ep = 0;
                    epPoint = 0;
                    return true;
                }
                break;
        }

        return false; //不能进行动作
    }

    #region 动画帧事件
    public void Combo1Check()
    {
        //设定攻击范围
        checkPoint = transform.position; //范围球心
        atkRadius = 3.0f; //范围半径
        atkAngle = 160; //范围内 有效角度
        realAtk = atk * 1.0f; //真实伤害
        atkDirection = Vector3.Lerp(transform.forward, transform.right, 0.3f); //范围内 攻击方向
        epGain = 2.0f; //能量值获得量

        impactHori = 6.0f; //水平冲击力
        impactVeti = 6.0f; //垂直冲击力

        //球形检测
        SphereForeach();
    }

    public void Combo2Check()
    {
        checkPoint = transform.position;
        atkRadius = 3.0f;
        atkAngle = 360;
        realAtk = atk * 1.5f;
        atkDirection = transform.forward;
        epGain = 2.0f;
        impactHori = 15.0f;
        impactVeti = 3.0f;
        SphereForeach();
    }

    public void Combo3Check()
    {
        checkPoint = transform.position;
        atkRadius = 3.0f;
        atkAngle = 180;
        realAtk = atk * 2.0f;
        atkDirection = Vector3.Lerp(transform.forward, transform.right, 0.3f);
        epGain = 3.0f;
        impactHori = 5.0f;
        impactVeti = 2.0f;
        SphereForeach();
    }

    public void Combo4Check()
    {
        checkPoint = transform.position;
        atkRadius = 3.0f;
        atkAngle = 180;
        realAtk = atk * 3.0f;
        atkDirection = Vector3.Lerp(transform.forward, -transform.right, 0.3f);
        epGain = 4.0f;
        impactHori = 15.0f;
        impactVeti = 6.0f;
        SphereForeach();
    }

    public void ThumpCheck0()
    {
        checkPoint = transform.position + transform.forward * 2.8f;
        atkRadius = 2.5f;
        atkAngle = 360;
        realAtk = atk * 2.0f;
        atkDirection = transform.forward;
        epGain = 4.0f;
        impactHori = 3.0f;
        impactVeti = 8.0f;
        SphereForeach();
    }

    public void ThumpCheck1()
    {
        checkPoint = transform.position + transform.forward * 2.8f;
        atkRadius = 2.5f;
        atkAngle = 360;
        realAtk = atk * 8.0f;
        atkDirection = transform.forward;
        epGain = 8.0f;
        impactHori = 5.0f;
        impactVeti = 10.0f;
        SphereForeach();
    }

    public void Thump1Check0()
    {
        checkPoint = transform.position;
        atkRadius = 3.0f;
        atkAngle = 180;
        realAtk = atk * 2.0f;
        atkDirection = Vector3.Lerp(transform.forward, -transform.right, 0.3f);
        epGain = 2.0f;
        impactHori = 2.0f;
        impactVeti = 6.0f;
        SphereForeach();
    }

    public void Thump1Check1()
    {
        checkPoint = transform.position;
        atkRadius = 2.5f;
        atkAngle = 180;
        realAtk = atk * 1.0f;
        atkDirection = transform.forward;
        epGain = 2.0f;
        impactHori = 0.5f;
        impactVeti = 4.0f;
        SphereForeach();
    }

    public void Thump2Check()
    {
        checkPoint = transform.position;
        atkRadius = 1.5f;
        atkAngle = 150;
        realAtk = atk * 3.0f;
        atkDirection = Vector3.Lerp(transform.forward, -transform.right, 0.3f);
        epGain = 3.0f;
        impactHori = 20.0f;
        impactVeti = 2.0f;
        SphereForeach();
    }

    public void Skill1Check0()
    {
        nowDirec += 1;
        if (nowDirec > circleDirection.Count - 1) //防止下标越界
            nowDirec = 0;

        checkPoint = transform.position;
        atkRadius = 4.5f;
        atkAngle = 30;
        realAtk = atk * 2.0f;
        atkDirection = circleDirection[nowDirec];
        epGain = 0f;
        impactHori = 3.0f;
        impactVeti = 2.0f;
        SphereForeach();
    }

    public void Skill1Check1()
    {
        checkPoint = transform.position;
        atkRadius = 5.0f;
        atkAngle = 360;
        realAtk = atk * 10.0f;
        atkDirection = transform.forward;
        epGain = 0f;
        impactHori = 15.0f;
        impactVeti = 15.0f;
        SphereForeach();
    }

    void SphereForeach()
    {
        //球形范围检测 获得collider数组
        Collider[] enemys = Physics.OverlapSphere(checkPoint, atkRadius, 1 << LayerMask.NameToLayer("Enemy"));
        //遍历 判断是否在 有效攻击范围内
        foreach (var enemy in enemys)
        {
            Vector3 vec = enemy.transform.position - checkPoint; //计算player到enemy的向量
            vec.y = 0; //忽略高度差
            float angle = Vector3.Angle(atkDirection, vec); //计算 player前方向量 与 enemy方向向量 的 夹角

            //如果夹角 小于 有效攻击角度
            if (angle < atkAngle / 2)
            {
                //有效攻击获得能量值
                ep += epGain;
                if (ep > epMax) //能量值上限
                    ep = epMax;

                //计算最终冲击力向量
                AtkImpact = enemy.transform.position - checkPoint; //计算冲击力方向
                AtkImpact.Normalize(); //单位化向量大小
                AtkImpact *= impactHori; //计算水平冲击力
                AtkImpact.y = impactVeti; //计算垂直冲击力

                //判断目标敌人
                switch (enemy.tag)
                {
                    case "OrcKing":
                        enemy.GetComponent<OrcKiCharacter>().Damage(realAtk, AtkImpact); //承受伤害与冲击力
                        break;
                    default:
                        enemy.GetComponent<EnemyCharacterBase>().Damage(realAtk, AtkImpact); //承受伤害与冲击力
                        break;
                }
                
            }
        }
    }
    #endregion
}
