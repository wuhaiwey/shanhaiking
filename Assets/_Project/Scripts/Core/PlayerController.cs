using UnityEngine;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 玩家输入控制器
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("控制的英雄")]
        public Hero.HeroBase controlledHero;
        
        [Header("移动控制")]
        public VirtualJoystick moveJoystick;
        
        [Header("技能按钮")]
        public UnityEngine.UI.Button skillQButton;
        public UnityEngine.UI.Button skillWButton;
        public UnityEngine.UI.Button skillEButton;
        public UnityEngine.UI.Button skillRButton;
        public UnityEngine.UI.Button attackButton;
        
        [Header("设置")]
        public LayerMask groundLayer;
        public LayerMask heroLayer;
        
        private Camera mainCamera;
        private Hero.HeroBase selectedTarget;
        
        void Start()
        {
            mainCamera = Camera.main;
            
            // 绑定技能按钮
            if (skillQButton != null)
                skillQButton.onClick.AddListener(() => CastSkill(0));
            if (skillWButton != null)
                skillWButton.onClick.AddListener(() => CastSkill(1));
            if (skillEButton != null)
                skillEButton.onClick.AddListener(() => CastSkill(2));
            if (skillRButton != null)
                skillRButton.onClick.AddListener(() => CastSkill(3));
            if (attackButton != null)
                attackButton.onClick.AddListener(BasicAttack);
        }
        
        void Update()
        {
            if (controlledHero == null || controlledHero.isDead) return;
            
            // 摇杆移动
            HandleMovement();
            
            // 鼠标/触摸选择目标
            HandleTargetSelection();
            
            // 键盘快捷键
            HandleKeyboardInput();
        }
        
        void HandleMovement()
        {
            if (moveJoystick != null && moveJoystick.isDragging)
            {
                Vector3 moveDirection = new Vector3(
                    moveJoystick.inputDirection.x,
                    0,
                    moveJoystick.inputDirection.y
                );
                
                controlledHero.Move(moveDirection);
            }
        }
        
        void HandleTargetSelection()
        {
            // 点击选择目标
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit, 100f, heroLayer))
                {
                    Hero.HeroBase hero = hit.collider.GetComponent<Hero.HeroBase>();
                    if (hero != null && hero != controlledHero)
                    {
                        selectedTarget = hero;
                        // TODO: 显示选中标记
                    }
                }
            }
        }
        
        void HandleKeyboardInput()
        {
            // QWER技能快捷键
            if (Input.GetKeyDown(KeyCode.Q)) CastSkill(0);
            if (Input.GetKeyDown(KeyCode.W)) CastSkill(1);
            if (Input.GetKeyDown(KeyCode.E)) CastSkill(2);
            if (Input.GetKeyDown(KeyCode.R)) CastSkill(3);
            
            // A键普通攻击
            if (Input.GetKeyDown(KeyCode.A)) BasicAttack();
        }
        
        void CastSkill(int skillIndex)
        {
            if (controlledHero == null) return;
            
            // 获取鼠标位置作为目标
            Vector3 targetPosition = GetMouseWorldPosition();
            
            controlledHero.CastSkill(skillIndex, targetPosition, selectedTarget);
        }
        
        void BasicAttack()
        {
            if (controlledHero == null) return;
            
            if (selectedTarget != null)
            {
                controlledHero.BasicAttack(selectedTarget);
            }
        }
        
        Vector3 GetMouseWorldPosition()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 100f, groundLayer))
            {
                return hit.point;
            }
            
            return controlledHero.transform.position;
        }
    }
}
