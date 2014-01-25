using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SettlersEngine
{
    public class MyPathNode : SettlersEngine.IPathNode<System.Object>
	{
	    public Int32 X { get; set; }
	    public Int32 Y { get; set; }
	    public Boolean IsWall {get; set;}
	
	    public bool IsWalkable(System.Object unused)
	    {
	        return !IsWall;
	    }
	}
	
	public class MySolver<TPathNode, TUserContext> : SettlersEngine.SpatialAStar<TPathNode, TUserContext> where TPathNode : SettlersEngine.IPathNode<TUserContext>
	{
	    protected override Double Heuristic(PathNode inStart, PathNode inEnd)
	    {
	        return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
	    }
	
	    protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
	    {
	        return Heuristic(inStart, inEnd);
	    }
	
	    public MySolver(TPathNode[,] inGrid)
	        : base(inGrid)
	    {
	    }
	}
	
	
}