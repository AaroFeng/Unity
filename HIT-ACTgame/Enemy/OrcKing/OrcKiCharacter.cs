using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrcKiCharacter : MonoBehaviour
{
    public Transform[] pathPoints; //巡逻路径点
    public Transform[] pathPointsIdle; //空闲状态路径点

    //敌人属性值
    string enemyName; //名字
    float height; //身高
    float hp; //体力值
    public float Hp { get { return hp; } set { hp = value; } }
    float hpMax; //最大体力值
    float atk; //攻击力
    int stage; //战斗阶段
    public int Stage { get { return stage; } }

    bool damage = false; //受伤触发
    public bool damage_ { get { return damage; } set { damage = value; } }
    bool death = false; //死亡触发
    public bool Death { get { return death; } set { death = value; } }

    //UI组件
    GameObject enemyCanvas; //敌人画布
    GameObject nameText; //名字显示文本
    GameObject hpSlider; //体力条

    //动态属性值 实现动态增减动画
    float hpMotion;

    //攻击 帧事件 范围检测
    Vector3 checkPoint; //球形攻击范围 球心 
    float atkRadius; //球形攻击范围 半径
    float atkAngle; //球形攻击范围 有效角度
    float realAtk; //真实伤害
    Vector3 atkDirection; //攻击方向
    //攻击动作冲击力
    float impactHori; //水平冲击力
    float impactVeti; //垂直冲击力
     Vector3 AtkImpact = new Vector3(); //最终冲击力向量
    
    //状态属性值
    float idleCDMin; //最小空闲时间
    public float IdleCDMin { get { return idleCDMin; } }
    float idleCDMax; //最大空闲时间
    public float IdleCDMax { get { return idleCDMax; } }

    float patrolSpeed; //巡逻状态 移动速度
    public float PatrolSpeed { get { return patrolSpeed; } }
    float chaseSpeed; //追逐状态 移动速度
    public float ChaseSpeed { get { return chaseSpeed; } }

    float patrolRadius; //巡逻 视野半径
    public float PatrolRadius { get { return patrolRadius; } }
    float patrolBackRadius; //巡逻 背面视野半径
    public float PatrolBackRadius { get { return patrolBackRadius; } }
    float patrolAngle; //巡逻 正面视野角度
    public float PatrolAngle { get { return patrolAngle; } }

    float chaseRadius; //追逐 视野半径
    public float ChaseRadius { get { return chaseRadius; } }
    float chaseBackRadius; //追逐 背面视野半径
    public float ChaseBackRadius { get { return chaseBackRadius; } }
    float chaseAngle; //追逐 正面视野角度
    public float ChaseAngle { get { return chaseAngle; } }

    float attackRadius; //普通攻击半径
    public float AtkRadius { get { return attackRadius; } }
    float atkCD; //普通攻击所需时间
    public float AtkCD { get { return atkCD; } }
    float atkTime; //当前累计时间
    public float AtkTime { get { return atkTime; } set { atkTime = value; } }

    float repelRadius; //击退技能 半径
    public float RepelRadius { get { return repelRadius; } }
    float repelCD; //击退技能 所需时间
    public float RepelCD { get { return repelCD; } }
    float repelTime; //击退技能 当前累计时间
    public float RepelTime { get { return repelTime; } set { repelTime = value; } }

    float flameJetRadius; //火焰喷射技能 半径
    public float FlameJetRadius { get { return flameJetRadius; } }
    float flameJetCD; //火焰喷射技能 所需时间
    public float FlameJetCD { get { return flameJetCD; } }
    float flameJetTime; //火焰喷射当前 累计时间
    public float FlameJetTime { get { return flameJetTime; } set { flameJetTime = value; } }

    float bulletShootRadius; //发射火焰弹技能 半径
    public float BulletShootRadius { get { return bulletShootRadius; } }
    float bulletShootCD; //发射火焰弹技能 所需时间
    public float BulletShootCD { get { return bulletShootCD; } }
    float bulletShootTime; //发射火焰弹技能 累计时间
    public float BulletShootTime { get { return bulletShootTime; } set { bulletShootTime = value; } }
    GameObject bullet; //火焰弹 引用
    float bulletSpeed; //火焰弹 速度

    float trackBulletShootRadius; //发射追踪弹技能 半径
    public float TrackBulletShootRadius { get { return trackBulletShootRadius; } }
    float trackBulletShootCD; //发射追踪弹技能 所需时间
    public float TrackBulletShootCD { get { return trackBulletShootCD; } }
    float trackBulletShootTime; //发射追踪弹技能 累计时间
    public float TrackBulletShootTime { get { return trackBulletShootTime; } set { trackBulletShootTime = value; } }
    List<GameObject> trackBullets = new List<GameObject>(); //追踪弹组 引用
    float trackBulletSpeed; //追踪弹 速度
    Vector3 trackTarget; //追踪目标点
    float trackTime; //追踪弹 追踪时长

    float rangeBulletShootRadius; //发射范围弹技能 半径
    public float RangeBulletShootRadius { get { return rangeBulletShootRadius; } }
    float rangeBulletShootCD; //发射范围弹技能 所需时间
    public float RangeBulletShootCD { get { return rangeBulletShootCD; } }
    float rangeBulletShootTime; //发射范围弹技能 累计时间
    public float RangeBulletShootTime { get { return rangeBulletShootTime; } set { rangeBulletShootTime = value; } }
    List<GameObject> rangeBulletsUp = new List<GameObject>(); //范围弹组 引用 上升时
    List<GameObject> rangeBulletsDown = new List<GameObject>(); //范围弹组 引用 下落时
    float rangeBulletSpeed; //范围弹 速度
    Vector3 rangeTarget; //范围目标点

    //受伤承受冲击力向量
    protected Vector3 damageImpact = new Vector3();
    public Vector3 DamageImpact
    {
        get { return damageImpact; }
    }

    void Awake()
    {
        //设定属性值
        enemyName = "兽人首领-咔隆"; //名字
        height = 3.8f; //身高
        hpMax = 1000; //最大体力值
        atk = 80; //攻击力

        //设定状态属性值
        idleCDMin = 15.0f; //最小空闲时间
        idleCDMax = 25.0f; //最大空闲时间
        patrolSpeed = 1.5f; //巡逻移动速度
        chaseSpeed = 3.0f; //追逐移动速度

        //设定视野检测范围
        patrolRadius = 15.0f; //巡逻视野
        patrolBackRadius = 5.0f; //巡逻 背面视野半径
        patrolAngle = 140; //巡逻 正面视野角度
        chaseRadius = 35.0f; //追逐视野
        chaseBackRadius = 25.0f; //追逐 背面视野半径
        chaseAngle = 200; //追逐 正面视野角度

        //设定技能检测范围与属性值
        attackRadius = 5.0f; //普通攻击 范围
        atkCD = 5.0f; //普通攻击 CD

        repelRadius = 3.0f; //击退技能 范围
        repelCD = 10.0f; //击退技能 CD

        flameJetRadius = 5.0f; //火焰喷射技能 范围
        flameJetCD = 20.0f; //火焰喷射技能 CD

        bulletShootRadius = chaseRadius - 3.0f; //发射火焰弹技能 范围
        bulletShootCD = 25.0f; //发射火焰弹技能 CD
        bulletSpeed = 50.0f; //火焰弹 速度

        trackBulletShootRadius = chaseRadius - 3.0f; //发射追踪弹技能 范围
        trackBulletShootCD = 35.0f; //发射追踪弹技能 CD
        trackBulletSpeed = 6.5f; //追踪弹 速度

        rangeBulletShootRadius = chaseRadius - 3.0f; //发射范围弹技能 范围
        rangeBulletShootCD = 45.0f; //发射范围弹技能 CD
        rangeBulletSpeed = 30.0f; //范围弹 速度
    }

    void Start()
    {
        //初始化属性值
        hp = hpMax;
        //初始化动态属性值
        hpMotion = hp;

        //初始化UI
        if (enemyCanvas == null)
        {
            if (GameObject.Find("OrcKingCanvas"))
                enemyCanvas = GameObject.Find("OrcKingCanvas");
            else
                enemyCanvas = SysModuleManager.Instance.GetSysModule<SysPool>().CreateObj("OrcKingCanvas");
        }
        //生成体力条
        if (hpSlider == null)
        {
            hpSlider = enemyCanvas.transform.Find("BossHP").gameObject;
            hpSlider.SetActive(false);
        }
        //生成名字文本
        if (nameText == null)
        {
            nameText = enemyCanvas.transform.Find("BossName").gameObject;
            nameText.GetComponent<Text>().text = enemyName; //设置文本内容
            nameText.SetActive(false);
        }
    }

    public virtual void Update()
    {
        //UI刷新
        RefreshUI();

        //火焰弹 碰撞检测
        if (bullet != null)
            BulletCheck();

        //追踪弹 碰撞检测
        if (trackBullets.Count > 0)
            TrackBulletCheck();

        //范围弹 碰撞检测
        if (rangeBulletsDown.Count > 0)
            RangeBulletCheck();
    }

    public void Damage(float playerAtk, Vector3 AtkImpact)
    {
        damage = true; //触发受伤

        if (!death)
            damageImpact = AtkImpact; //记录受到的冲击力
        else
        {
            damageImpact = Vector3.zero; //不允许击飞死亡目标
            return;
        }

        hp -= playerAtk; //减少hp
        if (hp <= 0) //是否死亡
        {
            hp = 0;
            death = true; //触发死亡
            Invoke("LateDeath", 3.0f);
        }

        StageSet(); //战斗阶段判断
    }

    protected void LateDeath()
    {
        GetComponent<CharacterController>().enabled = false; //关闭角色控制器
        GetComponent<OrcSaCharacter>().enabled = false; //关闭角色特性脚本
    }

    void StageSet() //战斗阶段判定
    {
        float ratio = hp / hpMax;
        if (ratio < 0.4f)
            stage = 2;
        else if (ratio < 0.7f)
            stage = 1;
        else
            stage = 0;
    }

    protected void RefreshUI()
    {
        if (Camera.main == null) //找不到主相机
            return;

        //计算摄像机到体力条向量
        Vector3 Pos = transform.position;
        Pos.y += height * 0.5f;
        Vector3 vec = Pos - GameObject.FindGameObjectWithTag("Player").transform.position;
        //是否在有效角度 和 距离
        if (vec.magnitude < patrolRadius + 5.0f)
        {
            if (hpSlider != null)
                hpSlider.SetActive(true);
            if (nameText != null)
                nameText.SetActive(true);
        }
        else if(vec.magnitude > chaseRadius)
        {
            if (hpSlider != null)
                hpSlider.SetActive(false);
            if (nameText != null)
                nameText.SetActive(false);

            return;
        }

        if (hpSlider == null)
            return;

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
    public void Attack1Check() //普通攻击
    {
        //设定攻击范围
        checkPoint = transform.position; //范围球心
        atkRadius = 5.5f; //范围半径
        atkAngle = 50; //范围内 有效角度
        realAtk = atk * 1.0f; //真实伤害
        atkDirection = transform.forward; //攻击范围 方向

        impactHori = 25.0f; //水平冲击力
        impactVeti = 0.0f; //垂直冲击力

        SphereForeach();
    }

    public void RepelCheck() //击退技能
    {
        checkPoint = transform.position;
        atkRadius = 6.0f;
        atkAngle = 360;
        realAtk = atk * 3.0f;
        atkDirection = transform.forward;

        impactHori = 60.0f;
        impactVeti = 5.0f;

        SphereForeach();
    }

    public void FlameJetCheck() //火焰喷射技能
    {
        checkPoint = transform.position;
        atkRadius = 8.0f;
        atkAngle = 60;
        realAtk = atk * 0.2f;
        atkDirection = transform.forward;

        impactHori = 2.0f;
        impactVeti = 0.0f;

        SphereForeach();
    }

    public void BulletShoot() //发射火焰弹
    {
        checkPoint = transform.position;
        atkRadius = 5.0f;
        atkAngle = 100;
        realAtk = atk * 1.0f;
        atkDirection = transform.forward;

        impactHori = 40.0f;
        impactVeti = 0.0f;

        SphereForeach();

        //实例化火焰弹
        bullet = SysModuleManager.Instance.GetSysModule<SysPool>().CreateObj("Bullet");
        bullet.transform.position = GetComponent<OrcKiParticle>().particleList[4].transform.position; //设置位置为 发射口
        Vector3 shootVec = GetComponent<OrcKiParticle>().particleList[4].transform.up;shootVec.y -= 0.1f; //发射方向 
        bullet.GetComponent<Rigidbody>().velocity = shootVec * bulletSpeed; //设置初始速度
    }
    void BulletCheck() //火焰弹 范围检测
    {
        checkPoint = bullet.transform.position;
        atkRadius = 0.8f;
        atkAngle = 360;
        realAtk = atk * 2.0f;
        atkDirection = transform.forward;

        impactHori = 20.0f;
        impactVeti = 0.0f;

        if (SphereForeach()) //碰撞检测
        {
            //播放碰撞爆炸特效
            ParticleSystem particle = GetComponent<OrcKiParticle>().particleList[5].GetComponent<ParticleSystem>();
            particle.transform.position = bullet.transform.position;
            particle.Play();

            SysModuleManager.Instance.GetSysModule<SysPool>().RemoveObj(bullet);
            bullet = null; //取消引用
        }
    }

    public void TrackBulletShoot() //发射追踪弹
    {
        checkPoint = transform.position;
        atkRadius = 5.0f;
        atkAngle = 100;
        realAtk = atk * 1.0f;
        atkDirection = transform.forward;

        impactHori = 40.0f;
        impactVeti = 0.0f;

        SphereForeach();

        trackTime = 0; //重置追踪弹时长

        //实例化追踪弹
        for (int i = 0; i < 15; i++)
        {
            GameObject trackBullet = SysModuleManager.Instance.GetSysModule<SysPool>().CreateObj("TrackBullet");
            Vector3 position = GetComponent<OrcKiParticle>().particleList[4].transform.position; //发射区域中心点
            position += transform.forward * 3.0f; //向前方移动1
            trackBullet.transform.position = position + Random.insideUnitSphere * 3.0f; //设置位置为 发射口

            trackBullets.Add(trackBullet); //添加进追踪弹组
        }  
    }
    void TrackBulletCheck() //追踪弹 范围检测
    {
        atkRadius = 0.3f;
        atkAngle = 360;
        realAtk = atk * 0.2f;
        atkDirection = transform.forward;

        impactHori = 10.0f;
        impactVeti = 0.0f;

        //更新追踪目标点
        trackTarget = GameObject.FindGameObjectWithTag("Player").transform.position;
        trackTarget.y += 1.3f;
        //追踪弹存在时长
        trackTime += Time.deltaTime;
        if (trackTime > 5.0f)
        {
            trackTime -= 0.1f;
            //遍历追踪弹组 全部爆炸
            ParticleSystem particle = GetComponent<OrcKiParticle>().particleList[6].GetComponent<ParticleSystem>();
            particle.transform.position = trackBullets[0].transform.position;
            particle.Play();

            SysModuleManager.Instance.GetSysModule<SysPool>().RemoveObj(trackBullets[0]);
            trackBullets.Remove(trackBullets[0]);
        }

        //遍历检测每个追踪弹的碰撞
        for (int i = 0; i < trackBullets.Count; i++)
        {
            checkPoint = trackBullets[i].transform.position; //更改检测范围球心位置
            //更新追踪弹速度方向
            Vector3 vec = trackTarget - trackBullets[i].transform.position; //指向目标点向量
            trackBullets[i].GetComponent<Rigidbody>().velocity = vec.normalized * trackBulletSpeed;

            if (SphereForeach())
            {
                ParticleSystem particle = GetComponent<OrcKiParticle>().particleList[6].GetComponent<ParticleSystem>();
                particle.transform.position = trackBullets[i].transform.position;
                particle.Play();

                SysModuleManager.Instance.GetSysModule<SysPool>().RemoveObj(trackBullets[i]);
                trackBullets.Remove(trackBullets[i]);
            }
        }
    }

    public void RangeBulletShoot() //发射范围弹
    {
        checkPoint = transform.position;
        atkRadius = 5.0f;
        atkAngle = 60;
        realAtk = atk * 1.0f;
        atkDirection = Vector3.Lerp(transform.forward, transform.up, 0.5f);

        impactHori = 40.0f;
        impactVeti = 0.0f;

        SphereForeach();

        //缓存池回收粒子效果
        if (rangeBulletsUp.Count == 0)
        {
            SysModuleManager.Instance.GetSysModule<SysPool>().RemoveAllObj("RangeBulletTarget");
            SysModuleManager.Instance.GetSysModule<SysPool>().RemoveAllObj("RangeBulletHit");
        }

        //实例化范围弹
        GameObject rangeBullet = SysModuleManager.Instance.GetSysModule<SysPool>().CreateObj("RangeBullet");
        rangeBullet.transform.position = GetComponent<OrcKiParticle>().particleList[4].transform.position;
        Vector3 shootVec = GetComponent<OrcKiParticle>().particleList[4].transform.up;
        rangeBullet.GetComponent<Rigidbody>().velocity = shootVec * rangeBulletSpeed;

        rangeBulletsUp.Add(rangeBullet);

        Invoke("RangeBulletTarget", 1.5f); //延迟 目标瞄准与弹道改变
    }
    void RangeBulletTarget() //范围弹 目标瞄准与弹道改变
    {
        //获取玩家位置
        rangeTarget = GameObject.FindGameObjectWithTag("Player").transform.position;
        rangeTarget.y += 1.0f;
        //获取玩家所在位置地面position
        RaycastHit hit;
        if (Physics.Raycast(rangeTarget, Vector3.down, out hit, 1 << LayerMask.NameToLayer("Standard"))) //碰撞到地面
            rangeTarget = hit.point; //玩家位置下方地面
        else
            rangeTarget.y -= 1.0f; //玩家脚底

        //播放目标点粒子特效
        ParticleSystem particle = SysModuleManager.Instance.GetSysModule<SysPool>().CreateObj("RangeBulletTarget").GetComponent<ParticleSystem>();
        particle.transform.position = rangeTarget;
        particle.Play();

        //改变范围弹位置与速度
        rangeTarget.y += 30.0f; //位于目标点上方
        rangeBulletsUp[0].transform.position = rangeTarget;
        rangeBulletsUp[0].GetComponent<Rigidbody>().velocity = Vector3.down * rangeBulletSpeed; //速度向下

        //从上升组移入下落组
        rangeBulletsDown.Add(rangeBulletsUp[0]);
        rangeBulletsUp.RemoveAt(0);
    }
    void RangeBulletCheck() //范围弹 范围检测
    {
        atkRadius = 0.3f;
        atkAngle = 360;
        realAtk = atk * 1.0f;
        atkDirection = transform.forward;

        impactHori = 10.0f;
        impactVeti = 0.0f;

        for (int i = 0; i < rangeBulletsDown.Count; i++)
        {
            checkPoint = rangeBulletsDown[i].transform.position;

            if (SphereForeach())
            {
                ParticleSystem particle = SysModuleManager.Instance.GetSysModule<SysPool>().CreateObj("RangeBulletHit").GetComponent<ParticleSystem>();
                particle.transform.position = rangeBulletsDown[i].transform.position;
                particle.Play();

                SysModuleManager.Instance.GetSysModule<SysPool>().RemoveObj(rangeBulletsDown[i]);
                rangeBulletsDown.Remove(rangeBulletsDown[i]);

                //爆炸范围伤害检测
                atkRadius = 6.0f;
                atkAngle = 360;
                realAtk = atk * 1.0f;
                atkDirection = transform.forward;

                impactHori = 25.0f;
                impactVeti = 15.0f;

                SphereForeach();
            }
        }
    }
}
