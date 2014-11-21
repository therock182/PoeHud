using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Preload
{
    public class PreloadAlert : HudPluginBase
    {
        private Dictionary<string, string> alertStrings;
        private HashSet<string> disp;
        private int lastCount;

        public override void OnEnable()
        {
            disp = new HashSet<string>();
            InitAlertStrings();
            GameController.Area.OnAreaChange += CurrentArea_OnAreaChange;
            CurrentArea_OnAreaChange(GameController.Area);
        }

        public override void OnDisable()
        {
        }

        private void CurrentArea_OnAreaChange(AreaController area)
        {
            if (Settings.GetBool("PreloadAlert"))
            {
                Parse();
            }
        }

        private void Parse()
        {
            disp.Clear();
            int pFileRoot = GameController.Memory.ReadInt(GameController.Memory.AddressOfProcess + GameController.Memory.offsets.FileRoot);
            int num2 = GameController.Memory.ReadInt(pFileRoot + 12);
            int listIterator = GameController.Memory.ReadInt(pFileRoot + 20);
            int areaChangeCount = GameController.Game.AreaChangeCount;
            for (int i = 0; i < num2; i++)
            {
                listIterator = GameController.Memory.ReadInt(listIterator);
                if (GameController.Memory.ReadInt(listIterator + 8) != 0 && GameController.Memory.ReadInt(listIterator + 12, 36) == areaChangeCount)
                {
                    string text = GameController.Memory.ReadStringU(GameController.Memory.ReadInt(listIterator + 8));
                    if (text.Contains("vaal_sidearea"))
                    {
                        disp.Add("Area contains Corrupted Area");
                    }
                    if (text.Contains('@'))
                    {
                        text = text.Split(new[] {'@'})[0];
                    }
                    if (text.StartsWith("Metadata/Monsters/Missions/MasterStrDex"))
                    {
                        Console.WriteLine("bad alert " + text);
                        disp.Add("Area contains Vagan, Weaponmaster");
                    }
                    if (alertStrings.ContainsKey(text))
                    {
                        Console.WriteLine("Alert because of " + text);
                        disp.Add(alertStrings[text]);
                    }
                    else
                    {
                        if (text.EndsWith("BossInvasion"))
                        {
                            disp.Add("Area contains Invasion Boss");
                        }
                    }
                }
            }
        }

        public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
        {
            if (!Settings.GetBool("PreloadAlert"))
            {
                return;
            }
            int num =
                GameController.Memory.ReadInt(
                    GameController.Memory.AddressOfProcess + GameController.Memory.offsets.FileRoot, 12);
            if (num != lastCount)
            {
                lastCount = num;
                Parse();
            }
            if (disp.Count > 0)
            {
                Vec2 vec = mountPoints[UiMountPoint.LeftOfMinimap];
                int num2 = vec.Y;
                int maxWidth = 0;
                int @int = Settings.GetInt("PreloadAlert.FontSize");
                int int2 = Settings.GetInt("PreloadAlert.BgAlpha");
                foreach (string current in disp)
                {
                    Vec2 vec2 = rc.AddTextWithHeight(new Vec2(vec.X, num2), current, Color.White, @int,
                        DrawTextFormat.Right);
                    if (vec2.X + 10 > maxWidth)
                    {
                        maxWidth = vec2.X + 10;
                    }
                    num2 += vec2.Y;
                }
                if (maxWidth > 0 && int2 > 0)
                {
                    var bounds = new Rect(vec.X - maxWidth + 5, vec.Y - 5, maxWidth, num2 - vec.Y + 10);
                    rc.AddBox(bounds, Color.FromArgb(int2, 1, 1, 1));
                    mountPoints[UiMountPoint.LeftOfMinimap] = new Vec2(vec.X, vec.Y + 5 + bounds.H);
                }
            }
        }

        private void InitAlertStrings()
        {
            alertStrings = new Dictionary<string, string>();
            alertStrings.Add("Metadata/Chests/StrongBoxes/Strongbox", "Area contains Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/Strongbox.ao", "Area contains Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/Ornate", "Area contains Ornate Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/OrnateStrongbox.ao", "Area contains Ornate Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/Large", "Area contains Large Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/LargeStrongbox.ao", "Area contains Large Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/Jeweller", "Area contains Jeweler's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/JewellerStrongBox.ao", "Area contains Jeweler's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/Gemcutter", "Area contains Gemcutter's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/GemcutterStrongBox.ao", "Area contains Gemcutter's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/Chemist", "Area contains Chemist's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/ChemistStrongBox.ao", "Area contains Chemist's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/Cartographer", "Area contains Cartographer's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/CartographerStrongBox.ao",
                "Area contains Cartographer's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/Artisan", "Area contains Artisan's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/ArtisanStrongBox.ao", "Area contains Artisan's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/Arsenal", "Area contains Blacksmith's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/ArsenalStrongBox.ao", "Area contains Blacksmith's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/Armory", "Area contains Armourer's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/ArmoryStrongBox.ao", "Area contains Armourer's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/Arcanist", "Area contains Arcanist's Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/ArcanistStrongBox.ao", "Area contains Arcanist's Strongbox");
            alertStrings.Add("Metadata/Chests/CopperChestEpic3", "Area contains Large Chest");
            alertStrings.Add("Metadata/Chests/CopperChests/CopperChestEpic3.ao", "Area contains Large Chest");
            alertStrings.Add("Metadata/Chests/StrongBoxes/PerandusBox.ao", "Area contains Perandus Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/KaomBox.ao", "Area contains Kaom Strongbox");
            alertStrings.Add("Metadata/Chests/StrongBoxes/MalachaisBox.ao", "Area contains Malachai Strongbox");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileRanger1", "Area contains Orra Greengate");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileRanger2", "Area contains Thena Moga");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileRanger3", "Area contains Antalie Napora");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileDuelist1", "Area contains Torr Olgosso");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileDuelist2", "Area contains Armios Bell");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileDuelist4", "Area contains Zacharie Desmarais");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileWitch1", "Area contains Minara Anenima");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileWitch2", "Area contains Igna Phoenix");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileMarauder1", "Area contains Jonah Unchained");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileMarauder2", "Area contains Damoi Tui");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileMarauder3", "Area contains Xandro Blooddrinker");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileMarauder5", "Area contains Vickas Giantbone");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileTemplar1", "Area contains Eoin Greyfur");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileTemplar2", "Area contains Tinevin Highdove");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileTemplar4", "Area contains Magnus Stonethorn");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileShadow1", "Area contains Ion Darkshroud");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileShadow2", "Area contains Ash Lessard");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileShadow4", "Area contains Wilorin Demontamer");
            alertStrings.Add("Metadata/Monsters/Exiles/ExileScion2", "Area contains Augustina Solaria");
            alertStrings.Add("Metadata/Monsters/Squid/SquidBossSideArea", "Area contains The All-seeing Eye");
            alertStrings.Add("Metadata/Monsters/Goatman/GoatmanLeapBossSideArea", "Area contains Konu, Maker of Wind");
            alertStrings.Add("Metadata/Monsters/GhostPirates/GhostPirateBossSideArea",
                "Area contains Coniraya, Shadow of Malice");
            alertStrings.Add("Metadata/Monsters/DemonModular/DemonModularElementsBossSideArea",
                "Area contains Atziri's Pride");
            alertStrings.Add("Metadata/Monsters/Goatman/GoatmanShamanBossSideArea",
                "Area contains Sheaq, Maker of Floods");
            alertStrings.Add("Metadata/Monsters/Skeleton/SkeletonMeleeLargeBossSideArea",
                "Area contains Ossecati, Boneshaper");
            alertStrings.Add("Metadata/Monsters/RootSpiders/RootSpiderBossSideArea", "Area contains Kamaq, Soilmaker");
            alertStrings.Add("Metadata/Monsters/Kiweth/KiwethBossSideArea", "Area contains Inti of the Blood Moon");
            alertStrings.Add("Metadata/Monsters/incaminion/FragmentBossSideArea", "Area contains Shrapnelbearer");
            alertStrings.Add("Metadata/Monsters/Snake/SnakeRoboBossSideArea",
                "Area contains Wiraqucha, Ancient Guardian");
            alertStrings.Add("Metadata/Monsters/DemonFemale/WhipDemonBossSideArea", "Area contains Cava, Artist of Pain");
            alertStrings.Add("Metadata/Monsters/Pyromaniac/PyromaniacBossSideArea", "Area contains Curator Miem");
            alertStrings.Add("Metadata/Monsters/BloodChieftain/MonkeyChiefBossSideArea",
                "Area contains Simi, the Nature Touched");
            alertStrings.Add("Metadata/Monsters/InsectSpawner/CarrionQueenBossSideArea",
                "Area contains The Sunburst Queen");
            alertStrings.Add("Metadata/Monsters/Totems/TotemBossSideArea", "Area contains M'gaska, the Living Pyre");
            alertStrings.Add("Metadata/Monsters/Spiders/SpiderBossSideArea", "Area contains Cintiq, the Inescapable");
            alertStrings.Add("Metadata/Monsters/Snake/SnakeScorpionBossSideArea", "Area contains Thornrunner");
            alertStrings.Add("Metadata/Monsters/Cannibal/CannibalBossSideArea", "Area contains Perquil the Lucky");
            alertStrings.Add("Metadata/Monsters/Skeletons/ConstructMeleeBossSideArea",
                "Area contains Haviri, Vaal Metalsmith");
            alertStrings.Add("Metadata/Monsters/Skeletons/ConstructRangedBossSideArea",
                "Area contains Kutec, Vaal Fleshsmith");
            alertStrings.Add("Metadata/Monsters/AnimatedItem/AnimatedArmourBossSideArea",
                "Area contains Shadow of Vengeance");
            alertStrings.Add("Metadata/Monsters/DemonBosses/DemonBoss1/Demon1BossSideArea",
                "Area contains Beheader Ataguchu");
            alertStrings.Add("Metadata/Monsters/DemonBosses/DemonBoss2/Demon2BossSideArea",
                "Area contains Wiraq, the Impaler");
            alertStrings.Add("Metadata/Monsters/DemonBosses/DemonBoss3/Demon3BossSideArea",
                "Area contains Ch'aska, Maker of Rain");
            alertStrings.Add("Metadata/Monsters/SandSpitterEmerge/SandSpitterBossSideArea",
                "Area contains Mother of the Hive");
            alertStrings.Add("Metadata/Monsters/Seawitch/SeaWitchBossSideArea", "Area contains Rima, Deep Temptress");
            alertStrings.Add("Metadata/Monsters/Axis/AxisCasterBossInvasion", "Area contains Evocata Apocalyptica");
            alertStrings.Add("Metadata/Monsters/Axis/AxisExperimenterBossInvasion", "Area contains Docere Incarnatis");
            alertStrings.Add("Metadata/Monsters/Axis/AxisSoldierBossInvasion", "Area contains Corrector Draconides");
            alertStrings.Add("Metadata/Monsters/Bandits/BanditMeleeBossInvasion", "Area contains Balus Stoneskull");
            alertStrings.Add("Metadata/Monsters/Bandits/BanditBowBossInvasion", "Area contains Kall Foxfly");
            alertStrings.Add("Metadata/Monsters/Beasts/BeastBossInvasion", "Area contains Marrowcrush");
            alertStrings.Add("Metadata/Monsters/Rhoas/RhoaBossInvasion", "Area contains The Cadaver Bull");
            alertStrings.Add("Metadata/Monsters/BloodChieftain/BloodChieftainBossInvasion", "Area contains Junglemare");
            alertStrings.Add("Metadata/Monsters/BloodElemental/BloodElementalBossInvasion",
                "Area contains The Sanguine Wave");
            alertStrings.Add("Metadata/Monsters/Cannibal/CannibalMaleBossInvasion", "Area contains Graveblood");
            alertStrings.Add("Metadata/Monsters/Cannibal/CannibalFemaleBossInvasion", "Area contains Nighteater");
            alertStrings.Add("Metadata/Monsters/Undying/CityStalkerMaleBossInvasion", "Area contains The Book Burner");
            alertStrings.Add("Metadata/Monsters/Undying/CityStalkerFemaleBossInvasion", "Area contains The Bolt Juggler");
            alertStrings.Add("Metadata/Monsters/DemonFemale/DemonFemaleBossInvasion", "Area contains Avatar of Pain");
            alertStrings.Add("Metadata/Monsters/DemonModular/DemonModularBossInvasion", "Area contains Rancor");
            alertStrings.Add("Metadata/Monsters/DemonModular/DemonFemaleRangedBossInvasion", "Area contains Hatespitter");
            alertStrings.Add("Metadata/Monsters/MossMonster/FireMonsterBossInvasion", "Area contains Bluntslag");
            alertStrings.Add("Metadata/Monsters/Monkeys/FlameBearerBossInvasion", "Area contains The Revenant");
            alertStrings.Add("Metadata/Monsters/incaminion/FragmentBossInvasion", "Area contains Judgement Apparatus");
            alertStrings.Add("Metadata/Monsters/Frog/FrogBossInvasion", "Area contains Spinesnap");
            alertStrings.Add("Metadata/Monsters/GemMonster/GemFrogBossInvasion", "Area contains Genesis Paradisae");
            alertStrings.Add("Metadata/Monsters/Goatman/GoatmanBossInvasion", "Area contains Death from Above");
            alertStrings.Add("Metadata/Monsters/Goatman/GoatmanShamanBossInvasion",
                "Area contains Guardian of the Mound");
            alertStrings.Add("Metadata/Monsters/Grappler/GrapplerBossInvasion", "Area contains Wonderwalker");
            alertStrings.Add("Metadata/Monsters/Guardians/GuardianFireBossInvasion", "Area contains The Raging Mask");
            alertStrings.Add("Metadata/Monsters/Guardians/GuardianLightningBossInvasion",
                "Area contains The Teetering Mask");
            alertStrings.Add("Metadata/Monsters/Guardians/GuardianHeadFireBossInvasion",
                "Area contains The Furious Mask");
            alertStrings.Add("Metadata/Monsters/Guardians/GuardianHeadColdBossInvasion",
                "Area contains The Callous Mask");
            alertStrings.Add("Metadata/Monsters/Guardians/GuardianHeadLightningBossInvasion",
                "Area contains The Capricious Mask");
            alertStrings.Add("Metadata/Monsters/GemMonster/IguanaBossInvasion", "Area contains Alpha Paradisae");
            alertStrings.Add("Metadata/Monsters/InsectSpawner/CarrionQueenBossInvasion",
                "Area contains Mother of the Swarm");
            alertStrings.Add("Metadata/Monsters/Kiweth/KiwethBossInvasion", "Area contains Deathflutter");
            alertStrings.Add("Metadata/Monsters/Lion/LionBossInvasion", "Area contains Bladetooth");
            alertStrings.Add("Metadata/Monsters/MossMonster/MossMonsterBossInvasion", "Area contains Granitecrush");
            alertStrings.Add("Metadata/Monsters/Necromancer/NecromancerBossInvasion", "Area contains Corpsestitch");
            alertStrings.Add("Metadata/Monsters/Pyromaniac/PyromaniacBossInvasion", "Area contains The Firestarter");
            alertStrings.Add("Metadata/Monsters/RootSpiders/RootSpiderBossInvasion", "Area contains Wrigglechaw");
            alertStrings.Add("Metadata/Monsters/SandSpitterEmerge/SandSpitterBossInvasion", "Area contains Blinkflame");
            alertStrings.Add("Metadata/Monsters/Seawitch/SeaWitchBossInvasion", "Area contains The Duchess");
            alertStrings.Add("Metadata/Monsters/ShieldCrabs/ShieldCrabBossInvasion", "Area contains Shivershell");
            alertStrings.Add("Metadata/Monsters/SandSpitters/SandSpitterFromCrabBossInvasion",
                "Area contains Shivershell");
            alertStrings.Add("Metadata/Monsters/Beasts/BeastSkeletonBossInvasion", "Area contains Mammothcage");
            alertStrings.Add("Metadata/Monsters/Skeletons/SkeletonElementalBossInvasion",
                "Area contains Harbinger of Elements");
            alertStrings.Add("Metadata/Monsters/Skeletons/SkeletonBowBossInvasion", "Area contains Nightsight");
            alertStrings.Add("Metadata/Monsters/Snake/SnakeMeleeBossInvasion", "Area contains Tailsinger");
            alertStrings.Add("Metadata/Monsters/Snake/SnakeRangedBossInvasion", "Area contains Razorleaf");
            alertStrings.Add("Metadata/Monsters/Spawn/SpawnBossInvasion", "Area contains Stranglecreep");
            alertStrings.Add("Metadata/Monsters/Spiders/SpiderBossInvasion", "Area contains Pewterfang");
            alertStrings.Add("Metadata/Monsters/Spikers/SpikerBossInvasion", "Area contains Bladeback Guardian");
            alertStrings.Add("Metadata/Monsters/Squid/SquidBossInvasion", "Area contains Strangledrift");
            alertStrings.Add("Metadata/Monsters/Totems/TotemBossInvasion", "Area contains Jikeji");
            alertStrings.Add("Metadata/Monsters/Rhoas/RhoaUndeadBossInvasion", "Area contains Ghostram");
            alertStrings.Add("Metadata/Monsters/Undying/UndyingBossInvasion", "Area contains Stranglecrawl");
            alertStrings.Add("Metadata/Monsters/WaterElemental/WaterElementalBossInvasion", "Area contains Mirageblast");
            alertStrings.Add("Metadata/Monsters/Zombies/ZombieBossInvasion", "Area contains The Walking Waste");
            alertStrings.Add("Metadata/Monsters/Skeletons/SkeletonMeleeBossInvasion", "Area contains Glassmaul");
            alertStrings.Add("Metadata/Monsters/Skeletons/SkeletonLargeBossInvasion", "Area contains Grath");
            alertStrings.Add("Metadata/Monsters/Skeletons/ConstructBossInvasion", "Area contains The Spiritless");
            alertStrings.Add("Metadata/Monsters/GhostPirates/GhostPirateBossInvasion", "Area contains Droolscar");
            alertStrings.Add("Metadata/Monsters/Rhoas/RhoaAlbino", "Area contains Albino Rhoa");
            alertStrings.Add("Metadata/NPC/Missions/Wild/Dex", "Area contains Tora, Master of the Hunt");
            alertStrings.Add("Metadata/NPC/Missions/Wild/DexInt", "Area contains Vorici, Master Assassin");
            alertStrings.Add("Metadata/NPC/Missions/Wild/Int", "Area contains Catarina, Master of the Dead");
            alertStrings.Add("Metadata/NPC/Missions/Wild/Str", "Area contains Haku, Armourmaster");
            alertStrings.Add("Metadata/NPC/Missions/Wild/StrDex", "Area contains Vagan, Weaponmaster");
            alertStrings.Add("Metadata/NPC/Missions/Wild/StrDexInt", "Area contains Zana, Master Cartographer");
            alertStrings.Add("Metadata/NPC/Missions/Wild/StrInt", "Area contains Elreon, Loremaster");
        }
    }
}