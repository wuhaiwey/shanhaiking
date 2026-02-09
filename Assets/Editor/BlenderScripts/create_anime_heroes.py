import bpy
import bmesh
import math
import os

# 清除场景
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

output_dir = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/"
os.makedirs(output_dir, exist_ok=True)

def create_anime_head(location, scale=1.0):
    """创建动漫风格头部 - 大眼、小脸"""
    # 头部基础 - 稍大的球体营造动漫感
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.25 * scale, location=location)
    head = bpy.context.active_object
    head.name = "Head"
    
    # 进入编辑模式调整形状
    bpy.context.view_layer.objects.active = head
    bpy.ops.object.mode_set(mode='EDIT')
    
    # 创建bmesh
    bm = bmesh.from_mesh(head.data)
    
    # 稍微压扁头部（动漫风格）
    for vert in bm.verts:
        vert.co.z *= 0.85  # 压扁
        vert.co.x *= 1.05  # 稍宽
    
    # 更新
    bmesh.to_mesh(bm, head.data)
    bm.free()
    
    bpy.ops.object.mode_set(mode='OBJECT')
    
    return head

def create_anime_eyes(location, scale=1.0):
    """创建动漫大眼睛"""
    eyes = []
    for side in [-1, 1]:  # 左眼和右眼
        bpy.ops.mesh.primitive_uv_sphere_add(
            radius=0.08 * scale, 
            location=(location[0] + side * 0.1 * scale, location[1] + 0.18 * scale, location[2] + 0.05 * scale)
        )
        eye = bpy.context.active_object
        eye.name = f"Eye_{'Left' if side == -1 else 'Right'}"
        eye.scale = (1, 0.6, 1)  # 压扁成 anime 眼形
        
        # 黑色材质
        mat = bpy.data.materials.new(name=f"EyeMat_{side}")
        mat.use_nodes = True
        mat.diffuse_color = (0.1, 0.1, 0.15, 1.0)
        eye.data.materials.append(mat)
        
        eyes.append(eye)
    
    return eyes

def create_chinese_hair(location, style="long", color=(0.1, 0.1, 0.15)):
    """创建中式发型"""
    hair_parts = []
    
    if style == "long":
        # 长发 - 后部
        bpy.ops.mesh.primitive_cylinder_add(
            radius=0.28, 
            depth=0.8,
            location=(location[0], location[1] - 0.15, location[2] - 0.3)
        )
        back_hair = bpy.context.active_object
        back_hair.name = "Hair_Back"
        back_hair.rotation_euler = (math.radians(15), 0, 0)
        hair_parts.append(back_hair)
        
        # 两侧鬓角
        for side in [-1, 1]:
            bpy.ops.mesh.primitive_cylinder_add(
                radius=0.1,
                depth=0.5,
                location=(location[0] + side * 0.22, location[1] + 0.05, location[2] - 0.15)
            )
            side_hair = bpy.context.active_object
            side_hair.name = f"Hair_Side_{side}"
            side_hair.rotation_euler = (0, 0, side * math.radians(10))
            hair_parts.append(side_hair)
    
    elif style == "bun":
        # 发髻
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.18, location=(location[0], location[1] - 0.1, location[2] + 0.25))
        bun = bpy.context.active_object
        bun.name = "Hair_Bun"
        hair_parts.append(bun)
    
    # 应用材质
    hair_mat = bpy.data.materials.new(name="HairMat")
    hair_mat.use_nodes = True
    hair_mat.diffuse_color = (*color, 1.0)
    
    for part in hair_parts:
        part.data.materials.append(hair_mat)
    
    return hair_parts

def create_chinese_armor(location, color=(0.6, 0.1, 0.1)):
    """创建中式盔甲"""
    armor_parts = []
    
    # 胸甲
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.35,
        depth=0.6,
        location=(location[0], location[1], location[2] + 0.3)
    )
    chest = bpy.context.active_object
    chest.name = "Armor_Chest"
    chest.scale = (1, 0.6, 1)
    armor_parts.append(chest)
    
    # 肩甲
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_uv_sphere_add(
            radius=0.18,
            location=(location[0] + side * 0.45, location[1], location[2] + 0.55)
        )
        shoulder = bpy.context.active_object
        shoulder.name = f"Armor_Shoulder_{side}"
        armor_parts.append(shoulder)
    
    # 腰甲
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.32,
        depth=0.25,
        location=(location[0], location[1], location[2] - 0.05)
    )
    waist = bpy.context.active_object
    waist.name = "Armor_Waist"
    waist.scale = (1, 0.65, 1)
    armor_parts.append(waist)
    
    # 裙摆
    bpy.ops.mesh.primitive_cone_add(
        radius1=0.35,
        radius2=0.5,
        depth=0.5,
        location=(location[0], location[1], location[2] - 0.4)
    )
    skirt = bpy.context.active_object
    skirt.name = "Armor_Skirt"
    armor_parts.append(skirt)
    
    # 金色装饰材质
    armor_mat = bpy.data.materials.new(name="ArmorMat")
    armor_mat.use_nodes = True
    armor_mat.diffuse_color = (*color, 1.0)
    armor_mat.metallic = 0.7
    armor_mat.roughness = 0.3
    
    for part in armor_parts:
        part.data.materials.append(armor_mat)
    
    return armor_parts

def create_detailed_weapon(weapon_type, location):
    """创建详细的武器"""
    if weapon_type == "sword":
        # 剑柄
        bpy.ops.mesh.primitive_cylinder_add(
            radius=0.04,
            depth=0.3,
            location=location
        )
        handle = bpy.context.active_object
        handle.name = "SwordHandle"
        
        # 护手
        bpy.ops.mesh.primitive_cube_add(
            size=0.25,
            location=(location[0], location[1], location[2] + 0.2)
        )
        guard = bpy.context.active_object
        guard.name = "SwordGuard"
        guard.scale = (1, 0.1, 0.3)
        
        # 剑刃
        bpy.ops.mesh.primitive_cube_add(
            size=0.08,
            location=(location[0], location[1], location[2] + 0.7)
        )
        blade = bpy.context.active_object
        blade.name = "SwordBlade"
        blade.scale = (0.15, 1, 3.5)
        
        # 剑尖
        bpy.ops.mesh.primitive_cone_add(
            radius1=0.06,
            radius2=0,
            depth=0.15,
            location=(location[0], location[1], location[2] + 1.4)
        )
        tip = bpy.context.active_object
        tip.name = "SwordTip"
        
        # 材质
        blade_mat = bpy.data.materials.new(name="BladeMat")
        blade_mat.use_nodes = True
        blade_mat.diffuse_color = (0.85, 0.9, 0.95, 1.0)
        blade_mat.metallic = 0.9
        blade_mat.roughness = 0.1
        
        for part in [blade, tip]:
            part.data.materials.append(blade_mat)
        
        return [handle, guard, blade, tip]
    
    elif weapon_type == "bow":
        # 弓身 - 弯曲的形状
        bpy.ops.mesh.primitive_torus_add(
            major_radius=0.5,
            minor_radius=0.03,
            location=location
        )
        bow = bpy.context.active_object
        bow.name = "BowBody"
        bow.rotation_euler = (0, math.radians(90), 0)
        bow.scale = (1, 1.5, 1)
        
        # 弓弦
        bpy.ops.mesh.primitive_cylinder_add(
            radius=0.005,
            depth=1.4,
            location=(location[0], location[1] + 0.1, location[2])
        )
        string = bpy.context.active_object
        string.name = "BowString"
        
        return [bow, string]
    
    elif weapon_type == "staff":
        # 法杖杆
        bpy.ops.mesh.primitive_cylinder_add(
            radius=0.05,
            depth=2.0,
            location=(location[0], location[1], location[2] + 1)
        )
        shaft = bpy.context.active_object
        shaft.name = "StaffShaft"
        
        # 顶部装饰
        bpy.ops.mesh.primitive_uv_sphere_add(
            radius=0.15,
            location=(location[0], location[1], location[2] + 2.1)
        )
        orb = bpy.context.active_object
        orb.name = "StaffOrb"
        
        # 发光材质
        orb_mat = bpy.data.materials.new(name="OrbMat")
        orb_mat.use_nodes = True
        orb_mat.diffuse_color = (0.3, 0.7, 1.0, 1.0)
        orb_mat.emission_strength = 2.0
        orb.data.materials.append(orb_mat)
        
        return [shaft, orb]

def create_hero_houyi():
    """创建后羿英雄 - 中国风动漫风格"""
    print("🏹 创建后羿英雄模型...")
    
    # 身体基础
    bpy.ops.mesh.primitive_cylinder_add(radius=0.15, depth=0.7, location=(0, 0, 0.9))
    body = bpy.context.active_object
    body.name = "Body"
    body.scale = (1, 0.6, 1)
    
    # 动漫风格头部
    head = create_anime_head((0, 0, 1.55), scale=1.2)
    
    # 动漫大眼睛
    eyes = create_anime_eyes((0, 0, 1.55), scale=1.2)
    
    # 中式长发
    hair = create_chinese_hair((0, 0, 1.7), style="long", color=(0.15, 0.1, 0.05))
    
    # 中式盔甲
    armor = create_chinese_armor((0, 0, 0.8), color=(0.7, 0.15, 0.1))  # 红金色盔甲
    
    # 手臂
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_cylinder_add(
            radius=0.08,
            depth=0.5,
            location=(side * 0.35, 0, 1.15)
        )
        arm = bpy.context.active_object
        arm.name = f"Arm_{side}"
        arm.rotation_euler = (0, 0, side * math.radians(10))
    
    # 腿
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_cylinder_add(
            radius=0.1,
            depth=0.7,
            location=(side * 0.15, 0, 0.35)
        )
        leg = bpy.context.active_object
        leg.name = f"Leg_{side}"
    
    # 弓
    bow = create_detailed_weapon("bow", (0.6, 0.3, 1.0))
    
    print("✅ 后羿模型创建完成")

def create_hero_daji():
    """创建妲己英雄 - 妖狐风格"""
    print("🦊 创建妲己英雄模型...")
    
    # 身体
    bpy.ops.mesh.primitive_cylinder_add(radius=0.14, depth=0.65, location=(0, 0, 0.85))
    body = bpy.context.active_object
    body.name = "Body"
    body.scale = (1, 0.55, 1)
    
    # 头部
    head = create_anime_head((0, 0, 1.45), scale=1.15)
    
    # 大眼睛
    eyes = create_anime_eyes((0, 0, 1.45), scale=1.15)
    
    # 狐耳
    for side in [-1, 1]:
        bpy.ops.mesh.primitive_cone_add(
            radius1=0.08,
            radius2=0,
            depth=0.2,
            location=(side * 0.15, 0, 1.7)
        )
        ear = bpy.context.active_object
        ear.name = f"FoxEar_{side}"
        ear.rotation_euler = (math.radians(-20), 0, side * math.radians(-15))
        
        # 粉色材质
        ear_mat = bpy.data.materials.new(name=f"FoxEarMat_{side}")
        ear_mat.use_nodes = True
        ear_mat.diffuse_color = (0.95, 0.7, 0.8, 1.0)
        ear.data.materials.append(ear_mat)
    
    # 紫色长发
    hair = create_chinese_hair((0, 0, 1.6), style="long", color=(0.4, 0.15, 0.5))
    
    # 紫色衣裙
    dress_mat = bpy.data.materials.new(name="DressMat")
    dress_mat.use_nodes = True
    dress_mat.diffuse_color = (0.6, 0.25, 0.7, 1.0)
    
    bpy.ops.mesh.primitive_cone_add(
        radius1=0.2,
        radius2=0.4,
        depth=0.8,
        location=(0, 0, 0.4)
    )
    dress = bpy.context.active_object
    dress.name = "Dress"
    dress.data.materials.append(dress_mat)
    
    # 尾巴
    bpy.ops.mesh.primitive_cone_add(
        radius1=0.1,
        radius2=0.02,
        depth=0.6,
        location=(0, -0.25, 0.4)
    )
    tail = bpy.context.active_object
    tail.name = "Tail"
    tail.rotation_euler = (math.radians(-30), 0, 0)
    
    tail_mat = bpy.data.materials.new(name="TailMat")
    tail_mat.use_nodes = True
    tail_mat.diffuse_color = (0.95, 0.7, 0.8, 1.0)
    tail.data.materials.append(tail_mat)
    
    print("✅ 妲己模型创建完成")

def export_all():
    """导出所有模型"""
    filepath = os.path.join(output_dir, "Hero_Anime_Style.fbx")
    bpy.ops.export_scene.fbx(
        filepath=filepath,
        use_selection=False,
        global_scale=1.0,
        apply_unit_scale=True,
        axis_forward='-Z',
        axis_up='Y',
        bake_space_transform=True,
        object_types={'MESH'},
        use_mesh_modifiers=True,
        mesh_smooth_type='FACE',
        use_tspace=True
    )
    print(f"✅ 已导出: {filepath}")

# ============ 主程序 ============
print("🎨 创建中国风动漫风格英雄模型...")
print("="*50)

# 创建后羿
create_hero_houyi()

# 导出
export_all()

print("\n" + "="*50)
print("✅ 高质量英雄模型创建完成！")
print("特点：")
print("  - 动漫风格头部比例")
print("  - 大眼睛设计")
print("  - 中式盔甲和服装")
print("  - 详细武器模型")
print("  - 正确的材质和颜色")
