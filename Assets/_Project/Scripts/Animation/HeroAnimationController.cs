using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Animation
{
    /// <summary>
    /// 动画控制器
    /// </summary>
    public class HeroAnimationController : MonoBehaviour
    {
        public Animator animator;
        
        [Header("动画状态")]
        public bool isIdle = true;
        public bool isMoving = false;
        public bool isAttacking = false;
        public bool isCasting = false;
        public bool isStunned = false;
        public bool isDead = false;
        
        [Header("动画参数")]
        private int speedHash;
        private int attackHash;
        private int skill1Hash;
        private int skill2Hash;
        private int skill3Hash;
        private int skill4Hash;
        private int deathHash;
        private int hurtHash;
        private int stunHash;
        private int idleHash;
        private int victoryHash;
        
        [Header("动画速度")]
        public float attackSpeed = 1f;
        public float moveSpeed = 1f;
        public float castSpeed = 1f;
        
        void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
            
            InitializeHashes();
        }
        
        void InitializeHashes()
        {
            speedHash = Animator.StringToHash("Speed");
            attackHash = Animator.StringToHash("Attack");
            skill1Hash = Animator.StringToHash("Skill1");
            skill2Hash = Animator.StringToHash("Skill2");
            skill3Hash = Animator.StringToHash("Skill3");
            skill4Hash = Animator.StringToHash("Skill4");
            deathHash = Animator.StringToHash("Death");
            hurtHash = Animator.StringToHash("Hurt");
            stunHash = Animator.StringToHash("Stun");
            idleHash = Animator.StringToHash("Idle");
            victoryHash = Animator.StringToHash("Victory");
        }
        
        void Update()
        {
            UpdateAnimatorParameters();
        }
        
        void UpdateAnimatorParameters()
        {
            if (animator == null) return;
            
            // 更新速度
            float currentSpeed = isMoving ? 1f : 0f;
            animator.SetFloat(speedHash, currentSpeed);
            
            // 更新攻击速度
            animator.SetFloat("AttackSpeed", attackSpeed);
        }
        
        #region 动作触发
        
        public void SetMoving(bool moving)
        {
            isMoving = moving;
            isIdle = !moving;
            
            if (animator != null)
            {
                animator.SetBool("IsMoving", moving);
                animator.SetBool("IsIdle", !moving);
            }
        }
        
        public void PlayIdle()
        {
            isIdle = true;
            isMoving = false;
            isAttacking = false;
            isCasting = false;
            
            if (animator != null)
            {
                animator.SetTrigger(idleHash);
            }
        }
        
        public void PlayAttack()
        {
            isAttacking = true;
            
            if (animator != null)
            {
                animator.SetTrigger(attackHash);
            }
            
            // 重置攻击状态
            Invoke(nameof(ResetAttack), 0.5f / attackSpeed);
        }
        
        void ResetAttack()
        {
            isAttacking = false;
        }
        
        public void PlaySkill(int skillIndex)
        {
            isCasting = true;
            
            if (animator != null)
            {
                switch (skillIndex)
                {
                    case 0: animator.SetTrigger(skill1Hash); break;
                    case 1: animator.SetTrigger(skill2Hash); break;
                    case 2: animator.SetTrigger(skill3Hash); break;
                    case 3: animator.SetTrigger(skill4Hash); break;
                }
            }
            
            Invoke(nameof(ResetCasting), 0.8f / castSpeed);
        }
        
        void ResetCasting()
        {
            isCasting = false;
        }
        
        public void PlayHurt()
        {
            if (animator != null && !isDead)
            {
                animator.SetTrigger(hurtHash);
            }
        }
        
        public void PlayDeath()
        {
            isDead = true;
            
            if (animator != null)
            {
                animator.SetTrigger(deathHash);
            }
        }
        
        public void PlayVictory()
        {
            if (animator != null)
            {
                animator.SetTrigger(victoryHash);
            }
        }
        
        public void SetStunned(bool stunned)
        {
            isStunned = stunned;
            
            if (animator != null)
            {
                animator.SetBool(stunHash, stunned);
            }
        }
        
        #endregion
        
        #region 动画事件
        
        // 动画事件回调
        public void OnAttackHit()
        {
            // 攻击命中时调用
            SendMessageUpwards("OnAnimationAttackHit", SendMessageOptions.DontRequireReceiver);
        }
        
        public void OnSkillCast()
        {
            // 技能释放时调用
            SendMessageUpwards("OnAnimationSkillCast", SendMessageOptions.DontRequireReceiver);
        }
        
        public void OnSkillEnd()
        {
            // 技能结束时调用
            SendMessageUpwards("OnAnimationSkillEnd", SendMessageOptions.DontRequireReceiver);
        }
        
        public void OnFootstep()
        {
            // 脚步声
            SendMessageUpwards("OnAnimationFootstep", SendMessageOptions.DontRequireReceiver);
        }
        
        #endregion
        
        #region 混合树控制
        
        public void SetBlendTreeValue(string parameterName, float value)
        {
            if (animator != null)
            {
                animator.SetFloat(parameterName, value);
            }
        }
        
        public void SetBlendTreeDirection(Vector2 direction)
        {
            if (animator != null)
            {
                animator.SetFloat("DirectionX", direction.x);
                animator.SetFloat("DirectionY", direction.y);
            }
        }
        
        #endregion
        
        #region 动画速度控制
        
        public void SetAttackSpeed(float speed)
        {
            attackSpeed = speed;
        }
        
        public void SetMoveSpeed(float speed)
        {
            moveSpeed = speed;
            if (animator != null)
            {
                animator.SetFloat("MoveSpeedMultiplier", speed);
            }
        }
        
        public void SetCastSpeed(float speed)
        {
            castSpeed = speed;
        }
        
        #endregion
        
        #region 动画状态检查
        
        public bool IsInTransition()
        {
            if (animator == null) return false;
            return animator.IsInTransition(0);
        }
        
        public bool IsAnimationPlaying(string animationName)
        {
            if (animator == null) return false;
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(animationName);
        }
        
        public float GetCurrentAnimationProgress()
        {
            if (animator == null) return 0f;
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime;
        }
        
        #endregion
    }
}
