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

def create_hero_ya_se():
    """亚瑟 - 圣骑之力"""
    print("⚔️ 创建亚瑟英雄...")
    
    # 魁梧身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.18, depth=0.75, location=(0, 0, 0.95))
    body = bpy.context.active_object
    body.name = "YaSe_Body"
    body.scale = (1.1, 0.75, 1)
    
    # 头部
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.27, location=(0, 0, 1.6))
    head = bpy.context.active_object
    head.name = "YaSe_Head"
    
    # 金色短发
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.28, location=(0, -0.05, 1.65))
    hair = bpy.context.active_object
    hair.name = "YaSe_Hair"
    hair.scale = (1, 0.8, 0.9)
    
    hair_mat = create_material("Hair_Gold", (0.9, 0.75, 0.3))
    hair.data.materials.append(hair_mat)
    
    # 金色皇冠
    bpy.ops.mesh.primitive_cylinder_add(radius=0.15, depth=0.08, location=(0, 0, 1.9))
    crown = bpy.context.active_object
    crown.name = "YaSe_Crown"
    
    crown_mat = create_material("Crown_Gold", (1.0, 0.85, 0.2), metallic=0.9, roughness=0.2)
    crown.data.materials.append(crown_mat)
    
    # 蓝色铠甲
    bpy.ops.mesh.primitive_cylinder_add(radius=0.38, depth=0.7, location=(0, 0, 0.9))
    armor = bpy.context.active_object
    armor.name = "YaSe_Armor"
    armor.scale = (1, 0.75, 1)
    
    armor_mat = create_material("Armor_BlueGold", (0.15, 0.35, 0.65), metallic=0.6, roughness=0.4)
    armor.data.materials.append(armor_mat)
    
    # 金色肩甲
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.18, location=(side * 0.45, 0, 1.15))
        shoulder = bpy.context.active_object
        shoulder.name = f"YaSe_Shoulder_{side}"
        shoulder.data.materials.append(crown_mat)
    
    # 蓝色裙摆
    bpy.ops.mesh.primitive_cone_add(radius1=0.35, radius2=0.5, depth=0.5, location=(0, 0, 0.35))
    skirt = bpy.context.active_object
    skirt.name = "YaSe_Skirt"
    
    skirt_mat = create_material("Skirt_Blue", (0.2, 0.4, 0.7))
    skirt.data.materials.append(skirt_mat)
    
    # 圣剑
    # 剑柄
    bpy.ops.mesh.primitive_cylinder_add(radius=0.04, depth=0.3, location=(0.6, 0.2, 0.8))
    handle = bpy.context.active_object
    handle.name = "YaSe_SwordHandle"
    
    # 护手
    bpy.ops.mesh.primitive_cylinder_add(radius=0.12, depth=0.05, location=(0.6, 0.2, 0.98))
    guard = bpy.context.active_object
    guard.name = "YaSe_SwordGuard"
    guard.scale = (1, 0.3, 1)
    
    # 剑刃
    bpy.ops.mesh.primitive_cube_add(size=0.08, location=(0.6, 0.2, 1.5))
    blade = bpy.context.active_object
    blade.name = "YaSe_SwordBlade"
    blade.scale = (0.15, 1, 3.5)
    
    blade_mat = create_material("Blade_Silver", (0.9, 0.93, 0.97), metallic=0.9, roughness=0.1)
    blade.data.materials.append(blade_mat)
    handle.data.materials.append(crown_mat)
    guard.data.materials.append(crown_mat)
    
    # 盾牌
    bpy.ops.mesh.primitive_cylinder_add(radius=0.35, depth=0.08, location=(-0.5, 0.1, 1.0))
    shield = bpy.context.active_object
    shield.name = "YaSe_Shield"
    shield.rotation_euler = (0, math.radians(90), 0)
    
    shield_mat = create_material("Shield_Blue", (0.18, 0.38, 0.68), metallic=0.5)
    shield.data.materials.append(shield_mat)
    
    # 肤色
    skin_mat = create_material("Skin", (0.95, 0.85, 0.75))
    head.data.materials.append(skin_mat)
    
    print("✅ 亚瑟模型创建完成")

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
print("🎨 创建亚瑟英雄模型...")
print("="*50)

create_hero_ya_se()
export_model("Hero_YaSe_Anime.fbx")

print("\n" + "="*50)
print("✅ 亚瑟模型创建完成！")
print("\n模型特征：")
print("  ⚔️ 亚瑟 - 圣骑之力")
print("  👑 金色皇冠")
print("  🛡️ 蓝色铠甲 + 金色肩甲")
print("  ⚔️ 圣剑 + 盾牌")
print("  💇 金色短发")
