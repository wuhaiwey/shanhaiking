#!/usr/bin/env python3
"""
🎨 AI生成3D模型工具 - Meshy.ai 集成
生成山海经英雄角色的3D模型
"""

import requests
import json
import os
import time
from pathlib import Path

class Meshy3DGenerator:
    """Meshy.ai 3D模型生成器"""
    
    def __init__(self, api_key=None):
        self.api_key = api_key or os.getenv('MESHY_API_KEY')
        self.base_url = "https://api.meshy.ai/v1"
        self.headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json"
        }
    
    def text_to_3d(self, prompt, style="realistic", art_style="chinese_fantasy"):
        """
        文字生成3D模型
        
        Args:
            prompt: 描述文本
            style: 风格 (realistic/stylized/low_poly)
            art_style: 艺术风格
        """
        endpoint = f"{self.base_url}/text-to-3d"
        
        payload = {
            "prompt": prompt,
            "style": style,
            "art_style": art_style,
            "negative_prompt": "low quality, blurry, deformed",
            "resolution": 1024,
            "enable_pbr": True
        }
        
        try:
            response = requests.post(endpoint, headers=self.headers, json=payload)
            response.raise_for_status()
            return response.json()
        except Exception as e:
            print(f"❌ 生成失败: {e}")
            return None
    
    def image_to_3d(self, image_path, style="realistic"):
        """
        图片生成3D模型
        
        Args:
            image_path: 图片文件路径
            style: 风格
        """
        endpoint = f"{self.base_url}/image-to-3d"
        
        with open(image_path, 'rb') as f:
            files = {'image': f}
            data = {'style': style}
            
            try:
                response = requests.post(
                    endpoint, 
                    headers={"Authorization": f"Bearer {self.api_key}"},
                    files=files,
                    data=data
                )
                response.raise_for_status()
                return response.json()
            except Exception as e:
                print(f"❌ 生成失败: {e}")
                return None
    
    def check_status(self, task_id):
        """检查生成状态"""
        endpoint = f"{self.base_url}/tasks/{task_id}"
        
        try:
            response = requests.get(endpoint, headers=self.headers)
            response.raise_for_status()
            return response.json()
        except Exception as e:
            print(f"❌ 检查失败: {e}")
            return None
    
    def download_model(self, model_url, output_path):
        """下载生成的模型"""
        try:
            response = requests.get(model_url)
            response.raise_for_status()
            
            with open(output_path, 'wb') as f:
                f.write(response.content)
            
            print(f"✅ 模型已下载: {output_path}")
            return True
        except Exception as e:
            print(f"❌ 下载失败: {e}")
            return False


# ==================== 山海经英雄生成配置 ====================

SHANHAI_HEROES = {
    "xingtian": {
        "name": "刑天",
        "prompt": """Chinese mythological warrior Xing Tian from Shan Hai Jing, 
        headless giant with eyes on chest and mouth on belly, 
        wielding axe and shield, fierce expression, 
        ancient Chinese armor, muscular body, 
        fantasy art style, detailed textures, 
        game character design, full body, standing pose""",
        "style": "stylized",
        "description": "战神 - 以乳为目，以脐为口"
    },
    
    "jiuweihu": {
        "name": "九尾狐",
        "prompt": """Nine-tailed fox spirit from Chinese mythology, 
        beautiful female fox demon with nine fluffy tails, 
        elegant flowing robes, mystical aura, 
        pink and white color scheme, 
        anime style, detailed fur, 
        fantasy character design, full body, graceful pose""",
        "style": "stylized",
        "description": "青丘妖狐 - 其状如狐而九尾"
    },
    
    "houyi": {
        "name": "后羿",
        "prompt": """Hou Yi the archer from Chinese mythology, 
        handsome male warrior with divine bow, 
        ancient Chinese hunter attire, 
        muscular build, confident stance, 
        sun motif decorations, 
        fantasy game character style, 
        detailed textures, full body, hero pose""",
        "style": "realistic",
        "description": "射日英雄 - 帝俊赐羿彤弓素矰"
    },
    
    "nuwa": {
        "name": "女娲",
        "prompt": """Nuwa the mother goddess from Chinese mythology, 
        elegant female deity with snake lower body, 
        creating humans from clay, 
        divine aura, flowing celestial robes, 
        rainbow colors, gentle expression, 
        fantasy art style, detailed scales, 
        full body, divine pose""",
        "style": "stylized",
        "description": "创世女神 - 女娲补天"
    },
    
    "gonggong": {
        "name": "共工",
        "prompt": """Gong Gong the water god from Chinese mythology, 
        fierce male deity with water powers, 
        blue scaled armor, trident weapon, 
        raging water effects, angry expression, 
        serpent-like features, 
        fantasy game character style, 
        detailed water textures, full body, battle pose""",
        "style": "realistic",
        "description": "水神 - 怒触不周山"
    }
}


def generate_hero_model(hero_key, api_key=None):
    """生成单个英雄模型"""
    hero = SHANHAI_HEROES.get(hero_key)
    if not hero:
        print(f"❌ 未知英雄: {hero_key}")
        return False
    
    print(f"\n🎨 正在生成: {hero['name']}")
    print(f"📝 描述: {hero['description']}")
    print(f"🎭 风格: {hero['style']}")
    print("-" * 50)
    
    # 检查API密钥
    if not api_key:
        api_key = os.getenv('MESHY_API_KEY')
    
    if not api_key:
        print("❌ 错误: 未设置 MESHY_API_KEY 环境变量")
        print("💡 请先在 https://www.meshy.ai/ 注册并获取API密钥")
        print("💡 然后设置: export MESHY_API_KEY='your_key_here'")
        return False
    
    # 创建生成器
    generator = Meshy3DGenerator(api_key)
    
    # 提交生成任务
    result = generator.text_to_3d(
        prompt=hero['prompt'],
        style=hero['style'],
        art_style="chinese_fantasy"
    )
    
    if not result:
        return False
    
    task_id = result.get('task_id')
    print(f"✅ 任务已提交: {task_id}")
    
    # 等待生成完成
    print("⏳ 等待生成完成...")
    max_wait = 300  # 最多等待5分钟
    elapsed = 0
    
    while elapsed < max_wait:
        time.sleep(10)
        elapsed += 10
        
        status = generator.check_status(task_id)
        if not status:
            continue
        
        state = status.get('status', 'pending')
        print(f"  状态: {state} ({elapsed}s)")
        
        if state == 'completed':
            model_url = status.get('model_url')
            if model_url:
                # 下载模型
                output_dir = Path("/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/AI_Generated")
                output_dir.mkdir(parents=True, exist_ok=True)
                
                output_path = output_dir / f"Hero_{hero_key}_AI.glb"
                
                if generator.download_model(model_url, str(output_path)):
                    print(f"✅ {hero['name']} 模型生成完成!")
                    return True
            break
        
        elif state == 'failed':
            print(f"❌ 生成失败: {status.get('error', 'Unknown error')}")
            return False
    
    print("⏱️ 生成超时")
    return False


def main():
    """主函数"""
    import sys
    
    print("="*60)
    print("🐉 山海经英雄 AI 3D模型生成器")
    print("   Powered by Meshy.ai")
    print("="*60)
    
    # 显示可用英雄
    print("\n📋 可用英雄列表:")
    for key, hero in SHANHAI_HEROES.items():
        print(f"  • {key}: {hero['name']} - {hero['description']}")
    
    # 检查命令行参数
    if len(sys.argv) > 1:
        hero_key = sys.argv[1]
        generate_hero_model(hero_key)
    else:
        print("\n💡 使用方法:")
        print(f"  python3 {sys.argv[0]} <hero_key>")
        print(f"\n示例:")
        print(f"  python3 {sys.argv[0]} xingtian")
        print(f"  python3 {sys.argv[0]} jiuweihu")
        
        # 询问要生成哪个
        print("\n🎯 请选择要生成的英雄 (输入key):")
        choice = input("> ").strip().lower()
        
        if choice in SHANHAI_HEROES:
            generate_hero_model(choice)
        else:
            print(f"❌ 无效的选择: {choice}")


if __name__ == "__main__":
    main()
