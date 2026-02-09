echo "[线程1] 创建英雄代码..."

# 线程2: 创建英雄-王昭君代码
cat > Hero_WangZhaoJun.cs << 'HEROEOF' &
echo "[线程2] 创建英雄代码..."

# 线程3: 创建系统代码
cd /Users/mili/Desktop/ShanHaiKing/Assets/_Project/Scripts/Core && \
cat > AchievementSystem.cs <> 'SYSEOF' &
echo "[线程3] 创建系统代码..."

# 线程4: 创建Blender模型脚本
cd /Users/mili/Desktop/ShanHaiKing/Assets/Editor/BlenderScripts && \
cat > create_hero_model_batch.py << 'BLENDEREOF' &
echo "[线程4] 创建模型脚本..."

# 线程5: 运行Blender生成模型
(sleep 2 && blender --background --python create_hero_model_batch.py > /tmp/blender.log 2>&1) &
echo "[线程5] 运行Blender..."

# 线程6: Git准备
cd /Users/mili/Desktop/ShanHaiKing && git add . &
echo "[线程6] Git准备..."

echo ""
echo "⏳ 等待所有线程完成..."
wait

echo ""
echo "========================================"
echo "✅ 6线程并发开发完成！"
echo "========================================"
echo ""
echo "📊 成果统计："
ls -la /Users/mili/Desktop/ShanHaiKing/Assets/_Project/Scripts/Hero/Hero_DiaoChan_Full.cs 2>/dev/null
ls -la /Users/mili/Desktop/ShanHaiKing/Assets/_Project/Scripts/Hero/Hero_WangZhaoJun.cs 2>/dev/null
ls -la /Users/mili/Desktop/ShanHaiKing/Assets/_Project/Scripts/Core/AchievementSystem.cs 2>/dev/null
ls -la /Users/mili/Desktop/ShanHaiKing/Assets/Editor/BlenderScripts/create_hero_model_batch.py 2>/dev/null
ls -la /Users/mili/Desktop/ShanHaiKing/Assets/_Project/Models/*.fbx 2>/dev/null | tail -3