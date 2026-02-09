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

def create_hero_sunwukong():
    """创建孙悟空 - 齐天大圣风格"""
    print("🐵 创建孙悟空英雄...")
    
    # 身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.14, depth=0.6, location=(0, 0, 0.8))
    body = bpy.context.active_object
    body.name = "Wukong_Body"
    body.scale = (1, 0.6, 1)
    
    # 头部 - 稍圆
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.24, location=(0, 0, 1.38))
    head = bpy.context.active_object
    head.name = "Wukong_Head"
    
    # 金色紧箍咒
    bpy.ops.mesh.primitive_torus_add(major_radius=0.22, minor_radius=0.03, location=(0, 0, 1.52))
    headband = bpy.context.active_object
    headband.name = "Wukong_Headband"
    
    headband_mat = create_material("Headband_Gold", (1.0, 0.85, 0.2), metallic=0.8, roughness=0.2)
    headband.data.materials.append(headband_mat)
    
    # 金色头发/猴毛
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.26, location=(0, -0.05, 1.55))
    hair = bpy.context.active_object
    hair.name = "Wukong_Hair"
    hair.scale = (1, 0.8, 0.9)
    
    hair_mat = create_material("Hair_Gold", (0.9, 0.75, 0.2))
    hair.data.materials.append(hair_mat)
    
    # 虎皮裙
    bpy.ops.mesh.primitive_cone_add(radius1=0.18, radius2=0.35, depth=0.5, location=(0, 0, 0.35))
    skirt = bpy.context.active_object
    skirt.name = "Wukong_Skirt"
    
    skirt_mat = create_material("TigerSkin", (0.9, 0.6, 0.15))
    skirt.data.materials.append(skirt_mat)
    
    # 金色铠甲
    bpy.ops.mesh.primitive_cylinder_add(radius=0.3, depth=0.35, location=(0, 0, 0.9))
    armor = bpy.context.active_object
    armor.name = "Wukong_Armor"
    armor.scale = (1, 0.65, 1)
    
    armor_mat = create_material("Armor_Gold", (0.95, 0.8, 0.3), metallic=0.7, roughness=0.3)
    armor.data.materials.append(armor_mat)
    
    # 金箍棒
    # 棒身
    bpy.ops.mesh.primitive_cylinder_add(radius=0.04, depth=2.2, location=(0.5, 0.2, 1.2))
    staff = bpy.context.active_object
    staff.name = "Wukong_Staff"
    
    # 金色两端
    for z in [-1.1, 1.1]:
        bpy.ops.mesh.primitive_cylinder_add(radius=0.06, depth=0.15, location=(0.5, 0.2, 1.2 + z))
        end = bpy.context.active_object
        end.name = f"Wukong_StaffEnd_{z}"
        end.data.materials.append(armor_mat)
    
    staff_mat = create_material("Staff_RedGold", (0.8, 0.2, 0.1))
    staff.data.materials.append(staff_mat)
    
    # 猴脸肤色
    skin_mat = create_material("MonkeySkin", (0.85, 0.75, 0.65))
    head.data.materials.append(skin_mat)
    body.data.materials.append(skin_mat)
    
    print("✅ 孙悟空模型创建完成")

def create_hero_guanyu():
    """创建关羽 - 武圣风格"""
    print("⚔️ 创建关羽英雄...")
    
    # 魁梧身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.18, depth=0.75, location=(0, 0, 0.95))
    body = bpy.context.active_object
    body.name = "Guanyu_Body"
    body.scale = (1.1, 0.7, 1)
    
    # 头部
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.27, location=(0, 0, 1.6))
    head = bpy.context.active_object
    head.name = "Guanyu_Head"
    
    # 绿色战袍
    bpy.ops.mesh.primitive_cylinder_add(radius=0.38, depth=0.7, location=(0, 0, 0.9))
    robe = bpy.context.active_object
    robe.name = "Guanyu_Robe"
    robe.scale = (1, 0.75, 1)
    
    robe_mat = create_material("Robe_Green", (0.15, 0.45, 0.2))
    robe.data.materials.append(robe_mat)
    
    # 长胡须
    bpy.ops.mesh.primitive_cylinder_add(radius=0.06, depth=0.4, location=(0, 0.15, 1.25))
    beard = bpy.context.active_object
    beard.name = "Guanyu_Beard"
    beard.rotation_euler = (math.radians(20), 0, 0)
    
    beard_mat = create_material("Beard_Black", (0.1, 0.08, 0.05))
    beard.data.materials.append(beard_mat)
    
    # 绿帽子
    bpy.ops.mesh.primitive_cylinder_add(radius=0.28, depth=0.15, location=(0, 0, 1.82))
    hat = bpy.context.active_object
    hat.name = "Guanyu_Hat"
    hat.data.materials.append(robe_mat)
    
    # 青龙偃月刀
    # 长柄
    bpy.ops.mesh.primitive_cylinder_add(radius=0.035, depth=2.5, location=(0.7, 0.3, 1.3))
    handle = bpy.context.active_object
    handle.name = "Guanyu_BladeHandle"
    
    # 大刀刃
    bpy.ops.mesh.primitive_cube_add(size=0.4, location=(0.7, 0.3, 2.6))
    blade = bpy.context.active_object
    blade.name = "Guanyu_Blade"
    blade.scale = (0.1, 1.5, 0.05)
    
    # 青龙装饰
    blade_mat = create_material("Blade_GreenDragon", (0.2, 0.5, 0.6), metallic=0.6)
    blade.data.materials.append(blade_mat)
    
    handle_mat = create_material("Handle_Wood", (0.4, 0.25, 0.12))
    handle.data.materials.append(handle_mat)
    
    # 肤色
    skin_mat = create_material("Skin", (0.92, 0.82, 0.72))
    head.data.materials.append(skin_mat)
    
    print("✅ 关羽模型创建完成")

def create_hero_luban():
    """创建鲁班 - 小个子工匠风格"""
    print("🔧 创建鲁班英雄...")
    
    # 小身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.11, depth=0.5, location=(0, 0, 0.65))
    body = bpy.context.active_object
    body.name = "Luban_Body"
    body.scale = (1, 0.6, 1)
    
    # 大头 - Q版比例
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.28, location=(0, 0, 1.25))
    head = bpy.context.active_object
    head.name = "Luban_Head"
    
    # 工匠帽
    bpy.ops.mesh.primitive_cone_add(radius1=0.3, radius2=0.15, depth=0.25, location=(0, 0, 1.55))
    hat = bpy.context.active_object
    hat.name = "Luban_Hat"
    
    hat_mat = create_material("Hat_Brown", (0.5, 0.3, 0.15))
    hat.data.materials.append(hat_mat)
    
    # 蓝色工匠服
    bpy.ops.mesh.primitive_cone_add(radius1=0.15, radius2=0.25, depth=0.45, location=(0, 0, 0.35))
    outfit = bpy.context.active_object
    outfit.name = "Luban_Outfit"
    
    outfit_mat = create_material("Outfit_Blue", (0.25, 0.4, 0.7))
    outfit.data.materials.append(outfit_mat)
    
    # 机械工具箱背包
    bpy.ops.mesh.primitive_cube_add(size=0.35, location=(0, -0.25, 0.8))
    backpack = bpy.context.active_object
    backpack.name = "Luban_Backpack"
    
    backpack_mat = create_material("Toolbox_Metal", (0.6, 0.6, 0.65), metallic=0.5)
    backpack.data.materials.append(backpack_mat)
    
    # 大机械手/武器
    bpy.ops.mesh.primitive_cylinder_add(radius=0.08, depth=0.6, location=(0.4, 0.2, 0.8))
    mech_arm = bpy.context.active_object
    mech_arm.name = "Luban_MechArm"
    mech_arm.rotation_euler = (0, 0, math.radians(-30))
    
    mech_mat = create_material("Mech_Gold", (0.85, 0.7, 0.25), metallic=0.7)
    mech_arm.data.materials.append(mech_mat)
    
    # 肤色
    skin_mat = create_material("ChildSkin", (0.95, 0.85, 0.75))
    head.data.materials.append(skin_mat)
    
    print("✅ 鲁班模型创建完成")

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
print("🎨 创建更多中国风动漫英雄模型...")
print("="*50)

# 创建孙悟空
create_hero_sunwukong()
export_model("Hero_SunWuKong_Anime.fbx")

bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# 创建关羽
create_hero_guanyu()
export_model("Hero_GuanYu_Anime.fbx")

bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# 创建鲁班
create_hero_luban()
export_model("Hero_LuBan_Anime.fbx")

print("\n" + "="*50)
print("✅ 更多英雄模型创建完成！")
print("\n新模型：")
print("  🐵 孙悟空 - 齐天大圣、金箍棒")
print("  ⚔️ 关羽 - 武圣、青龙偃月刀")
print("  🔧 鲁班 - Q版工匠、机械手臂")
