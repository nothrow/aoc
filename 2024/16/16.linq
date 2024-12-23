<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "test.txt")).Split("\r\n").Select(i => (i.Trim()).ToCharArray()).ToArray();

int Width() => input[0].Length;
int Height() => input.Length;

var directions = new[] {
	new P(1, 0),
	new P(0, -1),
	new P(-1, 0),
	new P(0, 1)
};

int Left(int dir) => (dir + 3) % 4;
int Right(int dir) => (dir + 1) % 4;


bool Inside(P p) => p.Y >= 0 && p.Y < Height() && p.X >= 0 && p.X <= Width();
IEnumerable<P> AllPoints() => Enumerable.Range(0, Height()).SelectMany(_ => Enumerable.Range(0, Width()), (x, y) => new P(x, y));

char Get(P p) => input[p.Y][p.X];
P Find(char c) => new P(input.Index().Select(x => (Array.IndexOf(x.Item, c), x.Index)).Single(x => x.Item1 >= 0));

var start = Find('S');
var end = Find('E');

long ManhattanE(P p) => Math.Abs(end.X - p.X) + Math.Abs(end.Y - p.Y);

var visited = new HashSet<P>();
var queue = new PriorityQueue<(P, int, long, (P, int)[]), long>();

queue.Enqueue((start, 0, 0, []), 0);
queue.Enqueue((start, 1, 1000, []), 1000);
queue.Enqueue((start, 3, 1000, []), 1000);

start.Dump();

T[] Add<T>(T[] t, T i) => t.Concat([i]).ToArray();

var pt = new HashSet<P>();

while(queue.Count > 0) {
	
	var ss = queue.Dequeue();
	
	if (ss.Item1 == end)
	{
		// part 1
		ss.Dump();
		foreach (var r in ss.Item4)
		{
			pt.Add(r.Item1);
		}
	}
	

	var wm = Add(ss.Item4, (ss.Item1, ss.Item2));
	
	var next = ss.Item1 + directions[ss.Item2];
	if (Get(next) == '#') continue;
	//if (!visited.Add(next)) continue;
	
	var mh = ManhattanE(next) * 100;
	
	queue.Enqueue((next, ss.Item2, ss.Item3 + 1, wm), ss.Item3 + 1 + mh);
	queue.Enqueue((next, Left(ss.Item2), ss.Item3 + 1001, wm), ss.Item3 + 1001 + mh);
	queue.Enqueue((next, Right(ss.Item2), ss.Item3 + 1001, wm), ss.Item3 + 1001 + mh);
}

pt.Count.Dump();


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
