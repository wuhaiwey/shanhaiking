import bpy
import os
from math import radians

# 清理场景
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# 设置输出目录
output_dir = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/ModelRenders/"
os.makedirs(output_dir, exist_ok=True)

# 设置渲染引擎为EEVEE（快速渲染）
bpy.context.scene.render.engine = 'BLENDER_EEVEE'
bpy.context.scene.render.resolution_x = 1024
bpy.context.scene.render.resolution_y = 1024
bpy.context.scene.render.resolution_percentage = 100

# 设置文件格式
bpy.context.scene.render.image_settings.file_format = 'PNG'
bpy.context.scene.render.image_settings.color_mode = 'RGBA'

# 创建相机
def create_camera(name, location, rotation):
    bpy.ops.object.camera_add(location=location)
    camera = bpy.context.active_object
    camera.name = name
    camera.rotation_euler = rotation
    return camera

# 创建灯光
def setup_lighting():
    # 主光源
    bpy.ops.object.light_add(type='SUN', location=(5, -5, 10))
    sun = bpy.context.active_object
    sun.name = "MainLight"
    sun.data.energy = 3
    sun.rotation_euler = (radians(60), 0, radians(45))
    
    # 补光
    bpy.ops.object.light_add(type='AREA', location=(-3, 3, 5))
    fill = bpy.context.active_object
    fill.name = "FillLight"
    fill.data.energy = 1.5
    fill.rotation_euler = (radians(45), 0, radians(-45))
    
    # 轮廓光
    bpy.ops.object.light_add(type='SPOT', location=(0, 5, 8))
    rim = bpy.context.active_object
    rim.name = "RimLight"
    rim.data.energy = 2
    rim.rotation_euler = (radians(80), 0, radians(180))

# 渲染九视图
def render_nine_views(model_path, model_name):
    """渲染模型的九视图"""
    
    print(f"🎨 开始渲染 {model_name} 的九视图...")
    
    # 导入模型
    if os.path.exists(model_path):
        bpy.ops.import_scene.fbx(filepath=model_path)
        print(f"✅ 已导入: {model_path}")
    else:
        print(f"❌ 模型文件不存在: {model_path}")
        return
    
    # 设置灯光
    setup_lighting()
    
    # 设置输出路径
    model_output_dir = os.path.join(output_dir, model_name)
    os.makedirs(model_output_dir, exist_ok=True)
    
    # 九视图配置
    views = [
        ("01_正面", (0, -4, 2), (radians(75), 0, 0)),
        ("02_背面", (0, 4, 2), (radians(75), 0, radians(180))),
        ("03_左面", (-4, 0, 2), (radians(75), 0, radians(-90))),
        ("04_右面", (4, 0, 2), (radians(75), 0, radians(90))),
        ("05_顶面", (0, 0, 6), (0, 0, radians(-90))),
        ("06_底面", (0, 0, -2), (radians(180), 0, radians(-90))),
        ("07_左前", (-3, -3, 3), (radians(60), 0, radians(-45))),
        ("08_右前", (3, -3, 3), (radians(60), 0, radians(45))),
        ("09_透视", (3, -4, 4), (radians(65), 0, radians(35))),
    ]
    
    # 创建相机
    bpy.ops.object.camera_add(location=(0, -4, 2))
    camera = bpy.context.active_object
    camera.name = "RenderCamera"
    bpy.context.scene.camera = camera
    
    # 设置相机参数
    camera.data.type = 'PERSP'
    camera.data.lens = 50
    
    for view_name, location, rotation in views:
        # 移动相机
        camera.location = location
        camera.rotation_euler = rotation
        
        # 设置输出文件
        filepath = os.path.join(model_output_dir, f"{view_name}.png")
        bpy.context.scene.render.filepath = filepath
        
        # 渲染
        bpy.ops.render.render(write_still=True)
        
        print(f"  ✅ 已渲染: {view_name}")
    
    print(f"🎨 {model_name} 九视图渲染完成！")
    print(f"📁 输出目录: {model_output_dir}")

# 主程序
if __name__ == "__main__":
    models_dir = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/"
    
    # 渲染亚瑟模型
    render_nine_views(
        os.path.join(models_dir, "Hero_YaSe_Anime.fbx"),
        "YaSe"
    )
    
    # 清理场景准备下一个
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete(use_global=False)
    
    # 渲染黄忠模型
    render_nine_views(
        os.path.join(models_dir, "Hero_HuangZhong_Anime.fbx"),
        "HuangZhong"
    )
    
    print("\n" + "="*50)
    print("✅ 所有模型九视图渲染完成！")
    print(f"📁 图片保存位置: {output_dir}")
    print("="*50)
