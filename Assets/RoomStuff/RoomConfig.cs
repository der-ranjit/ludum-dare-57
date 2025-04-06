using System;
using System.Collections;
using UnityEngine;


public class RoomConfig 
{
    public string roomName;
    public string wallMaterialName;
    public string floorMaterialName;
    public float width;
    public float height;
    public int slitCount;
    public int doorPos;
    public int fireCount;

    public int treeCount;
    public int stoneCount;
    public int enemyCount;
    public System.Action<GameObject> finalizeRoom;

    public RoomConfig(
        string roomName,
        string wallMaterialName,
        string floorMaterialName,
        float width,
        float height,
        int doorPos,
        int slitCount,
        int fireCount,
        int treeCount,
        int stoneCount,
        int enemyCount,
        System.Action<GameObject> finalizeRoom = null
    )
    {
        this.roomName = roomName;
        this.wallMaterialName = wallMaterialName;
        this.floorMaterialName = floorMaterialName;
        this.width = width;
        this.height = height;
        this.slitCount = slitCount;
        this.doorPos = doorPos;
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
        switch (roomNum) {
            case 0:
                return new RoomConfig(
                    "DefaultRoom",
                    "forestMaterial",
                    "forestFloorMaterial",
                    UnityEngine.Random.Range(10f, 25f), // width
                    UnityEngine.Random.Range(10f, 25f), // height
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
                    "forestMaterial",
                    "forestFloorMaterial",
                    7,
                    10,
                    3, // door pos
                    0, // slit count
                    0, // fire count
                    0, // tree count
                    0, // stone count
                    0, // enemy count
                    room => {
                        GameObject bedPrefab = Resources.Load<GameObject>("BedPrefab");
                        GameObject bedInstance = UnityEngine.Object.Instantiate(bedPrefab);
                        bedInstance.transform.position = new Vector3(2f, 0f, -2f);
                        bedInstance.transform.parent = room.transform;
                        bedInstance.name = "Bed";

                        DialogManager.Instance.StartDialog(new string[] {
                            "!wait 3",
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
                    "forestMaterial",
                    "forestFloorMaterial",
                    11,
                    7,
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
                    "forestMaterial",
                    "forestFloorMaterial",
                    15,
                    10,
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
                    "forestMaterial",
                    "forestFloorMaterial",
                    12,
                    8,
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
                    "forestMaterial",
                    "forestFloorMaterial",
                    UnityEngine.Random.Range(10f, 25f), // width
                    UnityEngine.Random.Range(10f, 25f), // height
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