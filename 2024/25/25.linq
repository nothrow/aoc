<Query Kind="Statements">
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

var input = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Split("\r\n\r\n");


K ParseKey(string s) {
	var lines = s.Split("\n").Select(x => x.Trim()).ToArray();
	var isKey = !lines[0].All(w => w == '#');
	var tooth = Enumerable.Range(0, 5).Select(x => lines.Select(l => l[x]).Count(x => x =='#') - 1).ToArray();

	return new K(isKey, tooth);
}

bool Overlaps((K, K) a) => a.Item1.Tooth.Zip(a.Item2.Tooth).Select(x => x.First + x.Second).All(x => x <= 5);

var keys = input.Select(ParseKey).Where(x => x.IsKey).ToArray();
var locks = input.Select(ParseKey).Where(x => !x.IsKey).ToArray();

keys.SelectMany(k => locks.Select(l => (l, k))).Count(Overlaps).Dump();


record K(bool IsKey, int[] Tooth);