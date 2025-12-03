<Query Kind="Program" />

(int, char) LargestWithIndex(ReadOnlySpan<char> d)
{
	var ret = (0, '\0');
	for(int i = 0; i < d.Length; i++) {
		if (d[i] > ret.Item2) {
			ret = (i, d[i]);
		}
	}
	return ret;
}

ReadOnlySpan<char> SliceMax(ReadOnlySpan<char> b, int maxlen) 
{
	return b.Length > maxlen ? b[0..maxlen] : b;
}

long Joltage(string bank, int batteries)
{
	Span<char> dat = stackalloc char[batteries];
	ReadOnlySpan<char> buf = bank.AsSpan();
	
	var first = 0;
	var last = bank.Length - batteries + 1;
	
	for (int b = 0; b < batteries; b++)
	{
		(var f, dat[b]) = LargestWithIndex(buf[first..(last+b)]);
		first += f + 1;
	}	
	
	return long.Parse(dat);
}

void Main()
{
	var input = File.ReadLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"));
	input.Select(l => Joltage(l, 2)).Sum().Dump();
	input.Select(l => Joltage(l, 12)).Sum().Dump();
}

// You can define other methods, fields, classes and namespaces here
