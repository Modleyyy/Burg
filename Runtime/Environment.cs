namespace Burg.Runtime;

using Burg.FrontEnd.AST;
using Burg.FrontEnd.Tokenizing;

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

    public IRuntimeValue GetValue(string valueName)
    {
        return Resolve(valueName).values[valueName];
    }

    public Environment Resolve(string valueName)
    {
        if (values.ContainsKey(valueName))
            return this;

        if (parent is null) {
            throw new($"Runtime Error:\n Value \"{valueName}\" not defined in scope.");
        }

        return parent.Resolve(valueName);
    }

    private static void SetupGlobal(Environment env)
    {
        // global values
        env.DeclareValue("null", new NullValue());
        env.DeclareValue("true", new BoolValue(true));
        env.DeclareValue("false", new BoolValue(false));

        // global native functions
        env.DeclareValue("out", new FunctionValue((args,_) => {
            string outstr = "";
            int l = args.Count;
            for (int i = 0; i < l; i++)
            {
                if (i == l - 1)
                    outstr += args[i].value;
                else
                    outstr += args[i].value + " ";
            }
            Console.WriteLine(outstr);
            return new NullValue();
        }));
        env.DeclareValue("in", new FunctionValue((args,_) => {
            if (args.Count > 0)
            {
                throw new("The native function 'in' only takes no arguments.");
            }

            var text = Console.ReadLine();

            return new StringValue(text is null ? "" : text);
        }));
        env.DeclareValue("type", new FunctionValue((args,_) => {
            if (args.Count == 1)
            {
                switch (args[0].type)
                {
                    case ValueTypes.Null:
                        return new StringValue("null");
                    case ValueTypes.Integer:
                        return new StringValue("integer");
                    case ValueTypes.Float:
                        return new StringValue("float");
                    case ValueTypes.Bool:
                        return new StringValue("bool");
                    case ValueTypes.String:
                        return new StringValue("string");
                    case ValueTypes.Array:
                        return new StringValue("array");
                    case ValueTypes.Dictionary:
                        return new StringValue("dictionary");
                    case ValueTypes.Function:
                        return new StringValue("function");
                    default:
                        return new StringValue("wait what the actual fuck how what is thissAEZOFH");
                }
            }
            else if (args.Count == 0)
            {
                throw new("The native function 'type' must pass in a value argument.");
            }
            else
            {
                throw new("The native function 'type' only takes a single argument.");
            }
        }));
        env.DeclareValue("require", new FunctionValue((args,_) => {
            if (args.Count == 0)
            {
                throw new("The native function 'require' must pass in a value argument.");
            }
            else if (args.Count > 1)
            {
                throw new("The native function 'require' only takes a single argument.");
            }
            else if (args[0].type != ValueTypes.String)
            {
                throw new("The native function 'require' must pass in an argument of type 'string'");
            }

            string resultPath = Path.Combine(Path.GetDirectoryName(Program.path)!, (string)args[0].value);
            string code = new StreamReader(resultPath).ReadToEnd();

            Environment globalEnv = new(new());
            Chunk program = Parser.ParseAST(Tokenizer.Tokenize(code));
            IRuntimeValue value = Interpreter.EvaluateChunk(program, globalEnv);

            return value;
        }));
        env.DeclareValue("exit", new FunctionValue( (_,_) => {
            System.Environment.Exit(0);
            return new StringValue("exit lol ig this code is unreachable so uh yeah cool");
        }));
        env.DeclareValue("time", new FunctionValue( (_,_) => new IntegerValue(DateTime.UnixEpoch.Millisecond)));

        // global helper
        Dictionary<IRuntimeValue, IRuntimeValue> math = new() {
            [new StringValue("abs")] = new FunctionValue((args,_) => {
                if (args.Count == 0)
                {
                    throw new("The native function 'math.abs' must pass in a value argument.");
                }
                else if (args.Count > 1)
                {
                    throw new("The native function 'math.abs' only takes a single argument.");
                }
                else if (args[0].type != ValueTypes.Integer && args[0].type != ValueTypes.Float)
                {
                    throw new("The native function 'math.abs' must pass in an argument of type 'integer' or 'float'");
                }

                return args[0].type == ValueTypes.Integer ?
                        new IntegerValue(Math.Abs(((IntegerValue)args[0]).value)) :
                        new   FloatValue(Math.Abs(((  FloatValue)args[0]).value)) ;
            }),
            [new StringValue("sin")] = new FunctionValue((args,_) => {
                if (args.Count == 0)
                {
                    throw new("The native function 'math.sin' must pass in a value argument.");
                }
                else if (args.Count > 1)
                {
                    throw new("The native function 'math.sin' only takes a single argument.");
                }
                else if (args[0].type != ValueTypes.Integer && args[0].type != ValueTypes.Float)
                {
                    throw new("The native function 'math.sin' must pass in an argument of type 'integer' or 'float'");
                }

                return args[0].type == ValueTypes.Integer ?
                        new IntegerValue(Convert.ToInt32(Math.Sin(((IntegerValue)args[0]).value))) :
                        new FloatValue(Math.Sin(((FloatValue)args[0]).value));
            }),
            [new StringValue("cos")] = new FunctionValue((args,_) => {
                if (args.Count == 0)
                {
                    throw new("The native function 'math.cos' must pass in a value argument.");
                }
                else if (args.Count > 1)
                {
                    throw new("The native function 'math.cos' only takes a single argument.");
                }
                else if (args[0].type != ValueTypes.Integer && args[0].type != ValueTypes.Float)
                {
                    throw new("The native function 'math.cos' must pass in an argument of type 'integer' or 'float'");
                }

                return args[0].type == ValueTypes.Integer ?
                        new IntegerValue(Convert.ToInt32(Math.Cos(((IntegerValue)args[0]).value))) :
                        new FloatValue(Math.Cos(((FloatValue)args[0]).value));
            }),
            [new StringValue("tan")] = new FunctionValue((args,_) => {
                if (args.Count == 0)
                {
                    throw new("The native function 'math.tan' must pass in a value argument.");
                }
                else if (args.Count > 1)
                {
                    throw new("The native function 'math.tan' only takes a single argument.");
                }
                else if (args[0].type != ValueTypes.Integer && args[0].type != ValueTypes.Float)
                {
                    throw new("The native function 'math.tan' must pass in an argument of type 'integer' or 'float'");
                }

                return args[0].type == ValueTypes.Integer ?
                        new IntegerValue(Convert.ToInt32(Math.Tan(((IntegerValue)args[0]).value))) :
                        new FloatValue(Math.Tan(((FloatValue)args[0]).value));
            }),
        };
        Dictionary<IRuntimeValue, IRuntimeValue> str = new() {
            [new StringValue("length")] = new FunctionValue((args,_) => {
                if (args.Count == 0)
                {
                    throw new("The native function 'str.length' must pass in a value argument.");
                }
                else if (args.Count > 1)
                {
                    throw new("The native function 'str.length' only takes a single argument.");
                }
                else if (args[0].type != ValueTypes.String)
                {
                    throw new("The native function 'str.length' must pass in an argument of type 'string'");
                }

                return new IntegerValue(((StringValue)args[0]).value.Length);
            }),
        };
        Dictionary<IRuntimeValue, IRuntimeValue> arr = new() {
            [new StringValue("length")] = new FunctionValue((args,_) => {
                if (args.Count == 0)
                {
                    throw new("The native function 'arr.length' must pass in a value argument.");
                }
                else if (args.Count > 1)
                {
                    throw new("The native function 'arr.length' only takes a single argument.");
                }
                else if (args[0].type != ValueTypes.Array)
                {
                    throw new("The native function 'arr.length' must pass in an argument of type 'array'");
                }

                return new IntegerValue(((ArrayValue)args[0]).list.Count);
            }),
            [new StringValue("map")] = new FunctionValue((args,env) => {
                if (args.Count != 2)
                {
                    throw new("The native function 'arr.map' takes exactly two arguments.");
                }
                else if (args[0].type != ValueTypes.Array)
                {
                    throw new("The first argument of 'arr.map' must be of type 'array'.");
                }
                else if (args[1].type != ValueTypes.Function)
                {
                    throw new("The second argument of 'arr.map' must be a function.");
                }

                var array = (ArrayValue)args[0];
                var func = (FunctionValue)args[1];

                List<IRuntimeValue> mapped = new();
                foreach (var item in array.list)
                {
                    mapped.Add(func.call(new List<IRuntimeValue> { item }, env));
                }
                return new ArrayValue(mapped);
            }),
        };
        Dictionary<IRuntimeValue, IRuntimeValue> dict = new() {
            [new StringValue("length")] = new FunctionValue((args,_) => {
                if (args.Count == 0)
                {
                    throw new("The native function 'dict.length' must pass in a value argument.");
                }
                else if (args.Count > 1)
                {
                    throw new("The native function 'dict.length' only takes a single argument.");
                }
                else if (args[0].type != ValueTypes.Dictionary)
                {
                    throw new("The native function 'dict.length' must pass in an argument of type 'dictionary'");
                }

                return new IntegerValue(((DictionaryValue)args[0]).props.Keys.Count);
            }),
            [new StringValue("keys")] = new FunctionValue((args,_) => {
                if (args.Count == 0)
                {
                    throw new("The native function 'dict.keys' must pass in a value argument.");
                }
                else if (args.Count > 1)
                {
                    throw new("The native function 'dict.keys' only takes a single argument.");
                }
                else if (args[0].type != ValueTypes.Dictionary)
                {
                    throw new("The native function 'dict.keys' must pass in an argument of type 'dictionary'");
                }

                return new ArrayValue(((DictionaryValue)args[0]).props.Keys.ToList());
            }),
            [new StringValue("values")] = new FunctionValue((args,_) => {
                if (args.Count == 0)
                {
                    throw new("The native function 'dict.values' must pass in a value argument.");
                }
                else if (args.Count > 1)
                {
                    throw new("The native function 'dict.values' only takes a single argument.");
                }
                else if (args[0].type != ValueTypes.Dictionary)
                {
                    throw new("The native function 'dict.values' must pass in an argument of type 'dictionary'");
                }

                return new ArrayValue(((DictionaryValue)args[0]).props.Values.ToList());
            }),
        };

        env.DeclareValue("math", new DictionaryValue() { props = math });
        env.DeclareValue("str", new DictionaryValue() { props = str });
        env.DeclareValue("arr", new DictionaryValue() { props = arr });
        env.DeclareValue("dict", new DictionaryValue() { props = dict });
    }
}
