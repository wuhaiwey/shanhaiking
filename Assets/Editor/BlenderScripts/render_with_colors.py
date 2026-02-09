import bpy
import os
from math import radians

# 清理场景
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# 设置输出目录
output_dir = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/ModelRenders_Color/"
os.makedirs(output_dir, exist_ok=True)

# 设置渲染引擎 - 使用Cycles获得更好的材质效果
bpy.context.scene.render.engine = 'CYCLES'
bpy.context.scene.cycles.device = 'CPU'  # Mac用CPU渲染
bpy.context.scene.render.resolution_x = 1024
bpy.context.scene.render.resolution_y = 1024
bpy.context.scene.render.resolution_percentage = 100

# 设置文件格式
bpy.context.scene.render.image_settings.file_format = 'PNG'
bpy.context.scene.render.image_settings.color_mode = 'RGBA'

# 创建彩色材质预设
def create_colored_material(name, color, metallic=0.0, roughness=0.5):
    """创建带颜色的材质"""
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    
    # 清除默认节点
    nodes = mat.node_tree.nodes
    nodes.clear()
    
    # 创建输出节点
    output = nodes.new('ShaderNodeOutputMaterial')
    output.location = (300, 0)
    
    # 创建原理化BSDF节点
    bsdf = nodes.new('ShaderNodeBsdfPrincipled')
    bsdf.location = (0, 0)
    bsdf.inputs['Base Color'].default_value = (*color, 1.0)
    bsdf.inputs['Metallic'].default_value = metallic
    bsdf.inputs['Roughness'].default_value = roughness
    
    # 连接
    links = mat.node_tree.links
    links.new(bsdf.outputs['BSDF'], output.inputs['Surface'])
    
    return mat

def setup_colored_lighting():
    """设置彩色灯光环境"""
    # 清除现有灯光
    for obj in bpy.data.objects:
        if obj.type == 'LIGHT':
            bpy.data.objects.remove(obj)
    
    # 主光源 - 暖色
    bpy.ops.object.light_add(type='AREA', location=(4, -4, 6))
    main = bpy.context.active_object
    main.name = "MainLight"
    main.data.energy = 300
    main.data.color = (1.0, 0.95, 0.9)
    main.rotation_euler = (radians(60), 0, radians(45))
    
    # 补光 - 冷色
    bpy.ops.object.light_add(type='AREA', location=(-4, 3, 5))
    fill = bpy.context.active_object
    fill.name = "FillLight"
    fill.data.energy = 150
    fill.data.color = (0.85, 0.9, 1.0)
    fill.rotation_euler = (radians(45), 0, radians(-45))
    
    # 轮廓光 - 中性
    bpy.ops.object.light_add(type='SPOT', location=(0, 5, 8))
    rim = bpy.context.active_object
    rim.name = "RimLight"
    rim.data.energy = 200
    rim.data.color = (1.0, 1.0, 1.0)
    rim.rotation_euler = (radians(80), 0, radians(180))
    
    # 环境光
    bpy.ops.object.light_add(type='SUN', location=(0, 0, 10))
    sun = bpy.context.active_object
    sun.name = "SunLight"
    sun.data.energy = 3
    sun.data.color = (1.0, 0.98, 0.95)

def create_sample_hero_with_colors():
    """创建一个带颜色的示例英雄"""
    
    # 创建材质库
    skin_mat = create_colored_material("Skin", (0.95, 0.82, 0.72))
    gold_mat = create_colored_material("Gold", (1.0, 0.85, 0.2), metallic=0.8, roughness=0.2)
    blue_mat = create_colored_material("Blue", (0.15, 0.35, 0.65), metallic=0.4)
    red_mat = create_colored_material("Red", (0.75, 0.15, 0.1))
    silver_mat = create_colored_material("Silver", (0.9, 0.92, 0.95), metallic=0.9)
    black_mat = create_colored_material("Black", (0.1, 0.1, 0.12))
    
    # 身体 - 蓝色铠甲
    bpy.ops.mesh.primitive_cylinder_add(radius=0.35, depth=0.6, location=(0, 0, 0.9))
    body = bpy.context.active_object
    body.name = "Body"
    body.scale = (1, 0.7, 1)
    body.data.materials.append(blue_mat)
    
    # 头部 - 肤色
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.26, location=(0, 0, 1.5))
    head = bpy.context.active_object
    head.name = "Head"
    head.data.materials.append(skin_mat)
    
    # 头发 - 黑色
    bpy.ops.mesh.primitive_cylinder_add(radius=0.28, depth=0.7, location=(0, -0.15, 1.55))
    hair = bpy.context.active_object
    hair.name = "Hair"
    hair.rotation_euler = (radians(25), 0, 0)
    hair.data.materials.append(black_mat)
    
    # 皇冠 - 金色
    bpy.ops.mesh.primitive_cylinder_add(radius=0.15, depth=0.08, location=(0, 0, 1.82))
    crown = bpy.context.active_object
    crown.name = "Crown"
    crown.data.materials.append(gold_mat)
    
    # 肩甲 - 金色
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.16, location=(side * 0.42, 0, 1.15))
        shoulder = bpy.context.active_object
        shoulder.name = f"Shoulder_{side}"
        shoulder.data.materials.append(gold_mat)
    
    # 裙摆 - 红色
    bpy.ops.mesh.primitive_cone_add(radius1=0.32, radius2=0.48, depth=0.5, location=(0, 0, 0.35))
    skirt = bpy.context.active_object
    skirt.name = "Skirt"
    skirt.data.materials.append(red_mat)
    
    # 剑 - 银色
    bpy.ops.mesh.primitive_cylinder_add(radius=0.03, depth=1.2, location=(0.6, 0.2, 1.0))
    sword = bpy.context.active_object
    sword.name = "Sword"
    sword.rotation_euler = (0, radians(30), 0)
    sword.data.materials.append(silver_mat)
    
    # 盾牌 - 金色+蓝色
    bpy.ops.mesh.primitive_cylinder_add(radius=0.32, depth=0.06, location=(-0.5, 0.1, 1.0))
    shield = bpy.context.active_object
    shield.name = "Shield"
    shield.rotation_euler = (0, radians(90), 0)
    shield.data.materials.append(gold_mat)

def render_nine_views_color(output_name):
    """渲染带颜色的九视图"""
    
    print(f"🎨 开始渲染 {output_name} 的彩色九视图...")
    
    # 设置灯光
    setup_colored_lighting()
    
    # 创建输出目录
    model_output_dir = os.path.join(output_dir, output_name)
    os.makedirs(model_output_dir, exist_ok=True)
    
    # 九视图配置
    views = [
        ("01_正面", (0, -4, 2.5), (radians(70), 0, 0)),
        ("02_背面", (0, 4, 2.5), (radians(70), 0, radians(180))),
        ("03_左面", (-4, 0, 2.5), (radians(70), 0, radians(-90))),
        ("04_右面", (4, 0, 2.5), (radians(70), 0, radians(90))),
        ("05_顶面", (0, 0, 6), (0, 0, radians(-90))),
        ("06_底面", (0, 0, -1), (radians(180), 0, radians(-90))),
        ("07_左前", (-3, -3, 3.5), (radians(60), 0, radians(-45))),
        ("08_右前", (3, -3, 3.5), (radians(60), 0, radians(45))),
        ("09_透视", (3.5, -4, 4.5), (radians(65), 0, radians(35))),
    ]
    
    # 创建相机
    bpy.ops.object.camera_add(location=(0, -4, 2.5))
    camera = bpy.context.active_object
    camera.name = "RenderCamera"
    bpy.context.scene.camera = camera
    camera.data.type = 'PERSP'
    camera.data.lens = 50
    
    # 渲染每个角度
    for view_name, location, rotation in views:
        camera.location = location
        camera.rotation_euler = rotation
        
        filepath = os.path.join(model_output_dir, f"{view_name}.png")
        bpy.context.scene.render.filepath = filepath
        
        print(f"  📷 渲染 {view_name}...")
        bpy.ops.render.render(write_still=True)
        print(f"  ✅ 完成: {view_name}")
    
    print(f"🎨 {output_name} 彩色九视图渲染完成！")

# 主程序
if __name__ == "__main__":
    print("🎨 创建带颜色的示例模型并渲染...")
    print("="*50)
    
    # 创建带颜色的示例英雄
    create_sample_hero_with_colors()
    
    # 渲染九视图
    render_nine_views_color("SampleHero_Color")
    
    print("\n" + "="*50)
    print("✅ 彩色渲染完成！")
    print(f"📁 图片保存位置: {output_dir}")
    print("="*50)
