using UnityEngine;

public static class LoadResources
{
    public static Material vertex_color = (Material)Resources.Load("Materials/Vertex Color");
    public static GameObject wheeled_robot_prefab = Resources.Load<GameObject>("Prefabs/Body/WheeledRobot/WheeledRobotPrefab");
}
