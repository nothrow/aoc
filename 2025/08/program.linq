<Query Kind="Program" />

class P {
	public int i;
	
	public int X, Y, Z;
	
	public P(int i, int X, int Y, int Z) {
		Junctions.Add(this);
		this.i = i;
		this.X = X;
		this.Y = Y;
		this.Z = Z;
	}
	public HashSet<P> Junctions {get;set;} = new HashSet<P>(ReferenceEqualityComparer.Instance);
}
record PD((P, P) Pair, double Dist);

long D(P a, P b) {
	long dx = a.X - b.X;
	long dy = a.Y - b.Y;
	long dz = a.Z - b.Z;
	
	return dx*dx+dy*dy+dz*dz;
}

HashSet<(int, int)> seen = new HashSet<(int, int)>();

IEnumerable<(int, int)> Pairs(P[] boxes)
{
	while(true) {
		(int, int) minimal = default;
		long mdist = long.MaxValue;
	
		for(int i = 0; i < boxes.Length; i++)
		{
			for (int j = i + 1; j < boxes.Length; j++) {
				var dist = D(boxes[i], boxes[j]);
				if (dist < mdist && !seen.Contains((i, j))) {
					mdist = dist;
					minimal = (i, j);
				}
			}
		}
		
		seen.Add(minimal);
		yield return minimal;
	}
}

void Main()
{
	var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"));
	var boxes = input.Select(x => x.Split(',')).Select((x, i) => new P(
		i,
		int.Parse(x[0]),
		int.Parse(x[1]),
		int.Parse(x[2])
	)).ToArray();
	
	var networks = new Dictionary<int, HashSet<int>>();
	for (int i = 0; i < boxes.Length; i++)
	{
		networks[i] = new HashSet<int> { i };
	}
		
	foreach(var (id1, id2) in Pairs(boxes).Take(0)) {
		var nd = new HashSet<int>(networks[id2].Count + networks[id1].Count);
	
		foreach(var c in networks[id2])
			nd.Add(c);

		foreach (var c in networks[id1])
			nd.Add(c);
			
		foreach(var r in nd) {
			networks[r] = nd;
		}
	}
	
	networks.Values.DistinctBy(v => (object)v).OrderByDescending(x => x.Count).		
	Take(3).Select(x => x.Count).Aggregate(1, (x, y) => x*y).Dump();
	
	foreach (var (id1, id2) in Pairs(boxes))
	{
		var nd = new HashSet<int>(networks[id2].Count + networks[id1].Count);

		foreach (var c in networks[id2])
			nd.Add(c);

		foreach (var c in networks[id1])
			nd.Add(c);

		foreach (var r in nd)
		{
			networks[r] = nd;
		}
		
		if (nd.Count == boxes.Length) {
			// done
			
			(boxes[id1].X * boxes[id2].X).Dump();
			break;
		}
	}
}
