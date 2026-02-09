using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 伏羲 - 法师型英雄（人皇）
    /// </summary>
    public class Hero_FuXi : HeroBase
    {
        [Header("伏羲特性")]
        public int yinYangStacks = 0;
        public int maxYinYangStacks = 8;
        public bool isYinDominant = true;
        public List<GameObject> trigramMarks = new List<GameObject>();
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "伏羲";
            heroTitle = "人皇";
            heroType = HeroType.Mage;
            
            stats.maxHealth = 2700f;
            stats.maxMana = 900f;
            stats.attackDamage = 60f;
            stats.abilityPower = 210f;
            stats.armor = 55f;
            stats.magicResist = 70f;
            stats.moveSpeed = 340f;
            stats.attackRange = 6.5f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_FuXi_Q>();
            skills[1] = gameObject.AddComponent<Skill_FuXi_W>();
            skills[2] = gameObject.AddComponent<Skill_FuXi_E>();
            skills[3] = gameObject.AddComponent<Skill_FuXi_R>();
        }
        
        public void AddYinYangStack()
        {
            if (yinYangStacks < maxYinYangStacks)
            {
                yinYangStacks++;
                isYinDominant = !isYinDominant;
                UpdatePassiveEffect();
            }
        }
        
        void UpdatePassiveEffect()
        {
            // 阴/阳状态切换效果
            if (isYinDominant)
            {
                // 阴：增加法术吸血
                stats.spellVamp = 0.15f + (yinYangStacks * 0.02f);
            }
            else
            {
                // 阳：增加法术强度
                stats.abilityPower *= 1f + (yinYangStacks * 0.03f);
            }
        }
        
        public void ConsumeStacks(int count)
        {
            yinYangStacks = Mathf.Max(0, yinYangStacks - count);
        }
    }
    
    public class Skill_FuXi_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "阴阳鱼";
            description = "发射阴阳鱼，根据当前状态附加额外效果";
            hotkey = KeyCode.Q;
            manaCost = 55f;
            cooldown = 4f;
            baseDamage = 100f;
            damagePerLevel = 30f;
            range = 9f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_FuXi fuxi = caster as Hero_FuXi;
            
            GameObject yinYangFish = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            yinYangFish.transform.position = caster.transform.position + Vector3.up;
            yinYangFish.transform.localScale = Vector3.one * 0.6f;
            
            Renderer renderer = yinYangFish.GetComponent<Renderer>();
            
            if (fuxi != null && fuxi.isYinDominant)
            {
                // 阴鱼 - 黑色
                renderer.material.color = Color.black;
            }
            else
            {
                // 阳鱼 - 白色
                renderer.material.color = Color.white;
            }
            
            Rigidbody rb = yinYangFish.AddComponent<Rigidbody>();
            rb.useGravity = false;
            
            Vector3 direction = (targetPosition - caster.transform.position).normalized;
            rb.velocity = direction * 10f;
            
            YinYangFishProjectile proj = yinYangFish.AddComponent<YinYangFishProjectile>();
            proj.Initialize(CurrentDamage, caster, fuxi?.isYinDominant ?? true);
            
            Destroy(yinYangFish, 4f);
            
            // 增加阴阳层数
            fuxi?.AddYinYangStack();
        }
    }
    
    public class YinYangFishProjectile : MonoBehaviour
    {
        private float damage;
        private HeroBase owner;
        private bool isYin;
        
        public void Initialize(float dmg, HeroBase caster, bool yinDominant)
        {
            damage = dmg;
            owner = caster;
            isYin = yinDominant;
        }
        
        void OnCollisionEnter(Collision collision)
        {
            HeroBase enemy = collision.gameObject.GetComponent<HeroBase>();
            if (enemy != null && enemy != owner)
            {
                enemy.TakeDamage(damage, Core.DamageType.Magic, owner);
                
                if (isYin)
                {
                    // 阴效果：减速
                    // enemy.ApplyBuff(BuffType.SpeedDown, 0.3f, 2f);
                }
                else
                {
                    // 阳效果：额外伤害
                    enemy.TakeDamage(damage * 0.3f, Core.DamageType.Magic, owner);
                }
                
                Destroy(gameObject);
            }
        }
    }
    
    public class Skill_FuXi_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "八卦阵";
            description = "布置八卦阵，阵内友军获得增益，敌军受到减益";
            hotkey = KeyCode.W;
            manaCost = 80f;
            cooldown = 14f;
            range = 8f;
            duration = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(CreateTrigramFormation(caster, targetPosition));
        }
        
        IEnumerator CreateTrigramFormation(HeroBase caster, Vector3 center)
        {
            GameObject[] trigrams = new GameObject[8];
            float radius = 4f;
            
            // 创建8个卦象
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI / 4f;
                Vector3 pos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                
                trigrams[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trigrams[i].transform.position = pos + Vector3.up;
                trigrams[i].transform.localScale = Vector3.one * 0.8f;
                trigrams[i].transform.LookAt(center);
                
                Renderer renderer = trigrams[i].GetComponent<Renderer>();
                renderer.material.color = Color.cyan;
                
                Destroy(trigrams[i].GetComponent<Collider>());
            }
            
            // 创建中心光柱
            GameObject centerPillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            centerPillar.transform.position = center;
            centerPillar.transform.localScale = new Vector3(6f, 0.1f, 6f);
            
            Renderer pillarRenderer = centerPillar.GetComponent<Renderer>();
            Material mat = new Material(pillarRenderer.material);
            mat.color = new Color(0, 1, 1, 0.3f);
            pillarRenderer.material = mat;
            
            Destroy(centerPillar.GetComponent<Collider>());
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 每秒对范围内单位施加效果
                if (elapsed % 1f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(center, 5f);
                    foreach (Collider hit in hits)
                    {
                        HeroBase unit = hit.GetComponent<HeroBase>();
                        if (unit != null)
                        {
                            if (unit == caster) // 友军
                            {
                                // 增益效果
                                unit.RestoreHealth(30f);
                            }
                            else // 敌军
                            {
                                unit.TakeDamage(60f, Core.DamageType.Magic, caster);
                            }
                        }
                    }
                }
                
                // 旋转八卦
                for (int i = 0; i < 8; i++)
                {
                    if (trigrams[i] != null)
                    {
                        trigrams[i].transform.RotateAround(center, Vector3.up, 20f * Time.deltaTime);
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 清理
            foreach (GameObject trigram in trigrams)
            {
                Destroy(trigram);
            }
            Destroy(centerPillar);
        }
    }
    
    public class Skill_FuXi_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "推演";
            description = "预测敌人位置，下次技能必中且伤害提高";
            hotkey = KeyCode.E;
            manaCost = 60f;
            cooldown = 10f;
            duration = 4f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_FuXi fuxi = caster as Hero_FuXi;
            if (fuxi == null) return;
            
            caster.StartCoroutine(DivinationBuff(caster, fuxi));
        }
        
        IEnumerator DivinationBuff(HeroBase caster, Hero_FuXi fuxi)
        {
            // 增加伤害
            float originalAP = caster.stats.abilityPower;
            caster.stats.abilityPower *= 1.5f;
            
            // 视觉提示
            GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            effect.transform.position = caster.transform.position + Vector3.up * 2f;
            effect.transform.localScale = Vector3.one * 2f;
            effect.transform.SetParent(caster.transform);
            
            Renderer renderer = effect.GetComponent<Renderer>();
            renderer.material.color = Color.cyan;
            Destroy(effect.GetComponent<Collider>());
            
            yield return new WaitForSeconds(duration);
            
            caster.stats.abilityPower = originalAP;
            Destroy(effect);
        }
    }
    
    public class Skill_FuXi_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "先天八卦";
            description = "召唤先天八卦之力，对大范围敌人造成巨额伤害并眩晕";
            hotkey = KeyCode.R;
            manaCost = 160f;
            cooldown = 70f;
            baseDamage = 450f;
            damagePerLevel = 200f;
            range = 12f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(PrimalTrigramUltimate(caster, targetPosition));
        }
        
        IEnumerator PrimalTrigramUltimate(HeroBase caster, Vector3 center)
        {
            // 预兆效果
            GameObject omen = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            omen.transform.position = center;
            omen.transform.localScale = new Vector3(10f, 0.1f, 10f);
            
            Renderer omenRenderer = omen.GetComponent<Renderer>();
            omenRenderer.material.color = Color.yellow;
            Destroy(omen.GetComponent<Collider>());
            
            yield return new WaitForSeconds(1f);
            Destroy(omen);
            
            // 创建8道卦象光束
            GameObject[] beams = new GameObject[8];
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI / 4f;
                Vector3 beamPos = center + new Vector3(Mathf.Cos(angle) * 5f, 0, Mathf.Sin(angle) * 5f);
                
                beams[i] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                beams[i].transform.position = beamPos;
                beams[i].transform.localScale = new Vector3(1f, 10f, 1f);
                
                Renderer beamRenderer = beams[i].GetComponent<Renderer>();
                beamRenderer.material.color = new Color(1f, 0.8f, 0f);
                
                Destroy(beams[i].GetComponent<Collider>());
            }
            
            // 伤害
            Collider[] hits = Physics.OverlapSphere(center, 8f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    // 眩晕
                    // enemy.ApplyBuff(BuffType.Stun, 0, 2f);
                }
            }
            
            yield return new WaitForSeconds(1.5f);
            
            foreach (GameObject beam in beams)
            {
                Destroy(beam);
            }
        }
    }
}
