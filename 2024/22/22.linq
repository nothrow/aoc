<Query Kind="Statements">
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Select(long.Parse).ToArray();

//input = [1, 2, 3, 2024];

long Produce(long l) {
	l = ((l * 64) ^ l) % 16777216;
	l = ((l / 32) ^ l) % 16777216;
	l = ((l * 2048) ^ l) % 16777216;
	return l;
}

IEnumerable<T> Generate<T>(T initial, Func<T, T> gen) {
	yield return initial;
	while(true) {
		yield return (initial = gen(initial));
	}
}


// part1
input.Select(x => Generate(x, Produce).Skip(2000).First()).Sum(	).Dump();

//.Sum().Dump();
// part 2
var sequences = input.Select(x =>
	Generate(x, Produce).Skip(1).Zip(Generate(x, Produce)).Select(x => ((int)(x.First % 10 - x.Second % 10), (int)x.First % 10)).Take(2000).ToArray()
	).ToArray();
	
var r = Enumerable.Range(-9, 19);
var combinations = r // all possible combinations
 .SelectMany(s => r.Select(i => (i, s)))
 .SelectMany(s => r.Select(i => (i, s.i, s.s)))
 .SelectMany(s => r.Select(i => (i, s.Item1, s.Item2, s.s)));
 
Dictionary<(int, int, int, int), int> BuildDict((int, int)[] buyer) {
	var dict = new Dictionary<(int, int, int, int), int>();
	for (int i = 0; i < buyer.Length - 4; i++)
	{
		var k = (buyer[i].Item1, buyer[i+1].Item1, buyer[i+2].Item1, buyer[i+3].Item1);
		if (!dict.ContainsKey(k))
			dict.Add(k, buyer[i+3].Item2);
	}
	return dict;
}

var dicts = sequences.Select(BuildDict).ToArray();
 
bool IsValid(params int[] c) {
	var s = c.Sum();
	return s > -10 && s < 10;
}
bool IsValidCombo((int a, int b, int c, int d) x) {
	var (a,b,c,d) = x;
	var s = a+b+c+d;
	return IsValid(a, b) && IsValid(b, c) && IsValid(c, d) && IsValid(a, b, c) && IsValid(b, c, d) && IsValid(a, b, c, d);
}
 
var validCombinations = combinations.Where(IsValidCombo);

validCombinations.Select(c => (c, dicts.Select(s => s.GetValueOrDefault(c, 0)).Sum())).Where(x => x.Item2 > 2000).Dump();
//.Max().Dump();

