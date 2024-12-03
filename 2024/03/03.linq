<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = "do()" + (await File.ReadAllTextAsync(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"))) + "don't()";
var rex = new Regex("mul\\((\\d+),(\\d+)\\)");

var dos = new Regex("do\\(\\)");
var donts = new Regex("don't\\(\\)");
// part1
var data = rex.Matches(input).Select(r => (int.Parse(r.Groups[1].Value), int.Parse(r.Groups[2].Value), r.Index));
data.Select(d => d.Item1 * d.Item2).Sum().Dump();

// part2
// filters do and don'ts in form (start index, true/false)
var events = dos.Matches(input).Select(r => r.Index).Select(x => (x, Start: true)).Concat(donts.Matches(input).Select(r => r.Index).Select(x => (x, Start: false))).OrderBy(x => x.Item1);

// combines them, to have ranges (x-y, where first (x) is 'do')
var ranges = events.Zip(events.Skip(1), (s, e) => ((s.x..e.x, (s.Start, e.Start)))).Where(e => e.Item2.Item1).Select(x => x.Item1).ToArray();

bool IsInRange(Range r, int x) => x > r.Start.Value && x < r.End.Value;

data.Where(d => ranges.Any(r => IsInRange(r, d.Index))).Select(d => d.Item1 * d.Item2).Sum().Dump();


