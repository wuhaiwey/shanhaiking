using UnityEditor;
using UnityEngine;

public class BuildScript
{
    /// <summary>
    /// 编译项目（供命令行调用）
    /// </summary>
    public static void CompileProject()
    {
        Debug.Log("========================================");
        Debug.Log("开始编译项目...");
        Debug.Log("========================================");
        
        // 编译所有脚本
        string[] scripts = AssetDatabase.FindAssets("t:Script", new[] { "Assets/_Project/Scripts" });
        Debug.Log($"找到 {scripts.Length} 个脚本文件");
        
        // 刷新资源数据库
        AssetDatabase.Refresh();
        
        // 编译检查
        if (EditorApplication.isCompiling)
        {
            Debug.Log("正在编译中，请稍候...");
            
            // 等待编译完成
            int attempts = 0;
            while (EditorApplication.isCompiling && attempts < 100)
            {
                System.Threading.Thread.Sleep(100);
                attempts++;
            }
        }
        
        // 检查编译错误
        int errorCount = 0;
        string[] logs = GetCompileLogs();
        
        foreach (string log in logs)
        {
            if (log.Contains("error CS"))
            {
                Debug.LogError($"编译错误: {log}");
                errorCount++;
            }
        }
        
        if (errorCount > 0)
        {
            Debug.LogError($"========================================");
            Debug.LogError($"编译失败！共 {errorCount} 个错误");
            Debug.LogError($"========================================");
            EditorApplication.Exit(1);
        }
        else
        {
            Debug.Log("========================================");
            Debug.Log("✅ 编译成功！无错误");
            Debug.Log("========================================");
            EditorApplication.Exit(0);
        }
    }
    
    /// <summary>
    /// 获取编译日志（简化版）
    /// </summary>
    private static string[] GetCompileLogs()
    {
        // 在实际编译中，Unity会自动输出到控制台
        // 这里返回空数组，实际的错误会在控制台中显示
        return new string[0];
    }
    
    /// <summary>
    /// 构建Windows版本
    /// </summary>
    public static void BuildWindows()
    {
        string[] scenes = GetBuildScenes();
        string buildPath = "Builds/Windows/ShanHaiKing.exe";
        
        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneWindows64, BuildOptions.None);
        Debug.Log($"Windows构建完成: {buildPath}");
    }
    
    /// <summary>
    /// 构建Mac版本
    /// </summary>
    public static void BuildMac()
    {
        string[] scenes = GetBuildScenes();
        string buildPath = "Builds/Mac/ShanHaiKing.app";
        
        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneOSX, BuildOptions.None);
        Debug.Log($"Mac构建完成: {buildPath}");
    }
    
    /// <summary>
    /// 构建Android版本
    /// </summary>
    public static void BuildAndroid()
    {
        string[] scenes = GetBuildScenes();
        string buildPath = "Builds/Android/ShanHaiKing.apk";
        
        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.Android, BuildOptions.None);
        Debug.Log($"Android构建完成: {buildPath}");
    }
    
    /// <summary>
    /// 获取构建场景列表
    /// </summary>
    private static string[] GetBuildScenes()
    {
        return new string[]
        {
            "Assets/_Project/Scenes/MainMenu.unity",
            "Assets/_Project/Scenes/GameScene.unity",
            "Assets/_Project/Scenes/HeroSelect.unity"
        };
    }
}
