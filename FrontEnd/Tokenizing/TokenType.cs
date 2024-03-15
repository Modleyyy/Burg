namespace Burg.FrontEnd.Tokenizing;

public enum TokenType {
    Identifier, // The identifier name of a value, function, parameter, ect...
    EOF, // End of File
    EndLine, // The ; character, ends the current statement line
    Seperator, // The , character, seperates stuff
    Accessor, // The . character, to access member properties

    // Keywords
    Val,
    Function,
    Lambda,
    Return,
    If,
    Then,
    Else,
    End, // Closes the code block selected by the next keyword

    // Operations
    BinaryOperator,
    SetValue,

    // Grouping
    OpenParen,    // (
    CloseParen,   // )
    OpenBrace,    // {
    CloseBrace,   // }
    OpenBracket,  // [
    CloseBracket, // ]

    // Literals
    IntegerLit ,
    FloatLit ,
    StringLit ,
}
