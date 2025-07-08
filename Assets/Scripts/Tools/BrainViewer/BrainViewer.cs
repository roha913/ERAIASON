using System.Collections.Generic;
using UnityEngine;
using static Brain;


public class BrainViewer : MonoBehaviour
{
    CPPNGenome genome;
    public Animat animat;
    Brain brain;

    // prefabs
    GameObject neuron_prefab;
    GameObject sensor_neuron_prefab;
    GameObject motor_neuron_prefab;
    GameObject synapse_prefab;

    public Dictionary<int, BrainViewerNeuron> neurons_GOs;
    public Dictionary<int, BrainViewerSynapse> synapse_GOs;

    private Vector3 spacing;

    bool is_alive = false;


    public bool SHOW_SYNAPSES = false;
    public bool did_develop = false;
    public bool did_show_synapses = false;



  
    public bool initialize_on_start = true;

    // Start is called before the first frame update
    void Awake()
    {
        // limit framerate to prevent GPU from going overdrive for no reason
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 45;

        // load prefabs
        neuron_prefab = (GameObject)Resources.Load("Prefabs/Tools/Neuron");
        sensor_neuron_prefab = (GameObject)Resources.Load("Prefabs/Tools/SensoryNeuron");
        motor_neuron_prefab = (GameObject)Resources.Load("Prefabs/Tools/MotorNeuron");
        synapse_prefab = (GameObject)Resources.Load("Prefabs/Tools/Synapse");

        this.spacing = Vector3.one * 100;
    }

    public void ClearScene()
    {
        if (this.neurons_GOs == null) return;
        foreach (KeyValuePair<int, BrainViewerNeuron> neuron in this.neurons_GOs)
        {
            Destroy(neuron.Value.gameObject);
        }
        foreach (KeyValuePair<int, BrainViewerSynapse> synapse in this.synapse_GOs)
        {
             Destroy(synapse.Value.gameObject);     
        }

        this.neurons_GOs.Clear();
        this.synapse_GOs.Clear();
        this.did_develop = false;
        this.did_show_synapses = false;
    }

    public void Initialize()
    {
        Debug.Log("Brain Viewer: Initialization Started.");
        ClearScene();
        this.neurons_GOs = new();
        this.synapse_GOs = new();
        this.animat = StaticSceneManager.animat;
        Debug.Log("Brain Viewer: Initialization Completed.");
    }


    public void SpawnNeurons()
    {
        Vector3 position;
        Vector3 default_location = Vector3.zero; //x=sensor, y=hidden, z=motor
        for (int i=0; i< this.brain.GetNumberOfNeurons();i++)
        {
            Neuron neuron = this.GetCurrentNeuron(i);
      
             position = new Vector3(neuron.position_normalized.x, neuron.position_normalized.y, neuron.position_normalized.z);  
            
            GameObject neuronGO;
            if (neuron.neuron_role == Neuron.NeuronRole.Motor)
            {
                neuronGO = Instantiate(motor_neuron_prefab, this.transform);
                neuronGO.transform.localScale *= 3;
            }
            else if (neuron.neuron_role == Neuron.NeuronRole.Sensor)
            {
                neuronGO = Instantiate(sensor_neuron_prefab, this.transform);
                neuronGO.transform.localScale *= 3;
            }
            else//if (neuron.sensory == Neuron.NeuronClass.Motor)
            {
                neuronGO = Instantiate(neuron_prefab, this.transform);
                neuronGO.transform.localScale *= 1.5f;
            }


            // offset the extra hidden neurons by 0.5f
            if (GlobalConfig.BRAIN_GENOME_METHOD == GlobalConfig.BrainGenomeMethod.CPPN)
            {
                position.x += (neuron.position_normalized.w * 0.5f / (CPPNGenome.SOFT_VOXEL_SUBSTRATE_DIMENSIONS.w));
                position.y += (neuron.position_normalized.v * 0.5f / (CPPNGenome.SOFT_VOXEL_SUBSTRATE_DIMENSIONS.v));
            }
            else
            {
                position.x += (neuron.position_normalized.w * 0.25f);
                position.y += (neuron.position_normalized.v * 0.5f);
            }

            //position.z += (neuron.position.w - 1) * 0.5f;


            var brainviewer_neuron = neuronGO.GetComponent<BrainViewerNeuron>();
            brainviewer_neuron.neuron = neuron;
            neuronGO.transform.localPosition = Vector3.Scale(position, spacing);
            this.neurons_GOs.Add(i, brainviewer_neuron);
        }

    }

    public void SpawnSynapses(bool update_only=false)
    {
        if(this.brain == null)
        {
            Debug.LogWarning("brain was null, cant view synapses");
            return;
        }
        did_show_synapses = true;
        
        for (int i = 0; i < this.brain.GetNumberOfNeurons(); i++)
        {
            GameObject to_neuron_GO = this.neurons_GOs[i].gameObject;
            Neuron to_neuron = this.GetCurrentNeuron(i);
            for (int j = to_neuron.synapse_start_idx; j < to_neuron.synapse_start_idx + to_neuron.synapse_count; j++)
            {
                Synapse synapse = this.GetCurrentSynapse(j);
                BrainViewerSynapse brain_viewer_synapse;
                if (!synapse.IsEnabled()) continue;
                if (!update_only)
                {
                    
                    GameObject from_neuron_GO = this.neurons_GOs[synapse.from_neuron_idx].gameObject;


                    if (synapse.from_neuron_idx != from_neuron_GO.GetComponent<BrainViewerNeuron>().neuron.idx
                        || synapse.to_neuron_idx != to_neuron_GO.GetComponent<BrainViewerNeuron>().neuron.idx) {
                        Debug.LogError("Error: neuron idxs dont match synapse");
                    }


                    GameObject synapseGO = Instantiate(synapse_prefab, this.transform);
                    brain_viewer_synapse = synapseGO.GetComponent<BrainViewerSynapse>();
                    brain_viewer_synapse.Initialize();

                    brain_viewer_synapse.gameObjectA = from_neuron_GO;
                    brain_viewer_synapse.gameObjectB = to_neuron_GO;
                    this.synapse_GOs[j] = brain_viewer_synapse;
                    synapseGO.GetComponent<LineRenderer>().SetPositions(new Vector3[] { brain_viewer_synapse.gameObjectA.transform.position, brain_viewer_synapse.gameObjectB.transform.position });
                }
                else
                {
                    brain_viewer_synapse = this.synapse_GOs[j];
                }

                brain_viewer_synapse.synapse = synapse;
             /*   brain_viewer_synapse.lr.startWidth = BrainViewerSynapse.START_WIDTH * synapse.weight;
                brain_viewer_synapse.lr.endWidth = BrainViewerSynapse.END_WIDTH * synapse.weight;*/

            }
        }
  
    }


    public bool UPDATE_SYNAPSES = false;
    uint frame = 0;
    void FixedUpdate()
    {

        if (this.animat == null || !this.animat.initialized) return;
       
    
        if (!this.did_develop)
        {
            if(animat.mind is Brain)
            {
                this.brain = ((Brain)animat.mind);
                this.SpawnNeurons();
            }
           
            this.did_develop = true;
        }
        if (this.brain == null) return;
        if (SHOW_SYNAPSES && !did_show_synapses) this.SpawnSynapses(false);

        if(frame % GlobalConfig.BRAIN_VIEWER_UPDATE_PERIOD == 0)
        {
            // update visual of neurons
            for (int i = 0; i < this.brain.GetNumberOfNeurons(); i++)
            {
                Neuron neuron = GetCurrentNeuron(i);
                GameObject neuronGO = this.neurons_GOs[i].gameObject;
                this.neurons_GOs[i].UpdateColor();
                this.neurons_GOs[i].neuron = neuron; // update current values
            }

            // update visual of synapses
            if (UPDATE_SYNAPSES) SpawnSynapses(true);
        }


        frame++;
        
    }

    public Neuron GetCurrentNeuron(int i)
    {
        Neuron neuron = this.brain.GetNeuronCurrentState(i);
        return neuron;
    }

    public Synapse GetCurrentSynapse(int i)
    {
        Synapse synapse;
        if (this.brain is BrainCPU)
        {
            synapse = ((BrainCPU)this.brain).current_state_synapses[i];
        }
        else //if (this.brain is BrainGPU)
        {
            synapse = ((BrainGPU)this.brain).GetCurrentSynapse(i);
        }
        return synapse;
    }
}
