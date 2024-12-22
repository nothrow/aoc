<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Split("\r\n\r\n");

static P FromRex(Match m) {
	return new P(int.Parse(m.Groups[1].ValueSpan), int.Parse(m.Groups[2].ValueSpan));
}

Machine ParseMachine(string s)
{
	var rex1 = new Regex(@"Button A: X\+(\d+), Y\+(\d+)");
	var rex2 = new Regex(@"Button B: X\+(\d+), Y\+(\d+)");
	var rex3 = new Regex(@"X=(\d+), Y=(\d+)");
	
	return new Machine(FromRex(rex1.Match(s)), FromRex(rex2.Match(s)), FromRex(rex3.Match(s)));

}

long Process(Machine m) {
	var det = m.A.X * m.B.Y - m.A.Y * m.B.X;
	if (det == 0) return 0;
	
	
	var deta = m.Target.X * m.B.Y - m.Target.Y * m.B.X;
	var detb = m.A.X * m.Target.Y - m.A.Y * m.Target.X;

	var a = deta / det;
	var b = detb / det;
	
	if ((m.A * a + m.B * b) == m.Target) {
		return a*3 + b;
	}
	
	return 0;
}

Machine Part2(Machine m) => new Machine(m.A, m.B, m.Target + new P(10000000000000L, 10000000000000L));

// part1
input.Select(ParseMachine).Select(Process).Sum().Dump();

// part2
input.Select(ParseMachine).Select(Part2).Select(Process).Sum().Dump();


record P(long X, long Y) {
	public static P operator *(P p, long x) => new P(p.X * x, p.Y * x);
	public static P operator +(P a, P b) => new P(a.X + b.X, a.Y + b.Y);
	public static bool operator >(P a, P b) => a.X > b.X || a.Y > b.Y;
	public static bool operator <(P a, P b) => a.X < b.X || a.Y < b.Y;
}
record Machine(P A, P B, P Target);