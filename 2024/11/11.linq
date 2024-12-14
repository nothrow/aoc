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

Enumerable.Range(0, 75).Aggregate(input.AsEnumerable(), (i, _) => Blink(i)).Count().Dump();

// part 2
/*
var d = new Dictionary<string, int>
{
	["0"] = 1
};

V GetOrAdd<K, V>(Dictionary<K, V> d, K k, Func<K, V> gen) => d.ContainsKey(k) ? d[k] : d[k] = gen(k);

long ComputePower(string s) => GetOrAdd(d, s, s => 
	s == "0"
	
;

input.Select(ComputePower).Sum();
*/