namespace Glacier.Core.Parser.Visitor;

public class AntlrAstVisitorContext
{
    public Stack<HashSet<string>> Locals = [];

    public void PushBlock() 
        => Locals.Push([]);

    public void PopBlock() 
        => Locals.Pop();

    public void MergeCurrBlock(HashSet<string> with)
        => Locals.Peek().UnionWith(with);
    
    public bool ContainsVar(string name) 
        => Locals.Any(hashSet => hashSet.Contains(name));
}