<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"));

var program = input[4]
	.Split(':')[1].Trim().Split(',').Select(int.Parse).ToArray();

var expected = string.Join(",", program.Select(x => x.ToString()));

IEnumerable<ulong> Process(ulong rega) {
	ulong[] combo = [0, 1, 2, 3, rega, 0, 0];
	var ip = 0;


	while (ip >= 0 && ip < program.Length)
	{
		var instruction = program[ip];
		var op = program[ip + 1];

		switch (instruction)
		{
			case 0: combo[4] = combo[4] >> (int)combo[op]; break;
			case 1: combo[5] ^= (ulong)op; break;
			case 2: combo[5] = combo[op] % 8; break;
			case 3: ip = combo[4] == 0 ? ip : op - 2; break;
			case 4: combo[5] ^= combo[6]; break;
			case 5: yield return (combo[op] % 8); break;
			case 6: combo[5] = combo[4] >> (int)combo[op]; break;
			case 7: combo[6] = combo[4] >> (int)combo[op]; break;
			default: throw new InvalidOperationException();
		}

		ip += 2;
	}
}

ulong? Solve(int lf, ulong r)
{
	if (lf < 0) return r;
	for(int d = 0; d < 8; d++) {
		var a = ((ulong)(r << 3)) | ((ulong)d);
		
		var res = Process(a).First();

		if ((int)res == program[lf])
		{
			var ret = Solve(lf - 1, a);
			if (ret.HasValue) return ret;
		}
	}
	return null;
}

var i = Solve(program.Length - 1, 0);
i.Dump();
string.Join(",", Process(i.Value)).Dump();
expected.Dump();




