<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Index().Select(row => row.Item.ToCharArray().Select((antenna, i) => (coords: new C(i, row.Index), antenna))).SelectMany(row => row);

var width = input.Select(x => x.coords.x).Max() + 1;
var height = input.Select(x => x.coords.y).Max() + 1;

IEnumerable<(T, T)> Pairs<T>(IEnumerable<T> e) => e.SelectMany(f => e.Where(s => !EqualityComparer<T>.Default.Equals(s, f)), (f, s) => (f, s));

var antennas = input.Where(x => x.antenna != '.').GroupBy(x => x.antenna).Select(x => (antenna: x.Key, coords: x.Select(y => y.coords).ToArray()));

C Antinode((C a, C b) coords) => (coords.a - (coords.b - coords.a));

IEnumerable<C> Antinodes((C a, C b) coords) => Enumerable.Range(0, int.MaxValue).Select(p => (coords.a - p * (coords.b - coords.a))).TakeWhile(Inside);

bool Inside(C node) => node.x >= 0 && node.x < width && node.y >= 0 && node.y < height;

// step1
antennas.Select(a => a.coords).SelectMany(c => Pairs(c).Select(Antinode).Where(Inside)).Distinct().Count().Dump();

// step2
antennas.Select(a => a.coords).SelectMany(c => Pairs(c).SelectMany(Antinodes)).Distinct().Count().Dump();

record C(int x, int y) {
	public static C operator+(C a, C b) => new C(a.x + b.x, a.y + b.y);
	public static C operator-(C a, C b) => new C(a.x - b.x, a.y - b.y);
	public static C operator*(C a, int i) => new C(a.x * i, a.y * i);
	public static C operator*(int i, C a) => new C(a.x * i, a.y * i);
}


