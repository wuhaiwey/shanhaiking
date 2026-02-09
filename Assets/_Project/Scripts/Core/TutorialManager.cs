using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 新手教程系统
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }
        
        [Header("教程步骤")]
        public List<TutorialStep> tutorialSteps = new List<TutorialStep>();
        
        [Header("当前步骤")]
        public int currentStepIndex = 0;
        public bool isTutorialActive = false;
        
        void Awake()
        {
            Instance = this;
            InitializeTutorial();
        }
        
        void InitializeTutorial()
        {
            tutorialSteps.Add(new TutorialStep
            {
                stepName = "欢迎",
                description = "欢迎来到山海经王者荣耀！",
                instruction = "点击继续开始教程",
                actionRequired = TutorialAction.None
            });
            
            tutorialSteps.Add(new TutorialStep
            {
                stepName = "移动",
                description = "学习移动",
                instruction = "使用WASD或摇杆移动你的英雄",
                actionRequired = TutorialAction.Move
            });
            
            tutorialSteps.Add(new TutorialStep
            {
                stepName = "普通攻击",
                description = "学习攻击",
                instruction = "点击鼠标左键或A键攻击小兵",
                actionRequired = TutorialAction.Attack
            });
            
            tutorialSteps.Add(new TutorialStep
            {
                stepName = "释放技能",
                description = "学习技能",
                instruction = "按Q键释放第一个技能",
                actionRequired = TutorialAction.UseSkill
            });
            
            tutorialSteps.Add(new TutorialStep
            {
                stepName = "购买装备",
                description = "学习购物",
                instruction = "点击商店按钮购买一件装备",
                actionRequired = TutorialAction.BuyItem
            });
            
            tutorialSteps.Add(new TutorialStep
            {
                stepName = "摧毁防御塔",
                description = "学习推塔",
                instruction = "攻击并摧毁敌方防御塔",
                actionRequired = TutorialAction.DestroyTower
            });
            
            tutorialSteps.Add(new TutorialStep
            {
                stepName = "完成",
                description = "教程完成！",
                instruction = "你已完成新手教程，开始真正的战斗吧！",
                actionRequired = TutorialAction.None
            });
        }
        
        public void StartTutorial()
        {
            isTutorialActive = true;
            currentStepIndex = 0;
            ShowCurrentStep();
        }
        
        void ShowCurrentStep()
        {
            if (currentStepIndex < tutorialSteps.Count)
            {
                TutorialStep step = tutorialSteps[currentStepIndex];
                Debug.Log($"教程步骤 {currentStepIndex + 1}: {step.stepName}");
                Debug.Log(step.description);
                Debug.Log(step.instruction);
            }
        }
        
        public void CompleteCurrentStep()
        {
            currentStepIndex++;
            
            if (currentStepIndex >= tutorialSteps.Count)
            {
                EndTutorial();
            }
            else
            {
                ShowCurrentStep();
            }
        }
        
        void EndTutorial()
        {
            isTutorialActive = false;
            Debug.Log("教程完成！");
        }
        
        public void SkipTutorial()
        {
            isTutorialActive = false;
            Debug.Log("跳过教程");
        }
    }
    
    [System.Serializable]
    public class TutorialStep
    {
        public string stepName;
        public string description;
        public string instruction;
        public TutorialAction actionRequired;
        public bool isCompleted;
    }
    
    public enum TutorialAction
    {
        None,
        Move,
        Attack,
        UseSkill,
        BuyItem,
        DestroyTower
    }
}
