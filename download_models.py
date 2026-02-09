#!/usr/bin/env python3
"""批量下载完成的AI 3D模型"""

import requests
import os

API_KEY = "msy_fBXjw5NW2bHf0bx4nKUafEkWzGZileyUjy3u"
OUTPUT_DIR = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/AI_Generated"

heroes = {
    "JiuWeiHu": "019c43cf-1df9-7887-bf5e-256ac6c112c6",
    "HouYi": "019c43cf-1df4-709a-a04e-0f3dfef2d2a4",
    "GongGong": "019c43cf-1df4-709b-8b25-a7c0325acc52"
}

def download_model(task_id, hero_name):
    """下载单个模型"""
    print(f"\n📥 下载 {hero_name}...")
    
    # 获取下载链接
    url = f"https://api.meshy.ai/v2/text-to-3d/{task_id}"
    headers = {"Authorization": f"Bearer {API_KEY}"}
    
    resp = requests.get(url, headers=headers)
    data = resp.json()
    
    if data.get('status') != 'SUCCEEDED':
        print(f"  ❌ {hero_name} 未完成")
        return False
    
    model_urls = data.get('model_urls', {})
    thumb_url = data.get('thumbnail_url', '')
    
    downloaded = []
    
    # 下载GLB
    if model_urls.get('glb'):
        glb_path = os.path.join(OUTPUT_DIR, f"Hero_{hero_name}_AI.glb")
        r = requests.get(model_urls['glb'])
        with open(glb_path, 'wb') as f:
            f.write(r.content)
        downloaded.append(f"GLB ({len(r.content)//1024//1024}MB)")
        print(f"  ✅ GLB 下载完成")
    
    # 下载FBX
    if model_urls.get('fbx'):
        fbx_path = os.path.join(OUTPUT_DIR, f"Hero_{hero_name}_AI.fbx")
        r = requests.get(model_urls['fbx'])
        with open(fbx_path, 'wb') as f:
            f.write(r.content)
        downloaded.append(f"FBX ({len(r.content)//1024//1024}MB)")
        print(f"  ✅ FBX 下载完成")
    
    # 下载预览图
    if thumb_url:
        thumb_path = os.path.join(OUTPUT_DIR, f"Hero_{hero_name}_AI_preview.png")
        r = requests.get(thumb_url)
        with open(thumb_path, 'wb') as f:
            f.write(r.content)
        downloaded.append(f"Preview ({len(r.content)//1024}KB)")
        print(f"  ✅ 预览图 下载完成")
    
    return True

# 下载所有模型
print("="*50)
print("🎨 批量下载AI 3D模型")
print("="*50)

for hero_name, task_id in heroes.items():
    download_model(task_id, hero_name)

print("\n" + "="*50)
print("✅ 批量下载完成！")
print("="*50)

# 显示结果
import subprocess
result = subprocess.run(['ls', '-lh', OUTPUT_DIR], capture_output=True, text=True)
print("\n📊 文件列表:")
for line in result.stdout.split('\n')[1:]:
    if '.glb' in line or '.fbx' in line or '_preview.png' in line:
        print(line)
