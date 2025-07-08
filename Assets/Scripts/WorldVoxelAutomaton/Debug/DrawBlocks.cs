using UnityEngine;

public class DrawBlocks : MonoBehaviour
{

    public WorldAutomaton automaton;
    public Transform blocks1;
    public Transform blocks2;
    // Start is called before the first frame update
    void Start()
    {
        Transform block1 = blocks1.GetChild(0);
        Transform block2 = blocks2.GetChild(0);

        for(int x = 0; x < GlobalConfig.WORLD_DIMENSIONS.x / 2; x++)
        {
            for (int y = 0; y < GlobalConfig.WORLD_DIMENSIONS.y / 2; y++)
            {
                for (int z = 0; z < GlobalConfig.WORLD_DIMENSIONS.z / 2; z++)
                {
                    Instantiate(block1, new Vector3(x*2, y*2, z*2), Quaternion.identity, blocks1);
                    Instantiate(block2, new Vector3(x * 2, y * 2, z * 2), Quaternion.identity, blocks2);
                }
            }
        }
    }


    // Update is called once per frame
    uint frame = 0;
    void Update()
    {
        //if(frame < GlobalConfig.WORLD_AUTOMATA_UPDATE_PERIOD) frame++;
        //if (frame >= GlobalConfig.WORLD_AUTOMATA_UPDATE_PERIOD)
        //{
        //    blocks1.gameObject.SetActive(automaton.margolus_frame % 2 == 0);
        //    blocks2.gameObject.SetActive(automaton.margolus_frame % 2 == 1);
        //    frame = 0;
        //}
        
    }
}
