using System.Diagnostics;
using Burg.FrontEnd.AST;
using Burg.FrontEnd.Tokenizing;
using Burg.Runtime;
using Environment = Burg.Runtime.Environment;
using SMV = Burg.Runtime.Interpreter.StmtReturnValue;

public static class Program
{
    public static string path = "require doesn't work in repl mode dummy";
    public static void Main(string[] args)
    {
        bool debugMode = false;
        int debugIndex = Array.FindIndex(args, arg => arg == "-d" || arg == "--debug");

        if (debugIndex != -1)
        {
            debugMode = true;
            args = args.Where((arg, index) => index != debugIndex).ToArray();
        }

        if (args.Length > 0)
        {
            string path = args[0];
            Program.path = path;
            string code = File.ReadAllText(path);

            ExecuteCode(code, new(), debugMode);
        }
        else
        {
            Console.WriteLine("\tBurgR -- Burg REPL 1.0");
            Console.WriteLine("\tWrite 'exit' to leave");
            REPL(debugMode);
        }
    }

    private static SMV ExecuteCode(string code, Environment env, bool debugMode)
    {
        Stopwatch sw = new();
        if (debugMode)
            sw.Start();

        List<Token> tokens = Tokenizer.Tokenize(code);
        if (debugMode)
        {
            sw.Stop();
            Console.WriteLine($"Spent: {sw.ElapsedMilliseconds}ms tokenizing.");
            sw.Restart();
        }

        Chunk chunk = Parser.ParseAST(tokens);
        if (debugMode)
        {
            sw.Stop();
            Console.WriteLine($"Spent: {sw.ElapsedMilliseconds}ms parsing.");
            sw.Restart();
        }

        SMV value = Interpreter.EvaluateStmt(chunk, env);
        if (debugMode)
        {
            sw.Stop();
            Console.WriteLine($"Spent: {sw.ElapsedMilliseconds}ms interpreting.");
        }

        return value;
    }

    private static void REPL(bool debugMode)
    {
        Environment env = new();
        while (true)
        {
            Console.Write("> ");
            string code = Console.ReadLine() ?? "";
            if (code == "exit")
                return;

            env = new(env);
            Console.WriteLine(ExecuteCode(code, env, debugMode).value);
        }
    }
}
