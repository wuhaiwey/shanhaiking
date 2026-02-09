using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Battle
{
    /// <summary>
    /// 野怪系统 - 管理野区怪物
    /// </summary>
    public class JungleManager : MonoBehaviour
    {
        public static JungleManager Instance { get; private set; }
        
        [Header("野怪营地")]
        public List<JungleCamp> jungleCamps = new List<JungleCamp>();
        
        [Header("野怪预制体")]
        public GameObject wolfPrefab;
        public GameObject golemPrefab;
        public GameObject birdPrefab;
        public GameObject buffPrefab;
        public GameObject dragonPrefab;
        public GameObject baronPrefab;
        
        [Header("刷新设置")]
        public float respawnTime = 120f;
        public float firstSpawnDelay = 90f;
        
        [Header("增益效果")]
        public float blueBuffDuration = 120f;
        public float redBuffDuration = 120f;
        
        void Awake()
        {
            Instance = this;
        }
        
        void Start()
        {
            InitializeJungleCamps();
            Invoke(nameof(FirstSpawn), firstSpawnDelay);
        }
        
        void InitializeJungleCamps()
        {
            // 蓝方野区
            jungleCamps.Add(CreateCamp("Blue_Wolves", new Vector3(-30, 0, 20), JungleCampType.Wolves, MinionAI.Team.Blue));
            jungleCamps.Add(CreateCamp("Blue_Golems", new Vector3(-25, 0, 30), JungleCampType.Golems, MinionAI.Team.Blue));
            jungleCamps.Add(CreateCamp("Blue_Birds", new Vector3(-35, 0, 10), JungleCampType.Birds, MinionAI.Team.Blue));
            jungleCamps.Add(CreateCamp("Blue_Buff", new Vector3(-40, 0, 25), JungleCampType.BlueBuff, MinionAI.Team.Blue));
            
            // 红方野区
            jungleCamps.Add(CreateCamp("Red_Wolves", new Vector3(30, 0, -20), JungleCampType.Wolves, MinionAI.Team.Red));
            jungleCamps.Add(CreateCamp("Red_Golems", new Vector3(25, 0, -30), JungleCampType.Golems, MinionAI.Team.Red));
            jungleCamps.Add(CreateCamp("Red_Birds", new Vector3(35, 0, -10), JungleCampType.Birds, MinionAI.Team.Red));
            jungleCamps.Add(CreateCamp("Red_Buff", new Vector3(40, 0, -25), JungleCampType.RedBuff, MinionAI.Team.Red));
            
            // 龙坑
            jungleCamps.Add(CreateCamp("Dragon_Pit", new Vector3(0, 0, 35), JungleCampType.Dragon, MinionAI.Team.Neutral));
            jungleCamps.Add(CreateCamp("Baron_Pit", new Vector3(0, 0, -35), JungleCampType.Baron, MinionAI.Team.Neutral));
        }
        
        JungleCamp CreateCamp(string name, Vector3 position, JungleCampType type, MinionAI.Team team)
        {
            GameObject campObj = new GameObject(name);
            campObj.transform.position = position;
            
            JungleCamp camp = campObj.AddComponent<JungleCamp>();
            camp.campName = name;
            camp.campType = type;
            camp.team = team;
            camp.position = position;
            camp.respawnTime = this.respawnTime;
            
            return camp;
        }
        
        void FirstSpawn()
        {
            foreach (JungleCamp camp in jungleCamps)
            {
                camp.SpawnMonsters();
            }
        }
        
        public void OnCampCleared(JungleCamp camp)
        {
            // 给予击杀者奖励
            StartCoroutine(RespawnCamp(camp));
        }
        
        System.Collections.IEnumerator RespawnCamp(JungleCamp camp)
        {
            yield return new WaitForSeconds(respawnTime);
            camp.SpawnMonsters();
        }
        
        public void ApplyBlueBuff(Hero.HeroBase hero)
        {
            // 蓝Buff：法力回复 + 冷却缩减
            hero.StartCoroutine(BlueBuffEffect(hero));
        }
        
        System.Collections.IEnumerator BlueBuffEffect(Hero.HeroBase hero)
        {
            float originalManaRegen = hero.stats.manaRegen;
            hero.stats.manaRegen += 10f; // 增加法力回复
            
            yield return new WaitForSeconds(blueBuffDuration);
            
            hero.stats.manaRegen = originalManaRegen;
        }
        
        public void ApplyRedBuff(Hero.HeroBase hero)
        {
            // 红Buff：减速 + 持续伤害
            hero.StartCoroutine(RedBuffEffect(hero));
        }
        
        System.Collections.IEnumerator RedBuffEffect(Hero.HeroBase hero)
        {
            // 简化实现：增加攻击力
            float originalAttack = hero.stats.attackDamage;
            hero.stats.attackDamage *= 1.2f;
            
            yield return new WaitForSeconds(redBuffDuration);
            
            hero.stats.attackDamage = originalAttack;
        }
        
        public void ApplyDragonBuff(MinionAI.Team team)
        {
            // 龙Buff：全队增益
            // 增加攻击力、防御力
        }
        
        public void ApplyBaronBuff(MinionAI.Team team)
        {
            // 大龙Buff：强化小兵
        }
    }
    
    public class JungleCamp : MonoBehaviour
    {
        public string campName;
        public JungleCampType campType;
        public MinionAI.Team team;
        public Vector3 position;
        public float respawnTime;
        
        public List<GameObject> spawnedMonsters = new List<GameObject>();
        public bool isCleared = false;
        
        public void SpawnMonsters()
        {
            ClearMonsters();
            
            switch (campType)
            {
                case JungleCampType.Wolves:
                    SpawnWolves();
                    break;
                case JungleCampType.Golems:
                    SpawnGolems();
                    break;
                case JungleCampType.Birds:
                    SpawnBirds();
                    break;
                case JungleCampType.BlueBuff:
                    SpawnBlueBuff();
                    break;
                case JungleCampType.RedBuff:
                    SpawnRedBuff();
                    break;
                case JungleCampType.Dragon:
                    SpawnDragon();
                    break;
                case JungleCampType.Baron:
                    SpawnBaron();
                    break;
            }
            
            isCleared = false;
        }
        
        void SpawnWolves()
        {
            // 1只大狼 + 2只小狼
            SpawnMonster(JungleManager.Instance.wolfPrefab, Vector3.zero, 1.5f, 500f, 50f);
            SpawnMonster(JungleManager.Instance.wolfPrefab, new Vector3(2, 0, 0), 1f, 300f, 30f);
            SpawnMonster(JungleManager.Instance.wolfPrefab, new Vector3(-2, 0, 0), 1f, 300f, 30f);
        }
        
        void SpawnGolems()
        {
            // 1只大石像 + 2只小石像
            SpawnMonster(JungleManager.Instance.golemPrefab, Vector3.zero, 1.8f, 800f, 60f);
            SpawnMonster(JungleManager.Instance.golemPrefab, new Vector3(2.5f, 0, 0), 1.2f, 400f, 35f);
            SpawnMonster(JungleManager.Instance.golemPrefab, new Vector3(-2.5f, 0, 0), 1.2f, 400f, 35f);
        }
        
        void SpawnBirds()
        {
            // 3只鸟
            for (int i = 0; i < 3; i++)
            {
                Vector3 offset = new Vector3((i - 1) * 2, 0, 0);
                SpawnMonster(JungleManager.Instance.birdPrefab, offset, 1f, 350f, 40f);
            }
        }
        
        void SpawnBlueBuff()
        {
            // 蓝Buff怪
            SpawnMonster(JungleManager.Instance.buffPrefab, Vector3.zero, 2f, 1200f, 80f, true);
        }
        
        void SpawnRedBuff()
        {
            // 红Buff怪
            SpawnMonster(JungleManager.Instance.buffPrefab, Vector3.zero, 2f, 1200f, 80f, true);
        }
        
        void SpawnDragon()
        {
            // 龙
            SpawnMonster(JungleManager.Instance.dragonPrefab, Vector3.zero, 3f, 5000f, 200f, true);
        }
        
        void SpawnBaron()
        {
            // 大龙
            SpawnMonster(JungleManager.Instance.baronPrefab, Vector3.zero, 4f, 10000f, 400f, true);
        }
        
        void SpawnMonster(GameObject prefab, Vector3 offset, float scale, float health, float damage, bool isBuffMonster = false)
        {
            if (prefab == null) return;
            
            GameObject monster = Instantiate(prefab, position + offset, Quaternion.identity, transform);
            monster.transform.localScale = Vector3.one * scale;
            
            JungleMonster monsterScript = monster.AddComponent<JungleMonster>();
            monsterScript.Initialize(health, damage, this, isBuffMonster);
            
            spawnedMonsters.Add(monster);
        }
        
        void ClearMonsters()
        {
            foreach (GameObject monster in spawnedMonsters)
            {
                if (monster != null)
                    Destroy(monster);
            }
            spawnedMonsters.Clear();
        }
        
        public void OnMonsterDeath(JungleMonster monster)
        {
            spawnedMonsters.Remove(monster.gameObject);
            
            if (spawnedMonsters.Count == 0)
            {
                isCleared = true;
                JungleManager.Instance.OnCampCleared(this);
            }
        }
    }
    
    public class JungleMonster : MonoBehaviour
    {
        public float maxHealth;
        public float currentHealth;
        public float attackDamage;
        public JungleCamp parentCamp;
        public bool isBuffMonster;
        
        private Hero.HeroBase killer;
        
        public void Initialize(float health, float damage, JungleCamp camp, bool isBuff)
        {
            maxHealth = health;
            currentHealth = health;
            attackDamage = damage;
            parentCamp = camp;
            isBuffMonster = isBuff;
        }
        
        public void TakeDamage(float damage, Hero.HeroBase attacker)
        {
            currentHealth -= damage;
            killer = attacker;
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        void Die()
        {
            // 给予奖励
            if (killer != null)
            {
                int goldReward = isBuffMonster ? 100 : 60;
                int expReward = isBuffMonster ? 200 : 100;
                
                // 增加金币和经验
                Core.ShopManager.Instance?.AddGold(goldReward);
                killer.GainExperience(expReward);
                
                // 如果是Buff怪，给予Buff
                if (isBuffMonster)
                {
                    if (parentCamp.campType == JungleCampType.BlueBuff)
                    {
                        JungleManager.Instance.ApplyBlueBuff(killer);
                    }
                    else if (parentCamp.campType == JungleCampType.RedBuff)
                    {
                        JungleManager.Instance.ApplyRedBuff(killer);
                    }
                }
            }
            
            parentCamp?.OnMonsterDeath(this);
            Destroy(gameObject);
        }
    }
    
    public enum JungleCampType
    {
        Wolves,     // 狼群
        Golems,     // 石像
        Birds,      // 鸟类
        BlueBuff,   // 蓝Buff
        RedBuff,    // 红Buff
        Dragon,     // 小龙
        Baron       // 大龙
    }
}
