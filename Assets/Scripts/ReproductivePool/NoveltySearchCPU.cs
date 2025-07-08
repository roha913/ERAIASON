using Priority_Queue;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using System.Threading.Tasks;

public class NoveltySearchCPU : NoveltySearch
{
    public List<BehaviorCharacterizationCPUDatapoint[]> novelty_archive = new();
   

    public NoveltySearchCPU() : base()
    {
       
    }

    public override void AddToArchive(object behavior)
    {
        this.novelty_archive.Add(((BehaviorCharacterizationCPU)behavior).datapoints);

        if (LIMIT_SIZE)
        {
            if(this.novelty_archive.Count > SIZE_LIMIT)
            {
                this.novelty_archive.RemoveAt(0);
            }
        }

        Debug.Log("Added to archive, size is now " + this.novelty_archive.Count);
    }


    public override (bool, float) GetBehaviorNoveltyScoreAndTryAddToArchive(object behavior)
    {
        Debug.LogError("this is out of date compare to GPU code");
        bool added = false;
        if (NeedsMoreEntries())
        {
            added = true;
            // just add it
            // we can't calculate its novelty we need more in the archive
            // so just give it a default novelty.
            this.AddToArchive(behavior);
            return (true,chance_to_add_to_archive);
        }

        float novelty = GetAnimatNoveltyScore(behavior);

        if (novelty > 0.03)
        {
            added = true;
            this.AddToArchive(behavior);
        }

        //Debug.Log("Calculated novelty: " + novelty);
        return (added,novelty);
    }


    public override float GetAnimatNoveltyScore(object behavior_characterization)
    {
        if(this.novelty_archive.Count == 0) return chance_to_add_to_archive;
        // novelty score is the animat's sparsity compared to the archive
 

     

        float CalculateSumOfSquareDistances(BehaviorCharacterizationCPUDatapoint[] p1, BehaviorCharacterizationCPUDatapoint[] p2)
        {
            int bigger_array_size = math.max(p1.Length, p2.Length);

            float[] distances = new float[bigger_array_size];
            Parallel.For(0, bigger_array_size, i =>
            {
                float3 v1 = float3.zero; float food1 = 0; float foodvision1 = 0;
                float3 v2 = float3.zero; float food2 = 0; float foodvision2 = 0;


                int i1 = math.min(p1.Length - 1, i);
                int i2 = math.min(p2.Length - 1, i);

                if(p1.Length > 0)
                {
                   
                    v1 = p1[i1].offset_from_birthplace;
                    food1 = p1[i1].food_eaten_so_far;
                    foodvision1 = p1[i1].food_seen;
                }

                if (p2.Length > 0)
                {
                    v2 = p2[i2].offset_from_birthplace;
                    food2 = p2[i2].food_eaten_so_far;
                    foodvision2 = p2[i2].food_seen;
                }
                distances[i] += Vector2.Distance(v1.xz, v2.xz) / 10; // displacement
                distances[i] += math.abs(food1 - food2); // food eaten
                //total_distance += math.abs(foodvision1 - foodvision2); // food seen
            });
 
            return distances.Sum();
        }

        float total_novelty = 0;

        PriorityQueue<float> distances = new();/*Comparer<float>.Create((x, y) =>
        {
            return y.CompareTo(x);
        }));*/
        int amount = math.min(k_for_kNN, this.novelty_archive.Count);
    
        // find its average distance to k-nearest-neighbors in the archive
        //foreach (BehaviorCharacterization behavior in this.novelty_archive)
        //{
        //    float distance = CalculateSumOfSquareDistances(behavior, behavior_characterization);
        //    distances.Enqueue(distance, distance);
        //}


        Parallel.For(0, this.novelty_archive.Count, i =>
        {
            float distance = CalculateSumOfSquareDistances(this.novelty_archive[i], ((BehaviorCharacterizationCPU)behavior_characterization).datapoints);

            distances.Enqueue(distance, distance);
            //lock (distances) {
            //    if (distances.Count > amount) distances.Dequeue();
            //}

        });

        //var result = Enumerable.Range(0, this.novelty_archive.Count)
        //.AsParallel()
        //.Select((value, index) => new { Value = , Index = index }) // Include value and index
        //.OrderBy(x => x.Value) // Sort by the value
        //.Take(amount) // Take the smallest 5
        //.ToArray();
        //foreach (var item in result)
        //{
        //    Console.WriteLine($"Value: {item.Value}, Index: {item.Index}");
        //    total_novelty += item.Value; // dequeue gets the smallest element, i.e., the nearest neighbor
        //    i++;
        //}
        //}
        int a = 0;
        while (a < amount && distances.Count > 0)
        {
            float result = distances.Dequeue();  // dequeue gets the smallest element, i.e., the nearest neighbor
            total_novelty += result;
            a++;
        }
        total_novelty /= a;

        return total_novelty;
    }




    public override bool NeedsMoreEntries()
    {
        return this.novelty_archive.Count < k_for_kNN;
    }

}
