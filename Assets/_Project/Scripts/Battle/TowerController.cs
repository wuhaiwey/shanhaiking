using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Battle
{
    /// <summary>
    /// 防御塔控制器
    /// </summary>
    public class TowerController : MonoBehaviour
    {
        [Header("阵营")]
        public MinionAI.Team team = MinionAI.Team.Blue;
        
        [Header("属性")]
        public float maxHealth = 3000f;
        public float currentHealth;
        public float attackDamage = 150f;
        public float attackRange = 8f;
        public float attackSpeed = 1.5f;
        
        [Header("攻击设置")]
        public float detectionRange = 10f;
        public Transform target;
        public Transform projectileSpawnPoint;
        public GameObject projectilePrefab;
        
        [Header("组件")]
        public Transform turretHead; // 炮塔头部（可旋转）
        public Animator animator;
        
        private float lastAttackTime;
        private List<Transform> enemiesInRange = new List<Transform>();
        
        void Start()
        {
            currentHealth = maxHealth;
            
            // 设置阵营颜色
            SetTeamColor();
        }
        
        void Update()
        {
            if (currentHealth <= 0) return;
            
            // 寻找目标
            FindTarget();
            
            // 攻击逻辑
            if (target != null)
            {
                AimAtTarget();
                
                if (Time.time >= lastAttackTime + 1f / attackSpeed)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
            }
        }
        
        void SetTeamColor()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(renderer.material);
                mat.color = team == MinionAI.Team.Blue ? new Color(0.2f, 0.4f, 1f) : new Color(1f, 0.3f, 0.2f);
                renderer.material = mat;
            }
        }
        
        void FindTarget()
        {
            // 清理无效目标
            enemiesInRange.RemoveAll(t => t == null);
            
            // 寻找新的敌人
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
            
            foreach (Collider col in colliders)
            {
                // 检测敌方小兵
                MinionAI minion = col.GetComponent<MinionAI>();
                if (minion != null && minion.team != team && minion.currentHealth > 0)
                {
                    if (!enemiesInRange.Contains(minion.transform))
                    {
                        enemiesInRange.Add(minion.transform);
                    }
                }
                
                // 检测敌方英雄
                Hero.HeroBase hero = col.GetComponent<Hero.HeroBase>();
                if (hero != null && !hero.isDead)
                {
                    if (!enemiesInRange.Contains(hero.transform))
                    {
                        enemiesInRange.Add(hero.transform);
                    }
                }
            }
            
            // 选择优先级最高的目标
            target = SelectPriorityTarget();
        }
        
        Transform SelectPriorityTarget()
        {
            Transform bestTarget = null;
            float closestDistance = float.MaxValue;
            
            foreach (Transform enemy in enemiesInRange)
            {
                if (enemy == null) continue;
                
                float distance = Vector3.Distance(transform.position, enemy.position);
                
                // 优先攻击英雄
                Hero.HeroBase hero = enemy.GetComponent<Hero.HeroBase>();
                if (hero != null)
                {
                    return enemy; // 发现英雄立即攻击
                }
                
                // 其次选择最近的小兵
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = enemy;
                }
            }
            
            return bestTarget;
        }
        
        void AimAtTarget()
        {
            if (target == null || turretHead == null) return;
            
            // 计算方向
            Vector3 direction = target.position - turretHead.position;
            direction.y = 0; // 只在水平面旋转
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                turretHead.rotation = Quaternion.Slerp(turretHead.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
        
        void Attack()
        {
            if (target == null) return;
            
            // 播放攻击动画
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // 发射 projectile
            if (projectilePrefab != null && projectileSpawnPoint != null)
            {
                FireProjectile();
            }
            else
            {
                // 直接造成伤害（如果没有 projectile）
                DirectDamage();
            }
        }
        
        void FireProjectile()
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            TowerProjectile proj = projectile.GetComponent<TowerProjectile>();
            
            if (proj != null)
            {
                proj.Initialize(target, attackDamage, team);
            }
        }
        
        void DirectDamage()
        {
            // 直接对目标造成伤害
            MinionAI minion = target.GetComponent<MinionAI>();
            if (minion != null)
            {
                minion.TakeDamage(attackDamage);
            }
            
            Hero.HeroBase hero = target.GetComponent<Hero.HeroBase>();
            if (hero != null)
            {
                hero.TakeDamage(attackDamage, Core.DamageType.Physical, null);
            }
        }
        
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            
            // 受伤特效
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
            
            if (currentHealth <= 0)
            {
                DestroyTower();
            }
        }
        
        void DestroyTower()
        {
            // 播放摧毁动画
            if (animator != null)
            {
                animator.SetTrigger("Destroy");
            }
            
            // TODO: 通知游戏管理器防御塔被摧毁
            
            // 延迟销毁
            Destroy(gameObject, 2f);
        }
        
        void OnDrawGizmosSelected()
        {
            // 绘制攻击范围
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            // 绘制检测范围
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
    
    /// <summary>
    /// 防御塔 projectile
    /// </summary>
    public class TowerProjectile : MonoBehaviour
    {
        private Transform target;
        private float damage;
        private MinionAI.Team team;
        private float speed = 20f;
        
        public void Initialize(Transform target, float damage, MinionAI.Team team)
        {
            this.target = target;
            this.damage = damage;
            this.team = team;
        }
        
        void Update()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
            
            // 追踪目标
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
            
            // 检查碰撞
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < 0.5f)
            {
                HitTarget();
            }
        }
        
        void HitTarget()
        {
            // 造成伤害
            if (target != null)
            {
                MinionAI minion = target.GetComponent<MinionAI>();
                if (minion != null)
                {
                    minion.TakeDamage(damage);
                }
                
                Hero.HeroBase hero = target.GetComponent<Hero.HeroBase>();
                if (hero != null)
                {
                    hero.TakeDamage(damage, Core.DamageType.Physical, null);
                }
            }
            
            // 播放命中特效
            // TODO: 实例化命中特效
            
            Destroy(gameObject);
        }
    }
}
