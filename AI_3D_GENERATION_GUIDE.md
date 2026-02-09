# 🎨 AI生成3D模型完整指南

## 方案B：使用AI工具生成3D模型

### 推荐的AI 3D生成工具

#### 1️⃣ **Meshy.ai** ⭐ 首选
- **网址**: https://www.meshy.ai/
- **特点**: 
  - 文字/图片生成3D
  - 支持多种风格（写实/卡通/低多边形）
  - 免费额度：每月200积分
  - 生成时间：1-5分钟

#### 2️⃣ **CSM.ai** (Common Sense Machines)
- **网址**: https://csm.ai/
- **特点**:
  - 图片转3D效果最佳
  - 支持视频转3D
  - 免费试用

#### 3️⃣ **Tripo3D**
- **网址**: https://www.tripo3d.ai/
- **特点**:
  - 文字/图片生成
  - 快速生成
  - 中文支持好

#### 4️⃣ **Spline AI**
- **网址**: https://spline.design/
- **特点**:
  - 在线3D设计工具
  - AI辅助建模
  - 可直接导出到Unity

---

## 🚀 快速开始 - Meshy.ai

### 步骤1：注册账号

1. 访问 https://www.meshy.ai/
2. 点击 "Get Started" 或 "Sign Up"
3. 使用邮箱/Google账号注册
4. 验证邮箱

### 步骤2：获取API密钥

1. 登录后点击右上角头像
2. 选择 "API Keys"
3. 点击 "Create New API Key"
4. 复制密钥（保存好，只显示一次）

### 步骤3：设置环境变量

```bash
# 在终端中执行
export MESHY_API_KEY="your_api_key_here"

# 验证
echo $MESHY_API_KEY
```

### 步骤4：运行生成脚本

```bash
cd /Users/mili/Desktop/ShanHaiKing

# 生成刑天
python3 ai_generate_3d.py xingtian

# 生成九尾狐
python3 ai_generate_3d.py jiuweihu

# 生成后羿
python3 ai_generate_3d.py houyi
```

---

## 📝 生成提示词示例

### 刑天（战神）
```
Chinese mythological warrior Xing Tian from Shan Hai Jing, 
headless giant with eyes on chest and mouth on belly, 
wielding axe and shield, fierce expression, 
ancient Chinese armor, muscular body, 
fantasy art style, detailed textures, 
game character design, full body, standing pose
```

### 九尾狐
```
Nine-tailed fox spirit from Chinese mythology, 
beautiful female fox demon with nine fluffy tails, 
elegant flowing robes, mystical aura, 
pink and white color scheme, 
anime style, detailed fur, 
fantasy character design, full body, graceful pose
```

### 后羿
```
Hou Yi the archer from Chinese mythology, 
handsome male warrior with divine bow, 
ancient Chinese hunter attire, 
muscular build, confident stance, 
sun motif decorations, 
fantasy game character style, 
detailed textures, full body, hero pose
```

---

## 🎨 手动使用Meshy.ai网页版

如果不想使用脚本，可以直接在网页上操作：

### 文字生成3D
1. 进入 https://www.meshy.ai/text-to-3d
2. 在输入框粘贴提示词
3. 选择风格（推荐 "Stylized" 或 "Realistic"）
4. 点击 "Generate"
5. 等待1-5分钟
6. 预览并下载（格式：GLB/OBJ/FBX）

### 图片生成3D
1. 准备角色概念图（正面/侧面）
2. 进入 https://www.meshy.ai/image-to-3d
3. 上传图片
4. 点击 "Generate"
5. 等待生成完成
6. 下载模型

---

## 📦 导出设置

### 推荐格式
- **GLB/GLTF**: 推荐，Unity原生支持
- **FBX**: 通用格式，需要转换
- **OBJ**: 基础格式，带材质

### Unity导入设置
1. 将下载的模型拖入 `Assets/_Project/Models/`
2. 选中模型，在Inspector中设置：
   - Scale Factor: 1
   - Convert Units: 勾选
   - Import Materials: 勾选
   - Material Naming: By Base Texture Name

---

## 💰 费用说明

### Meshy.ai 免费版
- 每月200积分
- 文字生成3D：约10积分/次
- 图片生成3D：约20积分/次
- 可生成约10-20个模型/月

### 付费版
- Pro: $16/月
- 更多积分和高级功能

---

## 🎯 工作流程建议

### 阶段1：快速原型（现在）
1. 使用AI生成基础模型
2. 放入Unity测试玩法
3. 使用占位材质

### 阶段2：精细化（后期）
1. 在Blender中优化AI模型
2. 重拓扑和UV展开
3. 手绘贴图
4. 骨骼绑定

### 阶段3：最终资产
1. 导入Unity
2. 设置材质和动画
3. 优化性能

---

## ⚠️ 注意事项

1. **AI生成的模型需要优化**
   - 面数可能过高
   - 需要清理和重拓扑
   - UV可能需要调整

2. **版权问题**
   - Meshy.ai生成的模型可用于商业项目
   - 建议查看最新的使用条款

3. **质量控制**
   - AI生成结果不稳定
   - 可能需要多次尝试
   - 复杂角色可能需要分部分生成

---

## 🔧 替代方案

如果Meshy.ai不满意，可以尝试：

1. **购买现成资产**
   - Unity Asset Store
   - Sketchfab Store
   - CGTrader

2. **使用Mixamo角色**
   - Adobe免费角色库
   - 自动骨骼绑定
   - 可直接使用

3. **Blender + AI辅助**
   - Blender的AI插件
   - 手工优化AI生成结果

---

## 📞 需要帮助？

Meshy.ai支持：
- Discord社区: https://discord.gg/meshy
- 文档: https://docs.meshy.ai/

---

**下一步：请注册Meshy.ai账号，获取API密钥，然后运行生成脚本！**
