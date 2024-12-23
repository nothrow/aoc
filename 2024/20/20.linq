<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Select(x => x.ToCharArray()).ToArray();

int Width() => input[0].Length;
int Height() => input.Length;

var directions = new[] {
	new P(1, 0),
	new P(0, -1),
	new P(-1, 0),
	new P(0, 1)
};

IEnumerable<P> Nexts(P p) => directions.Select(d => d + p).Where(Inside);
P Find(char c) => new P(input.Index().Select(x => (Array.IndexOf(x.Item, c), x.Index)).Single(x => x.Item1 >= 0));

bool Inside(P point) => (point.X >= 0 && point.Y >= 0 && point.Y < Height() && point.X < Width());
IEnumerable<P> AllPoints() => Enumerable.Range(0, Height()).SelectMany(_ => Enumerable.Range(0, Width()), (x, y) => new P(x, y));

char Get(P p) => input[p.Y][p.X];
char Set(P p, char c) { var ret = input[p.Y][p.X]; input[p.Y][p.X] = c; return ret; }

var start = Find('S');
var end = Find('E');

long ManhattanE(P p) => Math.Abs(end.X - p.X) + Math.Abs(end.Y - p.Y);

int Price(P p)
{
	var old = Set(p, '.');
	try
	{

		var visited = new HashSet<P>();
		var queue = new PriorityQueue<(P, int), long>();

		queue.Enqueue((start, 0), 0);

		while (queue.Count > 0)
		{

			var ss = queue.Dequeue();

			if (ss.Item1 == end)
			{
				return ss.Item2;
			}

			foreach (var next in Nexts(ss.Item1))
			{
				if (Get(next) == '#') continue;
				if (!visited.Add(next)) continue;

				var mh = ManhattanE(next);
				queue.Enqueue((next, ss.Item2 + 1), ss.Item2 + mh);
			}
		}

		throw new InvalidOperationException();
	}
	finally {
		Set(p, old);
	}
}

var initialPrice = Price(new P(0, 0));

initialPrice.Dump();

AllPoints().Where(x => Get(x) == '#').Count(p => (initialPrice - Price(p)) >= 100).Dump();


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
