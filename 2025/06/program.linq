<Query Kind="Program" />


void Calc(ref long a, long b, char op)
{
	switch (op)
	{
		case '+':
			a += b;
			break;
		case '*':
			a *= b;
			break;
		default: throw new Exception();
	}
}

long[] AggregateRow(long[] a, long[] b, char[] operations) {
	for(int i = 0; i < operations.Length; i++){
		Calc(ref a[i], b[i], operations[i]);
	}
	return a;
}

long SeedFor(char x) => x == '+' ? 0L : x == '*' ? 1L : throw new Exception();

long[] MakeSeed(char[] operations) => operations.Select(SeedFor).ToArray();

void Main()
{
	var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"));
	var operations = input.Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x[0]).ToArray();
	
	var data = input.Take(input.Length - 1).Select(y => y.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToArray());
	
	// part1
	data.Aggregate(MakeSeed(operations), (r, y) => AggregateRow(r, y, operations)).Sum().Dump();
	
	// part2
	var zu = new Regex("[+*][ ]+");
	var columnWidths = zu.Matches(input.Last()).Select(m => m.Length).ToArray();
	
	var flop = 0;
	
	var sum = 0L;
	
	for(int i = 0; i < columnWidths.Length; i++)
	{
		var seed = SeedFor(operations[i]);
		for (int j = 0; j < columnWidths[i]; j++)
		{
			var ss = new string(input.Take(input.Length - 1).Select(z => z[flop]).ToArray()).Trim();
			if (ss != "")
			{
				var l = long.Parse(ss);
				Calc(ref seed, l, operations[i]);
			}
			flop++;
			
		}
		sum += seed;
	}
	
	sum.Dump();
}
