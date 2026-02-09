import bpy
import math
import os

bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

output_dir = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/"

def create_material(name, color, metallic=0.0, roughness=0.5):
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    mat.diffuse_color = (*color, 1.0)
    mat.metallic = metallic
    mat.roughness = roughness
    return mat

def create_hero_zhao_yun():
    """创建赵云 - 银甲白袍"""
    print("⚔️ 创建赵云英雄...")
    
    # 身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.16, depth=0.7, location=(0, 0, 0.9))
    body = bpy.context.active_object
    body.name = "ZhaoYun_Body"
    
    # 头部
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.26, location=(0, 0, 1.5))
    head = bpy.context.active_object
    head.name = "ZhaoYun_Head"
    
    # 银色头盔
    bpy.ops.mesh.primitive_cylinder_add(radius=0.28, depth=0.25, location=(0, 0, 1.7))
    helmet = bpy.context.active_object
    helmet.name = "ZhaoYun_Helmet"
    
    helmet_mat = create_material("Silver_Armor", (0.85, 0.88, 0.92), metallic=0.8, roughness=0.2)
    helmet.data.materials.append(helmet_mat)
    
    # 银色铠甲
    bpy.ops.mesh.primitive_cylinder_add(radius=0.36, depth=0.6, location=(0, 0, 0.85))
    armor = bpy.context.active_object
    armor.name = "ZhaoYun_Armor"
    armor.scale = (1, 0.7, 1)
    armor.data.materials.append(helmet_mat)
    
    # 白色披风
    bpy.ops.mesh.primitive_cube_add(size=0.6, location=(0, -0.35, 0.9))
    cape = bpy.context.active_object
    cape.name = "ZhaoYun_Cape"
    cape.scale = (0.8, 0.1, 1.2)
    
    cape_mat = create_material("Cape_White", (0.95, 0.95, 0.98))
    cape.data.materials.append(cape_mat)
    
    # 龙胆亮银枪
    # 枪杆
    bpy.ops.mesh.primitive_cylinder_add(radius=0.025, depth=2.8, location=(0.6, 0.2, 1.4))
    spear = bpy.context.active_object
    spear.name = "ZhaoYun_Spear"
    
    # 枪头
    bpy.ops.mesh.primitive_cone_add(radius1=0.06, radius2=0, depth=0.4, location=(0.6, 0.2, 3.0))
    tip = bpy.context.active_object
    tip.name = "ZhaoYun_SpearTip"
    
    spear_mat = create_material("Spear_Silver", (0.9, 0.92, 0.95), metallic=0.85)
    spear.data.materials.append(spear_mat)
    tip.data.materials.append(spear_mat)
    
    # 肤色
    skin_mat = create_material("Skin", (0.93, 0.83, 0.73))
    head.data.materials.append(skin_mat)
    
    print("✅ 赵云模型创建完成")

def create_hero_diao_chan():
    """创建貂蝉 - 绝世舞姬"""
    print("💃 创建貂蝉英雄...")
    
    # 纤细身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.12, depth=0.65, location=(0, 0, 0.8))
    body = bpy.context.active_object
    body.name = "DiaoChan_Body"
    body.scale = (1, 0.5, 1)
    
    # 头部
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.25, location=(0, 0, 1.42))
    head = bpy.context.active_object
    head.name = "DiaoChan_Head"
    
    # 华丽发髻
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.22, location=(0, -0.1, 1.7))
    hair_bun = bpy.context.active_object
    hair_bun.name = "DiaoChan_HairBun"
    
    # 粉色长发
    bpy.ops.mesh.primitive_cylinder_add(radius=0.26, depth=0.8, location=(0, -0.2, 1.35))
    hair = bpy.context.active_object
    hair.name = "DiaoChan_Hair"
    hair.rotation_euler = (math.radians(20), 0, 0)
    
    hair_mat = create_material("Hair_Pink", (0.85, 0.55, 0.7))
    hair.data.materials.append(hair_mat)
    hair_bun.data.materials.append(hair_mat)
    
    # 华丽舞裙
    bpy.ops.mesh.primitive_cone_add(radius1=0.15, radius2=0.45, depth=0.8, location=(0, 0, 0.4))
    dress = bpy.context.active_object
    dress.name = "DiaoChan_Dress"
    
    dress_mat = create_material("Dress_PinkGold", (0.9, 0.6, 0.75))
    dress.data.materials.append(dress_mat)
    
    # 飘带
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_cylinder_add(radius=0.04, depth=1.2, location=(side * 0.4, -0.2, 0.8))
        ribbon = bpy.context.active_object
        ribbon.name = f"DiaoChan_Ribbon_{side}"
        ribbon.rotation_euler = (math.radians(30), 0, side * math.radians(20))
        
        ribbon_mat = create_material(f"Ribbon_{side}", (0.95, 0.75, 0.85))
        ribbon.data.materials.append(ribbon_mat)
    
    # 肤色
    skin_mat = create_material("Skin_Pale", (0.97, 0.88, 0.82))
    head.data.materials.append(skin_mat)
    
    print("✅ 貂蝉模型创建完成")

def create_hero_bai_qi():
    """创建白起 - 暗黑杀神"""
    print("💀 创建白起英雄...")
    
    # 魁梧身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.2, depth=0.8, location=(0, 0, 1.0))
    body = bpy.context.active_object
    body.name = "BaiQi_Body"
    body.scale = (1.2, 0.8, 1)
    
    # 头部
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.28, location=(0, 0, 1.65))
    head = bpy.context.active_object
    head.name = "BaiQi_Head"
    
    # 黑色角盔
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_cone_add(radius1=0.06, radius2=0.02, depth=0.3, location=(side * 0.2, 0, 1.95))
        horn = bpy.context.active_object
        horn.name = f"BaiQi_Horn_{side}"
        horn.rotation_euler = (math.radians(-30), 0, side * math.radians(-15))
        
        horn_mat = create_material(f"Horn_{side}", (0.15, 0.15, 0.18))
        horn.data.materials.append(horn_mat)
    
    # 暗黑色铠甲
    bpy.ops.mesh.primitive_cylinder_add(radius=0.42, depth=0.75, location=(0, 0, 0.95))
    armor = bpy.context.active_object
    armor.name = "BaiQi_Armor"
    armor.scale = (1, 0.8, 1)
    
    armor_mat = create_material("Armor_Dark", (0.2, 0.22, 0.28), metallic=0.4, roughness=0.7)
    armor.data.materials.append(armor_mat)
    
    # 巨大镰刀
    # 长柄
    bpy.ops.mesh.primitive_cylinder_add(radius=0.035, depth=2.2, location=(0.7, 0.3, 1.2))
    handle = bpy.context.active_object
    handle.name = "BaiQi_ScytheHandle"
    
    # 镰刀刃
    bpy.ops.mesh.primitive_cylinder_add(radius=0.08, depth=0.6, location=(0.7, 0.6, 2.3))
    blade = bpy.context.active_object
    blade.name = "BaiQi_ScytheBlade"
    blade.rotation_euler = (0, math.radians(90), 0)
    blade.scale = (0.5, 2, 0.1)
    
    blade_mat = create_material("Scythe_Dark", (0.1, 0.12, 0.15), metallic=0.3)
    blade.data.materials.append(blade_mat)
    handle.data.materials.append(blade_mat)
    
    # 苍白肤色
    skin_mat = create_material("Skin_Pale", (0.88, 0.85, 0.82))
    head.data.materials.append(skin_mat)
    
    print("✅ 白起模型创建完成")

def export_model(filename):
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
print("🎨 创建更多中国风英雄模型...")
print("="*50)

create_hero_zhao_yun()
export_model("Hero_ZhaoYun_Anime.fbx")

bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

create_hero_diao_chan()
export_model("Hero_DiaoChan_Anime.fbx")

bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

create_hero_bai_qi()
export_model("Hero_BaiQi_Anime.fbx")

print("\n" + "="*50)
print("✅ 新英雄模型创建完成！")
print("\n新增模型：")
print("  ⚔️ 赵云 - 银甲白袍、龙胆亮银枪")
print("  💃 貂蝉 - 绝世舞姬、粉色舞裙")
print("  💀 白起 - 暗黑杀神、巨镰")
