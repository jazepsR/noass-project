using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationPointCalculator : MonoBehaviour
{
    public static DestinationPointCalculator instance;
    Dictionary<Destination, int> currentDistribution = new Dictionary<Destination, int>();
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public int GetTypeCountInDistribution(Destination destination, List<SnapPoint> snapPoints)
    {
        CalculateTypeDistribution(snapPoints);
        int count = 0;
        currentDistribution.TryGetValue(destination, out count);
        return count;
    }
    public void CalculateTypeDistribution(List<SnapPoint> snapPoints)
    {
        currentDistribution = new Dictionary<Destination, int>();
        foreach (SnapPoint snapPoint in snapPoints)
        {
            if (!snapPoint.isTrash && snapPoint.occupied)
            {
                Destination[] destinations = snapPoint.content.GetComponent<TileScript>().data.possibleDestinations;
                currentDistribution = AddToDestinationList(currentDistribution, destinations);
            }
        }
    }

    public Dictionary<Destination, int> AddToDestinationList(Dictionary<Destination, int> destinationList, Destination[] destinationsToAdd)
    {
        if (destinationsToAdd.Length > 0)
        {
            foreach (Destination dest in destinationsToAdd)
            {
                if (destinationList.ContainsKey(dest))
                {
                    destinationList[dest] = destinationList[dest] + 1;
                }
                else
                {
                    destinationList.Add(dest, 1);
                }
            }
        }
        return destinationList;
    }
}
