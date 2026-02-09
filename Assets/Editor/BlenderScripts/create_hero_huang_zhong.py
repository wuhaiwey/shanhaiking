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
def create_hero_huang_zhong():
    print("🏹 创建黄忠英雄...")
    bpy.ops.mesh.primitive_cylinder_add(radius=0.16, depth=0.7, location=(0, 0, 0.9))
    body = bpy.context.active_object
    body.name = "HuangZhong_Body"
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.26, location=(0, 0, 1.5))
    head = bpy.context.active_object
    head.name = "HuangZhong_Head"
    bpy.ops.mesh.primitive_cylinder_add(radius=0.28, depth=0.7, location=(0, -0.15, 1.5))
    hair = bpy.context.active_object
    hair.name = "HuangZhong_Hair"
    hair.rotation_euler = (math.radians(30), 0, 0)
    hair_mat = create_material("Hair_White", (0.92, 0.9, 0.88))
    hair.data.materials.append(hair_mat)
    bpy.ops.mesh.primitive_cylinder_add(radius=0.32, depth=0.6, location=(0, 0, 0.85))
    armor = bpy.context.active_object
    armor.name = "HuangZhong_Armor"
    armor.scale = (1, 0.7, 1)
    armor_mat = create_material("Armor_Gold", (0.85, 0.7, 0.25), metallic=0.6)
    armor.data.materials.append(armor_mat)
    bpy.ops.mesh.primitive_cylinder_add(radius=0.04, depth=2.0, location=(0.5, 0.2, 1.1))
    bow = bpy.context.active_object
    bow.name = "HuangZhong_Bow"
    bow.rotation_euler = (0, math.radians(90), 0)
    bow_mat = create_material("Bow_Wood", (0.45, 0.28, 0.12))
    bow.data.materials.append(bow_mat)
    skin_mat = create_material("Skin_Old", (0.9, 0.8, 0.7))
    head.data.materials.append(skin_mat)
    print("✅ 黄忠模型创建完成")
def export_model(filename):
    filepath = os.path.join(output_dir, filename)
    bpy.ops.export_scene.fbx(filepath=filepath, use_selection=False, global_scale=1.0, apply_unit_scale=True, axis_forward='-Z', axis_up='Y', bake_space_transform=True, object_types={'MESH'}, use_mesh_modifiers=True)
    print(f"✅ 已导出: {filepath}")
print("🎨 创建黄忠英雄模型...")
create_hero_huang_zhong()
export_model("Hero_HuangZhong_Anime.fbx")
print("✅ 黄忠模型完成！")
