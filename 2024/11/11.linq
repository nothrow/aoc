<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Split(' ');

string ZeroIfEmpty(string s) => s.Length == 0 ? "0" : s;
string TrimZeroes(string s) => ZeroIfEmpty(s.TrimStart('0'));


IEnumerable<string> Spliti(string i) => 
	[ i.Substring(0, i.Length / 2), TrimZeroes(i.Substring(i.Length / 2)) ];
	

IEnumerable<string> Engrave(string i) =>
	i == "0" ? ["1"] :
	i.Length % 2 == 0 ? Spliti(i) :
	[(long.Parse(i) * 2024).ToString()];
	
IEnumerable<string> Blink(IEnumerable<string> input) => input.SelectMany(Engrave);
// part 1
Enumerable.Range(0, 25).Aggregate(input.AsEnumerable(), (i, _) => Blink(i)).Count().Dump();

// part 2
var cache = new Dictionary<(string, int Depth), long>();
long DFS(string s, int maxdepth, int actualdepth) {
	if (cache.ContainsKey((s, actualdepth)))
		return cache[(s, actualdepth)];
		
	if (actualdepth == maxdepth) {
		return 1;
	}
	
	var ret = Engrave(s).Select(x => DFS(x, maxdepth, actualdepth + 1)).Sum();
	cache[(s, actualdepth)] = ret;
	return ret;
}

input.Select(i => DFS(i, 75, 0)).Sum().Dump();
