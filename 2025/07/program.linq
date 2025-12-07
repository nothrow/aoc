<Query Kind="Program" />


void Swap<T>(ref T a, ref T b)
{
	T c = a;
	a = b;
	b = c;
}
void Main()
{
	var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "test.txt"));
	var beam = input[0].IndexOf('S');
	
	var front = new long[256];
	var back = new long[256];
	int triggers = 0;
	front[beam]++;

	for (int row = 1; row < input.Length; row++)
	{
		Array.Clear(back);
		
		foreach (var it in front.Index().Where(x => x.Item > 0))
		{
			var (b, val) = it; 
			if (input[row][b] == '.')
			{
				back[b] += val;
			}
			else if (input[row][b] == '^') // HIT!
			{ 
				back[b-1] += val;
				back[b+1] += val;

				++triggers;
			}
		}

		Swap(ref back, ref front);
	}
	
	triggers.Dump();
	front.Sum().Dump();
}
