<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "test.txt")).Select(x => x.Split('-')).ToArray();

var conns = new Dictionary<string, HashSet<string>>();
void AddCon(string a, string b) {
	if (!conns.TryGetValue(a, out var h))
		conns[a] = h = new HashSet<string>();
		
	h.Add(b);
}

foreach(var k in input) {
	AddCon(k[0], k[1]);
	AddCon(k[1], k[0]);
}

IEnumerable<(string, string, string)> Connections()
{
	foreach (var (p1, connected) in conns)
	{
		foreach (var p2 in connected)
		{
			foreach (var p3 in connected)
			{
				if (conns[p2].Contains(p3))
				{
					var t = new[] { p1, p2, p3 }.OrderBy(x => x).ToArray();
					yield return (t[0], t[1], t[2]);
				}
			}
		}
	}
}

var hs = new HashSet<(string, string, string)>(Connections());
hs.OrderBy(x => x.Item1).ThenBy(x => x.Item2).ThenBy(x => x.Item3).Dump();

bool HT(string s) => s.StartsWith('t');
bool HTA(params string[] s) => s.Any(HT);

hs.Count(x => HTA(x.Item1, x.Item2, x.Item3)).Dump();
