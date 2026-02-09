#!/bin/bash

# Unity编译脚本
# 使用：./compile_and_push.sh "提交信息"

PROJECT_PATH="/Users/mili/Desktop/ShanHaiKing"
UNITY_PATH="/Applications/Unity/Hub/Editor/6000.3.7f1/Unity.app/Contents/MacOS/Unity"
LOG_FILE="$PROJECT_PATH/compile_log.txt"

# 颜色输出
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${YELLOW}========================================${NC}"
echo -e "${YELLOW}  山海经王者荣耀 - Unity编译脚本${NC}"
echo -e "${YELLOW}========================================${NC}"
echo ""

# 检查Unity路径
if [ ! -f "$UNITY_PATH" ]; then
    echo -e "${RED}错误：找不到Unity可执行文件${NC}"
    echo "路径: $UNITY_PATH"
    exit 1
fi

echo -e "${YELLOW}正在编译Unity项目...${NC}"
echo "项目路径: $PROJECT_PATH"
echo ""

# 清理日志
rm -f "$LOG_FILE"

# 运行Unity编译（批处理模式）
"$UNITY_PATH" -batchmode -nographics \
    -projectPath "$PROJECT_PATH" \
    -executeMethod BuildScript.CompileProject \
    -logFile "$LOG_FILE" \
    -quit

# 检查编译结果
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Unity编译成功！${NC}"
    
    # 检查日志中是否有错误
    if grep -q "error CS" "$LOG_FILE" 2>/dev/null; then
        echo -e "${RED}❌ 检测到编译错误：${NC}"
        grep "error CS" "$LOG_FILE" | head -10
        exit 1
    fi
    
    # 检查日志中是否有异常
    if grep -q "Exception:" "$LOG_FILE" 2>/dev/null; then
        echo -e "${RED}❌ 检测到异常：${NC}"
        grep "Exception:" "$LOG_FILE" | head -5
        exit 1
    fi
    
    echo ""
    echo -e "${GREEN}✅ 编译检查通过！${NC}"
    echo ""
    
    # Git提交和推送
    cd "$PROJECT_PATH"
    
    # 添加所有文件
    git add .
    
    # 提交（如果有变更）
    if ! git diff --cached --quiet; then
        if [ -z "$1" ]; then
            COMMIT_MSG="🎮 Unity编译通过 - 自动提交"
        else
            COMMIT_MSG="$1"
        fi
        
        git commit -m "$COMMIT_MSG"
        
        echo -e "${YELLOW}正在推送到GitHub...${NC}"
        git push origin main
        
        if [ $? -eq 0 ]; then
            echo ""
            echo -e "${GREEN}========================================${NC}"
            echo -e "${GREEN}  ✅ 提交成功！${NC}"
            echo -e "${GREEN}========================================${NC}"
            echo ""
            echo "提交信息: $COMMIT_MSG"
            echo "仓库地址: https://github.com/wuhaiwey/shanhaiking"
        else
            echo -e "${RED}❌ 推送到GitHub失败${NC}"
            exit 1
        fi
    else
        echo -e "${YELLOW}没有需要提交的变更${NC}"
    fi
    
else
    echo -e "${RED}❌ Unity编译失败${NC}"
    echo ""
    echo "编译日志:"
    tail -50 "$LOG_FILE"
    exit 1
fi
