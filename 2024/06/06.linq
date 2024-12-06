<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).ToArray();
var directions = new[] { (0, -1), (1, 0), (0, 1), (-1, 0) };

IEnumerable<T> Generate<T>(T input, Func<T, T> generator) {
	while(true) { yield return input; input = generator(input); }
}

bool HasLoop<T>(IEnumerable<T> input) {
	var hs = new HashSet<T>();
	foreach(var item in input) {
		if (!hs.Add(item))
			return true;
	}
	return false;
}

// true when point is in input
bool Inside((int x, int y) point) => (point.x >= 0 && point.y >= 0 && point.y < input.Length && point.x < input[point.y].Length);
bool IInside((((int x, int y), int), (int, int)) point) => Inside(point.Item1.Item1);

char At((int x, int y) point, (int, int) obstacle) => Inside(point) ? (point == obstacle ? '#' : input[point.y][point.x]) : '.';

// tuple + tuple
static (int, int) Add((int, int) a, (int, int) b) => (a.Item1 + b.Item1, a.Item2 + b.Item2);

var initialPosition = input.Index().Where(x => x.Item.Contains('^')).Select(x => (x.Item.IndexOf('^'), x.Index)).Single();

((int x, int y) position, int rotation) Step(((int x, int y) position, int rotation) i) => (Add(i.position, directions[i.rotation]), i.rotation);

IEnumerable<(((int x, int y) position, int rotation) obj, (int, int))> Walkthrough((int, int) obstacle) => Generate((obj: (position: initialPosition, rotation: 0), obstacle), p => At(Step(p.obj).position, p.obstacle) == '#' ? ((p.Item1.position, (p.Item1.rotation+1)%4), p.obstacle) : (Step(p.obj), p.obstacle));

var wentThrough = Walkthrough((-1, -1)).TakeWhile(IInside).Select(x => x.obj.position).Distinct();


wentThrough.Count().Dump(); // step1

wentThrough.Skip(1).Select(x => Walkthrough(x).TakeWhile(IInside).Select(x => x.obj)).Count(HasLoop).Dump(); // step2
		   		




