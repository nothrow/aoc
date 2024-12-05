<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"))
				.Split("\r\n\r\n");
				
var rules = input[0].Trim().Split(Environment.NewLine).Select(x => x.Split('|').Select(int.Parse).ToArray()).GroupBy(x => x[0])
	.ToDictionary(x => x.Key, x => x.Select(y => y[1]).ToArray());

var updates = input[1].Trim().Split(Environment.NewLine).Select(x => x.Split(",").Select(int.Parse).ToArray());

T Head<T>(IEnumerable<T> e) => e.First();
IEnumerable<T> Tail<T>(IEnumerable<T> e) => e.Skip(1);

IEnumerable<(T, T)> Pairs<T>(IEnumerable<T> e) => Tail(e).Select(x => (Head(e), x));
bool ValidPair((int l, int r) p) => rules.GetValueOrDefault(p.l, [p.r]).Contains(p.r);

bool ValidUpdate(IEnumerable<int> i) => !i.Any() || (Pairs(i).All(ValidPair) && ValidUpdate(Tail(i)));
T Middle<T>(IEnumerable<T> e) => e.ElementAt(e.Count() / 2);

// part1
updates.Where(ValidUpdate).Select(Middle).Sum().Dump();

// part2

// gets, how many items from input are AFTER i
int ComputeRank(IEnumerable<int> input, int i) => rules[i].Union(input).Count();
IEnumerable<int> Reorder(IEnumerable<int> input) => input.OrderBy(x => ComputeRank(input, x));

updates.Where(x => !ValidUpdate(x)).Select(Reorder).Select(Middle).Sum().Dump();