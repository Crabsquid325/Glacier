using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;
using Antlr4.Runtime.Tree;
using Glacier.Core.Interpreter.Instructions;
using Glacier.Core.Interpreter.Instructions.Concrete;
using Glacier.Core.Interpreter.Instructions.Concrete.Compare;
using Glacier.Core.Interpreter.Instructions.Concrete.Math;
using Glacier.Core.Interpreter.Instructions.Concrete.Return;
using Glacier.Core.Interpreter.Values;
using Glacier.Core.Parser.AntlrUtils;
using Glacier.Core.Parser.Grammar;
using ExecutionContext = Glacier.Core.Interpreter.ExecutionContext;

namespace Glacier.Core.Parser.Visitor;


public class AntlrAstVisitor : GlacierParserBaseVisitor<object?>
{
    private AntlrAstVisitorContext _visitorCtx = new();
    private static HashSet<string> FindAllIds(IParseTree ctx)
    {
        HashSet<string> res = [];

        void Walker(IParseTree ctx)
        {
            foreach (var child in ctx.GetChildren())
            {
                if (child is GlacierParser.IdContext idContext) 
                    res.Add(child.GetText());
                Walker(child);
            }
        }

        Walker(ctx);
        return res;
    }
    
    //TODO optimize, horrible performance
    private static HashSet<string> FindOrphans(IParseTree ctx, List<string> args)
    {
        HashSet<string> res  = [];
        HashSet<string> decl = [];
        void Walker(IParseTree ctx)
        {
            foreach (var child in ctx.GetChildren())
            {
                switch (child)
                {
                    case GlacierParser.AtomContext atomCtx when atomCtx.id() != null && !decl.Contains(atomCtx.id().GetText()):
                        res.Add(atomCtx.id().GetText());
                        break;
                    
                    case GlacierParser.AssignContext assignCtx:
                        Walker(assignCtx.expr());
                        decl.Add(assignCtx.id().GetText());
                        break;
                    
                    case (GlacierParser.FndeclContext fndeclCtx):
                        for (int i = 1; i < int.MaxValue; ++i)
                        {
                            var nxt = fndeclCtx.id(i);
                            if  (nxt == null)
                                break;
                            decl.Add(nxt.GetText());
                        }
                        decl.Add(fndeclCtx.id(0).GetText());
                        break;
                }
                Walker(child);
            }
        }

        Walker(ctx);
        res.ExceptWith(args);
        return res;
    }
    public override object? VisitProgram(GlacierParser.ProgramContext context)
    {
        var seq = new List<IGlacierInstruction>();
        var stmts = context.children;
        foreach (var stmt in stmts)
        {
            if (stmt is TerminalNodeImpl tNode && (tNode.GetText().Contains('\n') || tNode.GetText() == "<EOF>"))
                continue;
            var instruction = (IGlacierInstruction)stmt.Accept(this)!;
            ArgumentNullException.ThrowIfNull(instruction);
            seq.Add(instruction);
        }
        return new Block(seq);
    }

    public override object? VisitId(GlacierParser.IdContext context)
    {
         return new GetVar(context.ID().GetText());
    }

    public override object? VisitConst_int(GlacierParser.Const_intContext context)
    {
        return new Const(new Number(BigInteger.Parse(context.INT().GetText())));
    }

    public override object? VisitConst_float(GlacierParser.Const_floatContext context)
    {
        //TODO add float support
        return new Const(new Number(BigInteger.Parse(context.FLOAT().GetChild(0).GetText())));
    }

    public override object? VisitConst_str(GlacierParser.Const_strContext context)
    {
        return new Const(new Keyword(context.STRING().GetText()[1..(context.STRING().GetText().Length - 1)]));
    }

    public override object? VisitConst_bool(GlacierParser.Const_boolContext context)
    {
        return new Const(new Bool(bool.Parse(context.BOOL().GetText())));
    }

    public override object? VisitConst_none(GlacierParser.Const_noneContext context)
    {
        return new Const(None.Instance);
    }

    public override object? VisitConst_val(GlacierParser.Const_valContext context)
    {
        return context.GetChild(0).Accept(this);
    }

    public override object? VisitAssign(GlacierParser.AssignContext context)
    {
        var targetVal = context.id().GetText()!;
        var valueSrc = context.expr().Accept(this);
        return new PushVar(targetVal, (IGlacierInstruction)valueSrc!);
    }

    public override object? VisitAddi(GlacierParser.AddiContext context)
    {
        if (context.ADD() != null)
        {
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Add((IGlacierInstruction)a, (IGlacierInstruction)b);
        }

        if (context.SUB() != null)
        {
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Sub((IGlacierInstruction)a, (IGlacierInstruction)b);
        }
        
        return context.GetChild(0).Accept(this);
    }

    public override object? VisitExpr(GlacierParser.ExprContext context)
    {
        if (context.EQ() != null)
        {
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Eq((IGlacierInstruction)a, (IGlacierInstruction)b);
        }

        if (context.NEQ() != null)
        {
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Neq((IGlacierInstruction)a, (IGlacierInstruction)b);
        }

        if (context.GT() != null)
        {
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Gt((IGlacierInstruction)a, (IGlacierInstruction)b);
        }

        if (context.LT() != null)
        {
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Lt((IGlacierInstruction)a, (IGlacierInstruction)b);
        }

        if (context.GTI() != null)
        {
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Gti((IGlacierInstruction)a, (IGlacierInstruction)b);
        }
        
        if (context.LTI() != null)
        {
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Lti((IGlacierInstruction)a, (IGlacierInstruction)b);
        }

        if (context.addi() != null)
        {
            var a = context.GetChild(0).Accept(this);
            return a;
        }

        throw new UnreachableException();
    }

    public override object? VisitTerm(GlacierParser.TermContext context)
    {
        if (context.MUL() != null)
        {
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Mul((IGlacierInstruction)a, (IGlacierInstruction)b);
        }
        
        if (context.DIV() != null)
        {
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Div((IGlacierInstruction)a, (IGlacierInstruction)b);
        }
        
        if (context.REM() != null)
        {
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Rem((IGlacierInstruction)a, (IGlacierInstruction)b);
        }
        
        if (context.IDIV() != null)
        {
            throw new NotImplementedException();
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            return new Mul((IGlacierInstruction)a, (IGlacierInstruction)b);
        }
        
        return context.GetChild(0).Accept(this);
    }
    
    public override object? VisitPow_expr(GlacierParser.Pow_exprContext context)
    {
        if (context.POW() != null)
        {
            throw new NotImplementedException();
            var a = context.GetChild(0).Accept(this);
            var b = context.GetChild(2).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
        }
        return context.GetChild(0).Accept(this);
    }
    
    public override object? VisitFactor(GlacierParser.FactorContext context)
    {
        if (context.SUB() != null)
        {
            var a = context.GetChild(1).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            return new Mul((IGlacierInstruction)a, new Const(new Number(new BigInteger(-1))));
        }
        return context.GetChild(0).Accept(this);
    }

    public override object? VisitAtom(GlacierParser.AtomContext context)
    {
        if (context.const_val() != null)
        {
            var a = context.GetChild(0).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            return a;
        }

        if (context.LPAREN() != null && context.LPAREN() != null)
        {
            var a =  context.GetChild(1).Accept(this);
            ArgumentNullException.ThrowIfNull(a);
            return a;
        }

        if (context.id() != null)
        {
            return new GetVar(context.id().GetText());
        }

        if (context.fncall() != null)
        {
            
        }
        var stmt =  context.GetChild(0).Accept(this);
        ArgumentNullException.ThrowIfNull(stmt);
        return stmt;
    }

    public override object? VisitStmt(GlacierParser.StmtContext context)
    {
        return context.GetChild(0).Accept(this);
        return null;
    }

    public override object? VisitBlock(GlacierParser.BlockContext context)
    {
        var seq = new List<IGlacierInstruction>();
        var stmts = new Span<IParseTree>(context.children.ToArray(), 1, context.children.Count - 2);
        foreach (var stmt in stmts)
        {
            if (stmt is TerminalNodeImpl tNode && (tNode.GetText().Contains('\n') || tNode.GetText() == "<EOF>"))
                continue;
            var instruction = (IGlacierInstruction)stmt.Accept(this)!;
            ArgumentNullException.ThrowIfNull(instruction);
            seq.Add(instruction);
        }
        return new Block(seq);
    }

    public override object? VisitFndecl(GlacierParser.FndeclContext context)
    {
        List<string> args = [];
        for (int i = 1; i < int.MaxValue; ++i)
        {
            var nxt = context.id(i);
            if  (nxt == null)
                break;
            args.Add(nxt.GetText());
        }

        var capturedVars = FindOrphans(context.block(), args);

        var metadata = new FunctionMetadata
                .FunctionMetadataBuilder()
                .WithCaptureGroup(capturedVars.ToList())
                .WithParamNames(args)
                .WithBody((IGlacierInstruction)context.block().Accept(this)!)
                .WithName(context.id(0).GetText())
                .Build();
        return new PushFn(context.id(0).GetText(), metadata);
    }

    public override object? VisitFncall(GlacierParser.FncallContext context)
    {
        var targetFn = context.id().GetText();
        List<IGlacierInstruction> args = [];
        for (int i = 0; i < int.MaxValue; ++i)
        {
            var nxt = context.expr(i);
            if  (nxt == null)
                break;
            var nxtExpr = (IGlacierInstruction)nxt.Accept(this)!;
            ArgumentNullException.ThrowIfNull(nxtExpr);
            args.Add(nxtExpr);
        }

        var fnCall = new Call(new GetVar(targetFn), args);
        return fnCall;
    }

    public override object? VisitReturn(GlacierParser.ReturnContext context)
    {
        var a = context.expr().Accept(this);
        ArgumentNullException.ThrowIfNull(a);
        return new Return((IGlacierInstruction) a);
    }

    public override object? VisitPrintstmt(GlacierParser.PrintstmtContext context)
    {
        var src = (IGlacierInstruction)context.expr().Accept(this)!;
        ArgumentNullException.ThrowIfNull(src);
        return new SpecialNoReturn(ctx => Console.WriteLine(src.Exec(ctx)));
    }

    public override object? VisitIfstmt(GlacierParser.IfstmtContext context)
    {
        var hasElse = context.ELSE() !=  null;
        var cond=  (IGlacierInstruction)context.expr().Accept(this)!;
        ArgumentNullException.ThrowIfNull(cond);
        
        var trueBlock = (Block)context.block(0).Accept(this)!;
        ArgumentNullException.ThrowIfNull(trueBlock);

        var falseBlock = hasElse ? (Block)context.block(1).Accept(this)! : null;
        if (hasElse)
            ArgumentNullException.ThrowIfNull(falseBlock);
        return new If(cond, trueBlock, falseBlock);
    }

    public override object? VisitStrexpr(GlacierParser.StrexprContext context)
    {
        var src = (IGlacierInstruction)context.expr().Accept(this)!;
        ArgumentNullException.ThrowIfNull(src);
        return new Special(ctx => new Keyword(src.Exec(ctx).ToString()!));
    }

    public override object? VisitErrstmt(GlacierParser.ErrstmtContext context)
    {
        var src = (IGlacierInstruction)context.expr().Accept(this)!;
        ArgumentNullException.ThrowIfNull(src);
        
        var fn = (ExecutionContext ctx) =>
        {
            throw new Exception(src.Exec(ctx).ToString());
            
        };
        
        Action<string> throwException = (string msg) => throw new Exception(msg);
        return new SpecialNoReturn(ctx => throwException(src.Exec(ctx).ToString()!));
    }

    public override object? VisitDebugstmt(GlacierParser.DebugstmtContext context)
    {
        return context.id().GetText() switch
        {
            "stacktrace" => new SpecialNoReturn(ctx => ctx.DebugPrint()),
            //Add more debug things as time goes on
            _ => throw new InvalidOperationException()
        };
    }
}