<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Select(row => row.ToCharArray()).ToArray();

var directions = Enumerable.Range(-1, 3).SelectMany(_ => Enumerable.Range(-1, 3), ToTuple).Where(x => x != (0, 0)).ToArray();
var directionsX = new[] { (-1, -1), (1, -1) };

int Width() => input[0].Length;
int Height() => input.Length;

(T, T) ToTuple<T>(T x, T y) => (x, y);
// true when point is in input
bool Inside((int x, int y) point) => (point.x >= 0 && point.y >= 0 && point.y < input.Length && point.x < input[point.y].Length);
// tuple * int
static (int, int) Multiply((int, int) a, int b) => (a.Item1 * b, a.Item2 * b);
// tuple + tuple
static (int, int) Add((int, int) a, (int, int) b) => (a.Item1 + b.Item1, a.Item2 + b.Item2);
// coords of points in given direction
IEnumerable<(int, int)> Ray(((int, int) point, (int, int) direction) x) => Enumerable.Range(0, int.MaxValue).Select(c => Add(x.point, Multiply(x.direction, c))).TakeWhile(Inside);
// value at given position
char Get((int x, int y) pos) => input[pos.y][pos.x];
// all points in given input
IEnumerable<(int, int)> AllPoints() => Enumerable.Range(0, Height()).SelectMany(_ => Enumerable.Range(0, Width()), ToTuple);
// coords of points in given direction, starting with -1
IEnumerable<(int, int)> XRay(((int, int) point, (int, int) direction) x) => Enumerable.Range(-1, int.MaxValue).Select(c => Add(x.point, Multiply(x.direction, c))).TakeWhile(Inside);
//
bool IsMas(string s) => s == "MAS" || s == "SAM";
// part 1
AllPoints().SelectMany(_ => directions, ToTuple).Select(x => string.Concat(Ray(x).Take(4).Select(Get))).Count(x => x == "XMAS").Dump();

// part 2
AllPoints().Select(point => directionsX.Select(dir => string.Concat(XRay((point, dir)).Take(3).Select(Get)) )).Count(x => x.All(IsMas)).Dump();
