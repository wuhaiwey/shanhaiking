using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 应龙 - 战士型英雄（龙神）
    /// </summary>
    public class Hero_YingLong : HeroBase
    {
        [Header("应龙特性")]
        public int dragonPower = 0;
        public int maxDragonPower = 100;
        public bool isDragonForm = false;
        public float flightDuration = 0f;
        public float maxFlightTime = 8f;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "应龙";
            heroTitle = "龙神";
            heroType = HeroType.Warrior;
            
            stats.maxHealth = 3800f;
            stats.maxMana = 600f;
            stats.attackDamage = 175f;
            stats.attackSpeed = 1.2f;
            stats.armor = 85f;
            stats.magicResist = 65f;
            stats.moveSpeed = 360f;
            
            stats.Initialize();
        }
        
        protected override void Update()
        {
            base.Update();
            
            // 龙之力自然回复
            if (dragonPower < maxDragonPower)
            {
                dragonPower += 1;
            }
            
            // 飞行状态管理
            if (isDragonForm)
            {
                flightDuration += Time.deltaTime;
                if (flightDuration >= maxFlightTime)
                {
                    ExitDragonForm();
                }
            }
        }
        
        public void AddDragonPower(int amount)
        {
            dragonPower = Mathf.Min(dragonPower + amount, maxDragonPower);
        }
        
        public void EnterDragonForm()
        {
            if (!isDragonForm && dragonPower >= 50)
            {
                isDragonForm = true;
                flightDuration = 0f;
                
                // 变身加成
                stats.attackDamage *= 1.3f;
                stats.armor *= 1.2f;
                stats.moveSpeed *= 1.4f;
                
                // 视觉效果
                transform.localScale = Vector3.one * 1.5f;
                
                Debug.Log("应龙变身！");
            }
        }
        
        public void ExitDragonForm()
        {
            if (isDragonForm)
            {
                isDragonForm = false;
                
                // 恢复属性
                stats.attackDamage /= 1.3f;
                stats.armor /= 1.2f;
                stats.moveSpeed /= 1.4f;
                
                // 恢复体型
                transform.localScale = Vector3.one;
                
                dragonPower = 0;
            }
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_YingLong_Q>();
            skills[1] = gameObject.AddComponent<Skill_YingLong_W>();
            skills[2] = gameObject.AddComponent<Skill_YingLong_E>();
            skills[3] = gameObject.AddComponent<Skill_YingLong_R>();
        }
    }
    
    public class Skill_YingLong_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "龙息";
            description = "喷吐龙息对前方敌人造成伤害";
            hotkey = KeyCode.Q;
            manaCost = 45f;
            cooldown = 5f;
            baseDamage = 140f;
            damagePerLevel = 35f;
            range = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_YingLong yinglong = caster as Hero_YingLong;
            
            // 创建龙息效果
            GameObject dragonBreath = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            dragonBreath.transform.position = caster.transform.position + Vector3.up + caster.transform.forward * 2f;
            dragonBreath.transform.localScale = new Vector3(1.5f, 4f, 1.5f);
            dragonBreath.transform.rotation = caster.transform.rotation * Quaternion.Euler(90, 0, 0);
            
            Renderer renderer = dragonBreath.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.3f, 0f);
            
            Destroy(dragonBreath.GetComponent<Collider>());
            
            // 范围伤害
            Collider[] hits = Physics.OverlapSphere(caster.transform.position + caster.transform.forward * 5f, 3f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    float damage = CurrentDamage;
                    if (yinglong != null && yinglong.isDragonForm)
                    {
                        damage *= 1.5f; // 龙形态伤害加成
                    }
                    enemy.TakeDamage(damage, Core.DamageType.Magic, caster);
                }
            }
            
            Destroy(dragonBreath, 1f);
            
            // 增加龙之力
            yinglong?.AddDragonPower(10);
        }
    }
    
    public class Skill_YingLong_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "龙尾横扫";
            description = "用龙尾横扫周围敌人，造成范围伤害和击退";
            hotkey = KeyCode.W;
            manaCost = 55f;
            cooldown = 8f;
            baseDamage = 160f;
            damagePerLevel = 40f;
            range = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_YingLong yinglong = caster as Hero_YingLong;
            
            // 龙尾视觉效果
            GameObject tailSwipe = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tailSwipe.transform.position = caster.transform.position;
            tailSwipe.transform.localScale = new Vector3(10f, 0.5f, 10f);
            
            Renderer renderer = tailSwipe.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(1f, 0.5f, 0f, 0.3f);
            renderer.material = mat;
            
            Destroy(tailSwipe.GetComponent<Collider>());
            Destroy(tailSwipe, 0.5f);
            
            // 范围伤害和击退
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, range);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    float damage = CurrentDamage;
                    enemy.TakeDamage(damage, Core.DamageType.Physical, caster);
                    
                    // 击退
                    Vector3 knockbackDir = (enemy.transform.position - caster.transform.position).normalized;
                    float knockbackForce = yinglong != null && yinglong.isDragonForm ? 6f : 4f;
                    enemy.transform.position += knockbackDir * knockbackForce;
                }
            }
            
            // 增加龙之力
            yinglong?.AddDragonPower(15);
        }
    }
    
    public class Skill_YingLong_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "龙腾";
            description = "腾空而起，无视地形移动并可以俯冲攻击";
            hotkey = KeyCode.E;
            manaCost = 70f;
            cooldown = 12f;
            range = 15f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_YingLong yinglong = caster as Hero_YingLong;
            if (yinglong != null)
            {
                yinglong.EnterDragonForm();
            }
            
            caster.StartCoroutine(DragonFlight(caster, targetPosition, yinglong));
        }
        
        IEnumerator DragonFlight(HeroBase caster, Vector3 targetPos, Hero_YingLong yinglong)
        {
            Vector3 startPos = caster.transform.position;
            float flightTime = 1f;
            float elapsed = 0f;
            
            // 升空
            while (elapsed < flightTime * 0.3f)
            {
                caster.transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * 5f, elapsed / (flightTime * 0.3f));
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 飞行到目标
            Vector3 flyStart = caster.transform.position;
            elapsed = 0f;
            
            while (elapsed < flightTime * 0.4f)
            {
                caster.transform.position = Vector3.Lerp(flyStart, targetPos + Vector3.up * 5f, elapsed / (flightTime * 0.4f));
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 俯冲
            Vector3 diveStart = caster.transform.position;
            elapsed = 0f;
            
            while (elapsed < flightTime * 0.3f)
            {
                caster.transform.position = Vector3.Lerp(diveStart, targetPos, elapsed / (flightTime * 0.3f));
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            caster.transform.position = targetPos;
            
            // 俯冲伤害
            Collider[] hits = Physics.OverlapSphere(targetPos, 4f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(200f, Core.DamageType.Physical, caster);
                }
            }
            
            // 地面冲击效果
            Effects.ScreenEffectManager.Instance?.ShakeCamera(0.4f, 0.3f);
        }
    }
    
    public class Skill_YingLong_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "真龙形态";
            description = "化身为真龙，大幅提升全属性并可以飞行";
            hotkey = KeyCode.R;
            manaCost = 120f;
            cooldown = 80f;
            duration = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_YingLong yinglong = caster as Hero_YingLong;
            if (yinglong != null)
            {
                caster.StartCoroutine(TrueDragonForm(caster, yinglong));
            }
        }
        
        IEnumerator TrueDragonForm(HeroBase caster, Hero_YingLong yinglong)
        {
            // 完全变身
            yinglong.EnterDragonForm();
            
            // 额外加成
            float originalHealth = caster.MaxHealth;
            caster.stats.maxHealth *= 1.5f;
            caster.RestoreHealth(caster.MaxHealth * 0.3f);
            
            // 龙翼效果
            GameObject leftWing = CreateWing(caster, true);
            GameObject rightWing = CreateWing(caster, false);
            
            // 龙光环
            GameObject dragonAura = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dragonAura.transform.SetParent(caster.transform);
            dragonAura.transform.localPosition = Vector3.zero;
            dragonAura.transform.localScale = Vector3.one * 4f;
            
            Renderer auraRenderer = dragonAura.GetComponent<Renderer>();
            Material auraMat = new Material(auraRenderer.material);
            auraMat.color = new Color(1f, 0.5f, 0f, 0.3f);
            auraRenderer.material = auraMat;
            
            Destroy(dragonAura.GetComponent<Collider>());
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 持续回复
                if (elapsed % 1f < Time.deltaTime)
                {
                    caster.RestoreHealth(caster.MaxHealth * 0.05f);
                }
                
                // 扇动翅膀
                if (leftWing != null)
                    leftWing.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(elapsed * 10f) * 20f);
                if (rightWing != null)
                    rightWing.transform.localRotation = Quaternion.Euler(0, 0, -Mathf.Sin(elapsed * 10f) * 20f);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 恢复
            yinglong.ExitDragonForm();
            caster.stats.maxHealth = originalHealth;
            
            Destroy(leftWing);
            Destroy(rightWing);
            Destroy(dragonAura);
        }
        
        GameObject CreateWing(HeroBase caster, bool isLeft)
        {
            GameObject wing = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wing.name = isLeft ? "LeftWing" : "RightWing";
            wing.transform.SetParent(caster.transform);
            
            float xOffset = isLeft ? -0.8f : 0.8f;
            wing.transform.localPosition = new Vector3(xOffset, 0.5f, -0.5f);
            wing.transform.localScale = new Vector3(0.2f, 1.5f, 1f);
            
            Renderer renderer = wing.GetComponent<Renderer>();
            renderer.material.color = new Color(0.8f, 0.2f, 0f);
            
            Destroy(wing.GetComponent<Collider>());
            
            return wing;
        }
    }
}
