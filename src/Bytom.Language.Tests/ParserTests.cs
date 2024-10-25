namespace Bytom.Language.Tests;

public class ParserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestFunction()
    {
        bool result = Parser.TryParse(@"
            function square(x: i32): i32
            {
                return mul(x, x);
            }
            ",
            out var value,
            out var error,
            out var errorPosition
        );
        Assert.That(error, Is.Null);
        Assert.That(result, Is.True);
        Assert.That(value, Is.InstanceOf<AST.Module>());

        var module = (AST.Module)value;
        Assert.That(module!.statements, Has.Length.EqualTo(1));

        var statement = module!.statements[0];
        Assert.That(statement, Is.InstanceOf<AST.FunctionDefinition>());

        var function = (AST.FunctionDefinition)statement;
        Assert.That(function!.name.name, Is.EqualTo("square"));
        Assert.That(function!.arguments, Has.Length.EqualTo(1));

        var argument = function!.arguments[0];
        Assert.That(argument!.name.name, Is.EqualTo("x"));
        Assert.That(argument!.type.type_name, Is.EqualTo("i32"));
        Assert.That(argument!.type.pointer_level, Is.EqualTo(0));

        Assert.That(function!.return_type.type_name, Is.EqualTo("i32"));
        Assert.That(function!.return_type.pointer_level, Is.EqualTo(0));

        Assert.That(function!.body, Has.Length.EqualTo(1));
        Assert.That(function!.body[0], Is.InstanceOf<AST.Return>());

        var returnStatement = (AST.Return)function!.body[0];
        Assert.That(returnStatement!.value, Is.InstanceOf<AST.FunctionCall>());

        var functionCall = (AST.FunctionCall)returnStatement!.value;
        Assert.That(functionCall!.name.name, Is.EqualTo("mul"));
        Assert.That(functionCall!.arguments, Has.Length.EqualTo(2));

        Assert.That(functionCall!.arguments[0], Is.InstanceOf<AST.Name>());
        var argument1 = (AST.Name)functionCall!.arguments[0];
        Assert.That(argument1!.name, Is.EqualTo("x"));

        Assert.That(functionCall!.arguments[1], Is.InstanceOf<AST.Name>());
        var argument2 = (AST.Name)functionCall!.arguments[1];
        Assert.That(argument2!.name, Is.EqualTo("x"));
    }
}