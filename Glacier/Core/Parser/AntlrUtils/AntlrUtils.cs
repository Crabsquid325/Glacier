using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Glacier.Core.Parser.Grammar;

namespace Glacier.Core.Parser.AntlrUtils;

public static class ParserUtils
{
    public static string GetDebugInfo(this IToken token)
    {
        return $"{GlacierLexer.DefaultVocabulary.GetSymbolicName(token.Type)} : '{token.Text.Replace("\t", @"\t").Replace("\n", @"\n")}'";
    }

    public static void PrintTokens(this GlacierLexer lexer)
    {
        List<IToken> tokens = [];
        IToken? token;
        do
        {   token = lexer.NextToken();
            tokens.Add(token);
        } while (token.Type != TokenConstants.Eof);

        Console.WriteLine(tokens.Select(t => t.GetDebugInfo()).Aggregate((a, b) => a + "\n" + b));
    }

    public static void PrintTokens(this string s)
    {
        var lexer = new GlacierLexer(new AntlrInputStream(s));
        lexer.PrintTokens();
    }

    public static IEnumerable<IParseTree> GetChildren(this IParseTree tree)
    {
        for (int i = 0; i < tree.ChildCount; ++i)
        {
            yield return tree.GetChild(i);
        }
    }

    public static string Prettify(this IParseTree tree)
    {
        return $"{tree.GetType().Name} : {tree.GetText().Replace("\t", @"\t").Replace("\n", @"\n")}";
    }
    public static void VisualizeProgram(this IParseTree ctx, int depth = 0)
    {
        foreach (var child in ctx.GetChildren())
        {
            Console.WriteLine(string.Concat(Enumerable.Repeat("--", depth)) + child.Prettify());
            VisualizeProgram(child, depth + 1);
        }
    }
}