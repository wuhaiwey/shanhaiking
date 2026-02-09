#!/bin/bash
# 🎨 AI生成3D模型 - 使用curl直接调用Meshy API

API_KEY="msy_fBXjw5NW2bHf0bx4nKUafEkWzGZileyUjy3u"

# 尝试不同的API端点
echo "🧪 测试API端点..."

# 测试1: /v2/text-to-3d
echo "测试 /v2/text-to-3d..."
curl -s -X POST "https://api.meshy.ai/v2/text-to-3d" \
  -H "Authorization: Bearer $API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "prompt": "Chinese warrior",
    "mode": "preview"
  }' | head -100

echo ""
echo "---"

# 测试2: /api/v1/text-to-3d
echo "测试 /api/v1/text-to-3d..."
curl -s -X POST "https://api.meshy.ai/api/v1/text-to-3d" \
  -H "Authorization: Bearer $API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "prompt": "Chinese warrior",
    "mode": "preview"
  }' | head -100

echo ""
echo "---"

# 测试3: /openapi/v2/text-to-3d
echo "测试 /openapi/v2/text-to-3d..."
curl -s -X POST "https://api.meshy.ai/openapi/v2/text-to-3d" \
  -H "Authorization: Bearer $API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "prompt": "Chinese warrior",
    "mode": "preview"
  }' | head -100
