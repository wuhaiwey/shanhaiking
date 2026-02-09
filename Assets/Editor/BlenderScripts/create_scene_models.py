import bpy
import os

# 清除现有对象
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

output_dir = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/"
os.makedirs(output_dir, exist_ok=True)

def create_material(name, color, metallic=0.0, roughness=0.5, emit=0.0):
    """创建材质 - 简化版兼容Blender 5.0"""
    mat = bpy.data.materials.new(name=name)
    # 使用旧版材质系统更稳定
    mat.diffuse_color = (*color, 1.0)
    mat.metallic = metallic
    mat.roughness = roughness
    return mat

def create_defense_tower():
    """创建防御塔模型"""
    
    # 塔基
    bpy.ops.mesh.primitive_cylinder_add(radius=2, depth=1, location=(0, 0, 0.5))
    base = bpy.context.active_object
    base.name = "Tower_Base"
    
    # 塔身
    bpy.ops.mesh.primitive_cylinder_add(radius=1.2, depth=4, location=(0, 0, 3))
    body = bpy.context.active_object
    body.name = "Tower_Body"
    
    # 塔顶
    bpy.ops.mesh.primitive_cone_add(radius1=1.5, radius2=0, depth=1.5, location=(0, 0, 5.75))
    roof = bpy.context.active_object
    roof.name = "Tower_Roof"
    
    # 装饰环
    bpy.ops.mesh.primitive_torus_add(major_radius=1.3, minor_radius=0.1, location=(0, 0, 4.5))
    ring = bpy.context.active_object
    ring.name = "Tower_Ring"
    
    # 材质
    stone_color = (0.6, 0.6, 0.65)
    roof_color = (0.8, 0.2, 0.2)
    
    stone_mat = create_material("Tower_Stone", stone_color, metallic=0.1, roughness=0.9)
    roof_mat = create_material("Tower_Roof", roof_color, metallic=0.3, roughness=0.6)
    
    base.data.materials.append(stone_mat)
    body.data.materials.append(stone_mat)
    roof.data.materials.append(roof_mat)
    ring.data.materials.append(stone_mat)
    
    print("✅ 防御塔模型创建完成")

def create_minion():
    """创建小兵模型"""
    
    # 身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.3, depth=0.8, location=(0, 0, 0.8))
    body = bpy.context.active_object
    body.name = "Minion_Body"
    
    # 头
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.25, location=(0, 0, 1.5))
    head = bpy.context.active_object
    head.name = "Minion_Head"
    
    # 武器（剑）
    bpy.ops.mesh.primitive_cube_add(size=0.05, location=(0.5, 0.3, 1.0))
    sword_blade = bpy.context.active_object
    sword_blade.name = "Minion_Sword"
    sword_blade.scale = (0.1, 0.8, 0.02)
    
    # 材质
    armor_color = (0.3, 0.4, 0.6)
    skin_color = (0.9, 0.8, 0.7)
    
    armor_mat = create_material("Minion_Armor", armor_color)
    skin_mat = create_material("Minion_Skin", skin_color)
    
    body.data.materials.append(armor_mat)
    head.data.materials.append(skin_mat)
    sword_blade.data.materials.append(create_material("Steel", (0.7, 0.7, 0.75), metallic=0.8))
    
    print("✅ 小兵模型创建完成")

def create_nexus():
    """创建基地水晶模型"""
    
    # 底座
    bpy.ops.mesh.primitive_cylinder_add(radius=3, depth=1, location=(0, 0, 0.5))
    base = bpy.context.active_object
    base.name = "Nexus_Base"
    
    # 水晶主体
    bpy.ops.mesh.primitive_ico_sphere_add(radius=1.5, location=(0, 0, 3))
    crystal = bpy.context.active_object
    crystal.name = "Nexus_Crystal"
    
    # 能量环
    for i in range(3):
        bpy.ops.mesh.primitive_torus_add(
            major_radius=2 + i * 0.3, 
            minor_radius=0.1, 
            location=(0, 0, 2 + i * 1.5)
        )
        ring = bpy.context.active_object
        ring.name = f"Nexus_Ring_{i}"
        ring.rotation_euler = (1.57, 0, 0)
    
    # 材质
    base_mat = create_material("Nexus_Base", (0.4, 0.4, 0.5), metallic=0.2)
    crystal_mat = create_material("Nexus_Crystal", (0.2, 0.6, 1.0), metallic=0.0, roughness=0.1)
    ring_mat = create_material("Nexus_Ring", (1.0, 0.8, 0.2), metallic=0.8, emit=2.0)
    
    base.data.materials.append(base_mat)
    crystal.data.materials.append(crystal_mat)
    
    for obj in bpy.data.objects:
        if obj.name.startswith("Nexus_Ring"):
            obj.data.materials.append(ring_mat)
    
    print("✅ 基地水晶模型创建完成")

def create_dragon():
    """创建龙模型"""
    
    # 身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.8, depth=4, location=(0, 0, 2))
    body = bpy.context.active_object
    body.name = "Dragon_Body"
    body.rotation_euler = (1.57, 0, 0)
    
    # 头部
    bpy.ops.mesh.primitive_cone_add(radius1=0.6, radius2=0.3, depth=1.5, location=(0, 2.5, 2))
    head = bpy.context.active_object
    head.name = "Dragon_Head"
    head.rotation_euler = (1.57, 0, 0)
    
    # 翅膀
    bpy.ops.mesh.primitive_cube_add(size=1, location=(-1.5, 0, 2.5))
    left_wing = bpy.context.active_object
    left_wing.name = "Dragon_LeftWing"
    left_wing.scale = (0.1, 2, 1.5)
    left_wing.rotation_euler = (0, 0, -0.5)
    
    bpy.ops.mesh.primitive_cube_add(size=1, location=(1.5, 0, 2.5))
    right_wing = bpy.context.active_object
    right_wing.name = "Dragon_RightWing"
    right_wing.scale = (0.1, 2, 1.5)
    right_wing.rotation_euler = (0, 0, 0.5)
    
    # 材质
    scale_color = (0.8, 0.3, 0.1)
    wing_color = (0.6, 0.2, 0.1)
    
    scale_mat = create_material("Dragon_Scales", scale_color, metallic=0.3, roughness=0.4)
    wing_mat = create_material("Dragon_Wing", wing_color, metallic=0.0, roughness=0.8)
    
    body.data.materials.append(scale_mat)
    head.data.materials.append(scale_mat)
    left_wing.data.materials.append(wing_mat)
    right_wing.data.materials.append(wing_mat)
    
    print("✅ 龙模型创建完成")

def export_fbx(filename):
    """导出FBX"""
    filepath = os.path.join(output_dir, filename)
    bpy.ops.export_scene.fbx(
        filepath=filepath,
        use_selection=False,
        global_scale=1.0,
        apply_unit_scale=True,
        axis_forward='-Z',
        axis_up='Y'
    )
    print(f"✅ 已导出: {filepath}")

# ============ 创建所有模型 ============
print("🎨 创建游戏场景模型...")
print("="*50)

# 防御塔
print("\n🏗️ 创建防御塔...")
create_defense_tower()
export_fbx("Tower_Defense.fbx")

# 清理
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# 小兵
print("\n⚔️ 创建小兵...")
create_minion()
export_fbx("Minion_Warrior.fbx")

# 清理
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# 基地水晶
print("\n💎 创建基地水晶...")
create_nexus()
export_fbx("Nexus_Base.fbx")

# 清理
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# 龙
print("\n🐲 创建龙...")
create_dragon()
export_fbx("Monster_Dragon.fbx")

print("\n" + "="*50)
print("✅ 所有场景模型创建完成！")
