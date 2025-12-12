<Query Kind="Program" />


record struct P(int X, int Y);
record struct VLine(int X, int Y1, int Y2) {
	public static VLine Create(int x, int y1, int y2) => new VLine(x, Math.Min(y1, y2), Math.Max(y1, y2));
	
	public bool Intersects(in Rect r) {
		if (X <= r.TopLeft.X || X >= r.BottomRight.X) return false;
		if (Y2 <= r.TopLeft.Y) return false;
		if (Y1 >= r.BottomRight.Y) return false;
		return true;
	}
}

record struct HLine(int X1, int Y, int X2) {
	public static HLine Create(int x1, int y, int x2) => new HLine(Math.Min(x1, x2), y, Math.Max(x1, x2));

	public bool Intersects(in Rect r)
	{
		if (Y <= r.TopLeft.Y || Y >= r.BottomRight.Y) return false;
		if (X2 <= r.TopLeft.X) return false;
		if (X1 >= r.BottomRight.X) return false;
		return true;
	}
}
record struct Rect(P TopLeft, P BottomRight);

long Area(P a, P b)
{
	long dx = (Math.Abs(a.X - b.X) + 1);
	long dy = (Math.Abs(a.Y - b.Y) + 1);

	return Math.Abs(dx*dy);
}

long LargestRectangle(P[] points)
{
	long size = 0;
	
	// step1
	for (int i = 0; i < points.Length; i++)
	{
		for (int j = i + 1; j < points.Length; j++)
		{
			long nd = Area(points[i], points[j]);
			if (nd > size)
			{
				size = nd;
			}
		}
	}
	
	return size;
}

Rect MakeRect(P[] points, int i, int j)
{
	var px = points[i];
	var py = points[j];
	var tl = new P(Math.Min(px.X, py.X), Math.Min(px.Y, py.Y));
	var br = new P(Math.Max(px.X, py.X), Math.Max(px.Y, py.Y));

	return new Rect(tl, br);
}

long LargestRectangle2(P[] points, VLine[] vlines, HLine[] hlines)
{
	long size = 0;

	// step1
	for (int i = 0; i < points.Length; i++)
	{
		for (int j = i + 1; j < points.Length; j++)
		{
			var r = MakeRect(points, i, j);

			if (vlines.Any(vl => vl.Intersects(r))) continue;
			if (hlines.Any(hl => hl.Intersects(r))) continue;

			long nd = Area(points[i], points[j]);
			
			if (nd > size)
			{
				size = nd;
			}
		}
	}

	return size;
}

void Main()
{
	var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt"));
	var points = input.Select(x => x.Split(",")).Select(x => new P(int.Parse(x[0]), int.Parse(x[1]))).ToArray();
	
	// step1
	LargestRectangle(points).Dump();

	var vlines = points.Zip(points.Skip(1)).Where(x => x.First.X == x.Second.X).Select(x => VLine.Create(x.First.X, x.First.Y, x.Second.Y)).OrderBy(x => x.X).ToArray();
	var hlines = points.Zip(points.Skip(1)).Where(x => x.First.Y == x.Second.Y).Select(x => HLine.Create(x.First.X, x.First.Y, x.Second.X)).OrderBy(x => x.Y).ToArray();
		
	LargestRectangle2(points,
		vlines,
		hlines
	).Dump();
}
