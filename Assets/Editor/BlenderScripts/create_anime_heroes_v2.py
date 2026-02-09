import bpy
import math
import os

# 清除场景
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

output_dir = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/"
os.makedirs(output_dir, exist_ok=True)

def create_material(name, color, metallic=0.0, roughness=0.5):
    """创建材质"""
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    mat.diffuse_color = (*color, 1.0)
    mat.metallic = metallic
    mat.roughness = roughness
    return mat

def create_hero_houyi_anime():
    """创建后羿 - 中国风动漫风格"""
    print("🏹 创建后羿英雄...")
    
    # 身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.15, depth=0.7, location=(0, 0, 0.9))
    body = bpy.context.active_object
    body.name = "HouYi_Body"
    body.scale = (1, 0.6, 1)
    
    # 动漫风格头部 - 稍大的比例
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.28, location=(0, 0, 1.55))
    head = bpy.context.active_object
    head.name = "HouYi_Head"
    head.scale = (1, 0.85, 1)  # 稍微压扁营造动漫感
    
    # 头发 - 黑色长发
    bpy.ops.mesh.primitive_cylinder_add(radius=0.3, depth=0.8, location=(0, -0.15, 1.4))
    hair_back = bpy.context.active_object
    hair_back.name = "HouYi_Hair_Back"
    hair_back.rotation_euler = (math.radians(15), 0, 0)
    
    # 鬓角
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_cylinder_add(radius=0.08, depth=0.5, location=(side * 0.25, 0.05, 1.35))
        side_hair = bpy.context.active_object
        side_hair.name = f"HouYi_Hair_Side_{side}"
        side_hair.rotation_euler = (0, 0, side * math.radians(10))
    
    # 发髻
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.12, location=(0, -0.1, 1.8))
    bun = bpy.context.active_object
    bun.name = "HouYi_Bun"
    
    # 红金色盔甲
    bpy.ops.mesh.primitive_cylinder_add(radius=0.35, depth=0.6, location=(0, 0, 0.85))
    chest = bpy.context.active_object
    chest.name = "HouYi_Chest"
    chest.scale = (1, 0.65, 1)
    
    # 肩甲
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.16, location=(side * 0.42, 0, 1.1))
        shoulder = bpy.context.active_object
        shoulder.name = f"HouYi_Shoulder_{side}"
    
    # 金色腰甲
    bpy.ops.mesh.primitive_cylinder_add(radius=0.3, depth=0.2, location=(0, 0, 0.45))
    waist = bpy.context.active_object
    waist.name = "HouYi_Waist"
    waist.scale = (1, 0.7, 1)
    
    # 红色裙摆
    bpy.ops.mesh.primitive_cone_add(radius1=0.32, radius2=0.45, depth=0.45, location=(0, 0, 0.15))
    skirt = bpy.context.active_object
    skirt.name = "HouYi_Skirt"
    
    # 手臂
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_cylinder_add(radius=0.07, depth=0.45, location=(side * 0.32, 0, 1.05))
        arm = bpy.context.active_object
        arm.name = f"HouYi_Arm_{side}"
        arm.rotation_euler = (0, 0, side * math.radians(15))
    
    # 腿
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_cylinder_add(radius=0.09, depth=0.6, location=(side * 0.15, 0, 0.35))
        leg = bpy.context.active_object
        leg.name = f"HouYi_Leg_{side}"
    
    # 弓箭 - 详细设计
    # 弓身
    bpy.ops.mesh.primitive_torus_add(major_radius=0.45, minor_radius=0.025, location=(0.6, 0.2, 1.0))
    bow = bpy.context.active_object
    bow.name = "HouYi_Bow"
    bow.rotation_euler = (0, math.radians(90), 0)
    bow.scale = (1, 1.4, 1)
    
    # 弓弦
    bpy.ops.mesh.primitive_cylinder_add(radius=0.003, depth=1.2, location=(0.6, 0.2, 1.0))
    bowstring = bpy.context.active_object
    bowstring.name = "HouYi_BowString"
    
    # 箭
    bpy.ops.mesh.primitive_cylinder_add(radius=0.015, depth=0.7, location=(0.6, 0.2, 1.0))
    arrow = bpy.context.active_object
    arrow.name = "HouYi_Arrow"
    arrow.rotation_euler = (math.radians(90), 0, 0)
    
    # ===== 应用材质 =====
    
    # 皮肤
    skin_mat = create_material("Skin", (0.95, 0.82, 0.72))
    head.data.materials.append(skin_mat)
    for side in [-1, 1]:
        bpy.data.objects[f"HouYi_Arm_{side}"].data.materials.append(skin_mat)
    
    # 头发 - 黑色
    hair_mat = create_material("Hair_Black", (0.08, 0.06, 0.05))
    hair_back.data.materials.append(hair_mat)
    bun.data.materials.append(hair_mat)
    
    # 盔甲 - 红金色
    armor_mat = create_material("Armor_RedGold", (0.75, 0.15, 0.08), metallic=0.6, roughness=0.4)
    chest.data.materials.append(armor_mat)
    for side in [-1, 1]:
        bpy.data.objects[f"HouYi_Shoulder_{side}"].data.materials.append(armor_mat)
    waist.data.materials.append(armor_mat)
    
    # 裙摆
    skirt_mat = create_material("Skirt_Red", (0.65, 0.1, 0.08))
    skirt.data.materials.append(skirt_mat)
    
    # 武器 - 木质和金属
    wood_mat = create_material("Wood", (0.4, 0.25, 0.12))
    bow.data.materials.append(wood_mat)
    
    steel_mat = create_material("Steel", (0.8, 0.85, 0.9), metallic=0.8, roughness=0.2)
    arrow.data.materials.append(steel_mat)
    
    print("✅ 后羿模型创建完成")

def create_hero_daji_anime():
    """创建妲己 - 妖狐风格"""
    print("🦊 创建妲己英雄...")
    
    # 身体 - 较纤细
    bpy.ops.mesh.primitive_cylinder_add(radius=0.13, depth=0.6, location=(0, 0, 0.8))
    body = bpy.context.active_object
    body.name = "DaJi_Body"
    body.scale = (1, 0.55, 1)
    
    # 头部 - 动漫比例
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.26, location=(0, 0, 1.4))
    head = bpy.context.active_object
    head.name = "DaJi_Head"
    head.scale = (1, 0.9, 1)
    
    # 大眼睛效果 - 使用稍大的球体
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.06, location=(side * 0.09, 0.22, 1.42))
        eye = bpy.context.active_object
        eye.name = f"DaJi_Eye_{side}"
        eye.scale = (1, 1.3, 0.5)
        
        eye_mat = create_material(f"Eye_{side}", (0.8, 0.2, 0.4))
        eye.data.materials.append(eye_mat)
    
    # 狐耳
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_cone_add(radius1=0.06, radius2=0.02, depth=0.15, location=(side * 0.12, 0.05, 1.62))
        ear = bpy.context.active_object
        ear.name = f"DaJi_Ear_{side}"
        ear.rotation_euler = (math.radians(-25), 0, side * math.radians(-20))
        
        ear_mat = create_material(f"Ear_{side}", (0.95, 0.75, 0.85))
        ear.data.materials.append(ear_mat)
    
    # 粉紫色长发
    bpy.ops.mesh.primitive_cylinder_add(radius=0.28, depth=0.9, location=(0, -0.15, 1.35))
    hair = bpy.context.active_object
    hair.name = "DaJi_Hair"
    hair.rotation_euler = (math.radians(10), 0, 0)
    
    hair_mat = create_material("Hair_Purple", (0.5, 0.2, 0.6))
    hair.data.materials.append(hair_mat)
    
    # 紫色连衣裙
    bpy.ops.mesh.primitive_cone_add(radius1=0.18, radius2=0.35, depth=0.7, location=(0, 0, 0.35))
    dress = bpy.context.active_object
    dress.name = "DaJi_Dress"
    
    dress_mat = create_material("Dress_Purple", (0.65, 0.25, 0.75))
    dress.data.materials.append(dress_mat)
    
    # 狐尾
    bpy.ops.mesh.primitive_cone_add(radius1=0.08, radius2=0.02, depth=0.5, location=(0, -0.25, 0.35))
    tail = bpy.context.active_object
    tail.name = "DaJi_Tail"
    tail.rotation_euler = (math.radians(-35), 0, 0)
    
    tail_mat = create_material("Tail_Pink", (0.95, 0.7, 0.8))
    tail.data.materials.append(tail_mat)
    
    # 手臂
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_cylinder_add(radius=0.055, depth=0.4, location=(side * 0.28, 0, 0.95))
        arm = bpy.context.active_object
        arm.name = f"DaJi_Arm_{side}"
        arm.data.materials.append(create_material(f"Skin_{side}", (0.95, 0.82, 0.72)))
    
    print("✅ 妲己模型创建完成")

def export_model(filename):
    """导出模型"""
    filepath = os.path.join(output_dir, filename)
    bpy.ops.export_scene.fbx(
        filepath=filepath,
        use_selection=False,
        global_scale=1.0,
        apply_unit_scale=True,
        axis_forward='-Z',
        axis_up='Y',
        bake_space_transform=True,
        object_types={'MESH'},
        use_mesh_modifiers=True
    )
    print(f"✅ 已导出: {filepath}")

# ============ 主程序 ============
print("🎨 创建中国风动漫风格英雄模型...")
print("="*50)

# 创建后羿
create_hero_houyi_anime()
export_model("Hero_HouYi_Anime.fbx")

# 清理场景
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# 创建妲己
create_hero_daji_anime()
export_model("Hero_DaJi_Anime.fbx")

print("\n" + "="*50)
print("✅ 高质量动漫风格英雄模型创建完成！")
print("\n特点：")
print("  ✓ 动漫风格头部比例（大头）")
print("  ✓ 中式服装和盔甲")
print("  ✓ 详细的武器设计")
print("  ✓ 丰富的材质和颜色")
print("  ✓ 符合王者荣耀风格")
