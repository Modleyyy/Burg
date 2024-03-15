# Burg, a simple and clean functional programming language.

***Welcome to Burg Land!*** 

Burg is a simple functional programming language wrote in C# with all features you'd need from a simple functional programming language!

## Features

- **Functional Paradigm**: Burg embraces the functional programming paradigm, including every feature you'd expect from a functional language, such as functions! Cool, right?
  
- **Simplicity**: Burg is designed to have a syntax similar to Lua, and thus be intuitive and easy to read, making it easy for beginners to learn and for masters to work, and ultimately enabling developers to write code that is both readable and maintainable (or not lmao).

- **Built-in Functions**: Burg provides a wide set of native built-in functions for common programming tasks such as math and array operations with the global dictionaries `math`, `arr`, `dict`, ect... reducing the need for your average spaghetti-type-of code you write on a daily basis for breakfast.

- **Scalability**: While Burg is not really the best language and might be a little slow, it allows for scalability and dividing code into different module files thanks to the native `require` function. All you need to do is make sure you end your module files by returning what you wanted to export using a `return` statement! <u>eg</u>:
    ```burg
        ### something.brg
    val something = lm(x)
        out(x);
        return x + 2;
    end lm;

    return something; # return something at the end of the file

        ### main.brg
    val required_sm = require("./something.brg");
    out(required_sm(53)); # prints 53, then prints 55
    ```

## Getting Started

To start using Burg, follow these simple steps:

1. **Installation**: The Burg interpreter and REPL can be installed via the [Releases page](https://github.com/Modleyyy/Burg/releases).

1. **Hello, World!**: Dive into Burg by creating your first program, the classic "Hello, World!" example.
    ```burg
    out("Hello, World!");
    ```

1. **Explore Documentation**: Check out the [official wiki](https://github.com/Modleyyy/Burg/wiki) for detailed information on Burg's syntax, features, and usage.

1. **Join the Fun**: Engage with fellow Burgers and developers on [this repository's Discussions page](https://github.com/Modleyyy/Burg/discussions) to share ideas, ask questions, and collaborate.

## Example

Here's a simple example demonstrating Burg's syntax and features:

```burg
# Calculate the factorial of a number
fn factorial(n)
    if (n <= 1) then
        return 1;
    else
        return n * factorial(n - 1);
    end if
end fn

val result = factorial(5);
out(result); # Output: 120
```
