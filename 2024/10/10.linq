<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Select(row => row.ToCharArray().Select(x => int.Parse($"{x}")).ToArray()).ToArray();

var directions = Enumerable.Range(-1, 3).SelectMany(_ => Enumerable.Range(-1, 3), ToTuple).Where(x => x != (0, 0)).Where(x => x.Item1 == 0 || x.Item2 == 0).ToArray();
var directionsX = new[] { (-1, -1), (1, -1) };

int Width() => input[0].Length;
int Height() => input.Length;

(T, T) ToTuple<T>(T x, T y) => (x, y);
// true when point is in input
bool Inside((int x, int y) point) => (point.x >= 0 && point.y >= 0 && point.y < input.Length && point.x < input[point.y].Length);
// tuple + tuple
static (int, int) Add((int, int) a, (int, int) b) => (a.Item1 + b.Item1, a.Item2 + b.Item2);
// coords of points in given direction
int Get((int x, int y) pos) => input[pos.y][pos.x];

// all points in given input
IEnumerable<(int, int)> AllPoints() => Enumerable.Range(0, Height()).SelectMany(_ => Enumerable.Range(0, Width()), ToTuple);

T[] Concat<T>(T[] t, T item) => t.Concat(new[] { item }).ToArray();

(int, int)[] ComputeTrailhead((int, int)[] visited, (int, int) current, int expected)
{
	return Get(current) != expected ? [] :
		expected == 9 ? [current] : directions.Select(x => Add(current, x)).Where(Inside).Where(p => !visited.Contains(p)).SelectMany(p => ComputeTrailhead(Concat(visited, current), p, expected + 1)).ToArray();
}

// part 1
AllPoints().Where(x => Get(x) == 0).Select(p => ComputeTrailhead([], p, 0).Distinct().Count()).Sum().Dump();

// part 2
AllPoints().Where(x => Get(x) == 0).Select(p => ComputeTrailhead([], p, 0).Count()).Sum().Dump();
