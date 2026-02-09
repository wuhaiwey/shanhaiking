#!/usr/bin/env python3
"""
🎨 AI生成3D模型工具 - Meshy.ai (先preview后refine彩色版)
先生成preview，再自动refine获得彩色纹理
"""

import requests
import json
import os
import time
from pathlib import Path

class Meshy3DGenerator:
    """Meshy.ai 3D模型生成器 - 两阶段生成"""
    
    def __init__(self, api_key=None):
        self.api_key = api_key or os.getenv('MESHY_API_KEY')
        self.base_url = "https://api.meshy.ai/v2"
        self.headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json"
        }
    
    def text_to_3d_preview(self, prompt, art_style="realistic"):
        """第一阶段：生成preview（快速预览）"""
        endpoint = f"{self.base_url}/text-to-3d"
        
        payload = {
            "prompt": prompt,
            "mode": "preview",
            "art_style": art_style,
            "negative_prompt": "low quality, blurry, deformed, ugly, bad anatomy"
        }
        
        print(f"📤 阶段1: 生成Preview...")
        
        try:
            response = requests.post(endpoint, headers=self.headers, json=payload)
            if response.status_code not in [200, 202]:
                print(f"❌ Preview错误: {response.text}")
                return None
            return response.json()
        except Exception as e:
            print(f"❌ Preview请求失败: {e}")
            return None
    
    def refine_to_color(self, preview_task_id, prompt, art_style="realistic"):
        """第二阶段：Refine为彩色纹理版"""
        endpoint = f"{self.base_url}/text-to-3d"
        
        payload = {
            "prompt": prompt,
            "mode": "refine",
            "preview_task_id": preview_task_id,  # ✅ 需要preview任务ID
            "art_style": art_style,
            "texture_richness": "high",
            "negative_prompt": "low quality, blurry, deformed, ugly, bad anatomy, monochrome"
        }
        
        print(f"📤 阶段2: Refine为彩色版...")
        print(f"🎨 使用Preview ID: {preview_task_id}")
        
        try:
            response = requests.post(endpoint, headers=self.headers, json=payload)
            if response.status_code not in [200, 202]:
                print(f"❌ Refine错误: {response.text}")
                return None
            return response.json()
        except Exception as e:
            print(f"❌ Refine请求失败: {e}")
            return None
    
    def check_status(self, task_id):
        """检查生成状态"""
        endpoint = f"{self.base_url}/text-to-3d/{task_id}"
        try:
            response = requests.get(endpoint, headers=self.headers)
            response.raise_for_status()
            return response.json()
        except Exception as e:
            print(f"❌ 检查失败: {e}")
            return None
    
    def download_model(self, model_url, output_path):
        """下载模型文件"""
        try:
            response = requests.get(model_url)
            response.raise_for_status()
            with open(output_path, 'wb') as f:
                f.write(response.content)
            print(f"✅ 已下载: {output_path}")
            return True
        except Exception as e:
            print(f"❌ 下载失败: {e}")
            return False


# ==================== 山海经英雄配置（彩色版）====================

SHANHAI_HEROES = {
    "xingtian_color": {
        "name": "刑天(彩色版)",
        "prompt": "Chinese mythological warrior Xing Tian from Shan Hai Jing, headless giant with eyes on chest and mouth on belly, wielding golden axe and bronze shield, fierce expression, ancient Chinese armor with red and gold colors, muscular body, fantasy art style, detailed colorful textures, vibrant colors, game character design, full body, standing pose, highly detailed",
        "art_style": "realistic",
        "description": "战神 - 带彩色纹理"
    },
    "jiuweihu_color": {
        "name": "九尾狐(彩色版)",
        "prompt": "Nine-tailed fox spirit from Chinese mythology, beautiful female fox demon with nine fluffy tails in pink and white, elegant flowing robes in red and gold, mystical aura, bright glowing eyes, detailed colorful fur textures, anime style, fantasy character design, full body, graceful pose, vibrant colors",
        "art_style": "realistic",
        "description": "青丘妖狐 - 带彩色纹理"
    },
    "houyi_color": {
        "name": "后羿(彩色版)",
        "prompt": "Hou Yi the archer from Chinese mythology, handsome male warrior with divine golden bow, ancient Chinese hunter attire in brown and green, muscular build, confident stance, sun motif decorations in gold and orange, fantasy game character style, detailed colorful textures, full body, hero pose, vibrant colors",
        "art_style": "realistic",
        "description": "射日英雄 - 带彩色纹理"
    }
}


def wait_for_task(generator, task_id, task_name):
    """等待任务完成"""
    print(f"⏳ 等待{task_name}完成...")
    max_wait = 900  # 15分钟
    elapsed = 0
    check_interval = 10
    
    while elapsed < max_wait:
        time.sleep(check_interval)
        elapsed += check_interval
        
        status = generator.check_status(task_id)
        if not status:
            continue
        
        task_status = status.get('status', 'pending')
        progress = status.get('progress', 0)
        
        bar_length = 20
        filled = int(bar_length * progress / 100)
        bar = '█' * filled + '░' * (bar_length - filled)
        print(f"\r  [{bar}] {progress}% | {task_status} | {elapsed}s", end='', flush=True)
        
        if task_status == 'succeeded':
            print("\n")
            return status
        elif task_status == 'failed':
            print(f"\n❌ {task_name}失败")
            return None
    
    print("\n⏱️ 超时")
    return None


def generate_color_hero(hero_key, api_key=None):
    """生成带彩色纹理的英雄 - 两阶段流程"""
    hero = SHANHAI_HEROES.get(hero_key)
    if not hero:
        print(f"❌ 未知英雄: {hero_key}")
        return False
    
    print(f"\n{'='*60}")
    print(f"🎨 彩色纹理生成: {hero['name']}")
    print(f"📝 {hero['description']}")
    print(f"{'='*60}")
    
    if not api_key:
        api_key = os.getenv('MESHY_API_KEY')
    if not api_key:
        print("❌ 未设置API密钥")
        return False
    
    generator = Meshy3DGenerator(api_key)
    output_dir = Path("/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/AI_Generated_Color")
    output_dir.mkdir(parents=True, exist_ok=True)
    
    # ========== 阶段1: Preview ==========
    print("\n📍 阶段1/2: 生成Preview模型...")
    preview_result = generator.text_to_3d_preview(hero['prompt'], hero['art_style'])
    
    if not preview_result:
        return False
    
    preview_task_id = preview_result.get('result')
    print(f"✅ Preview任务: {preview_task_id}")
    
    # 等待preview完成
    preview_status = wait_for_task(generator, preview_task_id, "Preview")
    if not preview_status:
        return False
    
    print("✅ Preview完成！")
    
    # 下载preview版本（备用）
    preview_urls = preview_status.get('model_urls', {})
    if preview_urls.get('glb'):
        generator.download_model(preview_urls['glb'], output_dir / f"Hero_{hero_key}_preview.glb")
    
    # ========== 阶段2: Refine to Color ==========
    print("\n📍 阶段2/2: Refine为彩色纹理版...")
    refine_result = generator.refine_to_color(preview_task_id, hero['prompt'], hero['art_style'])
    
    if not refine_result:
        return False
    
    refine_task_id = refine_result.get('result')
    print(f"✅ Refine任务: {refine_task_id}")
    
    # 等待refine完成（需要更长时间）
    print("⏳ Refine需要5-15分钟，请耐心等待...")
    refine_status = wait_for_task(generator, refine_task_id, "Refine彩色版")
    
    if not refine_status:
        return False
    
    # 下载彩色版本
    print("\n📥 下载彩色模型...")
    model_urls = refine_status.get('model_urls', {})
    texture_urls = refine_status.get('texture_urls', [])
    
    downloaded = []
    
    for fmt, url in model_urls.items():
        if url:
            path = output_dir / f"Hero_{hero_key}_COLOR.{fmt}"
            if generator.download_model(url, path):
                downloaded.append(path)
    
    for i, tex_url in enumerate(texture_urls):
        if tex_url:
            path = output_dir / f"Hero_{hero_key}_COLOR_texture_{i}.png"
            if generator.download_model(tex_url, path):
                downloaded.append(path)
    
    if downloaded:
        print(f"\n✅ {hero['name']} 彩色模型生成完成！")
        print(f"📁 文件列表:")
        for f in downloaded:
            size_mb = f.stat().st_size / 1024 / 1024
            print(f"   • {f.name} ({size_mb:.1f} MB)")
        return True
    
    return False


def main():
    import sys
    
    print("="*60)
    print("🐉 山海经英雄 彩色纹理3D模型生成器")
    print("   两阶段: Preview → Refine")
    print("="*60)
    
    print("\n📋 可用英雄:")
    for key, hero in SHANHAI_HEROES.items():
        print(f"  • {key} - {hero['name']}")
    
    if len(sys.argv) > 1:
        hero_key = sys.argv[1]
        if hero_key in SHANHAI_HEROES:
            success = generate_color_hero(hero_key)
            return 0 if success else 1
        else:
            print(f"\n❌ 未知英雄: {hero_key}")
            return 1
    else:
        print("\n💡 用法: python3 ai_generate_3d_color_v2.py xingtian_color")
        print("\n⏱️  总时间约: 5-15分钟")
        return 0


if __name__ == "__main__":
    exit(main())
