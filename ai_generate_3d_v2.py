#!/usr/bin/env python3
"""
🎨 AI生成3D模型工具 - Meshy.ai 集成 (修复版)
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
        self.base_url = "https://api.meshy.ai/openapi/v1"  # 修复：正确的API端点
        self.headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json"
        }
    
    def text_to_3d(self, prompt, mode="preview", art_style="realistic"):
        """
        文字生成3D模型 - Meshy.ai最新API
        
        Args:
            prompt: 描述文本
            mode: preview/refine (preview快速，refine高质量)
            art_style: realistic/stylized/low-poly
        """
        endpoint = f"{self.base_url}/text-to-3d"
        
        payload = {
            "prompt": prompt,
            "mode": mode,
            "art_style": art_style,
            "negative_prompt": "low quality, blurry, deformed, ugly"
        }
        
        print(f"📤 发送请求到: {endpoint}")
        print(f"📝 提示词: {prompt[:50]}...")
        
        try:
            response = requests.post(endpoint, headers=self.headers, json=payload)
            print(f"📊 响应状态: {response.status_code}")
            
            if response.status_code != 200:
                print(f"❌ 错误响应: {response.text}")
                return None
                
            return response.json()
        except Exception as e:
            print(f"❌ 生成失败: {e}")
            return None
    
    def image_to_3d(self, image_path, mode="preview"):
        """
        图片生成3D模型
        
        Args:
            image_path: 图片文件路径
            mode: preview/refine
        """
        endpoint = f"{self.base_url}/image-to-3d"
        
        with open(image_path, 'rb') as f:
            files = {'image': f}
            data = {'mode': mode}
            headers = {"Authorization": f"Bearer {self.api_key}"}
            
            try:
                response = requests.post(endpoint, headers=headers, files=files, data=data)
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
    
    def download_model(self, model_urls, output_dir, hero_name):
        """下载生成的模型"""
        output_path = Path(output_dir)
        output_path.mkdir(parents=True, exist_ok=True)
        
        downloaded = []
        
        for format_type, url in model_urls.items():
            if url:
                try:
                    response = requests.get(url)
                    response.raise_for_status()
                    
                    file_path = output_path / f"{hero_name}.{format_type}"
                    with open(file_path, 'wb') as f:
                        f.write(response.content)
                    
                    print(f"✅ 已下载: {file_path}")
                    downloaded.append(file_path)
                except Exception as e:
                    print(f"❌ 下载{format_type}失败: {e}")
        
        return downloaded


# ==================== 山海经英雄生成配置 ====================

SHANHAI_HEROES = {
    "xingtian": {
        "name": "刑天",
        "prompt": """Chinese mythological warrior Xing Tian from Shan Hai Jing, headless giant with eyes on chest and mouth on belly, wielding axe and shield, fierce expression, ancient Chinese armor, muscular body, fantasy art style, detailed textures, game character design, full body, standing pose""",
        "art_style": "stylized",
        "mode": "preview",
        "description": "战神 - 以乳为目，以脐为口"
    },
    
    "jiuweihu": {
        "name": "九尾狐",
        "prompt": """Nine-tailed fox spirit from Chinese mythology, beautiful female fox demon with nine fluffy tails, elegant flowing robes, mystical aura, pink and white color scheme, anime style, detailed fur, fantasy character design, full body, graceful pose""",
        "art_style": "stylized",
        "mode": "preview",
        "description": "青丘妖狐 - 其状如狐而九尾"
    },
    
    "houyi": {
        "name": "后羿",
        "prompt": """Hou Yi the archer from Chinese mythology, handsome male warrior with divine bow, ancient Chinese hunter attire, muscular build, confident stance, sun motif decorations, fantasy game character style, detailed textures, full body, hero pose""",
        "art_style": "realistic",
        "mode": "preview",
        "description": "射日英雄 - 帝俊赐羿彤弓素矰"
    },
    
    "nuwa": {
        "name": "女娲",
        "prompt": """Nuwa the mother goddess from Chinese mythology, elegant female deity with snake lower body, creating humans from clay, divine aura, flowing celestial robes, rainbow colors, gentle expression, fantasy art style, detailed scales, full body, divine pose""",
        "art_style": "stylized",
        "mode": "preview",
        "description": "创世女神 - 女娲补天"
    },
    
    "gonggong": {
        "name": "共工",
        "prompt": """Gong Gong the water god from Chinese mythology, fierce male deity with water powers, blue scaled armor, trident weapon, raging water effects, angry expression, serpent-like features, fantasy game character style, detailed water textures, full body, battle pose""",
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
    
    print(f"\n🎨 正在生成: {hero['name']}")
    print(f"📝 描述: {hero['description']}")
    print(f"🎭 风格: {hero['art_style']}")
    print(f"⚙️ 模式: {hero['mode']}")
    print("-" * 50)
    
    # 检查API密钥
    if not api_key:
        api_key = os.getenv('MESHY_API_KEY')
    
    if not api_key:
        print("❌ 错误: 未设置 MESHY_API_KEY 环境变量")
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
        print("❌ 生成请求失败")
        return False
    
    task_id = result.get('id') or result.get('task_id')
    if not task_id:
        print(f"❌ 未获取到任务ID: {result}")
        return False
    
    print(f"✅ 任务已提交: {task_id}")
    
    # 等待生成完成
    print("⏳ 等待生成完成...")
    max_wait = 600  # 最多等待10分钟
    elapsed = 0
    
    while elapsed < max_wait:
        time.sleep(15)
        elapsed += 15
        
        status = generator.check_status(task_id)
        if not status:
            continue
        
        state = status.get('status', 'pending')
        progress = status.get('progress', 0)
        print(f"  状态: {state} | 进度: {progress}% ({elapsed}s)")
        
        if state == 'succeeded' or state == 'completed':
            model_urls = status.get('model_urls', {})
            if model_urls:
                # 下载模型
                output_dir = "/Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/AI_Generated"
                
                downloaded = generator.download_model(
                    model_urls, 
                    output_dir, 
                    f"Hero_{hero_key}_AI"
                )
                
                if downloaded:
                    print(f"\n✅ {hero['name']} 模型生成完成!")
                    print(f"📁 保存位置: {output_dir}")
                    return True
            break
        
        elif state == 'failed':
            print(f"❌ 生成失败: {status.get('error', 'Unknown error')}")
            return False
        elif state == 'cancelled':
            print("❌ 任务被取消")
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
        try:
            print("\n🎯 请选择要生成的英雄 (输入key):")
            choice = input("> ").strip().lower()
            
            if choice in SHANHAI_HEROES:
                generate_hero_model(choice)
            else:
                print(f"❌ 无效的选择: {choice}")
        except EOFError:
            print("\n⚠️ 非交互式模式，请提供命令行参数")


if __name__ == "__main__":
    main()
