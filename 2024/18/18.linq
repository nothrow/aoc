<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Select(x => x.Split(',').Select(int.Parse).ToArray()).Select(x => new P(x[0], x[1]));

int Width() => 71;
int Height() => 71;

var directions = new[] {
	new P(1, 0),
	new P(0, -1),
	new P(-1, 0),
	new P(0, 1)
};

var maze = new char[Height()][];
for(int i = 0; i < Height(); i++) {
	maze[i] = Enumerable.Repeat('.', Width()).ToArray();
}
IEnumerable<P> Nexts(P p) => directions.Select(d => d + p).Where(Inside);


bool Inside(P point) => (point.X >= 0 && point.Y >= 0 && point.Y < Height() && point.X < Width());

char Get(P p) => maze[p.Y][p.X];
P Set(P p, char c) { maze[p.Y][p.X] = c; return p; }

var start = new P(0, 0);
var end = new P(Width() - 1, Height() - 1);

long ManhattanE(P p) => Math.Abs(end.X - p.X) + Math.Abs(end.Y - p.Y);

bool Works()
{
	var visited = new HashSet<P>();
	var queue = new PriorityQueue<(P, long), long>();

	queue.Enqueue((start, 0), 0);
	
	while (queue.Count > 0)
	{

		var ss = queue.Dequeue();

		if (ss.Item1 == end)
		{
			return true;
		}

		foreach (var next in Nexts(ss.Item1))
		{
			if (Get(next) == '#') continue;
			if (!visited.Add(next)) continue;

			var mh = ManhattanE(next);
			queue.Enqueue((next, ss.Item2 + 1), ss.Item2 + mh);
		}
	}
	return false;
}

foreach(var c in input) {
	Set(c, '#');
	if (!Works())
	{
		c.Dump();
		break;
	}
}


record P(long X, long Y)
{
	public P((int, int) value)
		: this(value.Item1, value.Item2)
	{
	}

	static long Mod(long a, long b) => (((a % b) + b) % b);

	public static P operator *(P p, long x) => new P(p.X * x, p.Y * x);
	public static P operator *(int x, P p) => new P(p.X * x, p.Y * x);
	public static P operator +(P a, P b) => new P(a.X + b.X, a.Y + b.Y);
	public static P operator %(P a, P b) => new P(Mod(a.X, b.X), Mod(a.Y, b.Y));
	
	public static bool operator != (P a, (int, int) b) => a.X != b.Item1 || a.Y != b.Item2;
	public static bool operator == (P a, (int, int) b) => a.X == b.Item1 && a.Y == b.Item2;
}
