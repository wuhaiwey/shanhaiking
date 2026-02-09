using UnityEngine;

namespace ShanHaiKing.Hero
{
    public class Hero_ChiYou : HeroBase
    {
        protected override void Awake()
        {
            base.Awake();
            heroName = "蚩尤";
            heroTitle = "兵主";
            heroType = HeroType.Warrior;
            stats.maxHealth = 3800f;
            stats.attackDamage = 160f;
            stats.armor = 120f;
            stats.Initialize();
        }
    }
}

namespace ShanHaiKing.Hero
{
    public class Hero_TaoTie : HeroBase
    {
        protected override void Awake()
        {
            base.Awake();
            heroName = "饕餮";
            heroTitle = "贪婪之兽";
            heroType = HeroType.Tank;
            stats.maxHealth = 5000f;
            stats.attackDamage = 100f;
            stats.armor = 180f;
            stats.Initialize();
        }
    }
}

namespace ShanHaiKing.Hero
{
    public class Hero_BaiZe : HeroBase
    {
        protected override void Awake()
        {
            base.Awake();
            heroName = "白泽";
            heroTitle = "祥瑞之兽";
            heroType = HeroType.Support;
            stats.maxHealth = 2400f;
            stats.attackDamage = 70f;
            stats.Initialize();
        }
    }
}
