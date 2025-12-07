<Query Kind="Program" />

void Main()
{
	var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"));
	var ranges = input.TakeWhile(x => !string.IsNullOrEmpty(x))
		.Select(line => {
			var u = line.Split('-');
			return new Range(long.Parse(u[0]), long.Parse(u[1]));
		}).OrderBy(x => x.Min).ThenBy(x => x.Max).ToArray();
		
	var rs = BuildRanges(ranges);
		
	// step1
	input.Skip(ranges.Length + 1).Select(x => long.Parse(x)).Count(x => {
		foreach(var r in rs) {
			if (r.In(x)) return true;
		}
		return false;
	}).Dump();
	
	// step2
	rs.Select(r => r.Size).Sum().Dump();	
}

Range[] BuildRanges(IEnumerable<Range> input) {
	var s = new List<Range>();
	
	Range last = null;
	
	foreach(var line in input) {
		if (last == null)
			last = line;
		else {
			if (Overlaps(last, line)) {
				last = Merge(last, line);
			} else {
				s.Add(last);
				last = line;
			}
		}
	}
	
	if (last != null)
		s.Add(last);
	
	return s.ToArray();
}

class RangeCmp : IComparer<Range>
{
	public int Compare(Range x, Range y)
	{
		return x.Min.CompareTo(y.Min);
	}
}

bool Overlaps(Range a, Range b) {
	return a.In(b.Min) || a.In(b.Max) || b.In(a.Min) || b.In(a.Max);
}

Range Merge(Range a, Range b) {
	return new Range(Math.Min(a.Min, b.Min), Math.Max(a.Max, b.Max));
}

record Range(long Min, long Max) {
	public bool In(long i) => i >= Min && i <= Max;
	public long Size => Max - Min + 1;
}

/*
Tree BuildTree(IEnumerable<(long, long)> ranges) {
	var t = new Tree();
	foreach(var r in ranges) {
		if (t.Root == null) {
			t.Root = FromRange(r);
		} else {
			
			
			
		}
	}
	return t;
}

static bool Overlaps(TreeNode l, TreeNode r)
{
	return (r.InRange(l.Min) || r.InRange(l.Max)) ||
	       (l.InRange(r.Min) || l.InRange(r.Max));
}

static TreeNode FromRange((long, long) range)
{
	return new TreeNode
	{
		Min = range.Item1,
		Max = range.Item2
	};
}

class TreeNode {
	public TreeNode L;
	public TreeNode R;
	
	public bool InRange(long i) {
		return i >= Min && i <= Max;
	}
	public bool IsLeaf => L == null || R == null; 
	
	public long Min;
	public long Max;
}


class Tree {
	public TreeNode Root;
}*/