using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Modeling
{
    /// <summary>
    /// 程序化英雄模型生成器
    /// </summary>
    public class HeroModelGenerator : MonoBehaviour
    {
        public static HeroModelGenerator Instance { get; private set; }
        
        [Header("身体部件")]
        public GameObject headPrefab;
        public GameObject torsoPrefab;
        public GameObject armPrefab;
        public GameObject legPrefab;
        public GameObject weaponPrefab;
        
        [Header("材质库")]
        public Material[] skinMaterials;
        public Material[] armorMaterials;
        public Material[] weaponMaterials;
        
        [Header("特效预制体")]
        public GameObject auraEffectPrefab;
        public GameObject weaponGlowPrefab;
        
        void Awake()
        {
            Instance = this;
        }
        
        /// <summary>
        /// 生成完整英雄模型
        /// </summary>
        public GameObject GenerateHeroModel(string heroName, HeroModelData data)
        {
            GameObject heroRoot = new GameObject($"HeroModel_{heroName}");
            
            // 创建身体部件
            GameObject head = CreateHead(data.headScale, data.headMaterial);
            GameObject torso = CreateTorso(data.torsoScale, data.torsoMaterial);
            GameObject leftArm = CreateArm(data.armScale, data.armMaterial, true);
            GameObject rightArm = CreateArm(data.armScale, data.armMaterial, false);
            GameObject leftLeg = CreateLeg(data.legScale, data.legMaterial, true);
            GameObject rightLeg = CreateLeg(data.legScale, data.legMaterial, false);
            
            // 设置父子关系
            head.transform.SetParent(heroRoot.transform);
            torso.transform.SetParent(heroRoot.transform);
            leftArm.transform.SetParent(heroRoot.transform);
            rightArm.transform.SetParent(heroRoot.transform);
            leftLeg.transform.SetParent(heroRoot.transform);
            rightLeg.transform.SetParent(heroRoot.transform);
            
            // 设置位置
            head.transform.localPosition = new Vector3(0, data.height * 0.8f, 0);
            torso.transform.localPosition = new Vector3(0, data.height * 0.4f, 0);
            leftArm.transform.localPosition = new Vector3(-data.shoulderWidth * 0.5f, data.height * 0.6f, 0);
            rightArm.transform.localPosition = new Vector3(data.shoulderWidth * 0.5f, data.height * 0.6f, 0);
            leftLeg.transform.localPosition = new Vector3(-data.hipWidth * 0.3f, data.height * 0.2f, 0);
            rightLeg.transform.localPosition = new Vector3(data.hipWidth * 0.3f, data.height * 0.2f, 0);
            
            // 添加武器
            if (data.hasWeapon)
            {
                GameObject weapon = CreateWeapon(data.weaponType, data.weaponMaterial);
                weapon.transform.SetParent(rightArm.transform);
                weapon.transform.localPosition = new Vector3(0, -data.armScale.y * 0.3f, data.armScale.z * 0.5f);
                weapon.transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            
            // 添加特效
            if (data.hasAura)
            {
                GameObject aura = Instantiate(auraEffectPrefab, heroRoot.transform);
                aura.transform.localPosition = Vector3.zero;
            }
            
            // 添加动画组件
            Animator animator = heroRoot.AddComponent<Animator>();
            SetupAnimator(animator, heroName);
            
            // 添加碰撞体
            CapsuleCollider collider = heroRoot.AddComponent<CapsuleCollider>();
            collider.height = data.height;
            collider.radius = data.shoulderWidth * 0.3f;
            collider.center = new Vector3(0, data.height * 0.5f, 0);
            
            return heroRoot;
        }
        
        GameObject CreateHead(Vector3 scale, Material material)
        {
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.localScale = scale;
            
            Renderer renderer = head.GetComponent<Renderer>();
            if (material != null)
                renderer.material = material;
            
            Destroy(head.GetComponent<Collider>());
            return head;
        }
        
        GameObject CreateTorso(Vector3 scale, Material material)
        {
            GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            torso.name = "Torso";
            torso.transform.localScale = scale;
            
            Renderer renderer = torso.GetComponent<Renderer>();
            if (material != null)
                renderer.material = material;
            
            Destroy(torso.GetComponent<Collider>());
            return torso;
        }
        
        GameObject CreateArm(Vector3 scale, Material material, bool isLeft)
        {
            GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            arm.name = isLeft ? "LeftArm" : "RightArm";
            arm.transform.localScale = scale;
            
            Renderer renderer = arm.GetComponent<Renderer>();
            if (material != null)
                renderer.material = material;
            
            Destroy(arm.GetComponent<Collider>());
            return arm;
        }
        
        GameObject CreateLeg(Vector3 scale, Material material, bool isLeft)
        {
            GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            leg.name = isLeft ? "LeftLeg" : "RightLeg";
            leg.transform.localScale = scale;
            
            Renderer renderer = leg.GetComponent<Renderer>();
            if (material != null)
                renderer.material = material;
            
            Destroy(leg.GetComponent<Collider>());
            return leg;
        }
        
        GameObject CreateWeapon(WeaponType type, Material material)
        {
            GameObject weapon = new GameObject("Weapon");
            
            switch (type)
            {
                case WeaponType.Sword:
                    CreateSwordMesh(weapon, material);
                    break;
                case WeaponType.Spear:
                    CreateSpearMesh(weapon, material);
                    break;
                case WeaponType.Bow:
                    CreateBowMesh(weapon, material);
                    break;
                case WeaponType.Axe:
                    CreateAxeMesh(weapon, material);
                    break;
                case WeaponType.Staff:
                    CreateStaffMesh(weapon, material);
                    break;
            }
            
            return weapon;
        }
        
        void CreateSwordMesh(GameObject parent, Material material)
        {
            // 剑刃
            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.name = "Blade";
            blade.transform.SetParent(parent.transform);
            blade.transform.localScale = new Vector3(0.1f, 1.5f, 0.05f);
            blade.transform.localPosition = new Vector3(0, 0.75f, 0);
            
            // 剑柄
            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.name = "Handle";
            handle.transform.SetParent(parent.transform);
            handle.transform.localScale = new Vector3(0.08f, 0.4f, 0.08f);
            handle.transform.localPosition = new Vector3(0, 0, 0);
            
            // 护手
            GameObject guard = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            guard.name = "Guard";
            guard.transform.SetParent(parent.transform);
            guard.transform.localScale = new Vector3(0.3f, 0.05f, 0.1f);
            guard.transform.localPosition = new Vector3(0, 0.2f, 0);
            
            // 应用材质
            if (material != null)
            {
                blade.GetComponent<Renderer>().material = material;
                handle.GetComponent<Renderer>().material = material;
                guard.GetComponent<Renderer>().material = material;
            }
            
            Destroy(blade.GetComponent<Collider>());
            Destroy(handle.GetComponent<Collider>());
            Destroy(guard.GetComponent<Collider>());
        }
        
        void CreateSpearMesh(GameObject parent, Material material)
        {
            // 矛杆
            GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shaft.name = "Shaft";
            shaft.transform.SetParent(parent.transform);
            shaft.transform.localScale = new Vector3(0.06f, 2f, 0.06f);
            shaft.transform.localPosition = new Vector3(0, 1f, 0);
            
            // 矛头
            GameObject tip = GameObject.CreatePrimitive(PrimitiveType.Cone);
            tip.name = "Tip";
            tip.transform.SetParent(parent.transform);
            tip.transform.localScale = new Vector3(0.15f, 0.5f, 0.15f);
            tip.transform.localPosition = new Vector3(0, 2.2f, 0);
            
            if (material != null)
            {
                shaft.GetComponent<Renderer>().material = material;
                tip.GetComponent<Renderer>().material = material;
            }
            
            Destroy(shaft.GetComponent<Collider>());
            Destroy(tip.GetComponent<Collider>());
        }
        
        void CreateBowMesh(GameObject parent, Material material)
        {
            // 弓身
            GameObject bowCurve = new GameObject("BowCurve");
            bowCurve.transform.SetParent(parent.transform);
            
            // 创建弧形弓
            for (int i = 0; i < 10; i++)
            {
                float angle = i * 18f;
                GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                segment.transform.SetParent(bowCurve.transform);
                segment.transform.localScale = new Vector3(0.05f, 0.2f, 0.05f);
                segment.transform.localPosition = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * 0.5f, Mathf.Cos(angle * Mathf.Deg2Rad) * 0.8f, 0);
                segment.transform.localRotation = Quaternion.Euler(0, 0, -angle);
                
                if (material != null)
                    segment.GetComponent<Renderer>().material = material;
                
                Destroy(segment.GetComponent<Collider>());
            }
            
            // 弓弦
            GameObject stringLine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stringLine.name = "String";
            stringLine.transform.SetParent(parent.transform);
            stringLine.transform.localScale = new Vector3(0.01f, 1.6f, 0.01f);
            stringLine.transform.localPosition = new Vector3(0, 0.8f, 0);
        }
        
        void CreateAxeMesh(GameObject parent, Material material)
        {
            // 斧柄
            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.name = "Handle";
            handle.transform.SetParent(parent.transform);
            handle.transform.localScale = new Vector3(0.08f, 1.2f, 0.08f);
            handle.transform.localPosition = new Vector3(0, 0.6f, 0);
            
            // 斧刃
            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.name = "Blade";
            blade.transform.SetParent(parent.transform);
            blade.transform.localScale = new Vector3(0.5f, 0.3f, 0.1f);
            blade.transform.localPosition = new Vector3(0, 1.1f, 0);
            
            if (material != null)
            {
                handle.GetComponent<Renderer>().material = material;
                blade.GetComponent<Renderer>().material = material;
            }
            
            Destroy(handle.GetComponent<Collider>());
            Destroy(blade.GetComponent<Collider>());
        }
        
        void CreateStaffMesh(GameObject parent, Material material)
        {
            // 杖身
            GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shaft.name = "Shaft";
            shaft.transform.SetParent(parent.transform);
            shaft.transform.localScale = new Vector3(0.08f, 1.8f, 0.08f);
            shaft.transform.localPosition = new Vector3(0, 0.9f, 0);
            
            // 杖顶宝石
            GameObject gem = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gem.name = "Gem";
            gem.transform.SetParent(parent.transform);
            gem.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            gem.transform.localPosition = new Vector3(0, 1.9f, 0);
            
            if (material != null)
            {
                shaft.GetComponent<Renderer>().material = material;
                gem.GetComponent<Renderer>().material = material;
            }
            
            Destroy(shaft.GetComponent<Collider>());
            Destroy(gem.GetComponent<Collider>());
        }
        
        void SetupAnimator(Animator animator, string heroName)
        {
            // 创建Animator Controller（简化版）
            // 实际项目中应该加载预设的Animator Controller
        }
    }
    
    [System.Serializable]
    public class HeroModelData
    {
        public float height = 1.8f;
        public float shoulderWidth = 0.5f;
        public float hipWidth = 0.4f;
        
        public Vector3 headScale = new Vector3(0.2f, 0.25f, 0.22f);
        public Vector3 torsoScale = new Vector3(0.35f, 0.6f, 0.25f);
        public Vector3 armScale = new Vector3(0.12f, 0.5f, 0.12f);
        public Vector3 legScale = new Vector3(0.15f, 0.7f, 0.15f);
        
        public Material headMaterial;
        public Material torsoMaterial;
        public Material armMaterial;
        public Material legMaterial;
        public Material weaponMaterial;
        
        public bool hasWeapon = true;
        public WeaponType weaponType = WeaponType.Sword;
        public bool hasAura = false;
    }
    
    public enum WeaponType
    {
        Sword,      // 剑
        Spear,      // 矛
        Bow,        // 弓
        Axe,        // 斧
        Staff,      // 法杖
        Dagger,     // 匕首
        Hammer,     // 锤
        Fan,        // 扇子
        Orb         // 宝珠
    }
}
