using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using DxLibDLL;
using MyLib;

namespace Yılan
{
    public class Map
    {
        // マップチップ種類
        public const int None = 0;
        public const int Wall = 1;

        public int Width = 50; // マップ横幅
        public int Height = 28; // マップ縦幅
        public const int CellSize = 32; // マップチップサイズ

        BattlePhase battlePhase;

        int[,] terrain;

        public static List<EnemyBase> enemyBases = new List<EnemyBase>();
        public static List<HealingItem> healingItems = new List<HealingItem>();

        public static int count; // 敵の数
        public static int maxCount = -1; // 全敵数

        public Map(BattlePhase battlePhase, string stageName)
        {
            this.battlePhase = battlePhase;

            if (stageName != "Map11") // 最終ステージじゃなかったら
            {// ロード
                LoadTerrain("Resource/Map/" + stageName + "_terrain.csv");
                LoadObjects("Resource/Map/" + stageName + "_object.csv");
            }
            else
            {
                DX.PlaySoundMem(Music.endcredit_bgm, DX.DX_PLAYTYPE_LOOP); // エンドクレジットBGM
                Game.ChangeScene(new EndCredit()); // エンドクレジットへ
                AdvPhase.massegeNum = 1; // メッセージ番号初期化
                BattlePhase.stageNum = 1; // ステージ番号初期化
            }
        }

        void LoadTerrain(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            // 行数が0だった場合はアサートで警告
            Debug.Assert(lines.Length > 0, filePath + "の高さが不正です ;" + lines.Length);
            Height = lines.Length;
            // 【1行先読みしてWifth確定】1行をカンマで分割する
            string[] preSplitted = lines[0].Split(new char[] { ',' });
            // 1行先読みしてWifth確定
            Width = preSplitted.Length;
            terrain = new int[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                string[] splitted = lines[y].Split(new char[] { ',' });

                Debug.Assert(splitted.Length == Width, filePath + "の" + y + "行目の列数が不正です:" + splitted.Length);

                for (int x = 0; x < Width; x++)
                {
                    terrain[x, y] = int.Parse(splitted[x]);
                }
            }
        }

        void LoadObjects(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath); // ファイルを行ごとに読み込む

            // 行数の検証
            Debug.Assert(lines.Length == Height, filePath + "の高さが不正です : " + lines.Length);

            for (int y = 0; y < Height; y++)
            {
                // 行をカンマで分割する
                string[] splitted = lines[y].Split(new char[] { ',' });

                // 行数の検証
                Debug.Assert(splitted.Length == Width, filePath + "の" + y + "行目の列数が不正です : " + splitted.Length);

                for (int x = 0; x < Width; x++)
                {
                    // 文字から整数に変換して、番号に応じた敵を生成する
                    int id = int.Parse(splitted[x]);

                    // -1（何も配置されていない場所は何もしない 
                    if (id == -1) continue;

                    // オブジェクトを生成・配置する
                    SpawnObject(x, y, id);
                }
            }

            maxCount = count;
        }

        // オブジェクトを生成・配置する
        public void SpawnObject(int mapX, int mapY, int objectID)
        {
            // 生成位置
            float spawnX = mapX * CellSize;
            float spawnY = mapY * CellSize;

            if (objectID == 1) // Enemy1
            {
                battlePhase.enemyBases.Add(new Enemy1(battlePhase, spawnX, spawnY));
                if (maxCount != count)
                {
                    Enemy1 enemy1 = new Enemy1(battlePhase, spawnX, spawnY);
                    enemyBases.Add(enemy1);
                    count++;
                }
            }
            else if (objectID == 2) // Enemy2
            {
                battlePhase.enemyBases.Add(new Enemy2(battlePhase, spawnX, spawnY));
                if (maxCount != count)
                {
                    Enemy2 enemy2 = new Enemy2(battlePhase, spawnX, spawnY);
                    enemyBases.Add(enemy2);
                    count++;
                }
            }
            else if (objectID == 3) // Enemy3
            {
                battlePhase.enemyBases.Add(new Enemy3(battlePhase, spawnX, spawnY));
                if (maxCount != count)
                {
                    Enemy3 enemy3 = new Enemy3(battlePhase, spawnX, spawnY);
                    enemyBases.Add(enemy3);
                    count++;
                }
            }
            else if (objectID == 4) // Enemy4
            {
                battlePhase.enemyBases.Add(new Enemy4(battlePhase, spawnX, spawnY));
                if (maxCount != count)
                {
                    Enemy4 enemy4 = new Enemy4(battlePhase, spawnX, spawnY);
                    enemyBases.Add(enemy4);
                    count++;
                }
            }
            else if (objectID == 50) // Boss1
            {
                battlePhase.enemyBases.Add(new Boss1(battlePhase, spawnX, spawnY));
                if (maxCount != count)
                {
                    Boss1 boss1 = new Boss1(battlePhase, spawnX, spawnY);
                    enemyBases.Add(boss1);
                    count++;
                }
            }
            else if (objectID == 51) // Boss2
            {
                battlePhase.enemyBases.Add(new Boss2(battlePhase, spawnX, spawnY));
                if (maxCount != count)
                {
                    Boss2 boss2 = new Boss2(battlePhase, spawnX, spawnY);
                    enemyBases.Add(boss2);
                    count++;
                }
            }
            else if (objectID == 70) // Item1
            {
                HealingItem healingItem = new HealingItem(battlePhase, spawnX, spawnY);
                healingItems.Add(healingItem);

                battlePhase.gameObjects.Add(new HealingItem(battlePhase, spawnX, spawnY));
            }

            // 新しい種類のオブジェクトを作ったら、ここに生成処理を追加
            else
            {
                Debug.Assert(false, "オブジェクトID" + objectID + "番の生成処理は未実装です。");
            }
        }

        public void DrawTerrain()
        {
            int left = (int)(Camera.x / CellSize);
            int top = (int)(Camera.y / CellSize);
            int right = (int)(Camera.x / Screen.Width - 1);
            int bottom = (int)(Camera.y / Screen.Height - 1);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int id = terrain[x, y];

                    Camera.DrawGraph(x * CellSize, y * CellSize, 1, 0, Image.mapchip[id]);
                }
            }
        }

        public int GetTerrain(float worldX, float worldY)
        {
             if (worldX < 0 || worldY < 0)
                return None;

            int mapX = (int)(worldX / CellSize);
            int mapY = (int)(worldY / CellSize);

            if (mapX >= Width || mapY >= Height)
                return None;

            return terrain[mapX, mapY];
        }

        public bool IsWall(float worldX, float worldY)
        {
            int terrainID = GetTerrain(worldX, worldY);

            return terrainID == Wall;
        }
    }
}
