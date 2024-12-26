<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

var input = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Split("\r\n\r\n");

var wires = input[0].Split("\n").Select(x => x.Split(':')).Select(x => (x[0].Trim(), int.Parse(x[1])));

var dat = input[1].Split("\n").Select(c => c.Split(' ')).Select(c => (c[0].Trim(), c[1].Trim(), c[2].Trim(), c[4].Trim()));

var wrs = dat.SelectMany(d => new[] { d.Item1, d.Item3, d.Item4 }).Distinct().ToDictionary(x => x, _ => new TaskCompletionSource<int>());

int Op(string op, int a, int b) => op switch {
		"XOR" => a ^ b,
		"OR" => a | b,
		"AND" => a & b,
		_ => throw new InvalidOperationException()
	};

foreach (var w in wires)
{
	Task.Run(() => wrs[w.Item1].SetResult(w.Item2));
}

foreach(var row in dat) {
	var (wire1, operation, wire2, result) = row;
	
	Task.Run(async () => {

		var a = await wrs[wire1].Task;
		var b = await wrs[wire2].Task;
		var c = Op(operation, a, b);
		
		wrs[result].SetResult(c);
	});
}

await Task.WhenAll(wrs.Select(w => w.Value.Task));

ulong l = 0;
foreach(var row in wrs.Where(x => x.Key.StartsWith("z")).OrderByDescending(x => x.Key)) {
	l = (l << 1) | (ulong)(await row.Value.Task);
}

l.Dump();