<Query Kind="Statements">
  <NuGetReference>System.Linq.Async</NuGetReference>
</Query>

var input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt")).Select(row => row.ToCharArray()).ToArray();

