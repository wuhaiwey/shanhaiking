using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 姜子牙 - 法师型英雄（太公）
    /// </summary>
    public class Hero_JiangZiYa : HeroBase
    {
        [Header("姜子牙特性")]
        public int fishingStacks = 0;
        public int maxFishingStacks = 5;
        public bool isFishing = false;
        public GameObject fishingRod;
        public List<FishType> caughtFish = new List<FishType>();
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "姜子牙";
            heroTitle = "太公";
            heroType = HeroType.Mage;
            
            stats.maxHealth = 2600f;
            stats.maxMana = 1000f;
            stats.attackDamage = 45f;
            stats.abilityPower = 230f;
            stats.armor = 50f;
            stats.magicResist = 75f;
            stats.moveSpeed = 340f;
            stats.attackRange = 8f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_JiangZiYa_Q>();
            skills[1] = gameObject.AddComponent<Skill_JiangZiYa_W>();
            skills[2] = gameObject.AddComponent<Skill_JiangZiYa_E>();
            skills[3] = gameObject.AddComponent<Skill_JiangZiYa_R>();
        }
        
        public void AddFishingStack(FishType fish)
        {
            if (fishingStacks < maxFishingStacks)
            {
                fishingStacks++;
                caughtFish.Add(fish);
                
                // 根据鱼的类型获得不同增益
                switch (fish)
                {
                    case FishType.Gold:
                        stats.abilityPower += 15f;
                        break;
                    case FishType.Mana:
                        stats.maxMana += 50f;
                        break;
                    case FishType.Health:
                        stats.maxHealth += 100f;
                        break;
                }
            }
        }
    }
    
    public enum FishType
    {
        Normal,
        Gold,       // 金鱼 - 增加法强
        Mana,       // 蓝鱼 - 增加法力
        Health,     // 红鱼 - 增加生命
        Legendary   // 传说鱼 - 大幅增加全属性
    }
    
    public class Skill_JiangZiYa_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "打神鞭";
            description = "挥动打神鞭对敌人造成伤害并眩晕";
            hotkey = KeyCode.Q;
            manaCost = 60f;
            cooldown = 7f;
            baseDamage = 140f;
            damagePerLevel = 35f;
            range = 7f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 打神鞭效果
            GameObject whip = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            whip.transform.position = caster.transform.position + Vector3.up + caster.transform.forward * 3f;
            whip.transform.localScale = new Vector3(0.1f, 3f, 0.1f);
            whip.transform.rotation = caster.transform.rotation * Quaternion.Euler(90, 0, 0);
            
            Renderer renderer = whip.GetComponent<Renderer>();
            renderer.material.color = new Color(0.8f, 0.6f, 0.2f);
            
            Destroy(whip.GetComponent<Collider>());
            
            // 扇形范围伤害
            Collider[] hits = Physics.OverlapSphere(caster.transform.position + caster.transform.forward * 3f, 2f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    // 眩晕
                    // enemy.ApplyBuff(BuffType.Stun, 0, 1f);
                }
            }
            
            Destroy(whip, 0.3f);
            
            // 钓鱼积累
            Hero_JiangZiYa jiang = caster as Hero_JiangZiYa;
            if (jiang != null && Random.value > 0.7f)
            {
                jiang.AddFishingStack(FishType.Normal);
            }
        }
    }
    
    public class Skill_JiangZiYa_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "愿者上钩";
            description = "放下钓竿钓鱼，期间可以钓到各种增益";
            hotkey = KeyCode.W;
            manaCost = 80f;
            cooldown = 15f;
            channelTime = 3f;
        }
        
        public float channelTime = 3f;
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_JiangZiYa jiang = caster as Hero_JiangZiYa;
            if (jiang != null)
            {
                caster.StartCoroutine(Fishing(caster, jiang));
            }
        }
        
        IEnumerator Fishing(HeroBase caster, Hero_JiangZiYa jiang)
        {
            jiang.isFishing = true;
            
            // 创建钓竿
            GameObject rod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rod.transform.position = caster.transform.position + caster.transform.right * 0.5f + Vector3.up;
            rod.transform.localScale = new Vector3(0.05f, 2f, 0.05f);
            rod.transform.rotation = Quaternion.Euler(45, 0, 0);
            rod.transform.SetParent(caster.transform);
            
            Renderer renderer = rod.GetComponent<Renderer>();
            renderer.material.color = new Color(0.4f, 0.25f, 0.1f);
            
            Destroy(rod.GetComponent<Collider>());
            
            // 钓鱼线
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            line.transform.position = caster.transform.position + caster.transform.forward * 1.5f + Vector3.up * 0.5f;
            line.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
            
            Renderer lineRenderer = line.GetComponent<Renderer>();
            lineRenderer.material.color = Color.white;
            
            Destroy(line.GetComponent<Collider>());
            
            // 钓鱼浮标
            GameObject floatObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            floatObj.transform.position = caster.transform.position + caster.transform.forward * 1.5f;
            floatObj.transform.localScale = Vector3.one * 0.1f;
            
            Renderer floatRenderer = floatObj.GetComponent<Renderer>();
            floatRenderer.material.color = Color.red;
            
            Destroy(floatObj.GetComponent<Collider>());
            
            // 等待钓鱼
            float elapsed = 0f;
            while (elapsed < channelTime)
            {
                // 浮标浮动
                floatObj.transform.position = caster.transform.position + caster.transform.forward * 1.5f + 
                    new Vector3(0, Mathf.Sin(elapsed * 3f) * 0.1f, 0);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 随机钓到鱼
            float roll = Random.value;
            FishType caughtFish;
            
            if (roll > 0.95f)
                caughtFish = FishType.Legendary;
            else if (roll > 0.8f)
                caughtFish = FishType.Gold;
            else if (roll > 0.6f)
                caughtFish = FishType.Mana;
            else if (roll > 0.4f)
                caughtFish = FishType.Health;
            else
                caughtFish = FishType.Normal;
            
            jiang.AddFishingStack(caughtFish);
            
            // 清理
            Destroy(rod);
            Destroy(line);
            Destroy(floatObj);
            
            jiang.isFishing = false;
        }
    }
    
    public class Skill_JiangZiYa_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "封神榜";
            description = "召唤封神榜封印敌人技能";
            hotkey = KeyCode.E;
            manaCost = 70f;
            cooldown = 14f;
            range = 8f;
            duration = 3f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            if (target != null)
            {
                caster.StartCoroutine(SealTarget(caster, target));
            }
        }
        
        IEnumerator SealTarget(HeroBase caster, HeroBase target)
        {
            // 封神榜卷轴效果
            GameObject scroll = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            scroll.transform.position = target.transform.position + Vector3.up * 3f;
            scroll.transform.localScale = new Vector3(2f, 0.1f, 2f);
            
            Renderer renderer = scroll.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(1f, 0.9f, 0.6f, 0.8f);
            renderer.material = mat;
            
            Destroy(scroll.GetComponent<Collider>());
            
            // 封印效果
            // target.ApplyBuff(BuffType.Silence, 0, duration);
            target.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
            
            // 展开卷轴动画
            float elapsed = 0f;
            while (elapsed < duration)
            {
                scroll.transform.Rotate(Vector3.up, 90f * Time.deltaTime);
                scroll.transform.localScale = new Vector3(
                    2f + Mathf.Sin(elapsed * 2f) * 0.5f, 
                    0.1f, 
                    2f + Mathf.Sin(elapsed * 2f) * 0.5f
                );
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(scroll);
        }
    }
    
    public class Skill_JiangZiYa_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "天命封神";
            description = "召唤天雷惩罚敌人，伤害与钓鱼层数相关";
            hotkey = KeyCode.R;
            manaCost = 150f;
            cooldown = 80f;
            range = 15f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_JiangZiYa jiang = caster as Hero_JiangZiYa;
            
            // 根据钓鱼层数增加伤害
            float multiplier = 1f + (jiang?.fishingStacks ?? 0) * 0.2f;
            
            caster.StartCoroutine(DivineJudgment(caster, targetPosition, multiplier));
        }
        
        IEnumerator DivineJudgment(HeroBase caster, Vector3 center, float damageMultiplier)
        {
            // 预兆
            GameObject omen = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            omen.transform.position = center;
            omen.transform.localScale = new Vector3(10f, 0.1f, 10f);
            
            Renderer omenRenderer = omen.GetComponent<Renderer>();
            omenRenderer.material.color = Color.yellow;
            
            Destroy(omen.GetComponent<Collider>());
            
            yield return new WaitForSeconds(1f);
            
            Destroy(omen);
            
            // 天雷
            for (int i = 0; i < 5; i++)
            {
                Vector3 strikePos = center + Random.insideUnitSphere * 5f;
                strikePos.y = center.y;
                
                // 闪电柱
                GameObject lightning = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                lightning.transform.position = strikePos + Vector3.up * 10f;
                lightning.transform.localScale = new Vector3(0.5f, 20f, 0.5f);
                
                Renderer lightningRenderer = lightning.GetComponent<Renderer>();
                lightningRenderer.material.color = new Color(0.8f, 0.9f, 1f);
                
                Destroy(lightning.GetComponent<Collider>());
                Destroy(lightning, 0.3f);
                
                // 伤害
                Collider[] hits = Physics.OverlapSphere(strikePos, 2f);
                foreach (Collider hit in hits)
                {
                    HeroBase enemy = hit.GetComponent<HeroBase>();
                    if (enemy != null && enemy != caster)
                    {
                        float damage = (300f + caster.stats.abilityPower * 0.8f) * damageMultiplier;
                        enemy.TakeDamage(damage, Core.DamageType.Magic, caster);
                        // 眩晕
                        // enemy.ApplyBuff(BuffType.Stun, 0, 1f);
                    }
                }
                
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
