<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
  <Namespace>System.Drawing</Namespace>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Select(ParseRobot);

var size = new P(101, 103);

static P FromRex(Match m) {
	return new P(int.Parse(m.Groups[1].ValueSpan), int.Parse(m.Groups[2].ValueSpan));
}

Robot ParseRobot(string s)
{
	var p = new Regex(@"p=(-?\d+),(-?\d+)");
	var v = new Regex(@"v=(-?\d+),(-?\d+)");
	 
	return new Robot(FromRex(p.Match(s)), FromRex(v.Match(s)));
}


Robot Steps(Robot r, int s) => new Robot((r.P + (r.V * s)) % size, r.V);

int Quadrant(Robot r, P s) =>
	(r.P.X < s.X / 2 && r.P.Y < s.Y / 2) ? 1 :
	(r.P.X > s.X / 2 && r.P.Y < s.Y / 2) ? 2 : 
	(r.P.X < s.X / 2 && r.P.Y > s.Y / 2) ? 3 :
	(r.P.X > s.X / 2 && r.P.Y > s.Y / 2) ? 4 : 0;

Bitmap Pic(IEnumerable<Robot> r) {
	var b = new Bitmap((int)size.X, (int)size.Y);
	foreach(var c in r) {
		b.SetPixel((int)c.P.X, (int)c.P.Y, Color.Black);
	}
	return b;	
}


// part1
input.Select(r => Steps(r, 100)).GroupBy(r => Quadrant(r, size)).Select(x => (x.Key, x.Count())).Where(x => x.Key > 0).Aggregate (1, (acc, p) => acc * (p.Item2)).Dump();

// part2

// 
// interesting points 2, 23, 105, 124, 208, 225
//int D = 104*2;
//for (int i = D; i < D + 104; i++)
//	new { P = Pic(input.Select(r => Steps(r, i))), i}.Dump();

// no idea why
for (int i = 23; i < 10000; i+=101)
	new { P = Pic(input.Select(r => Steps(r, i))), i }.Dump();


record P(long X, long Y) {
	static long Mod(long a, long b) => (((a % b) + b) % b);

	public static P operator *(P p, long x) => new P(p.X * x, p.Y * x);
	public static P operator +(P a, P b) => new P(a.X + b.X, a.Y + b.Y);
	public static P operator %(P a, P b) => new P(Mod(a.X, b.X), Mod(a.Y, b.Y));
}

record Robot(P P, P V);
