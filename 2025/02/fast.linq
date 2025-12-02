<Query Kind="Program" />

Dictionary<int, int[]> _repeats = new ();

bool InvalidS(ReadOnlySpan<char> l)
{
	foreach (var repeatLength in _repeats[l.Length]) {
		var pattern = l[0..repeatLength];
		bool invalid = true;
		
		for(int i = repeatLength; i < l.Length; i += repeatLength) {
			if (!pattern.SequenceEqual(l.Slice(i, repeatLength)))
			{
				invalid = false;
				break;
			}
		}
		if (invalid) return true;
	}
	return false;
}

bool Invalid(long l) {
	Span<char> las = stackalloc char[21];
	l.TryFormat(las, out var cw);
	return InvalidS(las[0..cw]);	
}

void Main()
{
	for(int i = 1; i < 20; i++) {
		_repeats[i] = Enumerable.Range(1, i / 2).Where(x => (i % x) == 0).ToArray();
	}

	var input = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"));
	var acc = 0L;
	var l = new List<long>();
	
	foreach(var c in new SpanSplitComma(input)) {
		var z = c.IndexOf('-');
		var start = long.Parse(c[..z]);
		var end = long.Parse(c[(z+1)..]);

		for (var p = start; p <= end; p++)
		{
			if (Invalid(p))
			{
				acc += p;
			}
		}
	}
	
	acc.Dump();
}

// iterate without allocations
private ref struct SpanSplitComma {
	private ReadOnlySpan<char> _r;
	public ReadOnlySpan<char> Current { get; private set; }
	
	public SpanSplitComma(ReadOnlySpan<char> data) {
		_r = data;
		Current = default;
	}
	
	public SpanSplitComma GetEnumerator() => this;
	
	public bool MoveNext()
	{
		if (_r.Length == 0) return false;
		
		int idx = _r.IndexOf(',');
		if (idx == -1) {
			Current = _r;
			_r = default;
		} else {
			Current = _r[..idx];
			_r = _r[(idx+1)..];
		}
		return true;
	}
}
