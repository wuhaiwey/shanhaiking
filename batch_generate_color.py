#!/usr/bin/env python3
"""
🎨 批量生成彩色纹理3D模型 - Meshy.ai
两阶段：Preview → Refine (带纹理)
"""

import requests
import json
import os
import time
from pathlib import Path

API_KEY = "msy_fBXjw5NW2bHf0bx4nKUafEkWzGZileyUjy3u"
BASE_URL = "https://api.meshy.ai/v2"
OUTPUT_DIR = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/AI_Generated_Color"

headers = {
    "Authorization": f"Bearer {API_KEY}",
    "Content-Type": "application/json"
}

heroes = [
    {
        "key": "xingtian",
        "name": "刑天(彩色)",
        "prompt": "Chinese mythological warrior Xing Tian from Shan Hai Jing, headless giant with eyes on chest and mouth on belly, wielding golden axe and bronze shield, fierce expression, ancient Chinese armor with red and gold colors, muscular body, fantasy art style, detailed colorful textures, vibrant colors, game character design, full body, standing pose",
        "type": "warrior"
    },
    {
        "key": "nuwa", 
        "name": "女娲(彩色)",
        "prompt": "Nuwa the mother goddess from Chinese mythology, elegant female deity with colorful snake lower body in green and gold scales, creating humans from clay, divine aura, flowing celestial robes in rainbow colors, gentle expression, fantasy art style, detailed colorful scale textures, full body, divine pose, vibrant colors",
        "type": "mage"
    },
    {
        "key": "jiuweihu",
        "name": "九尾狐(彩色)",
        "prompt": "Nine-tailed fox spirit from Chinese mythology, beautiful female fox demon with nine fluffy tails in pink and white, elegant flowing robes in red and gold, mystical aura, bright glowing eyes, detailed colorful fur textures, anime style, fantasy character design, full body, graceful pose, vibrant colors",
        "type": "mage"
    },
    {
        "key": "houyi",
        "name": "后羿(彩色)",
        "prompt": "Hou Yi the archer from Chinese mythology, handsome male warrior with divine golden bow, ancient Chinese hunter attire in brown and green, muscular build, confident stance, sun motif decorations in gold and orange, fantasy game character style, detailed colorful textures, full body, hero pose, vibrant colors",
        "type": "archer"
    },
    {
        "key": "gonggong",
        "name": "共工(彩色)",
        "prompt": "Gong Gong the water god from Chinese mythology, fierce male deity with water powers, blue and cyan scaled armor, silver trident weapon, raging water effects in blue and white, angry expression, serpent-like features, fantasy game character style, detailed colorful water textures, full body, battle pose, vibrant colors",
        "type": "warrior"
    }
]

def create_preview(prompt):
    """创建preview任务"""
    url = f"{BASE_URL}/text-to-3d"
    payload = {
        "prompt": prompt,
        "mode": "preview",
        "art_style": "realistic"
    }
    
    resp = requests.post(url, headers=headers, json=payload)
    if resp.status_code in [200, 202]:
        data = resp.json()
        return data.get('result')
    print(f"❌ Preview创建失败: {resp.text}")
    return None

def wait_task(task_id, task_name="任务"):
    """等待任务完成"""
    print(f"⏳ 等待{task_name}: {task_id}")
    url = f"{BASE_URL}/text-to-3d/{task_id}"
    
    for i in range(60):  # 最多10分钟
        time.sleep(10)
        resp = requests.get(url, headers=headers)
        if resp.status_code == 200:
            data = resp.json()
            status = data.get('status')
            progress = data.get('progress', 0)
            
            bar = '█' * int(progress/5) + '░' * (20-int(progress/5))
            print(f"\r  [{bar}] {progress}% | {status}", end='')
            
            if status == 'succeeded':
                print("\n✅ 完成!")
                return data
            elif status == 'failed':
                print(f"\n❌ 失败: {data.get('error', 'Unknown')}")
                return None
    
    print("\n⏱️ 超时")
    return None

def refine_to_color(preview_task_id, prompt):
    """Refine为彩色纹理版"""
    url = f"{BASE_URL}/text-to-3d"
    payload = {
        "prompt": prompt,
        "mode": "refine",
        "preview_task_id": preview_task_id,
        "art_style": "realistic",
        "texture_richness": "high"
    }
    
    resp = requests.post(url, headers=headers, json=payload)
    if resp.status_code in [200, 202]:
        data = resp.json()
        return data.get('result')
    print(f"❌ Refine创建失败: {resp.text}")
    return None

def download_model(task_data, hero_key):
    """下载模型文件"""
    model_urls = task_data.get('model_urls', {})
    thumb_url = task_data.get('thumbnail_url', '')
    
    Path(OUTPUT_DIR).mkdir(parents=True, exist_ok=True)
    
    files = []
    
    # 下载GLB
    if model_urls.get('glb'):
        path = f"{OUTPUT_DIR}/Hero_{hero_key}_COLOR.glb"
        r = requests.get(model_urls['glb'])
        with open(path, 'wb') as f:
            f.write(r.content)
        files.append(f"GLB ({len(r.content)//1024//1024}MB)")
    
    # 下载FBX
    if model_urls.get('fbx'):
        path = f"{OUTPUT_DIR}/Hero_{hero_key}_COLOR.fbx"
        r = requests.get(model_urls['fbx'])
        with open(path, 'wb') as f:
            f.write(r.content)
        files.append(f"FBX ({len(r.content)//1024//1024}MB)")
    
    # 下载预览图
    if thumb_url:
        path = f"{OUTPUT_DIR}/Hero_{hero_key}_COLOR_preview.png"
        r = requests.get(thumb_url)
        with open(path, 'wb') as f:
            f.write(r.content)
        files.append(f"Preview ({len(r.content)//1024}KB)")
    
    return files

def process_hero(hero):
    """处理单个英雄"""
    print(f"\n{'='*60}")
    print(f"🎨 生成彩色纹理: {hero['name']}")
    print(f"{'='*60}")
    
    # 阶段1: Preview
    print("\n📍 阶段1/2: Preview...")
    preview_id = create_preview(hero['prompt'])
    if not preview_id:
        return False
    
    preview_data = wait_task(preview_id, "Preview")
    if not preview_data:
        return False
    
    # 阶段2: Refine彩色版
    print("\n📍 阶段2/2: Refine彩色纹理...")
    refine_id = refine_to_color(preview_id, hero['prompt'])
    if not refine_id:
        return False
    
    refine_data = wait_task(refine_id, "Refine彩色版")
    if not refine_data:
        return False
    
    # 下载
    print("\n📥 下载彩色模型...")
    files = download_model(refine_data, hero['key'])
    
    print(f"✅ {hero['name']} 完成!")
    for f in files:
        print(f"   • {f}")
    
    return True

def main():
    print("="*60)
    print("🐉 山海经英雄 - 彩色纹理3D模型批量生成")
    print("="*60)
    print(f"\n🎯 目标: 生成 {len(heroes)} 个英雄的彩色纹理版本")
    print("⏱️  每个英雄约需: 8-15分钟")
    print(f"⏱️  预计总时间: {len(heroes)*10}分钟")
    
    success_count = 0
    
    for hero in heroes:
        if process_hero(hero):
            success_count += 1
        print(f"\n📊 进度: {success_count}/{len(heroes)} 完成")
    
    print("\n" + "="*60)
    print(f"✅ 批量生成完成! {success_count}/{len(heroes)} 成功")
    print("="*60)

if __name__ == "__main__":
    main()
