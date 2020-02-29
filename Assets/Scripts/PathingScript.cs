using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathingScript : MonoBehaviour {

  private Drone drone;


	// Use this for initialization
	void Start () {
		drone = GameObject.Find("ThermalDrone").GetComponent<Drone>() as Drone;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public List<Zone> getPathSequence(int startZoneIdentifier, int endZoneIdentifier) {
    Zone sourZone = zones[startZoneIdentifier];
    Zone destZone = zones[endZoneIdentifier];
    List<Zone> path = dijkstra(sourZone, destZone); 

    return path;
  }

	//Returns the shortest distance between two nodes - all edge weights are 1
  private List<Zone> dijkstra(Zone source, Zone destination)
  {
  	int numZones = 20*20*20;
  	float[] distances = new float[numZones];
  	DijkstraNode dSource = new DijkstraNode(source.id);
  	DijkstraNode dDestination = new DijkstraNode(destination.id);
  	for (int i=0; i<numZones; i++)
  	{
  		distances[i] = Mathf.Infinity;
  	}
  	distances[dSource.identifier] = 0;
  	//This uses two new classes: DijkstraNode is simply a pair of identifier and distance
  	//DijkstraNodeComparator sorts DijkstraNodes by distance
  	//so that the priority queue always returns the closest node
  	List<DijkstraNode> dijkstraNodes = new List<DijkstraNode>();
  	//Start from the source node
  	DijkstraNode currentNode = dSource;

  	//while (!currentNode.equals(dDestination))
  	while (currentNode.identifier != dDestination.identifier)
  	{
  		//Get all edges from the current node
  		Edge[] newEdges = new Edge[26];
  		////////////// TODO get current available edges
  		// currentZone.getEdges();
  		//Convert edge destinations to DijkstraNodes with the updated distance
  		foreach (Edge e in newEdges) {
  			DijkstraNode newDNode = new DijkstraNode(e.endIdentifier);
  			newDNode.distance = currentNode.distance + drone.edgeCost(zones[currentNode.identifier], zones[newDNode.identifier]);
        newDNode.previousNode = currentNode;

  			//Check if node in question has already been added into the search tree with a
  			//longer route. If yes, replace it with a shorter one
  			if (dijkstraNodes.Contains(newDNode) && newDNode.distance < distances[newDNode.identifier]) {
  				//equals() has been defined for DNodes based on identifier attribute
  				dijkstraNodes.Remove(newDNode);
  				distances[newDNode.identifier] = newDNode.distance;

  				dijkstraNodes.Add(newDNode);
  			}
  			else if (distances[newDNode.identifier] == Mathf.Infinity) {
  				distances[newDNode.identifier] = newDNode.distance;
  				dijkstraNodes.Add(newDNode);
  			}
  		}

  		//Sort list
  		dijkstraNodes.Sort(new DijkstraNodeComparator());
  		currentNode = dijkstraNodes[0];
  		dijkstraNodes.Remove(currentNode);
  	}

  	//Once the destionation node is currentNode, return the distance
    List<Zone> shortestPath = new List<Zone>();
    DijkstraNode backwardsNode = currentNode;
    while (backwardsNode != dSource) {
      shortestPath.Add(zones[backwardsNode.identifier]);
      backwardsNode = backwardsNode.previousNode;
    }
    List<Zone> revShortestPath = shortestPath.Reverse();
  	return revShortestPath;
  }

  public class DijkstraNode : System.IEquatable<DijkstraNode>{
		public float distance;
		public int identifier;
    public DijkstraNode previousNode;

		public DijkstraNode(int initIdentifier) {
			identifier = initIdentifier;
			distance = 0.0f;
		}
		public bool Equals(DijkstraNode other) {
        if (other == null) return false;
        return (this.identifier.Equals(other.identifier));
    }
	}
	public class Edge {
		public float distance;
		public int startIdentifier;
		public int endIdentifier;

		public Edge(int IstartIdentifier, int IendIdentifier,  float initDistance) {
			startIdentifier = IstartIdentifier;
			endIdentifier = IendIdentifier;
			distance = initDistance;
		}
	}

	public class DijkstraNodeComparator : IComparer<DijkstraNode> {
		public int Compare(DijkstraNode a, DijkstraNode b) {
			return (int)Mathf.Round(a.distance - b.distance);
		}
	}
}