import bpy
import os
from math import radians

def render_model_nine_views(model_name, output_dir):
    """渲染模型的九视图"""
    
    # 设置渲染引擎
    bpy.context.scene.render.engine = 'BLENDER_EEVEE'
    bpy.context.scene.render.resolution_x = 512
    bpy.context.scene.render.resolution_y = 512
    
    # 九个角度：前、后、左、右、上、下、左上、右上、透视
    views = [
        ("front", (0, 0, 0)),
        ("back", (0, 0, 180)),
        ("left", (0, 0, -90)),
        ("right", (0, 0, 90)),
        ("top", (90, 0, 0)),
        ("bottom", (-90, 0, 0)),
        ("front_left", (0, 0, -45)),
        ("front_right", (0, 0, 45)),
        ("iso", (30, 0, 45))
    ]
    
    # 创建相机
    bpy.ops.object.camera_add(location=(0, -5, 2))
    camera = bpy.context.active_object
    camera.name = "RenderCamera"
    bpy.context.scene.camera = camera
    
    # 添加灯光
    bpy.ops.object.light_add(type='SUN', location=(5, 5, 10))
    light = bpy.context.active_object
    light.data.energy = 3
    
    for view_name, rotation in views:
        # 重置相机角度
        camera.rotation_euler = (radians(rotation[0]), radians(rotation[1]), radians(rotation[2]))
        
        # 设置输出路径
        filepath = os.path.join(output_dir, f"{model_name}_{view_name}.png")
        bpy.context.scene.render.filepath = filepath
        
        # 渲染
        bpy.ops.render.render(write_still=True)
        
        print(f"✅ 已渲染: {view_name}")
    
    print(f"🎨 {model_name} 九视图渲染完成！")

# 使用示例
if __name__ == "__main__":
    output_dir = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/ModelRenders/"
    os.makedirs(output_dir, exist_ok=True)
    
    # 渲染当前场景的模型
    render_model_nine_views("CurrentModel", output_dir)
