using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.Health
{
    public sealed class HealthBarSettings : SettingsBase
    {
        public HealthBarSettings()
        {
            Enable = false;
            ShowInTown = true;
            ShowES = true;
            ShowIncrements = true;
            ShowEnemies = true;
            Players = new AllyUnitSettings(0x008000ff, 0);
            Minions = new AllyUnitSettings(0x90ee90ff, 0);
            NormalEnemy = new EnemyUnitSettings(0xff0000ff, 0, 0x66ff66ff, false);
            MagicEnemy = new EnemyUnitSettings(0xff0000ff, 0x8888ffff, 0x66ff99ff, false);
            RareEnemy = new EnemyUnitSettings(0xff0000ff, 0xffff77ff, 0x66ff99ff, true);
            UniqueEnemy = new EnemyUnitSettings(0xff0000ff, 0xffa500ff, 0x66ff99ff, true);
        }

        public ToggleNode ShowInTown { get; set; }

        public ToggleNode ShowES { get; set; }

        public ToggleNode ShowIncrements { get; set; }

        public ToggleNode ShowEnemies { get; set; }

        public AllyUnitSettings Players { get; set; }

        public AllyUnitSettings Minions { get; set; }

        public EnemyUnitSettings NormalEnemy { get; set; }

        public EnemyUnitSettings MagicEnemy { get; set; }

        public EnemyUnitSettings RareEnemy { get; set; }

        public EnemyUnitSettings UniqueEnemy { get; set; }
    }
}