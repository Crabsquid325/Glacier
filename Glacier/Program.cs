using Antlr4.Runtime;
using Glacier.Core.Interpreter.Instructions;
using Glacier.Core.Parser.Grammar;
using Glacier.Core.Parser.Visitor;
using ExecutionContext = Glacier.Core.Interpreter.ExecutionContext;

var src     = File.ReadAllText(args[0]);

var lexer   = new GlacierLexer (new AntlrInputStream (src  ));
var parser  = new GlacierParser(new CommonTokenStream(lexer));

var ast     = (IGlacierInstruction)new AntlrAstVisitor().VisitProgram(parser.program()!)!;

ast.Exec(new ExecutionContext());