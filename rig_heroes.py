import bpy
import os
from mathutils import Vector, Quaternion
from math import radians

# 输出目录
OUTPUT_DIR = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/AI_Generated_Rigged"
os.makedirs(OUTPUT_DIR, exist_ok=True)

# 清理场景
def clear_scene():
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete(use_global=False)
    
    # 清除所有数据块
    for mesh in bpy.data.meshes:
        if mesh.users == 0:
            bpy.data.meshes.remove(mesh)
    
    for armature in bpy.data.armatures:
        if armature.users == 0:
            bpy.data.armatures.remove(armature)
    
    print("✅ 场景已清理")

# 导入GLB模型
def import_glb(filepath):
    print(f"📥 导入模型: {filepath}")
    bpy.ops.import_scene.gltf(filepath=filepath)
    
    # 获取导入的模型
    imported_objects = [obj for obj in bpy.context.selected_objects if obj.type == 'MESH']
    
    if not imported_objects:
        print("❌ 未找到网格对象")
        return None
    
    # 合并所有网格
    if len(imported_objects) > 1:
        bpy.context.view_layer.objects.active = imported_objects[0]
        bpy.ops.object.join()
    
    model = imported_objects[0]
    model.name = "Hero_Mesh"
    print(f"✅ 导入完成: {model.name}")
    return model

# 创建骨骼系统
def create_armature(model, hero_type="warrior"):
    print(f"🦴 创建骨骼系统 ({hero_type})...")
    
    # 创建骨架对象
    bpy.ops.object.armature_add(location=(0, 0, 0))
    armature = bpy.context.active_object
    armature.name = "Hero_Armature"
    
    # 进入编辑模式
    bpy.context.view_layer.objects.active = armature
    bpy.ops.object.mode_set(mode='EDIT')
    
    # 获取骨骼编辑数据
    edit_bones = armature.data.edit_bones
    
    # 清除默认骨骼
    for bone in edit_bones:
        edit_bones.remove(bone)
    
    # 根据英雄类型创建骨骼结构
    if hero_type == "warrior":  # 刑天、共工
        create_warrior_rig(edit_bones)
    elif hero_type == "mage":  # 九尾狐、女娲
        create_mage_rig(edit_bones)
    elif hero_type == "archer":  # 后羿
        create_archer_rig(edit_bones)
    else:
        create_humanoid_rig(edit_bones)
    
    # 返回物体模式
    bpy.ops.object.mode_set(mode='OBJECT')
    
    print(f"✅ 骨骼系统创建完成")
    return armature

# 创建战士型骨骼（刑天、共工）
def create_warrior_rig(edit_bones):
    # 根骨骼
    root = edit_bones.new("Root")
    root.head = (0, 0, 0)
    root.tail = (0, 0, 0.1)
    
    # 骨盆
    pelvis = edit_bones.new("Pelvis")
    pelvis.head = (0, 0, 0.9)
    pelvis.tail = (0, 0, 1.1)
    pelvis.parent = root
    
    # 脊柱
    spine = edit_bones.new("Spine")
    spine.head = (0, 0, 1.1)
    spine.tail = (0, 0, 1.4)
    spine.parent = pelvis
    
    spine1 = edit_bones.new("Spine1")
    spine1.head = (0, 0, 1.4)
    spine1.tail = (0, 0, 1.6)
    spine1.parent = spine
    
    # 胸部（刑天的眼睛在这里）
    chest = edit_bones.new("Chest")
    chest.head = (0, 0, 1.6)
    chest.tail = (0, 0, 1.8)
    chest.parent = spine1
    
    # 头部（刑天可能很小或没有）
    head = edit_bones.new("Head")
    head.head = (0, 0, 1.8)
    head.tail = (0, 0, 2.0)
    head.parent = chest
    
    # 脖子
    neck = edit_bones.new("Neck")
    neck.head = (0, 0, 1.8)
    neck.tail = (0, 0, 1.9)
    neck.parent = chest
    
    # 左臂
    shoulder_L = edit_bones.new("Shoulder_L")
    shoulder_L.head = (0.3, 0, 1.75)
    shoulder_L.tail = (0.5, 0, 1.75)
    shoulder_L.parent = chest
    
    upperarm_L = edit_bones.new("UpperArm_L")
    upperarm_L.head = (0.5, 0, 1.75)
    upperarm_L.tail = (0.7, 0, 1.5)
    upperarm_L.parent = shoulder_L
    
    forearm_L = edit_bones.new("ForeArm_L")
    forearm_L.head = (0.7, 0, 1.5)
    forearm_L.tail = (0.9, 0, 1.3)
    forearm_L.parent = upperarm_L
    
    hand_L = edit_bones.new("Hand_L")
    hand_L.head = (0.9, 0, 1.3)
    hand_L.tail = (1.0, 0, 1.2)
    hand_L.parent = forearm_L
    
    # 右臂（镜像）
    shoulder_R = edit_bones.new("Shoulder_R")
    shoulder_R.head = (-0.3, 0, 1.75)
    shoulder_R.tail = (-0.5, 0, 1.75)
    shoulder_R.parent = chest
    
    upperarm_R = edit_bones.new("UpperArm_R")
    upperarm_R.head = (-0.5, 0, 1.75)
    upperarm_R.tail = (-0.7, 0, 1.5)
    upperarm_R.parent = shoulder_R
    
    forearm_R = edit_bones.new("ForeArm_R")
    forearm_R.head = (-0.7, 0, 1.5)
    forearm_R.tail = (-0.9, 0, 1.3)
    forearm_R.parent = upperarm_R
    
    hand_R = edit_bones.new("Hand_R")
    hand_R.head = (-0.9, 0, 1.3)
    hand_R.tail = (-1.0, 0, 1.2)
    hand_R.parent = forearm_R
    
    # 左腿
    thigh_L = edit_bones.new("Thigh_L")
    thigh_L.head = (0.15, 0, 0.9)
    thigh_L.tail = (0.15, 0, 0.5)
    thigh_L.parent = pelvis
    
    calf_L = edit_bones.new("Calf_L")
    calf_L.head = (0.15, 0, 0.5)
    calf_L.tail = (0.15, 0, 0.1)
    calf_L.parent = thigh_L
    
    foot_L = edit_bones.new("Foot_L")
    foot_L.head = (0.15, 0, 0.1)
    foot_L.tail = (0.15, 0.1, 0)
    foot_L.parent = calf_L
    
    # 右腿
    thigh_R = edit_bones.new("Thigh_R")
    thigh_R.head = (-0.15, 0, 0.9)
    thigh_R.tail = (-0.15, 0, 0.5)
    thigh_R.parent = pelvis
    
    calf_R = edit_bones.new("Calf_R")
    calf_R.head = (-0.15, 0, 0.5)
    calf_R.tail = (-0.15, 0, 0.1)
    calf_R.parent = thigh_R
    
    foot_R = edit_bones.new("Foot_R")
    foot_R.head = (-0.15, 0, 0.1)
    foot_R.tail = (-0.15, 0.1, 0)
    foot_R.parent = calf_R
    
    # 武器骨骼（斧头/盾牌）
    weapon_L = edit_bones.new("Weapon_L")
    weapon_L.head = (1.0, 0, 1.2)
    weapon_L.tail = (1.2, 0, 1.4)
    weapon_L.parent = hand_L
    
    weapon_R = edit_bones.new("Weapon_R")
    weapon_R.head = (-1.0, 0, 1.2)
    weapon_R.tail = (-1.2, 0, 1.0)
    weapon_R.parent = hand_R

# 创建法师型骨骼
def create_mage_rig(edit_bones):
    # 类似战士但更优雅
    create_warrior_rig(edit_bones)  # 复用基础结构
    # 可以添加特殊骨骼如尾巴等

# 创建射手型骨骼
def create_archer_rig(edit_bones):
    create_warrior_rig(edit_bones)  # 复用基础结构

# 创建通用型骨骼
def create_humanoid_rig(edit_bones):
    create_warrior_rig(edit_bones)

# 绑定网格到骨骼
def bind_mesh_to_armature(mesh, armature):
    print("🔗 绑定网格到骨骼...")
    
    # 选择网格和骨骼
    bpy.ops.object.select_all(action='DESELECT')
    mesh.select_set(True)
    armature.select_set(True)
    bpy.context.view_layer.objects.active = armature
    
    # 设置父级（自动权重）
    bpy.ops.object.parent_set(type='ARMATURE_AUTO')
    
    print("✅ 绑定完成")

# 创建基础动画
def create_basic_animations(armature, hero_name="Hero"):
    print("🎬 创建基础动画...")
    
    # 确保在物体模式
    bpy.ops.object.mode_set(mode='OBJECT')
    bpy.context.view_layer.objects.active = armature
    
    # 进入姿态模式
    bpy.ops.object.mode_set(mode='POSE')
    
    # 1. 待机动画 (Idle)
    print("  📍 创建待机动画...")
    create_idle_animation(armature, hero_name)
    
    # 2. 行走动画 (Walk)
    print("  🚶 创建行走动画...")
    create_walk_animation(armature, hero_name)
    
    # 3. 攻击动画 (Attack)
    print("  ⚔️ 创建攻击动画...")
    create_attack_animation(armature, hero_name)
    
    # 4. 技能动画 (Skill)
    print("  ✨ 创建技能动画...")
    create_skill_animation(armature, hero_name)
    
    bpy.ops.object.mode_set(mode='OBJECT')
    print("✅ 动画创建完成")

# 待机动画
def create_idle_animation(armature, hero_name):
    action_name = f"{hero_name}_Idle"
    
    # 创建新动作
    action = bpy.data.actions.new(name=action_name)
    armature.animation_data_create()
    armature.animation_data.action = action
    
    # 设置关键帧
    frame = 1
    bpy.context.scene.frame_set(frame)
    
    # 呼吸效果 - 轻微上下移动胸部
    if "Chest" in armature.pose.bones:
        chest = armature.pose.bones["Chest"]
        chest.location = (0, 0, 0)
        chest.keyframe_insert(data_path="location", frame=frame)
        
        frame = 30
        bpy.context.scene.frame_set(frame)
        chest.location = (0, 0, 0.02)
        chest.keyframe_insert(data_path="location", frame=frame)
        
        frame = 60
        bpy.context.scene.frame_set(frame)
        chest.location = (0, 0, 0)
        chest.keyframe_insert(data_path="location", frame=frame)
    
    # 设置循环
    action.use_fake_user = True
    print(f"    ✅ {action_name} (60帧)")

# 行走动画
def create_walk_animation(armature, hero_name):
    action_name = f"{hero_name}_Walk"
    action = bpy.data.actions.new(name=action_name)
    
    # 简单的行走循环
    frame = 1
    bpy.context.scene.frame_set(frame)
    
    # 腿部摆动
    if "Thigh_L" in armature.pose.bones and "Thigh_R" in armature.pose.bones:
        thigh_L = armature.pose.bones["Thigh_L"]
        thigh_R = armature.pose.bones["Thigh_R"]
        
        # 左腿向前
        thigh_L.rotation_euler = (radians(30), 0, 0)
        thigh_L.keyframe_insert(data_path="rotation_euler", frame=1)
        
        thigh_R.rotation_euler = (radians(-30), 0, 0)
        thigh_R.keyframe_insert(data_path="rotation_euler", frame=1)
        
        # 交换
        thigh_L.rotation_euler = (radians(-30), 0, 0)
        thigh_L.keyframe_insert(data_path="rotation_euler", frame=30)
        
        thigh_R.rotation_euler = (radians(30), 0, 0)
        thigh_R.keyframe_insert(data_path="rotation_euler", frame=30)
        
        # 回到起始
        thigh_L.rotation_euler = (radians(30), 0, 0)
        thigh_L.keyframe_insert(data_path="rotation_euler", frame=60)
        
        thigh_R.rotation_euler = (radians(-30), 0, 0)
        thigh_R.keyframe_insert(data_path="rotation_euler", frame=60)
    
    action.use_fake_user = True
    print(f"    ✅ {action_name} (60帧)")

# 攻击动画
def create_attack_animation(armature, hero_name):
    action_name = f"{hero_name}_Attack"
    action = bpy.data.actions.new(name=action_name)
    
    frame = 1
    bpy.context.scene.frame_set(frame)
    
    # 右臂攻击动作
    if "UpperArm_R" in armature.pose.bones:
        upperarm_R = armature.pose.bones["UpperArm_R"]
        
        # 准备动作
        upperarm_R.rotation_euler = (0, 0, radians(-45))
        upperarm_R.keyframe_insert(data_path="rotation_euler", frame=1)
        
        # 攻击动作
        upperarm_R.rotation_euler = (0, 0, radians(45))
        upperarm_R.keyframe_insert(data_path="rotation_euler", frame=15)
        
        # 收回
        upperarm_R.rotation_euler = (0, 0, 0)
        upperarm_R.keyframe_insert(data_path="rotation_euler", frame=30)
    
    action.use_fake_user = True
    print(f"    ✅ {action_name} (30帧)")

# 技能动画
def create_skill_animation(armature, hero_name):
    action_name = f"{hero_name}_Skill"
    action = bpy.data.actions.new(name=action_name)
    
    frame = 1
    bpy.context.scene.frame_set(frame)
    
    # 举高武器施法动作
    if "UpperArm_L" in armature.pose.bones and "UpperArm_R" in armature.pose.bones:
        upperarm_L = armature.pose.bones["UpperArm_L"]
        upperarm_R = armature.pose.bones["UpperArm_R"]
        
        # 起始
        upperarm_L.rotation_euler = (0, 0, 0)
        upperarm_R.rotation_euler = (0, 0, 0)
        upperarm_L.keyframe_insert(data_path="rotation_euler", frame=1)
        upperarm_R.keyframe_insert(data_path="rotation_euler", frame=1)
        
        # 举起
        upperarm_L.rotation_euler = (0, 0, radians(90))
        upperarm_R.rotation_euler = (0, 0, radians(-90))
        upperarm_L.keyframe_insert(data_path="rotation_euler", frame=20)
        upperarm_R.keyframe_insert(data_path="rotation_euler", frame=20)
        
        # 释放
        upperarm_L.rotation_euler = (0, 0, 0)
        upperarm_R.rotation_euler = (0, 0, 0)
        upperarm_L.keyframe_insert(data_path="rotation_euler", frame=60)
        upperarm_R.keyframe_insert(data_path="rotation_euler", frame=60)
    
    action.use_fake_user = True
    print(f"    ✅ {action_name} (60帧)")

# 导出带骨骼和动画的模型
def export_rigged_model(armature, mesh, hero_name):
    print(f"📤 导出绑定模型: {hero_name}")
    
    # 选择要导出的对象
    bpy.ops.object.select_all(action='DESELECT')
    armature.select_set(True)
    mesh.select_set(True)
    bpy.context.view_layer.objects.active = armature
    
    # 导出FBX（支持骨骼和动画）
    output_path = os.path.join(OUTPUT_DIR, f"{hero_name}_Rigged.fbx")
    
    bpy.ops.export_scene.fbx(
        filepath=output_path,
        use_selection=True,
        add_leaf_bones=False,
        bake_anim=True,
        bake_anim_use_all_bones=True,
        bake_anim_use_nla_strips=False,
        bake_anim_use_all_actions=True,
        mesh_smooth_type='FACE',
        use_mesh_modifiers=True,
        use_armature_deform_only=True
    )
    
    print(f"✅ 导出完成: {output_path}")
    return output_path

# 主流程
def rig_hero_model(input_path, hero_name, hero_type="warrior"):
    print(f"\n{'='*60}")
    print(f"🎭 为 {hero_name} 添加骨骼绑定和动画")
    print(f"{'='*60}")
    
    # 1. 清理场景
    clear_scene()
    
    # 2. 导入模型
    mesh = import_glb(input_path)
    if not mesh:
        return False
    
    # 3. 创建骨骼
    armature = create_armature(mesh, hero_type)
    
    # 4. 绑定网格
    bind_mesh_to_armature(mesh, armature)
    
    # 5. 创建动画
    create_basic_animations(armature, hero_name)
    
    # 6. 导出
    output_path = export_rigged_model(armature, mesh, hero_name)
    
    print(f"\n✅ {hero_name} 骨骼绑定完成！")
    print(f"📁 输出: {output_path}")
    
    return True

# 批量处理所有英雄
if __name__ == "__main__":
    print("="*60)
    print("🦴 山海经英雄骨骼绑定工具")
    print("="*60)
    
    heroes = [
        ("Hero_XingTian_AI.glb", "XingTian", "warrior"),
        ("Hero_NuWa_AI.glb", "NuWa", "mage"),
        ("Hero_JiuWeiHu_AI.glb", "JiuWeiHu", "mage"),
        ("Hero_HouYi_AI.glb", "HouYi", "archer"),
        ("Hero_GongGong_AI.glb", "GongGong", "warrior")
    ]
    
    input_dir = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/AI_Generated"
    
    for filename, hero_name, hero_type in heroes:
        input_path = os.path.join(input_dir, filename)
        if os.path.exists(input_path):
            rig_hero_model(input_path, hero_name, hero_type)
        else:
            print(f"⚠️ 跳过: {filename} 不存在")
    
    print("\n" + "="*60)
    print("✅ 所有英雄骨骼绑定完成！")
    print("="*60)
