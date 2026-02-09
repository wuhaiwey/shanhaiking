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

def create_hero_li_bai():
    """李白 - 青莲剑仙"""
    print("⚔️ 创建李白英雄...")
    
    # 身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.15, depth=0.7, location=(0, 0, 0.9))
    body = bpy.context.active_object
    body.name = "LiBai_Body"
    
    # 头部
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.26, location=(0, 0, 1.5))
    head = bpy.context.active_object
    head.name = "LiBai_Head"
    
    # 白色长发
    bpy.ops.mesh.primitive_cylinder_add(radius=0.28, depth=0.9, location=(0, -0.15, 1.45))
    hair = bpy.context.active_object
    hair.name = "LiBai_Hair"
    hair.rotation_euler = (math.radians(25), 0, 0)
    
    hair_mat = create_material("Hair_White", (0.95, 0.95, 0.98))
    hair.data.materials.append(hair_mat)
    
    # 青色长袍
    bpy.ops.mesh.primitive_cone_add(radius1=0.18, radius2=0.4, depth=0.8, location=(0, 0, 0.4))
    robe = bpy.context.active_object
    robe.name = "LiBai_Robe"
    
    robe_mat = create_material("Robe_Cyan", (0.25, 0.65, 0.75))
    robe.data.materials.append(robe_mat)
    
    # 酒壶
    bpy.ops.mesh.primitive_cylinder_add(radius=0.08, depth=0.2, location=(-0.3, 0.1, 0.9))
    wine = bpy.context.active_object
    wine.name = "LiBai_Wine"
    
    # 长剑
    bpy.ops.mesh.primitive_cylinder_add(radius=0.025, depth=1.2, location=(0.5, 0.2, 1.0))
    sword = bpy.context.active_object
    sword.name = "LiBai_Sword"
    sword.rotation_euler = (0, math.radians(30), 0)
    
    sword_mat = create_material("Sword_Silver", (0.9, 0.92, 0.95), metallic=0.8)
    sword.data.materials.append(sword_mat)
    
    # 肤色
    skin_mat = create_material("Skin", (0.95, 0.85, 0.75))
    head.data.materials.append(skin_mat)
    
    print("✅ 李白模型创建完成")

def create_hero_hua_mulan():
    """花木兰 - 传说之刃"""
    print("⚔️ 创建花木兰英雄...")
    
    # 身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.14, depth=0.65, location=(0, 0, 0.85))
    body = bpy.context.active_object
    body.name = "HuaMulan_Body"
    
    # 头部
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.25, location=(0, 0, 1.42))
    head = bpy.context.active_object
    head.name = "HuaMulan_Head"
    
    # 高马尾
    bpy.ops.mesh.primitive_cylinder_add(radius=0.12, depth=0.7, location=(0, -0.25, 1.6))
    ponytail = bpy.context.active_object
    ponytail.name = "HuaMulan_Ponytail"
    ponytail.rotation_euler = (math.radians(60), 0, 0)
    
    hair_mat = create_material("Hair_Black", (0.1, 0.08, 0.06))
    ponytail.data.materials.append(hair_mat)
    
    # 红色战甲
    bpy.ops.mesh.primitive_cylinder_add(radius=0.32, depth=0.6, location=(0, 0, 0.85))
    armor = bpy.context.active_object
    armor.name = "HuaMulan_Armor"
    armor.scale = (1, 0.65, 1)
    
    armor_mat = create_material("Armor_Red", (0.75, 0.15, 0.12), metallic=0.5)
    armor.data.materials.append(armor_mat)
    
    # 双剑
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_cylinder_add(radius=0.02, depth=0.9, location=(side * 0.4, 0.15, 0.9))
        sword = bpy.context.active_object
        sword.name = f"HuaMulan_Sword_{side}"
        sword.rotation_euler = (0, 0, side * math.radians(20))
        
        sword_mat = create_material(f"Sword_{side}", (0.85, 0.88, 0.9), metallic=0.8)
        sword.data.materials.append(sword_mat)
    
    # 肤色
    skin_mat = create_material("Skin", (0.95, 0.85, 0.78))
    head.data.materials.append(skin_mat)
    
    print("✅ 花木兰模型创建完成")

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
print("🎨 并行创建多个英雄模型...")
print("="*50)

create_hero_li_bai()
export_model("Hero_LiBai_Anime.fbx")

bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

create_hero_hua_mulan()
export_model("Hero_HuaMulan_Anime.fbx")

print("\n" + "="*50)
print("✅ 英雄模型并行创建完成！")
print("\n新增：")
print("  ⚔️ 李白 - 青莲剑仙、酒壶")
print("  ⚔️ 花木兰 - 传说之刃、双剑")
