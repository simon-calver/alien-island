﻿using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {
	
	public bool walkable;
    public int gridX;
	public int gridY;

    public int movementPenalty;
    public int movementBonus;

    public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;
	
	public Node(bool _walkable, int _gridX, int _gridY, int _penalty, int _bonus) {
		walkable = _walkable;
		gridX = _gridX;
		gridY = _gridY;
        movementPenalty = _penalty;
        movementBonus = _bonus;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
