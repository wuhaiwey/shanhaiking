#!/bin/bash

# Unity项目验证脚本
# 检查代码语法错误，然后提交

PROJECT_PATH="/Users/mili/Desktop/ShanHaiKing"
LOG_FILE="$PROJECT_PATH/last_compile_check.txt"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${YELLOW}========================================${NC}"
echo -e "${YELLOW}  Unity项目验证脚本${NC}"
echo -e "${YELLOW}========================================${NC}"
echo ""

cd "$PROJECT_PATH"

# 检查C#语法错误
echo -e "${YELLOW}正在检查C#代码...${NC}"

# 查找所有CS文件并检查基本语法
CS_FILES=$(find Assets/_Project/Scripts -name "*.cs" 2>/dev/null)
FILE_COUNT=$(echo "$CS_FILES" | wc -l)

echo "找到 $FILE_COUNT 个C#脚本文件"
echo ""

# 基本的语法检查
SYNTAX_ERROR=0

for file in $CS_FILES; do
    # 检查文件是否包含基本的语法结构
    if ! grep -q "class" "$file" 2>/dev/null; then
        if ! grep -q "interface" "$file" 2>/dev/null; then
            if ! grep -q "enum" "$file" 2>/dev/null; then
                echo -e "${RED}警告: $file 可能缺少类型定义${NC}"
                SYNTAX_ERROR=1
            fi
        fi
    fi
    
    # 检查括号匹配
    OPEN_BRACES=$(grep -o "{" "$file" | wc -l)
    CLOSE_BRACES=$(grep -o "}" "$file" | wc -l)
    
    if [ "$OPEN_BRACES" -ne "$CLOSE_BRACES" ]; then
        echo -e "${RED}错误: $file 括号不匹配 (开: $OPEN_BRACES, 关: $CLOSE_BRACES)${NC}"
        SYNTAX_ERROR=1
    fi
done

if [ $SYNTAX_ERROR -eq 1 ]; then
    echo ""
    echo -e "${RED}❌ 代码检查发现问题，请先修复后再提交${NC}"
    exit 1
fi

# 检查Git状态
echo -e "${YELLOW}检查Git状态...${NC}"
git status --short

# 统计代码
echo ""
echo -e "${YELLOW}代码统计:${NC}"
find Assets/_Project/Scripts -name "*.cs" -exec wc -l {} + | tail -1

echo ""
echo -e "${GREEN}✅ 代码检查通过！${NC}"
echo ""

# Git提交
read -p "请输入提交信息（直接回车使用默认）: " msg

if [ -z "$msg" ]; then
    COMMIT_MSG="🎮 代码更新 - 通过验证"
else
    COMMIT_MSG="$msg"
fi

echo -e "${YELLOW}正在提交...${NC}"
git add .
git commit -m "$COMMIT_MSG"

echo -e "${YELLOW}正在推送到GitHub...${NC}"
git push origin main

if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}========================================${NC}"
    echo -e "${GREEN}  ✅ 提交成功！${NC}"
    echo -e "${GREEN}========================================${NC}"
    echo "提交: $COMMIT_MSG"
    echo "提交数: $(git log --oneline | wc -l)"
else
    echo -e "${RED}❌ 推送失败${NC}"
    exit 1
fi
