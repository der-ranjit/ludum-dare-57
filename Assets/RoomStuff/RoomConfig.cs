using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum RoomStyle
{
    All,
    Bedroom,
    Forest,
    Dungeon,
    Cave
}
public static class WallStyleExtensions
{
    public static string ToStringValue(this RoomStyle wallStyle)
    {
        return wallStyle switch
        {
            RoomStyle.Bedroom => "Bedroom",
            RoomStyle.Forest => "Forest",
            RoomStyle.Dungeon => "Dungeon",
            RoomStyle.Cave => "Cave",
            RoomStyle.All => "All",
            _ => "Unknown"
        };
    }
}

public class RoomConfig
{
    public string roomName;
    public RoomStyle[] wallStyles;
    public RoomStyle[] decoStyles;
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
    public int decoCount;
    public System.Action<GameObject> finalizeRoom;

    public RoomConfig(
        string roomName,
        RoomStyle[] wallStyles,
        RoomStyle[] decoStyles,
        float width,
        float height,
        float playerSpawnX,
        float playerSpawnY,
        float doorPos,
        int slitCount,
        int fireCount = 0,
        int treeCount = 0,
        int stoneCount = 0,
        int enemyCount = 0,
        int decoCount = 0,
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
        this.decoCount = decoCount;
        this.finalizeRoom = finalizeRoom;
    }
}

// Helper class to access the function
public static class RoomConfigs
{
    public static RoomConfig GetBasicRoomInfo(int roomNum)
    {
        Debug.Log("Creating room " + roomNum.ToString() + " with style " + RoomStyle.All.ToStringValue() + " and size " + 10f.ToString() + "x" + 10f.ToString() + ".");
        switch (roomNum)
        {
            case -1:
                // Testing room
                // Get all room styles except ones you want to exclude
                var availableStyles = Enum.GetValues(typeof(RoomStyle))
                    .Cast<RoomStyle>()
                    .Where(style => style != RoomStyle.All) // Exclude "All" style
                    .ToArray();

                // Get a random style from the filtered list
                RoomStyle randomStyle = availableStyles[UnityEngine.Random.Range(0, availableStyles.Length)];
                Debug.Log("Random style: " + randomStyle.ToStringValue());

                return new RoomConfig(
                    "DefaultRoom",
                    new RoomStyle[] { randomStyle },
                    new RoomStyle[] { randomStyle },
                    UnityEngine.Random.Range(10f, 25f), // width
                    UnityEngine.Random.Range(10f, 25f), // height
                    0.5f,
                    0.1f,
                    UnityEngine.Random.Range(1, 4), // door pos
                    UnityEngine.Random.Range(0, 10), // slit count
                    0, // fire count
                    0,
                    0,
                    // UnityEngine.Random.Range(0, 4), // tree count
                    // UnityEngine.Random.Range(0, 4), // stone count
                    UnityEngine.Random.Range(1, 5), // enemy count
                    10
                );
            case 0: // <- hacky: room 0 is also harmless bedroom, this is behind title screen; once title screen is removed, we recreate room again to have fade-in and camera movement
            case 1:
                return new RoomConfig(
                    "Bedroom",
                    new RoomStyle[] { RoomStyle.Bedroom },
                    new RoomStyle[] { RoomStyle.All, RoomStyle.Bedroom },
                    7, // room size
                    10,
                    0.82f, // player spawn
                    0.15f,
                    3.5f, // door pos
                    0, // slit count
                    0, // fire count
                    0, // tree count
                    0, // stone count
                    0, // enemy count
                    0, // deco count
                    room =>
                    {
                        Debug.Log("Finalizing Bedroom room: " + room.name);
                        // Bed
                        GameObject bedPrefab = Resources.Load<GameObject>("Rooms/Bedroom/Deco/BedPrefab");
                        GameObject bedInstance = UnityEngine.Object.Instantiate(bedPrefab);
                        bedInstance.transform.position = new Vector3(2.2f, 0f, -2.9f);
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
                        // Pot
                        GameObject potPrefab = Resources.Load<GameObject>("Rooms/Bedroom/Deco/PotPrefab");
                        GameObject potInstance = UnityEngine.Object.Instantiate(potPrefab);
                        potInstance.transform.position = new Vector3(0.45f, 0f, -4.44f);
                        potInstance.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        potInstance.transform.parent = room.transform;
                        potInstance.name = "Pot 1";
                        GameObject potInstance2 = UnityEngine.Object.Instantiate(potPrefab);
                        potInstance2.transform.position = new Vector3(2.93f, 0, 4.41f);
                        potInstance2.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        potInstance2.transform.parent = room.transform;
                        potInstance2.name = "Pot 2";
                        // Bookshelf
                        GameObject bookshelfPrefab = Resources.Load<GameObject>("Rooms/Bedroom/ShelfPrefab");
                        GameObject bookshelfInstance = UnityEngine.Object.Instantiate(bookshelfPrefab);
                        bookshelfInstance.transform.position = new Vector3(-2.35f, 0f, 4.99f);
                        bookshelfInstance.transform.parent = room.transform;
                        bookshelfInstance.name = "Bookshelf";
                        // Lamp
                        GameObject lampPrefab = Resources.Load<GameObject>("Rooms/Bedroom/Deco/LampPrefab");
                        GameObject lampInstance = UnityEngine.Object.Instantiate(lampPrefab);
                        lampInstance.transform.position = new Vector3(1.6f, 0, 4.56f);
                        lampInstance.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        lampInstance.transform.parent = room.transform;
                        lampInstance.name = "Lamp";

                        DialogManager.Instance.StartDialog(new string[] {
                            "!wait 4",
                            "!turn Player 90",
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
                            "1: I will go over and demand my depth back.",
                            "!turn Player 270"
                        });

                        GameObject.FindGameObjectWithTag("Player").GetComponent<Weapon>().Disable();
                    }
                );
            case 9:
            case 2:
                return new RoomConfig(
                    "KevinRoom",
                    new RoomStyle[] { RoomStyle.Bedroom },
                    new RoomStyle[] { RoomStyle.All, RoomStyle.Bedroom },
                    13,
                    9,
                    0.7f, // player pos
                    0.6f,
                    1.5f, // door pos
                    0, // slit count
                    0, // fire count
                    0, // tree count
                    0, // stone count
                    0, // enemy count
                    0, // deco count
                    room =>
                    {
                        // Create Kevin
                        string prefabName = GameManager.Instance.kevinToUse;
                        GameObject kevinPrefab = Resources.Load<GameObject>("Characters/" + prefabName);
                        GameObject kevinInstance = UnityEngine.Object.Instantiate(kevinPrefab);
                        kevinInstance.transform.position = new Vector3(-4.58f, 0f, 2.4f);
                        kevinInstance.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        kevinInstance.transform.parent = room.transform;
                        kevinInstance.name = "Kevin";

                        // Desk
                        GameObject deskPrefab = Resources.Load<GameObject>("Rooms/Bedroom/Deco/DeskPrefab");
                        GameObject deskInstance = UnityEngine.Object.Instantiate(deskPrefab);
                        deskInstance.transform.position = new Vector3(-4.6f, -0.37f, 3.4f);
                        deskInstance.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        deskInstance.transform.parent = room.transform;
                        // Bookshelf
                        GameObject bookshelfPrefab = Resources.Load<GameObject>("Rooms/Bedroom/ShelfPrefab");
                        GameObject bookshelfInstance = UnityEngine.Object.Instantiate(bookshelfPrefab);
                        bookshelfInstance.transform.position = new Vector3(4.84f, 0, 4.35f);
                        bookshelfInstance.transform.parent = room.transform;
                        bookshelfInstance.name = "Bookshelf";
                        // Bookshelf
                        GameObject bookshelfInstance2 = UnityEngine.Object.Instantiate(bookshelfPrefab);
                        bookshelfInstance2.transform.position = new Vector3(-6.48f, 0, -0.52f);
                        bookshelfInstance2.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        bookshelfInstance2.transform.parent = room.transform;
                        bookshelfInstance2.name = "Bookshelf 2";
                        // Lamp
                        GameObject lampPrefab = Resources.Load<GameObject>("Rooms/Bedroom/Deco/LampPrefab");
                        GameObject lampInstance = UnityEngine.Object.Instantiate(lampPrefab);
                        lampInstance.transform.position = new Vector3(-2.1f, 0, 4f);
                        lampInstance.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        lampInstance.transform.parent = room.transform;
                        lampInstance.name = "Lamp";

                        if (roomNum == 2)
                        {
                            GameObject.FindGameObjectWithTag("Player").GetComponent<Weapon>().Disable();
                            // Initial dialog
                            DialogManager.Instance.StartDialog(new string[] {
                                "!wait 3",
                                "!cam -1.35 3.31 -2.75",
                                "!wait 2",
                                // "!move Player 2 5",
                                "1: Kevin!",
                                "2: *types and stares at computer*",
                                "1: Kevin!!!",
                                "2: *still typing*",
                                "1: ping Kevin!",
                                "2: *typing away*",
                                "1: sudo pay_attention_to_me!",
                                "2: ...",
                                "!cam -5.55 1.77 3.35",
                                "!typestop",
                                "!wait 1",
                                "!turn Kevin 110",
                                "2: Uhhhh what?",
                                "1: What have you done? I'm flat!",
                                "2: Ahhh yeah that, sorry, messed up with a commit last night and called the .flatten() method on the wrong object.",
                                "1: Great! Can you please revert that???",
                                "2: Why are you so negative? My flat earther friends love it!",
                                "2: Also it's not a priority to fix that. It's crunch time, have to make the deadline to ship that thing I'm working on.",
                                "1: Unacceptable!",
                                "2: Tell that to the PM. And now please let me deal with this merge conflict from hell...",
                                "!type",
                                "!turn Kevin 180",
                                "!wait 2",
                                "!cam 3.38 0.53 0.51",
                                "!wait 2",
                                "1: (Alright then, same procedure as every time. I have to hypnotize him.)",
                                "1: (And for that, I need the legendary pendulum of hypnotic depth.)",
                                "1: (Which I will surely find by fighting my way through hordes of flat monsters!)",
                                "!GIVEGUN",
                                "1: ...",
                                "!turn Player 90",
                            });
                        }
                        else
                        {
                            // Ending dialog
                            DialogManager.Instance.StartDialog(new string[] {
                                "!wait 2",
                                "!cam -1.35 3.31 -2.75",
                                "!wait 2",
                                "1: Kevin!",
                                "2: ...focused typing...",
                                "1: ...",
                                "1: (takes out legendary pendulum of maximum hypnotic depth)",
                                "!cam -2.17 1.5 0.12",
                                "1: (waves it around in front of Kevin)",
                                "1: All you see is the legendary pendulum of maximum hypnotic depth",
                                "!typestop",
                                "!turn Kevin 110",
                                "2: O_O",
                                "1: Your eyes become very heavy...",
                                "2: o_o",
                                "1: You can barely keep them open any longer...",
                                "2: -_-",
                                "1: You fall into a deep, deep trance...",
                                "2: _._",
                                "1: We're entering the deepest depths of your consciousness",
                                "1: Kevin?",
                                "2: ... ... yes _._",
                                "1: Please revert that commit that flattened our world.",
                                "2: ... ... yes _._",
                                "1: *snaps fingers*",
                                "2: Woah, where am I.",
                                "2: I feel a strong urge to revert something.",
                                "1: \\o/",
                                "!type",
                                "!turn Kevin 180",
                                "!cam -1.35 3.31 -2.75",
                                "1: Hooray! Kevin might take a while to revert this.",
                                "1: But my work here is done.",
                                "1: Guess if I like, I can hop back into the forest and see how long I survive!",
                                "1: (But if this were a game, then I surely would have won by now. :))"
                            });
                        }
                    }
                );
            case 3:
                return new RoomConfig(
                    "WarmupRoom",
                    new RoomStyle[] { RoomStyle.Forest },
                    new RoomStyle[] { RoomStyle.All, RoomStyle.Forest },
                    6,
                    15,
                    0.5f,
                    0.05f,
                    0.5f, // door pos
                    0, // slit count
                    0, // fire count
                    0, // tree count
                    0, // stone count
                    0, // enemy count
                    0, // deco count
                    room =>
                    {
                        // Create 3 slits
                        GameObject slitPrefab = Resources.Load<GameObject>("Rooms/SlitPrefab");
                        for (int i = 0; i < 8; i++)
                        {
                            GameObject slit = Slit.CreateSlit(
                                room,
                                new Vector2(-1.5f + 1f * (i % 4), -3 + 1.2f * i),
                                true
                            );
                            slit.transform.parent = room.transform;
                        }

                        DialogManager.Instance.StartDialog(new string[] {
                            "!wait 4",
                            // "!turn Player 90",
                            // "!cam -2.8 3.1 -4.1",
                            // "!wait 2",
                            "1: The pendulum is probably somewhere deep in this forest. Or some cave or whatnot.",
                            "1: I'll have to dive deep into the depths(!) of this flat new world to find it.",
                            "1: Too bad that our world is full of these little crevices on the floor though!",
                            "1: I never minded them, but now I'm flat and need to be careful not to fall into one.",
                            "1: I should either [SPACE] jump over them, or [Q] [E] rotate accordingly...",
                        });
                    }
                );
            case 4:
                return new RoomConfig(
                    "SlitTutorialRoom",
                    new RoomStyle[] { RoomStyle.Forest },
                    new RoomStyle[] { RoomStyle.All, RoomStyle.Forest },
                    7,
                    20,
                    0.5f,
                    0.1f,
                    0.5f, // door pos
                    0, // slit count
                    0, // fire count
                    0, // tree count
                    0, // stone count
                    0, // enemy count
                    0, // deco count
                    room =>
                    {
                        LoadInPrefabContents(room, "FullRooms/Room4Prefab");
                    }
                );
            case 5:
                return new RoomConfig(
                    "DungeonRoom",
                    new RoomStyle[] { RoomStyle.Dungeon },
                    new RoomStyle[] { RoomStyle.Dungeon },
                    16,
                    11,
                    0.5f,
                    0.1f,
                    2, // door pos
                    8, // slit count
                    0, // fire count
                    0, // tree count
                    0, // stone count
                    3, // enemy count
                    4, // deco count
                    room =>
                    {
                        DialogManager.Instance.StartDialog(new string[] {
                            "!wait 4",
                            // "!turn Player 90",
                            // "!cam -2.8 3.1 -4.1",
                            // "!wait 2",
                            "1: Oh-oh!",
                            "1: A dangerous dungeon!",
                            "1: With dangerous monsters!",
                            "1: A quick analysis leads me to see three viable strategies:",
                            "1: 1. Attack them with my weapons. I can [TAB] switch between them and [CLICK] to attack.",
                            "1: 2. Jump on their heads. A cool yet dangerous move.",
                            "1: 3. Steer them into the crevices. I have a feeling they follow my [Q] [E] rotation, which I could exploit.",
                            "1: But the latter only works for non-flying ones.",
                            "1: Once all monsters are defeated, the door will surely open and allow me to proceed!"
                        });
                    }
                );
            case 8:
                return new RoomConfig(
                    "ForestRoom",
                    new RoomStyle[] { RoomStyle.Forest },
                    new RoomStyle[] { RoomStyle.All },
                    5,
                    15,
                    0.5f,
                    0.05f,
                    0.5f, // door pos
                    0, // slit count
                    0, // fire count
                    0, // tree count
                    0, // stone count
                    0, // enemy count
                    0, // deco count
                    room =>
                    {
                        // Spawn PendulumPrefab in middle of room
                        GameObject pendulumPrefab = Resources.Load<GameObject>("Rooms/All/PendulumPrefab");
                        GameObject pendulumInstance = UnityEngine.Object.Instantiate(pendulumPrefab);
                        pendulumInstance.transform.position = new Vector3(0f, 0f, 0f);
                        pendulumInstance.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        pendulumInstance.transform.parent = room.transform;
                        pendulumInstance.name = "Pendulum";
                    }
                );

            default:
                Debug.Log("Unspecific room number. Creating randomized room.");
                int roomProgress = roomNum - 5;
                availableStyles = Enum.GetValues(typeof(RoomStyle))
                    .Cast<RoomStyle>()
                    .Where(style => style != RoomStyle.All) // Exclude "All" style
                    .ToArray();

                // Get a random style from the filtered list
                randomStyle = availableStyles[UnityEngine.Random.Range(0, availableStyles.Length)];
                if (roomNum == 6) { randomStyle = RoomStyle.Forest; }
                if (roomNum == 7) { randomStyle = RoomStyle.Cave; }
                return new RoomConfig(
                    "RandomRoom",
                    new RoomStyle[] { randomStyle },
                    new RoomStyle[] { randomStyle },
                    UnityEngine.Random.Range(10f, 25f) + roomProgress, // width
                    UnityEngine.Random.Range(10f, 25f) + roomProgress, // height
                    0.5f,
                    0.1f,
                    UnityEngine.Random.Range(0, 4), // door pos
                    UnityEngine.Random.Range(0, 10) + roomProgress, // slit count
                    UnityEngine.Random.Range(0f, 1f) < 0.4f ? 1 : 0, // fire count
                                                                     // UnityEngine.Random.Range(0, 4), // tree count
                                                                     // UnityEngine.Random.Range(0, 4), // stone count
                    0,
                    0,
                    UnityEngine.Random.Range(1, 7) + roomProgress / 3, // enemy count
                    UnityEngine.Random.Range(2, 10) // deco count
                );
        }
    }

    private static string[] eligiblePrefabContentPrefixes = new string[]
            {
                "Tree",
                "Slit",
                "Stone",
                "Fire",
                "Bed",
                "Rug",
                "Table",
                "Desk"
            };

    private static void LoadInPrefabContents(GameObject room, string roomPrefabName)
    {
        GameObject prefab = Resources.Load<GameObject>(roomPrefabName);
        if (prefab == null)
        {
            Debug.LogError($"Failed to load prefab: {roomPrefabName}");
            return;
        }
        GameObject instance = UnityEngine.Object.Instantiate(prefab);
        // Take all the children of instance and attach them to room, go in reverse order
        foreach (Transform child in instance.transform.Cast<Transform>().Reverse())
        {
            // Only load prefabs that start with Tree, Slit, Stone, or Fire
            bool isEligible = eligiblePrefabContentPrefixes.Any(prefix => child.name.StartsWith(prefix));
            if (isEligible)
            {
                // Set the position and rotation of the child to match the prefab
                child.parent = room.transform;
            }
        }
        // Destroy the instance, we only need the children
        UnityEngine.Object.DestroyImmediate(instance);
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