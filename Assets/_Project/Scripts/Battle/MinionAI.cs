using UnityEngine;
using UnityEngine.AI;

namespace ShanHaiKing.Battle
{
    /// <summary>
    /// 小兵AI控制器
    /// </summary>
    public class MinionAI : MonoBehaviour
    {
        [Header("阵营")]
        public Team team = Team.Blue;
        
        [Header("属性")]
        public float maxHealth = 500f;
        public float currentHealth;
        public float attackDamage = 50f;
        public float attackRange = 2f;
        public float attackSpeed = 1f;
        public float moveSpeed = 3.5f;
        
        [Header("AI设置")]
        public float detectionRange = 10f;
        public Transform target;
        public Transform[] waypoints;
        public int currentWaypoint = 0;
        
        private NavMeshAgent agent;
        private float lastAttackTime;
        private Animator animator;
        
        public enum Team { Blue, Red }
        
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            currentHealth = maxHealth;
            
            // 设置移动速度
            if (agent != null)
            {
                agent.speed = moveSpeed;
            }
            
            // 设置颜色区分阵营
            SetTeamColor();
        }
        
        void Update()
        {
            if (currentHealth <= 0) return;
            
            // 寻找目标
            FindTarget();
            
            // 行为逻辑
            if (target != null)
            {
                AttackTarget();
            }
            else
            {
                Patrol();
            }
        }
        
        void SetTeamColor()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(renderer.material);
                mat.color = team == Team.Blue ? Color.blue : Color.red;
                renderer.material = mat;
            }
        }
        
        void FindTarget()
        {
            // 检测范围内的敌人
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
            
            float closestDistance = float.MaxValue;
            Transform closestEnemy = null;
            
            foreach (Collider col in colliders)
            {
                MinionAI enemyMinion = col.GetComponent<MinionAI>();
                if (enemyMinion != null && enemyMinion.team != team && enemyMinion.currentHealth > 0)
                {
                    float distance = Vector3.Distance(transform.position, enemyMinion.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemyMinion.transform;
                    }
                }
                
                // 检测敌方英雄
                Hero.HeroBase enemyHero = col.GetComponent<Hero.HeroBase>();
                if (enemyHero != null && !enemyHero.isDead)
                {
                    // TODO: 检查英雄阵营
                    float distance = Vector3.Distance(transform.position, enemyHero.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemyHero.transform;
                    }
                }
            }
            
            target = closestEnemy;
        }
        
        void AttackTarget()
        {
            if (target == null) return;
            
            float distance = Vector3.Distance(transform.position, target.position);
            
            if (distance <= attackRange)
            {
                // 在攻击范围内，停止移动并攻击
                agent.isStopped = true;
                
                // 面向目标
                transform.LookAt(target);
                
                // 攻击
                if (Time.time >= lastAttackTime + 1f / attackSpeed)
                {
                    PerformAttack();
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                // 追击目标
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
        }
        
        void PerformAttack()
        {
            // 播放攻击动画
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // 造成伤害
            if (target != null)
            {
                MinionAI enemyMinion = target.GetComponent<MinionAI>();
                if (enemyMinion != null)
                {
                    enemyMinion.TakeDamage(attackDamage);
                }
                
                Hero.HeroBase enemyHero = target.GetComponent<Hero.HeroBase>();
                if (enemyHero != null)
                {
                    enemyHero.TakeDamage(attackDamage, Core.DamageType.Physical, null);
                }
            }
        }
        
        void Patrol()
        {
            if (waypoints == null || waypoints.Length == 0) return;
            
            agent.isStopped = false;
            
            // 检查是否到达当前路点
            float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypoint].position);
            
            if (distanceToWaypoint < 1f)
            {
                // 切换到下一个路点
                currentWaypoint++;
                if (currentWaypoint >= waypoints.Length)
                {
                    currentWaypoint = 0; // 循环或到达终点
                }
            }
            
            // 移动到当前路点
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
        
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            
            // 受伤动画
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        void Die()
        {
            // 死亡动画
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }
            
            // 禁用AI
            agent.enabled = false;
            this.enabled = false;
            
            // 延迟销毁
            Destroy(gameObject, 3f);
        }
        
        void OnDrawGizmosSelected()
        {
            // 绘制检测范围
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            // 绘制攻击范围
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
