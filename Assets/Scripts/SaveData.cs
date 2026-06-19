using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public struct SaveData
{
    public static SaveData Instance;

    public HashSet<string> sceneNames;

    public string benchSceneName;
    public Vector2 benchPos;


    public int playerHealth;
    public float playerMana;
    public bool playerHalfMana;
    public Vector2 playerPosition;
    public string lastScene;

    public int maxAirJumps;

    public void Initialize()
    {
        if(!File.Exists(Application.persistentDataPath + "/save.bench.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.bench.data"));
        }

        if(!File.Exists(Application.persistentDataPath + "/save.bench.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.player.data"));
        }

        if(sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }

    public void SaveBench()
    {
        using(BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.bench.data")))
        {
            writer.Write(benchSceneName);
            writer.Write(benchPos.x);
            writer.Write(benchPos.y);
        }
    }

    public void LoadBench()
    {
        if(File.Exists(Application.persistentDataPath + "/save.bench.data"))
        {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.bench.data")))
            {
                benchSceneName = reader.ReadString();
                benchPos.x = reader.ReadSingle();
                benchPos.y = reader.ReadSingle();
            }
        }
    }

    public void SavePlayerData()
    {
        using(BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data")))
        {
            playerHealth = PlayerControl.Instance.Health;
            writer.Write(playerHealth);
            playerMana = PlayerControl.Instance.Mana;
            writer.Write(playerMana);
            playerHalfMana = PlayerControl.Instance.halfMana;
            writer.Write(playerHalfMana);


            playerPosition = PlayerControl.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);

            maxAirJumps = PlayerControl.Instance.maxAirJumps;
            writer.Write(maxAirJumps);
        }
    }

    public void LoadPlayerData()
    {
        if(File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data")))
            {
                playerHealth = reader.ReadInt32();
                playerMana = reader.ReadSingle();
                playerHalfMana = reader.ReadBoolean();
                playerPosition.x = reader.ReadSingle();
                playerPosition.y = reader.ReadSingle();
                lastScene = reader.ReadString();

                maxAirJumps = reader.ReadInt16();

                SceneManager.LoadScene(lastScene);
                PlayerControl.Instance.transform.position = playerPosition;
                PlayerControl.Instance.halfMana = playerHalfMana;
                PlayerControl.Instance.Health = playerHealth;
                PlayerControl.Instance.Mana = playerMana;

                PlayerControl.Instance.maxAirJumps = maxAirJumps;

            }
        }
        else
        {
            Debug.Log("File doesnt exist");
            PlayerControl.Instance.halfMana = false;
            PlayerControl.Instance.Health = PlayerControl.Instance.maxHealth;
            PlayerControl.Instance.Mana = 0.5f;

            PlayerControl.Instance.maxAirJumps = 0;
        }
    }
}
