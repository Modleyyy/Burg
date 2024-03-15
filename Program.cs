using Burg.FrontEnd.AST;
using Burg.FrontEnd.Tokenizing;
using Burg.Runtime;
using Environment = Burg.Runtime.Environment;

public static class Program
{
    public static string path = "require doesn't work in repl mode dummy";
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            string path = args[0];
            Program.path = path;
            string code = File.ReadAllText(path);

            ExecuteCode(code, new());
        }
        else
        {
            Console.WriteLine("\tBurgR -- Burg REPL 1.0");
            Console.WriteLine("\tWrite 'exit' to leave");
            REPL();
        }
    }

    private static IRuntimeValue ExecuteCode(string code, Environment env)
    {
        List<Token> tokens = Tokenizer.Tokenize(code);
        Chunk chunk = Parser.ParseAST(tokens);

        return Interpreter.Evaluate(chunk, env);
    }

    private static void REPL()
    {
        Environment env = new();
        while (true)
        {
            Console.Write("> ");
            string code = Console.ReadLine() ?? "";
            if (code == "exit")
                return;

            env = new(env);
            Console.WriteLine(ExecuteCode(code, env).value);
        }
    }
}
