namespace Burg.Runtime;

using Burg.FrontEnd.AST;
using Burg.FrontEnd.Tokenizing;
using SMV = Interpreter.StmtReturnValue;

public class Environment
{
    private readonly Environment? parent;
    private readonly Dictionary<string, IRuntimeValue> values;

    public Environment(Environment? parent = null)
    {
        values = new();
        this.parent = parent;

        if (parent is null)
            SetupGlobal(this);
    }

    public void DeclareValue(string valueName, IRuntimeValue value)
    {
        if (values.ContainsKey(valueName))
            throw new($"Runtime Error:\n Value \"{valueName}\" already defined in scope.");

        values.Add(valueName, value);
    }

    public IRuntimeValue GetValue(string valueName) =>
        Resolve(valueName).values[valueName];

    public Environment Resolve(string valueName)
    {
        if (values.ContainsKey(valueName))
            return this;

        if (parent is null)
            throw new($"Runtime Error:\n Value \"{valueName}\" not defined in scope.");

        return parent.Resolve(valueName);
    }

    private static void SetupGlobal(Environment env)
    {
        // global values
        env.DeclareValue("null", new NullValue());
        env.DeclareValue("true", new BoolValue(true));
        env.DeclareValue("false", new BoolValue(false));

        // global native functions
        env.DeclareValue("out", new FunctionValue((args) => {
            string outstr = "";
            int l = args.Count;
            for (int i = 0; i < l; i++)
            {
                outstr += args[i].value;
                if (i < l - 1)
                    outstr += ' ';
            }
            Console.WriteLine(outstr);
            return new NullValue();
        }));
        env.DeclareValue("in", new FunctionValue((args) => {
            if (args.Count > 0)
                throw new("The native function 'in' only takes no arguments.");

            var text = Console.ReadLine();

            return new StringValue(text is null ? "" : text);
        }));
        env.DeclareValue("type", new FunctionValue((args) => {
            if (args.Count == 1)
                return args[0].type switch
                {
                    ValueTypes.Null => new StringValue("null"),
                    ValueTypes.Integer => new StringValue("integer"),
                    ValueTypes.Float => new StringValue("float"),
                    ValueTypes.Bool => new StringValue("bool"),
                    ValueTypes.String => new StringValue("string"),
                    ValueTypes.Array => new StringValue("array"),
                    ValueTypes.Dictionary => new StringValue("dictionary"),
                    ValueTypes.Function => new StringValue("function"),
                    _ => (IRuntimeValue)new StringValue("wait what the actual fuck how what is thissAEZOFH"),
                };
            else if (args.Count == 0)
                throw new("The native function 'type' must pass in a value argument.");
            else
                throw new("The native function 'type' only takes a single argument.");
        }));
        env.DeclareValue("require", new FunctionValue((args) => {
            if (args.Count == 0)
                throw new("The native function 'require' must pass in a value argument.");
            else if (args.Count > 1)
                throw new("The native function 'require' only takes a single argument.");
            else if (args[0].type != ValueTypes.String)
                throw new("The native function 'require' must pass in an argument of type 'string'");

            string resultPath = Path.Combine(Path.GetDirectoryName(Program.path)!, (string)args[0].value);
            string code = new StreamReader(resultPath).ReadToEnd();

            Environment globalEnv = new(new());
            Chunk program = Parser.ParseAST(Tokenizer.Tokenize(code));
            SMV value = Interpreter.EvaluateChunk(program, globalEnv);

            return value.hasValue ? value.value! : throw new(
                $"There was no return statement in the code provided at: {resultPath}.");
        }));
        env.DeclareValue("exit", new FunctionValue( (_) => {
            System.Environment.Exit(0);
            return new StringValue("exit lol ig this code is unreachable so uh yeah cool");
        }));
        env.DeclareValue("time", new FunctionValue( (_) => new IntegerValue(DateTime.UnixEpoch.Millisecond)));
        env.DeclareValue("for", new FunctionValue((args) => {
            if (args.Count == 4)
            {
                if (args[0].type != ValueTypes.Integer)
                    throw new("The starting value must be an integer.");
                else if (args[1].type != ValueTypes.Function)
                    throw new("The condition must be a function.");
                else if (args[2].type != ValueTypes.Function)
                    throw new("The action must be a function.");
                else if (args[3].type != ValueTypes.Function)
                    throw new("The code block must be a function.");

                int startValue = ((IntegerValue)args[0]).value;
                FunctionValue.Function condition = ((FunctionValue)args[1]).call;
                FunctionValue.Function action    = ((FunctionValue)args[2]).call;
                FunctionValue.Function block     = ((FunctionValue)args[3]).call;
                for (int i = startValue;
                    ((BoolValue)condition(new() { new IntegerValue(i) })).value;
                    i = ((IntegerValue)action(new() { new IntegerValue(i) })).value
                    )
                    block(new() {new IntegerValue(i)});

                return new NullValue();
            }
            else if (args.Count == 0)
                throw new("The native function 'for' must pass in 4 value arguments.");
            else
                throw new("The native function 'for' must take 4 arguments.");
        }));
        env.DeclareValue("while", new FunctionValue((args) => {
            if (args.Count == 3)
            {
                if (args[1].type != ValueTypes.Function)
                    throw new("The condition must be a function.");
                else if (args[2].type != ValueTypes.Function)
                    throw new("The code block must be a function.");

                var state = args[0];
                var condition  = ((FunctionValue)args[1]).call;
                var block      = ((FunctionValue)args[2]).call;
                while (((BoolValue)condition(new() { state })).value)
                    state = block(new() { state });

                return new NullValue();
            }
            else if (args.Count == 0)
                throw new("The native function 'while' must pass in 3 value arguments.");
            else
                throw new("The native function 'while' must take 3 arguments.");
        }));

        // global helper
        Dictionary<IRuntimeValue, IRuntimeValue> math = new() {
            [new StringValue("abs")] = new FunctionValue((args) => {
                if (args.Count == 0)
                    throw new("The native function 'math.abs' must pass in a value argument.");
                else if (args.Count > 1)
                    throw new("The native function 'math.abs' only takes a single argument.");
                else if (args[0].type != ValueTypes.Integer && args[0].type != ValueTypes.Float)
                    throw new("The native function 'math.abs' must pass in an argument of type 'integer' or 'float'");

                return args[0].type == ValueTypes.Integer ?
                        new IntegerValue(Math.Abs(((IntegerValue)args[0]).value)) :
                        new   FloatValue(Math.Abs(((  FloatValue)args[0]).value)) ;
            }),
            [new StringValue("sin")] = new FunctionValue((args) => {
                if (args.Count == 0)
                    throw new("The native function 'math.sin' must pass in a value argument.");
                else if (args.Count > 1)
                    throw new("The native function 'math.sin' only takes a single argument.");
                else if (args[0].type != ValueTypes.Integer && args[0].type != ValueTypes.Float)
                    throw new("The native function 'math.sin' must pass in an argument of type 'integer' or 'float'");

                return args[0].type == ValueTypes.Integer ?
                        new IntegerValue(Convert.ToInt32(Math.Sin(((IntegerValue)args[0]).value))) :
                        new FloatValue(Math.Sin(((FloatValue)args[0]).value));
            }),
            [new StringValue("cos")] = new FunctionValue((args) => {
                if (args.Count == 0)
                    throw new("The native function 'math.cos' must pass in a value argument.");
                else if (args.Count > 1)
                    throw new("The native function 'math.cos' only takes a single argument.");
                else if (args[0].type != ValueTypes.Integer && args[0].type != ValueTypes.Float)
                    throw new("The native function 'math.cos' must pass in an argument of type 'integer' or 'float'");

                return args[0].type == ValueTypes.Integer ?
                        new IntegerValue(Convert.ToInt32(Math.Cos(((IntegerValue)args[0]).value))) :
                        new FloatValue(Math.Cos(((FloatValue)args[0]).value));
            }),
            [new StringValue("tan")] = new FunctionValue((args) => {
                if (args.Count == 0)
                    throw new("The native function 'math.tan' must pass in a value argument.");
                else if (args.Count > 1)
                    throw new("The native function 'math.tan' only takes a single argument.");
                else if (args[0].type != ValueTypes.Integer && args[0].type != ValueTypes.Float)
                    throw new("The native function 'math.tan' must pass in an argument of type 'integer' or 'float'");

                return args[0].type == ValueTypes.Integer ?
                        new IntegerValue(Convert.ToInt32(Math.Tan(((IntegerValue)args[0]).value))) :
                        new FloatValue(Math.Tan(((FloatValue)args[0]).value));
            }),
        };
        Dictionary<IRuntimeValue, IRuntimeValue> str = new() {
            [new StringValue("length")] = new FunctionValue((args) => {
                if (args.Count == 0)
                    throw new("The native function 'str.length' must pass in a value argument.");
                else if (args.Count > 1)
                    throw new("The native function 'str.length' only takes a single argument.");
                else if (args[0].type != ValueTypes.String)
                    throw new("The native function 'str.length' must pass in an argument of type 'string'");

                return new IntegerValue(((StringValue)args[0]).value.Length);
            }),
        };
        Dictionary<IRuntimeValue, IRuntimeValue> arr = new() {
            [new StringValue("length")] = new FunctionValue((args) => {
                if (args.Count == 0)
                    throw new("The native function 'arr.length' must pass in a value argument.");
                else if (args.Count > 1)
                    throw new("The native function 'arr.length' only takes a single argument.");
                else if (args[0].type != ValueTypes.Array)
                    throw new("The native function 'arr.length' must pass in an argument of type 'array'");

                return new IntegerValue(((ArrayValue)args[0]).list.Count);
            }),
            [new StringValue("map")] = new FunctionValue((args) => {
                if (args.Count != 2)
                    throw new("The native function 'arr.map' must take exactly two arguments.");
                else if (args[0].type != ValueTypes.Array)
                    throw new("The first argument of 'arr.map' must be of type 'array'.");
                else if (args[1].type != ValueTypes.Function)
                    throw new("The second argument of 'arr.map' must be a function.");

                var array = (ArrayValue)args[0];
                var func = (FunctionValue)args[1];

                List<IRuntimeValue> mapped = new();
                foreach (var item in array.list)
                    mapped.Add(func.call(new List<IRuntimeValue> { item }));

                return new ArrayValue(mapped);
            }),
        };
        Dictionary<IRuntimeValue, IRuntimeValue> dict = new() {
            [new StringValue("length")] = new FunctionValue((args) => {
                if (args.Count == 0)
                    throw new("The native function 'dict.length' must pass in a value argument.");
                else if (args.Count > 1)
                    throw new("The native function 'dict.length' only takes a single argument.");
                else if (args[0].type != ValueTypes.Dictionary)
                    throw new("The native function 'dict.length' must pass in an argument of type 'dictionary'");

                return new IntegerValue(((DictionaryValue)args[0]).props.Keys.Count);
            }),
            [new StringValue("keys")] = new FunctionValue((args) => {
                if (args.Count == 0)
                    throw new("The native function 'dict.keys' must pass in a value argument.");
                else if (args.Count > 1)
                    throw new("The native function 'dict.keys' only takes a single argument.");
                else if (args[0].type != ValueTypes.Dictionary)
                    throw new("The native function 'dict.keys' must pass in an argument of type 'dictionary'");

                return new ArrayValue(((DictionaryValue)args[0]).props.Keys.ToList());
            }),
            [new StringValue("values")] = new FunctionValue((args) => {
                if (args.Count == 0)
                    throw new("The native function 'dict.values' must pass in a value argument.");
                else if (args.Count > 1)
                    throw new("The native function 'dict.values' only takes a single argument.");
                else if (args[0].type != ValueTypes.Dictionary)
                    throw new("The native function 'dict.values' must pass in an argument of type 'dictionary'");

                return new ArrayValue(((DictionaryValue)args[0]).props.Values.ToList());
            }),
        };

        env.DeclareValue("math", new DictionaryValue() { props = math });
        env.DeclareValue("str", new DictionaryValue() { props = str });
        env.DeclareValue("arr", new DictionaryValue() { props = arr });
        env.DeclareValue("dict", new DictionaryValue() { props = dict });
    }
}
