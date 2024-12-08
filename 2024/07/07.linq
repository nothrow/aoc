<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Select(x => x.Split(':', 2)).Select(r => (long.Parse(r[0]), r[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray()));

Func<long, long, long>[] ops = new [] {
	(long x, long y) => x + y,
	(long x, long y) => x * y,
	(long x, long y) => long.Parse($"{x}{y}")
};

IEnumerable<Func<long, long, long>[]> OperationCombinations(int items) => Enumerable.Range(0, items)
	.Aggregate(
		new[] { Enumerable.Empty<Func<long, long, long>>() }.AsEnumerable(),
		(current, _) => current.SelectMany(
			combination => ops.Select(
				item => combination.Append(item)
			)
		)
	).Select(x => x.ToArray());

long ApplyOperation(Func<long, long, long>[] operation, long[] operations) => operations.Skip(1).Index().Aggregate((operations[0], 0), (acc, item) => (operation[item.Index](acc.Item1, item.Item), 0)).Item1;

bool IsValid((long result, long[] operations) dat) => OperationCombinations(dat.operations.Length).Any(x => ApplyOperation(x, dat.operations) == dat.result);

// part 1
input.Where(IsValid).Sum(x => x.Item1).Dump();

// part 2 - same as part1, just with additional op
// input.Where(IsValid).Sum(x => x.Item1).Dump();
