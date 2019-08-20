using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCharacterBase : MonoBehaviour
{
    public Transform[] pathPoints; //巡逻路径点
    public Transform[] pathPointsIdle; //空闲状态路径点

    //敌人属性值
    protected string enemyName; //名字
    protected float height; //身高
    protected float hp; //体力值
    public float Hp { get { return hp; } set { hp = value; } }
    protected float hpMax; //最大体力值
    protected float atk; //攻击力

    protected bool damage = false; //受伤触发
    public bool damage_ { get { return damage; } set { damage = value; } }
    protected bool death = false; //死亡触发
    public bool Death { get { return death; } set { death = value; } }

    //UI组件
    GameObject enemyCanvas; //敌人画布
    protected GameObject nameText; //名字显示文本
    protected GameObject hpSlider; //体力条

    //动态属性值 实现动态增减动画
    protected float hpMotion;

    //攻击 帧事件 范围检测
    protected Vector3 checkPoint; //球形攻击范围 球心 
    protected float atkRadius; //球形攻击范围 半径
    protected float atkAngle; //球形攻击范围 有效角度
    protected float realAtk; //真实伤害
    protected Vector3 atkDirection; //攻击方向
    //攻击动作冲击力
    protected float impactHori; //水平冲击力
    protected float impactVeti; //垂直冲击力
    protected Vector3 AtkImpact = new Vector3(); //最终冲击力向量

    //状态属性值
    protected float idleCDMin; //最小空闲时间
    public float IdleCDMin { get { return idleCDMin; } }
    protected float idleCDMax; //最大空闲时间
    public float IdleCDMax { get { return idleCDMax; } }

    protected float patrolSpeed; //巡逻状态 移动速度
    public float PatrolSpeed { get { return patrolSpeed; } }
    protected float chaseSpeed; //追逐状态 移动速度
    public float ChaseSpeed { get { return chaseSpeed; } }

    protected float patrolRadius; //巡逻 视野半径
    public float PatrolRadius { get { return patrolRadius; } }
    protected float patrolBackRadius; //巡逻 背面视野半径
    public float PatrolBackRadius { get { return patrolBackRadius; } }
    protected float patrolAngle; //巡逻 正面视野角度
    public float PatrolAngle { get { return patrolAngle; } }

    protected float chaseRadius; //追逐 视野半径
    public float ChaseRadius { get { return chaseRadius; } }
    protected float chaseBackRadius; //追逐 背面视野半径
    public float ChaseBackRadius { get { return chaseBackRadius; } }
    protected float chaseAngle; //追逐 正面视野角度
    public float ChaseAngle { get { return chaseAngle; } }

    protected float attackRadius; //攻击半径
    public float AtkRadius { get { return attackRadius; } } 
    protected float atkCD; //攻击所需时间
    public float AtkCD { get { return atkCD; } }
    float atkTime; //当前累计时间
    public float AtkTime { get { return atkTime; } set { atkTime = value; } }

    protected float chaseBlendSpeed = 0.2f; //追逐状态Blend变化速度
    public float ChaseBlendSpeed { get { return chaseBlendSpeed; } }

    //受伤承受冲击力向量
    protected Vector3 damageImpact = new Vector3();
    public Vector3 DamageImpact
    {
        get { return damageImpact; }
    }

    void Start()
    {
        //初始化属性值
        hp = hpMax;
        //初始化动态属性值
        hpMotion = hp;

        //敌人UI组件画布
        if (GameObject.Find("EnemyCanvas"))
            enemyCanvas = GameObject.Find("EnemyCanvas");
        else
            enemyCanvas = Instantiate(Resources.Load("Prefab/UI/EnemyCanvas")) as GameObject;
        //初始化UI组件
        hpSlider = Instantiate(Resources.Load("Prefab/UI/EnemyHP")) as GameObject;
        hpSlider.transform.SetParent(enemyCanvas.transform);
        nameText = Instantiate(Resources.Load("Prefab/UI/EnemyName")) as GameObject;
        nameText.transform.SetParent(enemyCanvas.transform);
        nameText.GetComponent<Text>().text = enemyName; //设置文本内容
    }

    public virtual void Update()
    {
        //UI刷新
        RefreshUI();
    }

    public void Damage(float playerAtk, Vector3 AtkImpact)
    {
        damage = true; //触发受伤

        if(!death)
        damageImpact = AtkImpact; //记录受到的冲击力
        else
        {
            damageImpact = Vector3.zero; //不允许击飞死亡目标
            return;
        }

        //减少hp
        hp -= playerAtk;
        if (hp <= 0) //是否死亡
        {
            hp = 0;
            death = true; //触发死亡
        }
    }

    public void LateDeath()
    {
        //销毁 体力条 名字
        Destroy(hpSlider);
        Destroy(nameText);

        GetComponent<CharacterController>().enabled = false; //关闭角色控制器
        GetComponent<EnemyCharacterBase>().enabled = false; //关闭角色特性脚本
    }

    protected void RefreshUI()
    {
        if (Camera.main == null) //找不到主相机
            return;
        if (hpSlider == null || nameText == null)
            return;

        //计算摄像机到体力条向量
        Vector3 hpSliderPos = transform.position;
        hpSliderPos.y += height + 0.15f; //计算体力条位置
        Vector3 vec = hpSliderPos - Camera.main.transform.position;
        //是否在有效角度 和 距离
        if (Vector3.Angle(vec, Camera.main.transform.forward) < 90 && vec.magnitude > 2.5f && vec.magnitude < 15.0f)
        {
            hpSlider.SetActive(true);
            nameText.SetActive(true);
        }
        else
        {
            hpSlider.SetActive(false);
            nameText.SetActive(false);
        }

        if (!hpSlider.activeSelf || !nameText.activeSelf) 
            return;

        //使血条始终处于Enemy头顶
        hpSlider.transform.position = Camera.main.WorldToScreenPoint(hpSliderPos);
        //根据距离缩放体力条大小
        float newSize = 10.0f / Vector3.Distance(hpSliderPos, Camera.main.transform.position);
        hpSlider.transform.localScale = Vector3.one * newSize;
        //设置名字文本的位置于大小
        hpSliderPos.y += 0.2f;
        nameText.transform.position = Camera.main.WorldToScreenPoint(hpSliderPos);
        nameText.transform.localScale = Vector3.one * newSize;

        //动态属性值
        if (Mathf.Abs(hp - hpMotion) > 1.0f)
        {
            hpMotion += (hp - hpMotion) * 0.1f;
        }
        else
            hpMotion = hp;
        //刷新血条显示
        hpSlider.GetComponent<Slider>().value = hpMotion / hpMax;
    }

    protected bool SphereForeach()
    {
        //球形范围检测 获得collider数组
        Collider[] players = Physics.OverlapSphere(checkPoint, atkRadius);
        //遍历 判断是否在 有效攻击范围内
        foreach (var player in players)
        {
            if (player.CompareTag("Player"))
            {
                Vector3 vec = player.transform.position - checkPoint; //计算player到enemy的向量
                float angle = Vector3.Angle(atkDirection, vec); //计算 player前方向量 与 enemy方向向量 的 夹角
                if (angle < atkAngle / 2) //如果夹角 小于 有效攻击角度
                {
                    //计算最终冲击力向量
                    AtkImpact = player.transform.position - checkPoint; //计算冲击力方向
                    AtkImpact.Normalize(); //单位化向量大小
                    AtkImpact *= impactHori; //计算水平冲击力
                    AtkImpact.y = impactVeti; //计算垂直冲击力

                    player.GetComponent<PlayerCharacter>().Damage(realAtk, AtkImpact); //enemy承受伤害

                    return true; //碰撞到玩家
                }

            }
        }
        
        if (players.Length > 0) //碰撞到其他物体
            return true;

        return false; //没有碰撞到物体
    }

    //动画帧事件
    public virtual void Attack1Check()
    {
        //设定攻击范围
        //checkPoint = transform.position; //范围球心
        //atkRadius = 3.0f; //范围半径
        //atkAngle = 180; //范围内 有效角度
        //realAtk = atk * 1.0f; //真实伤害
        //atkDirection = Vector3.Lerp(transform.forward, -transform.right, 0.3f); //攻击范围 方向

        //impactHori = 10.0f; //水平冲击力
        //impactVeti = 2.0f; //垂直冲击力

        //SphereForeach();
    }
}
