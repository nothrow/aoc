<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadLinesAsync(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"))
	.Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToArray());
	
var list1 = input.Select(i => i[0]).OrderBy(i => i);
var list2 = input.Select(i => i[1]).OrderBy(i => i);

var lists = AsyncEnumerable.Zip(list1, list2);

// part1
await lists.Select(l => Math.Abs(l.First - l.Second)).SumAsync().Dump();

var occurences = (await list2.ToArrayAsync()).GroupBy(l => l).ToDictionary(x => x.Key, x => x.Count());

// part2
await list1.Select(l => l * occurences.GetValueOrDefault(l)).SumAsync().Dump();
