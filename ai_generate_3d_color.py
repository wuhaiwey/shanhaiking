#!/usr/bin/env python3
"""
🎨 AI生成3D模型工具 - Meshy.ai (彩色纹理版)
生成带颜色纹理的山海经英雄3D模型
"""

import requests
import json
import os
import time
from pathlib import Path

class Meshy3DGenerator:
    """Meshy.ai 3D模型生成器 - 彩色纹理版"""
    
    def __init__(self, api_key=None):
        self.api_key = api_key or os.getenv('MESHY_API_KEY')
        self.base_url = "https://api.meshy.ai/v2"
        self.headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json"
        }
    
    def text_to_3d(self, prompt, mode="preview", art_style="realistic", texture_richness="high"):
        """
        文字生成带纹理的3D模型
        
        Args:
            prompt: 描述文本
            mode: preview/refine (refine有更好的纹理)
            art_style: realistic/stylized
            texture_richness: none/low/medium/high (纹理丰富度)
        """
        endpoint = f"{self.base_url}/text-to-3d"
        
        payload = {
            "prompt": prompt,
            "mode": mode,
            "art_style": art_style,
            "texture_richness": texture_richness,  # ✅ 启用彩色纹理
            "negative_prompt": "low quality, blurry, deformed, ugly, bad anatomy, monochrome, grayscale"
        }
        
        print(f"📤 发送请求...")
        print(f"📝 提示词: {prompt[:60]}...")
        print(f"🎨 纹理级别: {texture_richness}")
        
        try:
            response = requests.post(endpoint, headers=self.headers, json=payload)
            print(f"📊 响应状态: {response.status_code}")
            
            if response.status_code not in [200, 202]:
                print(f"❌ 错误: {response.text}")
                return None
                
            data = response.json()
            print(f"✅ 任务创建成功")
            return data
        except Exception as e:
            print(f"❌ 请求失败: {e}")
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
    
    def download_texture(self, texture_url, output_path):
        """下载纹理文件"""
        try:
            response = requests.get(texture_url)
            response.raise_for_status()
            
            with open(output_path, 'wb') as f:
                f.write(response.content)
            
            print(f"✅ 纹理已下载: {output_path}")
            return True
        except Exception as e:
            print(f"❌ 纹理下载失败: {e}")
            return False


# ==================== 山海经英雄配置（彩色纹理版）====================

SHANHAI_HEROES = {
    "xingtian_color": {
        "name": "刑天(彩色版)",
        "prompt": "Chinese mythological warrior Xing Tian from Shan Hai Jing, headless giant with eyes on chest and mouth on belly, wielding golden axe and bronze shield, fierce expression, ancient Chinese armor with red and gold colors, muscular body, fantasy art style, detailed colorful textures, vibrant colors, game character design, full body, standing pose, highly detailed",
        "art_style": "realistic",
        "mode": "refine",  # ✅ 使用refine模式获得更好纹理
        "texture_richness": "high",
        "description": "战神 - 带彩色纹理"
    },
    
    "jiuweihu_color": {
        "name": "九尾狐(彩色版)",
        "prompt": "Nine-tailed fox spirit from Chinese mythology, beautiful female fox demon with nine fluffy tails in pink and white, elegant flowing robes in red and gold, mystical aura, bright glowing eyes, detailed colorful fur textures, anime style, fantasy character design, full body, graceful pose, vibrant colors",
        "art_style": "realistic",
        "mode": "refine",
        "texture_richness": "high",
        "description": "青丘妖狐 - 带彩色纹理"
    },
    
    "houyi_color": {
        "name": "后羿(彩色版)",
        "prompt": "Hou Yi the archer from Chinese mythology, handsome male warrior with divine golden bow, ancient Chinese hunter attire in brown and green, muscular build, confident stance, sun motif decorations in gold and orange, fantasy game character style, detailed colorful textures, full body, hero pose, vibrant colors",
        "art_style": "realistic",
        "mode": "refine",
        "texture_richness": "high",
        "description": "射日英雄 - 带彩色纹理"
    },
    
    "nuwa_color": {
        "name": "女娲(彩色版)",
        "prompt": "Nuwa the mother goddess from Chinese mythology, elegant female deity with colorful snake lower body in green and gold scales, creating humans from clay, divine aura, flowing celestial robes in rainbow colors, gentle expression, fantasy art style, detailed colorful scale textures, full body, divine pose, vibrant colors",
        "art_style": "realistic",
        "mode": "refine",
        "texture_richness": "high",
        "description": "创世女神 - 带彩色纹理"
    },
    
    "gonggong_color": {
        "name": "共工(彩色版)",
        "prompt": "Gong Gong the water god from Chinese mythology, fierce male deity with water powers, blue and cyan scaled armor, silver trident weapon, raging water effects in blue and white, angry expression, serpent-like features, fantasy game character style, detailed colorful water textures, full body, battle pose, vibrant colors",
        "art_style": "realistic",
        "mode": "refine",
        "texture_richness": "high",
        "description": "水神 - 带彩色纹理"
    }
}


def generate_hero_model(hero_key, api_key=None):
    """生成单个带彩色纹理的英雄模型"""
    hero = SHANHAI_HEROES.get(hero_key)
    if not hero:
        print(f"❌ 未知英雄: {hero_key}")
        return False
    
    print(f"\n{'='*60}")
    print(f"🎨 正在生成: {hero['name']}")
    print(f"📝 描述: {hero['description']}")
    print(f"🎨 纹理: {hero['texture_richness']} | 模式: {hero['mode']}")
    print(f"{'='*60}")
    
    # 检查API密钥
    if not api_key:
        api_key = os.getenv('MESHY_API_KEY')
    
    if not api_key:
        print("❌ 错误: 未设置 MESHY_API_KEY")
        return False
    
    # 创建生成器
    generator = Meshy3DGenerator(api_key)
    
    # 提交生成任务（带纹理）
    result = generator.text_to_3d(
        prompt=hero['prompt'],
        mode=hero['mode'],
        art_style=hero['art_style'],
        texture_richness=hero['texture_richness']
    )
    
    if not result:
        return False
    
    task_id = result.get('result')
    if not task_id:
        print(f"❌ 未获取到任务ID")
        return False
    
    print(f"✅ 任务ID: {task_id}")
    print(f"⏳ 等待生成完成... (refine模式需要更长时间)")
    
    # 等待生成完成
    max_wait = 1800  # 最多30分钟（refine模式更慢）
    elapsed = 0
    check_interval = 15  # 每15秒检查一次
    
    while elapsed < max_wait:
        time.sleep(check_interval)
        elapsed += check_interval
        
        status = generator.check_status(task_id)
        if not status:
            continue
        
        task_status = status.get('status', 'pending')
        progress = status.get('progress', 0)
        
        # 显示进度条
        bar_length = 20
        filled = int(bar_length * progress / 100)
        bar = '█' * filled + '░' * (bar_length - filled)
        print(f"\r  [{bar}] {progress}% | {task_status} | {elapsed}s", end='', flush=True)
        
        if task_status == 'succeeded':
            print("\n")  # 换行
            
            # 获取模型URL
            model_urls = status.get('model_urls', {})
            texture_urls = status.get('texture_urls', [])
            
            if model_urls:
                # 创建输出目录
                output_dir = Path("/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/AI_Generated_Color")
                output_dir.mkdir(parents=True, exist_ok=True)
                
                downloaded_files = []
                
                # 下载所有可用格式
                for fmt, url in model_urls.items():
                    if url:
                        output_path = output_dir / f"Hero_{hero_key}.{fmt}"
                        if generator.download_model(url, output_path):
                            downloaded_files.append(output_path)
                
                # 下载纹理文件
                for i, tex_url in enumerate(texture_urls):
                    if tex_url:
                        tex_path = output_dir / f"Hero_{hero_key}_texture_{i}.png"
                        if generator.download_texture(tex_url, tex_path):
                            downloaded_files.append(tex_path)
                
                if downloaded_files:
                    print(f"\n✅ {hero['name']} 彩色模型生成完成!")
                    print(f"📁 文件列表:")
                    for f in downloaded_files:
                        size = f.stat().st_size / 1024 / 1024  # MB
                        print(f"   • {f.name} ({size:.1f} MB)")
                    return True
            
            print("❌ 未找到模型文件")
            return False
        
        elif task_status == 'failed':
            print(f"\n❌ 生成失败: {status.get('error', 'Unknown')}")
            return False
        elif task_status == 'cancelled':
            print("\n❌ 任务被取消")
            return False
    
    print("\n⏱️ 生成超时")
    return False


def main():
    """主函数"""
    import sys
    
    print("="*60)
    print("🐉 山海经英雄 AI 3D模型生成器")
    print("   彩色纹理版 - Powered by Meshy.ai")
    print("="*60)
    
    # 显示可用英雄
    print("\n📋 可用英雄列表:")
    for key, hero in SHANHAI_HEROES.items():
        print(f"  • {key:16} - {hero['name']}")
        print(f"    {hero['description']}")
    
    # 检查命令行参数
    if len(sys.argv) > 1:
        hero_key = sys.argv[1]
        if hero_key in SHANHAI_HEROES:
            success = generate_hero_model(hero_key)
            return 0 if success else 1
        else:
            print(f"\n❌ 未知英雄: {hero_key}")
            print("可用: xingtian_color, jiuweihu_color, houyi_color, nuwa_color, gonggong_color")
            return 1
    else:
        print("\n💡 使用方法:")
        print(f"  python3 {sys.argv[0]} <hero_key>")
        print(f"\n示例:")
        print(f"  python3 {sys.argv[0]} xingtian_color")
        print(f"\n⚠️  注意: refine模式生成时间更长(5-15分钟)，但有更好的彩色纹理")
        return 0


if __name__ == "__main__":
    exit(main())
