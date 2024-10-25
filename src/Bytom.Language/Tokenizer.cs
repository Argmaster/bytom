using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Superpower;
using Superpower.Display;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Bytom.Language
{
    public enum Tokens
    {
        [Token(Description = "inline assembly keyword")]
        Asm,

        [Token(Description = "struct keyword")]
        Struct,

        [Token(Description = "bitfield keyword")]
        BitField,

        [Token(Description = "return keyword")]
        Return,

        [Token(Description = "break keyword")]
        Break,

        [Token(Description = "continue keyword")]
        Continue,

        [Token(Description = "function keyword")]
        Function,

        [Token(Description = "const keyword")]
        Const,

        [Token(Description = "var keyword")]
        Var,

        [Token(Description = "if keyword")]
        If,

        [Token(Description = "elif keyword")]
        Elif,

        [Token(Description = "else keyword")]
        Else,

        [Token(Description = "while keyword")]
        While,

        [Token(Description = "for keyword")]
        For,

        [Token(Description = "arrow operator")]
        Arrow,

        [Token(Description = "assignment operator")]
        Assignment,

        [Token(Description = "Comma")]
        Comma,

        [Token(Description = "Colon")]
        Colon,


        [Token(Description = "semicolon")]
        Semicolon,

        [Token(Description = "left parenthesis")]
        LParen,

        [Token(Description = "right parenthesis")]
        RParen,

        [Token(Description = "left curly bracket")]
        LCurlyBracket,

        [Token(Description = "right curly bracket")]
        RCurlyBracket,

        [Token(Description = "left square bracket")]
        LSquareBracket,

        [Token(Description = "right square bracket")]
        RSquareBracket,

        [Token(Description = "left angle bracket")]
        LAngleBracket,

        [Token(Description = "right angle bracket")]
        RAngleBracket,

        [Token(Description = "asterisk")]
        Asterisk,

        [Token(Description = "dot")]
        Dot,

        [Token(Description = "integer literal")]
        IntegerLiteral,

        [Token(Description = "floating point literal")]
        FloatingPointLiteral,

        [Token(Description = "string literal")]
        StringLiteral,

        [Token(Description = "name")]
        Name,

        [Token(Description = "generic name")]
        GenericName,
    }

    public static class Tokenizer
    {
        static TextParser<Unit> StringToken { get; } =
            from open in Character.EqualTo('"')
            from content in Character.EqualTo('\\').IgnoreThen(Character.AnyChar).Value(Unit.Value).Try()
                .Or(Character.Except('"').Value(Unit.Value))
                .IgnoreMany()
            from close in Character.EqualTo('"')
            select Unit.Value;

        public static Tokenizer<Tokens> Instance { get; } =
            new TokenizerBuilder<Tokens>()
                .Ignore(Span.WhiteSpace)
                .Match(Character.EqualTo('{'), Tokens.LCurlyBracket)
                .Match(Character.EqualTo('}'), Tokens.RCurlyBracket)
                .Match(Character.EqualTo(';'), Tokens.Semicolon)
                .Match(Character.EqualTo(':'), Tokens.Colon)
                .Match(Character.EqualTo(','), Tokens.Comma)
                .Match(Character.EqualTo('['), Tokens.LSquareBracket)
                .Match(Character.EqualTo(']'), Tokens.RSquareBracket)
                .Match(Character.EqualTo('('), Tokens.LParen)
                .Match(Character.EqualTo(')'), Tokens.RParen)
                .Match(Character.EqualTo('<'), Tokens.LAngleBracket)
                .Match(Character.EqualTo('>'), Tokens.RAngleBracket)
                .Match(Character.EqualTo('*'), Tokens.Asterisk)
                .Match(Character.EqualTo('.'), Tokens.Dot)
                .Match(Span.EqualTo("=>"), Tokens.Arrow)
                .Match(Character.EqualTo('='), Tokens.Assignment)
                .Match(Span.EqualTo("asm"), Tokens.Asm)
                .Match(Span.EqualTo("struct"), Tokens.Struct)
                .Match(Span.EqualTo("bitfield"), Tokens.BitField)
                .Match(Span.EqualTo("return"), Tokens.Return)
                .Match(Span.EqualTo("break"), Tokens.Break)
                .Match(Span.EqualTo("continue"), Tokens.Continue)
                .Match(Span.EqualTo("function"), Tokens.Function)
                .Match(Span.EqualTo("const"), Tokens.Const)
                .Match(Span.EqualTo("var"), Tokens.Var)
                .Match(Span.EqualTo("while"), Tokens.While)
                .Match(Span.EqualTo("for"), Tokens.For)
                .Match(Span.EqualTo("if"), Tokens.If)
                .Match(Span.EqualTo("elif"), Tokens.Elif)
                .Match(Span.EqualTo("else"), Tokens.Else)
                .Match(StringToken, Tokens.StringLiteral)
                .Match(Numerics.Integer, Tokens.IntegerLiteral, requireDelimiters: true)
                .Match(Numerics.Integer, Tokens.IntegerLiteral, requireDelimiters: true)
                .Match(Identifier.CStyle, Tokens.Name, requireDelimiters: true)
                .Match(Span.Regex("[a-zA-Z_$]*"), Tokens.GenericName, requireDelimiters: true)
                .Build();
    }
}
