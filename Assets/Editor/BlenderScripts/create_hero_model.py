import bpy
import os

# 清除现有对象
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# 创建输出目录
output_dir = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/"
os.makedirs(output_dir, exist_ok=True)

def create_material(name, color, metallic=0.0, roughness=0.5):
    """创建材质"""
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes["Principled BSDF"]
    bsdf.inputs['Base Color'].default_value = (*color, 1.0)
    bsdf.inputs['Metallic'].default_value = metallic
    bsdf.inputs['Roughness'].default_value = roughness
    return mat

def create_humanoid_base():
    """创建人形基础模型 - 后羿"""
    
    # 创建躯干
    bpy.ops.mesh.primitive_cylinder_add(radius=0.35, depth=0.8, location=(0, 0, 1.0))
    torso = bpy.context.active_object
    torso.name = "Torso"
    torso.scale = (1, 0.6, 1)
    
    # 创建头部
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.2, location=(0, 0, 1.7))
    head = bpy.context.active_object
    head.name = "Head"
    
    # 创建左臂
    bpy.ops.mesh.primitive_cylinder_add(radius=0.08, depth=0.6, location=(-0.5, 0, 1.3))
    left_arm = bpy.context.active_object
    left_arm.name = "LeftArm"
    left_arm.rotation_euler = (0, 0, 0.3)
    
    # 创建右臂
    bpy.ops.mesh.primitive_cylinder_add(radius=0.08, depth=0.6, location=(0.5, 0, 1.3))
    right_arm = bpy.context.active_object
    right_arm.name = "RightArm"
    right_arm.rotation_euler = (0, 0, -0.3)
    
    # 创建左腿
    bpy.ops.mesh.primitive_cylinder_add(radius=0.1, depth=0.8, location=(-0.2, 0, 0.4))
    left_leg = bpy.context.active_object
    left_leg.name = "LeftLeg"
    
    # 创建右腿
    bpy.ops.mesh.primitive_cylinder_add(radius=0.1, depth=0.8, location=(0.2, 0, 0.4))
    right_leg = bpy.context.active_object
    right_leg.name = "RightLeg"
    
    # 应用材质
    skin_color = (0.96, 0.80, 0.69)  # 皮肤色
    armor_color = (0.8, 0.1, 0.1)    # 红色盔甲
    gold_color = (1.0, 0.84, 0.0)    # 金色装饰
    
    skin_mat = create_material("Skin", skin_color, metallic=0.0, roughness=0.8)
    armor_mat = create_material("Armor_Red", armor_color, metallic=0.7, roughness=0.3)
    gold_mat = create_material("Gold", gold_color, metallic=1.0, roughness=0.2)
    
    # 应用材质
    for obj in [head, left_arm, right_arm, left_leg, right_leg]:
        obj.data.materials.append(skin_mat)
    
    torso.data.materials.append(armor_mat)
    
    # 添加弓箭
    create_bow()
    
    print("✅ 后羿基础模型创建完成")

def create_bow():
    """创建弓箭"""
    # 弓身
    bpy.ops.mesh.primitive_torus_add(major_radius=0.4, minor_radius=0.03, location=(0.6, 0.3, 1.3))
    bow = bpy.context.active_object
    bow.name = "Bow"
    bow.rotation_euler = (0, 1.57, 0)
    
    # 弓弦
    bpy.ops.mesh.primitive_cylinder_add(radius=0.005, depth=0.7, location=(0.6, 0.3, 1.3))
    bowstring = bpy.context.active_object
    bowstring.name = "Bowstring"
    bowstring.scale = (1, 1, 0.1)
    
    # 箭
    bpy.ops.mesh.primitive_cylinder_add(radius=0.02, depth=0.8, location=(0.6, 0.3, 1.3))
    arrow = bpy.context.active_object
    arrow.name = "Arrow"
    arrow.rotation_euler = (1.57, 0, 0)
    
    # 材质
    wood_color = (0.4, 0.25, 0.1)
    wood_mat = create_material("Wood", wood_color, metallic=0.0, roughness=0.9)
    
    bow.data.materials.append(wood_mat)
    
    print("✅ 弓箭创建完成")

def create_weapon(weapon_type):
    """创建武器"""
    if weapon_type == "sword":
        # 剑刃
        bpy.ops.mesh.primitive_cube_add(size=0.1, location=(0, 0, 1.5))
        blade = bpy.context.active_object
        blade.name = "SwordBlade"
        blade.scale = (0.1, 1.5, 0.02)
        
        # 剑柄
        bpy.ops.mesh.primitive_cylinder_add(radius=0.04, depth=0.4, location=(0, 0, 0.7))
        handle = bpy.context.active_object
        handle.name = "SwordHandle"
        
    elif weapon_type == "spear":
        # 矛杆
        bpy.ops.mesh.primitive_cylinder_add(radius=0.03, depth=2.0, location=(0, 0, 1.5))
        shaft = bpy.context.active_object
        shaft.name = "SpearShaft"
        
        # 矛头
        bpy.ops.mesh.primitive_cone_add(radius1=0.08, radius2=0, depth=0.3, location=(0, 0, 2.6))
        tip = bpy.context.active_object
        tip.name = "SpearTip"
        
    elif weapon_type == "staff":
        # 法杖
        bpy.ops.mesh.primitive_cylinder_add(radius=0.04, depth=1.8, location=(0, 0, 1.4))
        staff = bpy.context.active_object
        staff.name = "Staff"
        
        # 宝石
        bpy.ops.mesh.primitive_ico_sphere_add(radius=0.1, location=(0, 0, 2.4))
        gem = bpy.context.active_object
        gem.name = "StaffGem"
        
        # 发光材质
        gem_mat = create_material("Gem", (0.2, 0.5, 1.0), metallic=0.0, roughness=0.1)
        gem.data.materials.append(gem_mat)

def export_fbx(filename):
    """导出FBX文件"""
    filepath = os.path.join(output_dir, filename)
    bpy.ops.export_scene.fbx(
        filepath=filepath,
        use_selection=False,
        global_scale=1.0,
        apply_unit_scale=True,
        apply_scale_options='FBX_SCALE_UNITS',
        axis_forward='-Z',
        axis_up='Y',
        bake_space_transform=True,
        object_types={'MESH', 'ARMATURE'},
        use_mesh_modifiers=True,
        mesh_smooth_type='OFF',
        use_tspace=True,
        use_custom_props=False,
        add_leaf_bones=False,
        primary_bone_axis='Y',
        secondary_bone_axis='X'
    )
    print(f"✅ 已导出: {filepath}")

# ============ 创建模型 ============
print("🎨 开始创建山海经王者荣耀3D模型...")
print("="*50)

# 创建后羿模型
create_humanoid_base()

# 导出模型
export_fbx("Hero_HouYi.fbx")

# 创建武器
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# 创建各种武器
print("\n🔨 创建武器模型...")
create_weapon("sword")
export_fbx("Weapon_Sword.fbx")

bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

create_weapon("spear")
export_fbx("Weapon_Spear.fbx")

bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

create_weapon("staff")
export_fbx("Weapon_Staff.fbx")

print("\n" + "="*50)
print("✅ 所有模型创建完成！")
print(f"📁 输出目录: {output_dir}")
