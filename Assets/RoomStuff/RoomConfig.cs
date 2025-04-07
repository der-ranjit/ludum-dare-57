using System;
using System.Collections;
using UnityEngine;

public enum WallStyle
{
    Bedroom,
    Forest,
    Dungeon
}

public static class WallStyleExtensions
{
    public static string ToStringValue(this WallStyle wallStyle)
    {
        return wallStyle switch
        {
            WallStyle.Bedroom => "Bedroom",
            WallStyle.Forest => "Forest",
            WallStyle.Dungeon => "Dungeon",
            _ => "Unknown"
        };
    }
}


public enum DecoStyle
{
    All,
    Bedroom,
    Forest
}

public static class DecoStyleExtensions
{
    public static string ToStringValue(this DecoStyle decoStyle)
    {
        return decoStyle switch
        {
            DecoStyle.All => "All",
            DecoStyle.Bedroom => "Bedroom",
            DecoStyle.Forest => "Forest",
            _ => "Unknown"
        };
    }
}

public class RoomConfig
{
    public string roomName;
    public WallStyle[] wallStyles;
    public DecoStyle[] decoStyles;
    public float width;
    public float height;
    public float playerSpawnX;
    public float playerSpawnY;
    public float doorPos;
    public int slitCount;
    public int fireCount;

    public int treeCount;
    public int stoneCount;
    public int enemyCount;
    public System.Action<GameObject> finalizeRoom;

    public RoomConfig(
        string roomName,
        WallStyle[] wallStyles,
        DecoStyle[] decoStyles,
        float width,
        float height,
        float playerSpawnX,
        float playerSpawnY,
        float doorPos,
        int slitCount,
        int fireCount,
        int treeCount,
        int stoneCount,
        int enemyCount,
        System.Action<GameObject> finalizeRoom = null
    )
    {
        this.roomName = roomName;
        this.wallStyles = wallStyles;
        this.decoStyles = decoStyles;
        this.width = width;
        this.height = height;
        this.playerSpawnX = playerSpawnX;
        this.playerSpawnY = playerSpawnY;
        this.doorPos = doorPos;
        this.slitCount = slitCount;
        this.fireCount = fireCount;
        this.treeCount = treeCount;
        this.stoneCount = stoneCount;
        this.enemyCount = enemyCount;
        this.finalizeRoom = finalizeRoom;
    }
}

// Helper class to access the function
public static class RoomConfigs
{
    public static RoomConfig GetBasicRoomInfo(int roomNum)
    {
        switch (roomNum)
        {
            case 0:
                return new RoomConfig(
                    "DefaultRoom",
                    new WallStyle[] { WallStyle.Forest },
                    new DecoStyle[] { DecoStyle.All, DecoStyle.Forest },
                    UnityEngine.Random.Range(10f, 25f), // width
                    UnityEngine.Random.Range(10f, 25f), // height
                    0.5f,
                    0.1f,
                    UnityEngine.Random.Range(1, 4), // door pos
                    UnityEngine.Random.Range(0, 10), // slit count
                    1, // fire count
                    UnityEngine.Random.Range(0, 4), // tree count
                    UnityEngine.Random.Range(0, 4), // stone count
                    UnityEngine.Random.Range(1, 5) // enemy count
                );
            case 1:
                return new RoomConfig(
                    "Bedroom",
                    new WallStyle[] { WallStyle.Bedroom },
                    new DecoStyle[] { DecoStyle.All, DecoStyle.Bedroom },
                    7, // room size
                    10,
                    0.8f, // player spawn
                    0.3f,
                    3.8f, // door pos
                    0, // slit count
                    0, // fire count
                    0, // tree count
                    0, // stone count
                    0, // enemy count
                    room =>
                    {
                        Debug.Log("Finalizing Bedroom room: " + room.name);
                        // Bed
                        GameObject bedPrefab = Resources.Load<GameObject>("Rooms/Bedroom/Deco/BedPrefab");
                        GameObject bedInstance = UnityEngine.Object.Instantiate(bedPrefab);
                        bedInstance.transform.position = new Vector3(2f, 0f, -2f);
                        bedInstance.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        bedInstance.transform.parent = room.transform;
                        bedInstance.name = "Bed";
                        // Rug
                        GameObject rugPrefab = Resources.Load<GameObject>("Rooms/Bedroom/RugPrefab");
                        GameObject rugInstance = UnityEngine.Object.Instantiate(rugPrefab);
                        rugInstance.transform.position = new Vector3(0.73f, 0.01f, -0.57f);
                        rugInstance.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
                        rugInstance.transform.parent = room.transform;
                        rugInstance.name = "Rug";
                        // Table
                        GameObject tablePrefab = Resources.Load<GameObject>("Rooms/Bedroom/TablePrefab");
                        GameObject tableInstance = UnityEngine.Object.Instantiate(tablePrefab);
                        tableInstance.transform.position = new Vector3(-2.79f, 0.81f, -4.23f);
                        tableInstance.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        tableInstance.transform.parent = room.transform;
                        tableInstance.name = "Table";

                        DialogManager.Instance.StartDialog(new string[] {
                            "!wait 4",
                            "!cam -2.8 3.1 -4.1",
                            "!wait 2",
                            // "!move Player 2 5",
                            "1: Ahhhhh!",
                            "1: Something is off.",
                            "1: Just yesterday, I was a regular 3-dimensional person.",
                            "1: And now it somehow appears I have lost my depth.",
                            "1: This is unacceptable!",
                            "1: There is only one possible cause for this madness!",
                            "1: My neighbor Kevin!!! Any time something strange happens, you can be sure he's at fault.",
                            "1: I will go over and demand my depth back."
                        });
                    }
                );
            case 2:
                return new RoomConfig(
                    "KevinRoom",
                    new WallStyle[] { WallStyle.Bedroom },
                    new DecoStyle[] { DecoStyle.All, DecoStyle.Bedroom },
                    11,
                    7,
                    0.5f,
                    0.1f,
                    3, // door pos
                    0, // slit count
                    0, // fire count
                    0, // tree count
                    0, // stone count
                    0 // enemy count
                );
            case 3:
                return new RoomConfig(
                    "WarmupRoom",
                    new WallStyle[] { WallStyle.Forest },
                    new DecoStyle[] { DecoStyle.All, DecoStyle.Forest },
                    15,
                    10,
                    0.5f,
                    0.1f,
                    2, // door pos
                    0, // slit count
                    1, // fire count
                    5, // tree count
                    4, // stone count
                    0 // enemy count
                );
            case 4:
                return new RoomConfig(
                    "SlitTutorialRoom",
                    new WallStyle[] { WallStyle.Forest },
                    new DecoStyle[] { DecoStyle.All, DecoStyle.Forest },
                    12,
                    8,
                    0.5f,
                    0.1f,
                    2, // door pos
                    8, // slit count
                    0, // fire count
                    0, // tree count
                    0, // stone count
                    0 // enemy count
                );
            case 5:
                return new RoomConfig(
                    "DungeonRoom",
                    new WallStyle[] { WallStyle.Dungeon },
                    new DecoStyle[] { DecoStyle.All },
                    12,
                    8,
                    0.5f,
                    0.1f,
                    2, // door pos
                    8, // slit count
                    0, // fire count
                    0, // tree count
                    0, // stone count
                    0 // enemy count
                );

            default:
                Debug.LogError("Invalid room number. Creating default room.");
                return new RoomConfig(
                    "RandomRoom",
                    new WallStyle[] { WallStyle.Forest },
                    new DecoStyle[] { DecoStyle.All, DecoStyle.Forest },
                    UnityEngine.Random.Range(10f, 25f), // width
                    UnityEngine.Random.Range(10f, 25f), // height
                    0.5f,
                    0.1f,
                    UnityEngine.Random.Range(0, 4), // door pos
                    UnityEngine.Random.Range(0, 10), // slit count
                    UnityEngine.Random.Range(0f, 1f) < 0.4f ? 1 : 0, // fire count
                    UnityEngine.Random.Range(0, 4), // tree count
                    UnityEngine.Random.Range(0, 4), // stone count
                    UnityEngine.Random.Range(1, 10) // enemy count
                );
        }
    }
}

public class DelayedAction : MonoBehaviour
{
    public void ExecuteAfterDelay(float seconds, System.Action callback)
    {
        StartCoroutine(DelayRoutine(seconds, callback));
    }

    private IEnumerator DelayRoutine(float seconds, System.Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }
}