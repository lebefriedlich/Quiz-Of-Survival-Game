using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class MazeLogic : MonoBehaviour
{
    // Start is called before the first frame update

    public int width = 30;
    public int depth = 30;
    public int scale = 6;
    public GameObject Character;
    public GameObject Scroll;
    public int ScrollCount = 5;
    public List<GameObject> Cube;
    public int RoomCount = 3;
    public int RoomMinSize = 6;
    public int RoomMaxSize = 10;
    public byte[,] map;


    void Start()
    {
        InitializeMap();
        AddRoom(RoomCount, RoomMinSize, RoomMaxSize);
        GenerateMaps();
        DrawMaps();
        GetComponent<NavMeshSurface>().BuildNavMesh();
        PlaceCharacter();
        PlaceScroll();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitializeMap()
    {
        map = new byte[width, depth];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1;
            }
        }
    }
    public virtual void AddRoom(int count, int minSize, int maxSize)
    {
        for (int c = 0; c < count; c++)
        {
            int StartX = Random.Range(3, width - 3);
            int StartZ = Random.Range(3, depth - 3);
            int RoomWidth = Random.Range(minSize, maxSize);
            int RoomDepth = Random.Range(minSize, maxSize);
            for (int x = StartX; x < width - 3 && x < StartX + RoomWidth; x++)
            {
                for (int z = StartZ; z < depth - 3 && z < StartZ + RoomDepth; z++)
                {
                    map[x, z] = 2;
                }
            }

        }
    }
    public virtual void GenerateMaps()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (Random.Range(0, 100) < 50)
                {
                    map[x, z] = 0;
                }
            }
        }
    }

    void DrawMaps()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1)
                {
                    Vector3 position = new Vector3(x * scale, 5, z * scale);
                    GameObject wall = Instantiate(Cube[Random.Range(0, Cube.Count)], position, Quaternion.identity);
                    wall.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
    }
    public int CountSquareNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        return count;
    }
    public class MapLocation
    {
        public int x; // x coordinate
        public int z; // z coordinate

        public MapLocation(int _x, int _z)
        {
            x = _x; // assign x coordinate
            z = _z; // assign z coordinate
        }
    }

    public virtual void PlaceCharacter()
    {
        bool PlayerSet = false;
        for (int i = 0; i < depth; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int x = Random.Range(0, width);
                int z = Random.Range(0, depth);
                if (map[x, z] == 0 && !PlayerSet)
                {
                    Debug.Log("Placing Character");
                    PlayerSet = true;
                    Character.transform.position = new Vector3(x * scale, 3, z * scale);
                }
                else if (PlayerSet)
                {
                    Debug.Log("Already placed character");
                    return;
                }
            }
        }
    }

    public virtual void PlaceScroll()
    {
        int Scrollset = 0;
        List<Vector3> placedScrolls = new List<Vector3>();

        while (Scrollset < ScrollCount)
        {
            int x = Random.Range(0, width);
            int z = Random.Range(0, depth);

            if (map[x, z] == 0)
            {
                Vector3 potentialPosition = new Vector3(x * scale, 3, z * scale);

                bool isFarEnough = true;
                foreach (Vector3 scrollPosition in placedScrolls)
                {
                    if (Vector3.Distance(potentialPosition, scrollPosition) < scale * 3)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                if (isFarEnough)
                {
                    GameObject newScroll = Instantiate(Scroll, potentialPosition, Quaternion.identity);
                    newScroll.transform.position = potentialPosition;
                    newScroll.name = "Scroll_" + Scrollset;

                    // Menambahkan Rigidbody
                    Rigidbody rb = newScroll.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.useGravity = false;
                        rb.isKinematic = true;
                    }

                    // Menambahkan script animasi ke scroll
                    FloatingAnimation floatAnim = newScroll.AddComponent<FloatingAnimation>();

                    placedScrolls.Add(potentialPosition);

                    Scrollset++;
                }
            }
        }

        Debug.Log($"Successfully placed {Scrollset} scrolls.");
    }
}
