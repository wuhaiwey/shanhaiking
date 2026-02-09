# 🎮 山海经王者荣耀 - Unity 项目

## 📂 项目位置
`/Users/mili/Desktop/ShanHaiKing/`

## 🚀 如何打开项目

### 方法1：通过 Unity Hub（推荐）
1. 打开 **Unity Hub**（在应用程序中）
2. 点击左侧的 **"Projects"**
3. 点击右上角的 **"Open"** 按钮
4. 选择文件夹：`/Users/mili/Desktop/ShanHaiKing`
5. 点击 **"Open"**
6. Unity 会自动加载项目

### 方法2：直接打开
1. 在 Finder 中进入 `/Users/mili/Desktop/ShanHaiKing`
2. 双击 `Assets` 文件夹（Unity 项目图标）

---

## 🎯 首次运行步骤

### 1. 等待项目加载
Unity 第一次打开项目时会：
- 导入资源（约1-2分钟）
- 编译脚本
- 生成 Library 文件夹

### 2. 打开主场景
1. 在 Project 窗口中找到 `Assets/_Project/Scenes/`
2. 双击 `MainScene.unity`

### 3. 运行游戏
- 点击顶部中间的 **Play** 按钮（▶️）
- 或者按 **Cmd+P**

---

## 🕹️ 游戏操作

### 移动
- **WASD** 或 **方向键**：移动英雄
- **虚拟摇杆**：移动端触摸控制

### 技能
- **Q**：技能1（连珠箭）
- **W**：技能2（穿云箭）
- **E**：技能3（逐日步）
- **R**：大招（射日）
- **鼠标左键**：普通攻击/选择目标

### 摄像机
- **鼠标滚轮**：缩放视角
- **双指缩放**：移动端缩放

---

## 📁 代码结构

```
Assets/_Project/Scripts/
├── Core/               # 核心系统
│   ├── GameManager.cs      # 游戏管理器
│   ├── PlayerController.cs # 玩家输入控制
│   ├── VirtualJoystick.cs  # 虚拟摇杆
│   ├── GameCamera.cs       # 摄像机控制
│   └── HeroStats.cs        # 英雄属性
├── Hero/               # 英雄系统
│   ├── HeroBase.cs         # 英雄基类
│   └── Hero_HouYi.cs       # 后羿英雄
└── Skill/              # 技能系统
    └── SkillBase.cs        # 技能基类
```

---

## 🐉 已实现功能

### ✅ 核心系统
- [x] 英雄基类系统
- [x] 属性成长系统
- [x] 技能系统框架
- [x] 伤害计算系统
- [x] 移动控制系统
- [x] 摄像机跟随
- [x] 虚拟摇杆

### ✅ 英雄
- [x] 后羿（射手）
  - [x] Q技能：连珠箭
  - [x] W技能：穿云箭
  - [x] E技能：逐日步
  - [x] R技能：射日

### 🔄 待开发
- [ ] 更多英雄（九尾狐、刑天等）
- [ ] 小兵的AI
- [ ] 防御塔
- [ ] 地图场景
- [ ] UI界面
- [ ] 网络联机
- [ ] 音效系统

---

## 🛠️ 开发计划

### Week 1: 基础框架 ✅
- [x] 项目创建
- [x] 核心系统搭建
- [x] 第一个英雄（后羿）

### Week 2: 战斗系统
- [ ] 小兵AI
- [ ] 防御塔
- [ ] 野区怪物
- [ ] 兵线系统

### Week 3: 地图与UI
- [ ] 昆仑仙境地图
- [ ] 血条UI
- [ ] 小地图
- [ ] 技能面板

### Week 4: 更多英雄
- [ ] 九尾狐
- [ ] 刑天
- [ ] 女娲

### Week 5-6: 网络联机
- [ ] Photon PUN2集成
- [ ] 房间系统
- [ ] 同步机制

---

## 💡 添加新英雄

以创建"九尾狐"为例：

```csharp
using UnityEngine;

namespace ShanHaiKing.Hero
{
    public class Hero_JiuWeiHu : HeroBase
    {
        protected override void Awake()
        {
            base.Awake();
            heroName = "九尾狐";
            heroType = HeroType.Assassin;
            // 设置属性...
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_JiuWeiHu_Q>();
            // ...
        }
    }
}
```

---

## 🎨 美术资源路径

```
Assets/_Project/Art/
├── Models/         # 3D模型
├── Textures/       # 贴图
├── Animations/     # 动画
├── Effects/        # 特效
└── UI/             # UI资源
```

---

## 🐛 常见问题

### Q: 打开项目时提示版本不匹配？
A: Unity 会提示升级/降级，选择 **"Continue"** 即可。

### Q: 脚本编译错误？
A: 检查 Unity Console 窗口的错误信息，通常是因为：
- 类名和文件名不匹配
- 缺少分号
- 命名空间错误

### Q: 如何添加自己的英雄？
A: 参考 `Hero_HouYi.cs`，创建新类继承 `HeroBase`。

---

## 📞 技术支持

如有问题，告诉我：
1. 错误信息截图
2. 复现步骤
3. Unity版本号

---

**现在请打开 Unity 项目开始游戏开发吧！** 🚀
