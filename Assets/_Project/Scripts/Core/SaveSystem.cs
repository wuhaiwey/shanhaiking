using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 存档系统
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }
        
        private string savePath;
        
        void Awake()
        {
            Instance = this;
            savePath = Application.persistentDataPath + "/save.dat";
        }
        
        public void SaveGame(GameData data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath, FileMode.Create);
            
            formatter.Serialize(stream, data);
            stream.Close();
            
            Debug.Log($"游戏已保存到: {savePath}");
        }
        
        public GameData LoadGame()
        {
            if (File.Exists(savePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(savePath, FileMode.Open);
                
                GameData data = formatter.Deserialize(stream) as GameData;
                stream.Close();
                
                Debug.Log("游戏存档已加载");
                return data;
            }
            else
            {
                Debug.Log("没有找到存档文件");
                return null;
            }
        }
        
        public void DeleteSave()
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("存档已删除");
            }
        }
    }
    
    [System.Serializable]
    public class GameData
    {
        public int playerLevel;
        public int playerExp;
        public int gold;
        public string[] unlockedHeroes;
        public string[] ownedItems;
        public float masterVolume;
        public float bgmVolume;
        public float sfxVolume;
    }
}
