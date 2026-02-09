#!/usr/bin/env python3
"""
🎨 AI生成3D模型工具 - Meshy.ai (v2 API 修复版)
生成山海经英雄角色的3D模型
"""

import requests
import json
import os
import time
from pathlib import Path

class Meshy3DGenerator:
    """Meshy.ai 3D模型生成器 - 使用v2 API"""
    
    def __init__(self, api_key=None):
        self.api_key = api_key or os.getenv('MESHY_API_KEY')
        self.base_url = "https://api.meshy.ai/v2"  # ✅ 正确的API版本
        self.headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json"
        }
    
    def text_to_3d(self, prompt, mode="preview", art_style="realistic"):
        """
        文字生成3D模型
        """
        endpoint = f"{self.base_url}/text-to-3d"
        
        payload = {
            "prompt": prompt,
            "mode": mode,
            "art_style": art_style,
            "negative_prompt": "low quality, blurry, deformed, ugly, bad anatomy"
        }
        
        print(f"📤 发送请求...")
        print(f"📝 提示词: {prompt[:60]}...")
        
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


# ==================== 山海经英雄配置 ====================

SHANHAI_HEROES = {
    "xingtian": {
        "name": "刑天",
        "prompt": "Chinese mythological warrior Xing Tian from Shan Hai Jing, headless giant with eyes on chest and mouth on belly, wielding axe and shield, fierce expression, ancient Chinese armor, muscular body, fantasy art style, detailed textures, game character design, full body, standing pose",
        "art_style": "realistic",
        "mode": "preview",
        "description": "战神 - 以乳为目，以脐为口"
    },
    "jiuweihu": {
        "name": "九尾狐",
        "prompt": "Nine-tailed fox spirit from Chinese mythology, beautiful female fox demon with nine fluffy tails, elegant flowing robes, mystical aura, pink and white color scheme, anime style, detailed fur, fantasy character design, full body, graceful pose",
        "art_style": "realistic",
        "mode": "preview",
        "description": "青丘妖狐 - 其状如狐而九尾"
    },
    "houyi": {
        "name": "后羿",
        "prompt": "Hou Yi the archer from Chinese mythology, handsome male warrior with divine bow, ancient Chinese hunter attire, muscular build, confident stance, sun motif decorations, fantasy game character style, detailed textures, full body, hero pose",
        "art_style": "realistic",
        "mode": "preview",
        "description": "射日英雄 - 帝俊赐羿彤弓素矰"
    },
    "nuwa": {
        "name": "女娲",
        "prompt": "Nuwa the mother goddess from Chinese mythology, elegant female deity with snake lower body, creating humans from clay, divine aura, flowing celestial robes, rainbow colors, gentle expression, fantasy art style, detailed scales, full body, divine pose",
        "art_style": "realistic",
        "mode": "preview",
        "description": "创世女神 - 女娲补天"
    },
    "gonggong": {
        "name": "共工",
        "prompt": "Gong Gong the water god from Chinese mythology, fierce male deity with water powers, blue scaled armor, trident weapon, raging water effects, angry expression, serpent-like features, fantasy game character style, detailed water textures, full body, battle pose",
        "art_style": "realistic",
        "mode": "preview",
        "description": "水神 - 怒触不周山"
    }
}


def generate_hero_model(hero_key, api_key=None):
    """生成单个英雄模型"""
    hero = SHANHAI_HEROES.get(hero_key)
    if not hero:
        print(f"❌ 未知英雄: {hero_key}")
        return False
    
    print(f"\n{'='*60}")
    print(f"🎨 正在生成: {hero['name']}")
    print(f"📝 描述: {hero['description']}")
    print(f"🎭 风格: {hero['art_style']}")
    print(f"{'='*60}")
    
    # 检查API密钥
    if not api_key:
        api_key = os.getenv('MESHY_API_KEY')
    
    if not api_key:
        print("❌ 错误: 未设置 MESHY_API_KEY")
        return False
    
    # 创建生成器
    generator = Meshy3DGenerator(api_key)
    
    # 提交生成任务
    result = generator.text_to_3d(
        prompt=hero['prompt'],
        mode=hero['mode'],
        art_style=hero['art_style']
    )
    
    if not result:
        return False
    
    task_id = result.get('result')
    if not task_id:
        print(f"❌ 未获取到任务ID")
        return False
    
    print(f"✅ 任务ID: {task_id}")
    print(f"⏳ 等待生成完成...")
    
    # 等待生成完成
    max_wait = 600  # 最多10分钟
    elapsed = 0
    check_interval = 10  # 每10秒检查一次
    
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
            
            if model_urls:
                # 创建输出目录
                output_dir = Path("/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/AI_Generated")
                output_dir.mkdir(parents=True, exist_ok=True)
                
                downloaded_files = []
                
                # 下载所有可用格式
                for fmt, url in model_urls.items():
                    if url:
                        output_path = output_dir / f"Hero_{hero_key}_AI.{fmt}"
                        if generator.download_model(url, output_path):
                            downloaded_files.append(output_path)
                
                if downloaded_files:
                    print(f"\n✅ {hero['name']} 模型生成完成!")
                    print(f"📁 文件列表:")
                    for f in downloaded_files:
                        size = f.stat().st_size / 1024  # KB
                        print(f"   • {f.name} ({size:.1f} KB)")
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
    print("   Powered by Meshy.ai (v2 API)")
    print("="*60)
    
    # 显示可用英雄
    print("\n📋 可用英雄列表:")
    for key, hero in SHANHAI_HEROES.items():
        print(f"  • {key:12} - {hero['name']} ({hero['description']})")
    
    # 检查命令行参数
    if len(sys.argv) > 1:
        hero_key = sys.argv[1]
        if hero_key in SHANHAI_HEROES:
            success = generate_hero_model(hero_key)
            return 0 if success else 1
        else:
            print(f"\n❌ 未知英雄: {hero_key}")
            print("可用: xingtian, jiuweihu, houyi, nuwa, gonggong")
            return 1
    else:
        print("\n💡 使用方法:")
        print(f"  python3 {sys.argv[0]} <hero_key>")
        print(f"\n示例:")
        print(f"  python3 {sys.argv[0]} xingtian    # 生成刑天")
        print(f"  python3 {sys.argv[0]} jiuweihu  # 生成九尾狐")
        print(f"  python3 {sys.argv[0]} houyi     # 生成后羿")
        return 0


if __name__ == "__main__":
    exit(main())
