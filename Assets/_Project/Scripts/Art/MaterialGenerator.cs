using UnityEngine;
using UnityEngine.Rendering;

namespace ShanHaiKing.Art
{
    /// <summary>
    /// 材质生成器 - 程序化生成游戏材质
    /// </summary>
    public class MaterialGenerator : MonoBehaviour
    {
        public static MaterialGenerator Instance { get; private set; }
        
        [Header("材质预设")]
        public Shader standardShader;
        public Shader urpShader;
        
        [Header("纹理尺寸")]
        public int textureSize = 512;
        
        void Awake()
        {
            Instance = this;
        }
        
        /// <summary>
        /// 创建金属材质
        /// </summary>
        public Material CreateMetalMaterial(Color baseColor, float metallic, float smoothness)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = "Metal_" + baseColor.ToString();
            
            mat.SetColor("_BaseColor", baseColor);
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Smoothness", smoothness);
            
            // 创建金属纹理
            Texture2D metalTexture = CreateMetalTexture(baseColor);
            mat.SetTexture("_BaseMap", metalTexture);
            
            // 创建法线贴图
            Texture2D normalMap = CreateNoiseNormalMap();
            mat.SetTexture("_BumpMap", normalMap);
            
            return mat;
        }
        
        /// <summary>
        /// 创建皮肤材质
        /// </summary>
        public Material CreateSkinMaterial(Color skinColor, float roughness)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = "Skin_" + skinColor.ToString();
            
            mat.SetColor("_BaseColor", skinColor);
            mat.SetFloat("_Metallic", 0f);
            mat.SetFloat("_Smoothness", 1f - roughness);
            
            // 创建皮肤纹理
            Texture2D skinTexture = CreateSkinTexture(skinColor);
            mat.SetTexture("_BaseMap", skinTexture);
            
            return mat;
        }
        
        /// <summary>
        /// 创建布料材质
        /// </summary>
        public Material CreateClothMaterial(Color clothColor, TexturePattern pattern)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = "Cloth_" + pattern.ToString();
            
            mat.SetColor("_BaseColor", clothColor);
            mat.SetFloat("_Metallic", 0f);
            mat.SetFloat("_Smoothness", 0.3f);
            
            // 创建布料纹理
            Texture2D clothTexture = CreatePatternTexture(clothColor, pattern);
            mat.SetTexture("_BaseMap", clothTexture);
            
            return mat;
        }
        
        /// <summary>
        /// 创建发光材质
        /// </summary>
        public Material CreateEmissiveMaterial(Color emissiveColor, float intensity)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = "Emissive_" + emissiveColor.ToString();
            
            mat.SetColor("_BaseColor", Color.black);
            mat.SetColor("_EmissionColor", emissiveColor * intensity);
            mat.SetFloat("_EmissionIntensity", intensity);
            mat.EnableKeyword("_EMISSION");
            
            return mat;
        }
        
        /// <summary>
        /// 创建宝石材质
        /// </summary>
        public Material CreateGemMaterial(Color gemColor, float transparency)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = "Gem_" + gemColor.ToString();
            
            mat.SetColor("_BaseColor", gemColor);
            mat.SetFloat("_Metallic", 0.9f);
            mat.SetFloat("_Smoothness", 0.95f);
            
            // 设置透明渲染模式
            mat.SetFloat("_Surface", 1); // Transparent
            mat.SetFloat("_Blend", 0);   // Alpha
            mat.SetFloat("_SrcBlend", (int)BlendMode.SrcAlpha);
            mat.SetFloat("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            mat.SetFloat("_ZWrite", 0);
            mat.SetFloat("_AlphaClip", 0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)RenderQueue.Transparent;
            
            return mat;
        }
        
        /// <summary>
        /// 创建地形材质
        /// </summary>
        public Material CreateTerrainMaterial(Color groundColor, Color grassColor)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Terrain/Lit"));
            mat.name = "Terrain_Ground";
            
            // 创建地形纹理
            Texture2D terrainTexture = CreateTerrainTexture(groundColor, grassColor);
            mat.SetTexture("_MainTex", terrainTexture);
            
            return mat;
        }
        
        #region 纹理生成
        
        Texture2D CreateMetalTexture(Color baseColor)
        {
            Texture2D tex = new Texture2D(textureSize, textureSize);
            
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    // 添加金属划痕效果
                    float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                    Color pixelColor = Color.Lerp(baseColor, baseColor * 1.2f, noise * 0.2f);
                    
                    // 添加划痕
                    if (Random.value > 0.995f)
                    {
                        pixelColor = Color.Lerp(pixelColor, Color.white, 0.3f);
                    }
                    
                    tex.SetPixel(x, y, pixelColor);
                }
            }
            
            tex.Apply();
            return tex;
        }
        
        Texture2D CreateSkinTexture(Color skinColor)
        {
            Texture2D tex = new Texture2D(textureSize, textureSize);
            
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    // 添加皮肤毛孔效果
                    float noise = Mathf.PerlinNoise(x * 0.05f, y * 0.05f);
                    Color pixelColor = Color.Lerp(skinColor, skinColor * 0.9f, noise * 0.1f);
                    
                    tex.SetPixel(x, y, pixelColor);
                }
            }
            
            tex.Apply();
            return tex;
        }
        
        Texture2D CreatePatternTexture(Color baseColor, TexturePattern pattern)
        {
            Texture2D tex = new Texture2D(textureSize, textureSize);
            
            switch (pattern)
            {
                case TexturePattern.Checkerboard:
                    CreateCheckerboardPattern(tex, baseColor);
                    break;
                case TexturePattern.Stripes:
                    CreateStripesPattern(tex, baseColor);
                    break;
                case TexturePattern.Dots:
                    CreateDotsPattern(tex, baseColor);
                    break;
                case TexturePattern.Dragon:
                    CreateDragonPattern(tex, baseColor);
                    break;
                case TexturePattern.Cloud:
                    CreateCloudPattern(tex, baseColor);
                    break;
                default:
                    CreateSolidTexture(tex, baseColor);
                    break;
            }
            
            tex.Apply();
            return tex;
        }
        
        void CreateCheckerboardPattern(Texture2D tex, Color baseColor)
        {
            int checkSize = textureSize / 8;
            Color altColor = Color.Lerp(baseColor, Color.white, 0.3f);
            
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    bool isCheck = ((x / checkSize) + (y / checkSize)) % 2 == 0;
                    tex.SetPixel(x, y, isCheck ? baseColor : altColor);
                }
            }
        }
        
        void CreateStripesPattern(Texture2D tex, Color baseColor)
        {
            int stripeWidth = textureSize / 16;
            Color altColor = Color.Lerp(baseColor, Color.black, 0.3f);
            
            for (int x = 0; x < textureSize; x++)
            {
                bool isStripe = (x / stripeWidth) % 2 == 0;
                for (int y = 0; y < textureSize; y++)
                {
                    tex.SetPixel(x, y, isStripe ? baseColor : altColor);
                }
            }
        }
        
        void CreateDotsPattern(Texture2D tex, Color baseColor)
        {
            Color altColor = Color.Lerp(baseColor, Color.white, 0.5f);
            int dotSpacing = textureSize / 8;
            int dotRadius = dotSpacing / 3;
            
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    int centerX = (x / dotSpacing) * dotSpacing + dotSpacing / 2;
                    int centerY = (y / dotSpacing) * dotSpacing + dotSpacing / 2;
                    
                    float dist = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                    bool isDot = dist < dotRadius;
                    
                    tex.SetPixel(x, y, isDot ? baseColor : altColor);
                }
            }
        }
        
        void CreateDragonPattern(Texture2D tex, Color baseColor)
        {
            // 简化的龙纹图案
            Color accentColor = Color.Lerp(baseColor, Color.red, 0.3f);
            
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    // 使用噪声生成云纹效果
                    float noise = Mathf.PerlinNoise(x * 0.01f, y * 0.01f);
                    float noise2 = Mathf.PerlinNoise(x * 0.03f + 100, y * 0.03f);
                    
                    Color pixelColor = Color.Lerp(baseColor, accentColor, noise * noise2);
                    tex.SetPixel(x, y, pixelColor);
                }
            }
        }
        
        void CreateCloudPattern(Texture2D tex, Color baseColor)
        {
            Color cloudColor = Color.Lerp(baseColor, Color.white, 0.4f);
            
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.008f, y * 0.008f);
                    float cloudShape = Mathf.PerlinNoise(x * 0.02f + 50, y * 0.02f);
                    
                    Color pixelColor = Color.Lerp(baseColor, cloudColor, noise * cloudShape);
                    tex.SetPixel(x, y, pixelColor);
                }
            }
        }
        
        void CreateSolidTexture(Texture2D tex, Color color)
        {
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    tex.SetPixel(x, y, color);
                }
            }
        }
        
        Texture2D CreateTerrainTexture(Color groundColor, Color grassColor)
        {
            Texture2D tex = new Texture2D(textureSize, textureSize);
            
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.02f, y * 0.02f);
                    Color pixelColor = Color.Lerp(groundColor, grassColor, noise);
                    tex.SetPixel(x, y, pixelColor);
                }
            }
            
            tex.Apply();
            return tex;
        }
        
        Texture2D CreateNoiseNormalMap()
        {
            Texture2D tex = new Texture2D(textureSize, textureSize);
            
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.05f, y * 0.05f);
                    Color normalColor = new Color(noise, noise, 1f);
                    tex.SetPixel(x, y, normalColor);
                }
            }
            
            tex.Apply();
            return tex;
        }
        
        #endregion
    }
    
    public enum TexturePattern
    {
        Solid,
        Checkerboard,
        Stripes,
        Dots,
        Dragon,     // 龙纹
        Cloud,      // 云纹
        Wave,       // 波纹
        Scale       // 鳞纹
    }
}
