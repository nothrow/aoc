<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = 
	File.ReadAllText(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Split("\r\n\r\n");
	
var maze = input[0].Split("\r\n").Select(i => (i.Trim()).ToCharArray()).ToArray();
var steps = string.Join("", input[1].Split("\r\n"));

int Width() => maze[0].Length;
int Height() => maze.Length;

int M = Math.Max(Width(), Height());

bool Inside(P p) => p.Y >= 0 && p.Y < Height() && p.X >= 0 && p.X <= Width();
IEnumerable<P> AllPoints() => Enumerable.Range(0, Height()).SelectMany(_ => Enumerable.Range(0, Width()), (x, y) => new P(x, y));

var directions = new Dictionary<char, P>
{
	['<'] = new P(-1, 0),
	['>'] = new P(1, 0),
	['^'] = new P(0, -1),
	['v'] = new P(0, 1),
};


char Get(P p) => maze[p.Y][p.X];

void Set(P p, char c)
{
	maze[p.Y][p.X] = c;
}

void Swap(P a, P b)
{
	var c = Get(a);
	Set(a, Get(b));
	Set(b, c);
}

long Gps(P p) => p.Y * 100 + p.X;

void DumpMaze() {
	Util.WithStyle(string.Join("\n", maze.Select(x => new string(x))), "font-family: consolas").Dump();
}

void StepBy(P how) {
	var pos = new P(maze.Index().Select(x => (Array.IndexOf(x.Item, '@'), x.Index)).Single(x => x.Item1 >= 0));
	
	var freeSpace = Enumerable.Range(1, M).Select(i => (pos + how * i, i)).TakeWhile(p => Inside(p.Item1) && (Get(p.Item1) == 'O' || Get(p.Item1) == '.')).FirstOrDefault(p => Get(p.Item1) == '.');
	if (freeSpace != default) {
		for(int i = freeSpace.i - 1; i >= 0; i--) {
			Swap(pos + i*how, pos + (i+1)*how);
		}
	}
}

foreach(var d in steps)
{
	StepBy(directions[d]);
//	DumpMaze();
}


// step1
AllPoints().Where(p => Get(p) == 'O').Select(Gps).Sum().Dump();


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
}
