<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"));

// input.Select(x => (int)(x - '0')).Sum().Dump();
var disk = input.Select((x, i) => new C(size: (int)(x - '0'), isfile: (i % 2 == 0), index: (i % 2 != 0) ? default(int?) : (i / 2))).ToList();

static C Generator<C>(C item, Func<C, C> mapping, Func<C, bool> valid) => !valid(item) ? item : Generator(mapping(item), mapping, valid);

static int IndexOf<T>(IEnumerable<T> dat, Func<T, bool> isf) => dat.Index().First(x => isf(x.Item2)).Item1;
static int IndexOfLast<T>(IEnumerable<T> dat, Func<T, bool> isf) => dat.Index().Last(x => isf(x.Item2)).Item1;

static bool IsFree(C c) => !c.isfile;
static bool IsNotFree(C c) => c.isfile;

static IEnumerable<C> Replaces(IEnumerable<C> input, int iof, int iol) => new[] {
	new C(Math.Min(input.ElementAt(iof).size, input.ElementAt(iol).size), true, input.ElementAt(iol).index),
	new C(input.ElementAt(iof).size - Math.Min(input.ElementAt(iof).size, input.ElementAt(iol).size), false, null)
};

static IEnumerable<C> ReplacesEnd(IEnumerable<C> input, int iof, int iol) => new[] {
	new C(input.ElementAt(iol).size - Math.Min(input.ElementAt(iof).size, input.ElementAt(iol).size), true, input.ElementAt(iol).index),
	new C(Math.Min(input.ElementAt(iof).size, input.ElementAt(iol).size), false, null),
};


static IEnumerable<C> T(IEnumerable<C> input)
{
	StringBuilder sb = new StringBuilder();
	foreach (var k in input)
	{
		for (int i = 0; i < k.size; i++)
		{
			sb.Append(k.index.HasValue ? k.index.Value.ToString() : ".");
		}
	}

	sb.ToString().Dump();
	return input;
}

static IEnumerable<C> Replace(IEnumerable<C> input, int iof, int iol) => 
	(input.Take(iof).Concat(Replaces(input, iof, iol)).Concat(input.Skip(iof+1).Take(iol-iof-1)).Concat(ReplacesEnd(input, iof, iol)).Concat(input.Skip(iol+1))).ToArray();

// static IEnumerable<C> Merge(IEnumerable<C> input) => input.Aggregate (Enumerable.Empty<C>(), (acc, cur) => acc.LastOrDefault()?.index == cur.index ? acc.SkipLast(1).Append(new C(acc.Last().size + cur.size, cur.isfile, cur.index)).ToArray() : acc.Append(cur).ToArray());

// allow fluent
static List<C> Add(List<C> l, C item) { l.Add(item); return l; }
static List<C> ReplaceLast(List<C> l, Func<C, C> item) { var k = item(l.Last()); l.RemoveAt(l.Count - 1); l.Add(k); return l; }

static IEnumerable<C> Merge(IEnumerable<C> input) => input.Aggregate (new List<C>(), (acc, cur) => acc.LastOrDefault()?.index == cur.index ? ReplaceLast(acc, last => new C(last.size + cur.size, cur.isfile, cur.index)) : Add(acc, cur));

static IEnumerable<C> Step(IEnumerable<C> input) => (Merge(Replace(input, IndexOf(input, IsFree), IndexOfLast(input, IsNotFree)).Where(x => x.size > 0)));

//var disk2 = Generator(disk.AsEnumerable(), Step, x => x.Count(z => !z.isfile) > 1);

// step1
// disk2.Where(d => d.index.HasValue).SelectMany(d => Enumerable.Repeat(d.index.Value, d.size)).Index().Select(x => (long)x.Index * (long)x.Item).Sum().Dump();


var lastFile = disk.Where(x => x.isfile).Max(x => x.index);

static void FillPosFromStart(IReadOnlyList<C> d) {
	int q = 0;
	for(int i = 0; i < d.Count; i++) {
		d[i].posFromStart = q;
		q+=d[i].size;
	}
}

while(lastFile >= 0) {
	FillPosFromStart(disk);
	var file = disk.Single(x => x.index == lastFile);
	
	var freeSpaceBigEnough = disk.FirstOrDefault(d => !d.isfile && d.size >= file.size && d.posFromStart < file.posFromStart);
	if (freeSpaceBigEnough != null) {
		disk = Merge(Replace(disk, disk.IndexOf(freeSpaceBigEnough), disk.IndexOf(file)).Where(x => x.size > 0)).ToList();
	}
	lastFile--;
}

FillPosFromStart(disk);
disk.Where(d => d.index.HasValue).SelectMany(d => (Enumerable.Repeat((long)d.index.Value, d.size).Select((x, i) => (x, d.posFromStart + i)))).Select(d => d.Item2 * d.x).Sum().Dump();

record C(int size, bool isfile, int? index) { public int posFromStart; }
