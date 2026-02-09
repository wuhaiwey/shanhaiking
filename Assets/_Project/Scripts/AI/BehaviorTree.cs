using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.AI
{
    /// <summary>
    /// AI行为树系统
    /// </summary>
    public class BehaviorTree : MonoBehaviour
    {
        private BTNode rootNode;
        private BTBlackboard blackboard;
        
        void Start()
        {
            blackboard = new BTBlackboard();
            BuildBehaviorTree();
        }
        
        void Update()
        {
            if (rootNode != null)
            {
                rootNode.Execute(blackboard);
            }
        }
        
        void BuildBehaviorTree()
        {
            // 创建选择器节点
            BTSelector selector = new BTSelector();
            
            // 子节点1：低生命值逃跑
            BTSequence fleeSequence = new BTSequence();
            fleeSequence.AddChild(new BTCheckLowHealth());
            fleeSequence.AddChild(new BTFlee());
            selector.AddChild(fleeSequence);
            
            // 子节点2：攻击敌人
            BTSequence attackSequence = new BTSequence();
            attackSequence.AddChild(new BTCheckEnemyInRange());
            attackSequence.AddChild(new BTAttack());
            selector.AddChild(attackSequence);
            
            // 子节点3：巡逻
            selector.AddChild(new BTPatrol());
            
            rootNode = selector;
        }
    }
    
    #region Behavior Tree Nodes
    
    public enum BTNodeState
    {
        Success,
        Failure,
        Running
    }
    
    public abstract class BTNode
    {
        public abstract BTNodeState Execute(BTBlackboard blackboard);
    }
    
    public class BTSelector : BTNode
    {
        private List<BTNode> children = new List<BTNode>();
        
        public void AddChild(BTNode child)
        {
            children.Add(child);
        }
        
        public override BTNodeState Execute(BTBlackboard blackboard)
        {
            foreach (BTNode child in children)
            {
                BTNodeState state = child.Execute(blackboard);
                if (state == BTNodeState.Success)
                    return BTNodeState.Success;
                if (state == BTNodeState.Running)
                    return BTNodeState.Running;
            }
            return BTNodeState.Failure;
        }
    }
    
    public class BTSequence : BTNode
    {
        private List<BTNode> children = new List<BTNode>();
        
        public void AddChild(BTNode child)
        {
            children.Add(child);
        }
        
        public override BTNodeState Execute(BTBlackboard blackboard)
        {
            foreach (BTNode child in children)
            {
                BTNodeState state = child.Execute(blackboard);
                if (state == BTNodeState.Failure)
                    return BTNodeState.Failure;
                if (state == BTNodeState.Running)
                    return BTNodeState.Running;
            }
            return BTNodeState.Success;
        }
    }
    
    public class BTBlackboard
    {
        public Dictionary<string, object> data = new Dictionary<string, object>();
        
        public void SetValue(string key, object value)
        {
            data[key] = value;
        }
        
        public T GetValue<T>(string key)
        {
            if (data.ContainsKey(key))
                return (T)data[key];
            return default(T);
        }
    }
    
    #endregion
    
    #region Specific Nodes
    
    public class BTCheckLowHealth : BTNode
    {
        public override BTNodeState Execute(BTBlackboard blackboard)
        {
            Hero.HeroBase hero = blackboard.GetValue<Hero.HeroBase>("hero");
            if (hero != null && hero.CurrentHealth / hero.MaxHealth < 0.3f)
                return BTNodeState.Success;
            return BTNodeState.Failure;
        }
    }
    
    public class BTFlee : BTNode
    {
        public override BTNodeState Execute(BTBlackboard blackboard)
        {
            // 逃跑逻辑
            Debug.Log("AI: 逃跑中...");
            return BTNodeState.Success;
        }
    }
    
    public class BTCheckEnemyInRange : BTNode
    {
        public override BTNodeState Execute(BTBlackboard blackboard)
        {
            // 检查范围内是否有敌人
            return BTNodeState.Success;
        }
    }
    
    public class BTAttack : BTNode
    {
        public override BTNodeState Execute(BTBlackboard blackboard)
        {
            // 攻击逻辑
            Debug.Log("AI: 攻击敌人");
            return BTNodeState.Success;
        }
    }
    
    public class BTPatrol : BTNode
    {
        public override BTNodeState Execute(BTBlackboard blackboard)
        {
            // 巡逻逻辑
            return BTNodeState.Success;
        }
    }
    
    #endregion
}
