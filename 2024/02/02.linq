<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadLinesAsync(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"))
	.Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToArray());


// part1
await input.CountAsync(CorrectRow).Dump();

// part2
await input.CountAsync(rows =>
{
	return Reduced(rows).Any(CorrectRow);
}).Dump();


static bool CorrectRow(IEnumerable<int> row)
{
	var zipped = row.Zip(row.Skip(1), (f, s) => f - s);
	if (zipped.Select(x => Math.Abs(x)).Any(x => x < 1 || x > 3)) return false;
	if (zipped.Select(x => Math.Sign(x)).Distinct().Count() > 1) return false;
	return true;
}

static IEnumerable<IEnumerable<int>> Reduced(int[] arr)
{
	for (int i = 0; i < arr.Length; i++)
	{
		yield return arr.Take(i).Concat(arr.Skip(i + 1));
	}
}
