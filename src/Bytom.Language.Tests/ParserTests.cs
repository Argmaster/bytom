namespace Bytom.Language.Tests;

public class ParserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestFunctionSquare()
    {
        bool result = Parser.TryParse(@"
            function square(var x: i32;): i32
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
        Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

        var function = (AST.Statements.FunctionDefinition)statement;
        Assert.That(function!.name.name, Is.EqualTo("square"));
        Assert.That(function!.arguments, Has.Length.EqualTo(1));

        var argument = (AST.Statements.VariableDeclaration)function!.arguments[0];
        Assert.That(argument!.name.name, Is.EqualTo("x"));
        Assert.That(argument!.type.type_name, Is.EqualTo("i32"));
        Assert.That(argument!.type.pointer_level, Is.EqualTo(0));

        Assert.That(function!.return_type.type_name, Is.EqualTo("i32"));
        Assert.That(function!.return_type.pointer_level, Is.EqualTo(0));

        Assert.That(function!.body, Has.Length.EqualTo(1));
        Assert.That(function!.body[0], Is.InstanceOf<AST.Statements.Return>());

        var returnStatement = (AST.Statements.Return)function!.body[0];
        Assert.That(returnStatement!.value, Is.InstanceOf<AST.Expressions.FunctionCall>());

        var functionCall = (AST.Expressions.FunctionCall)returnStatement!.value;
        Assert.That(functionCall!.name.name, Is.EqualTo("mul"));
        Assert.That(functionCall!.arguments, Has.Length.EqualTo(2));

        Assert.That(functionCall!.arguments[0], Is.InstanceOf<AST.Expressions.Name>());
        var argument1 = (AST.Expressions.Name)functionCall!.arguments[0];
        Assert.That(argument1!.name, Is.EqualTo("x"));

        Assert.That(functionCall!.arguments[1], Is.InstanceOf<AST.Expressions.Name>());
        var argument2 = (AST.Expressions.Name)functionCall!.arguments[1];
        Assert.That(argument2!.name, Is.EqualTo("x"));
    }

    [Test]
    public void TestVariableDeclaration()
    {
        bool result = Parser.TryParse(@"
            function main(): void
            {
                var y: i32;
                var x: i32 = 1;
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
        Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

        var function = (AST.Statements.FunctionDefinition)statement;
        Assert.That(function!.name.name, Is.EqualTo("main"));
        Assert.That(function!.arguments, Has.Length.EqualTo(0));
        Assert.That(function!.return_type.type_name, Is.EqualTo("void"));
        Assert.That(function!.body, Has.Length.EqualTo(2));

        Assert.That(function!.body[0], Is.InstanceOf<AST.Statements.VariableDeclaration>());
        var variableDeclaration1 = (AST.Statements.VariableDeclaration)function!.body[0];
        Assert.That(variableDeclaration1!.name.name, Is.EqualTo("y"));
        Assert.That(variableDeclaration1!.type.type_name, Is.EqualTo("i32"));
        Assert.That(variableDeclaration1!.value, Is.Null);

        Assert.That(function!.body[1], Is.InstanceOf<AST.Statements.VariableDeclaration>());
        var variableDeclaration2 = (AST.Statements.VariableDeclaration)function!.body[1];
        Assert.That(variableDeclaration2!.name.name, Is.EqualTo("x"));
        Assert.That(variableDeclaration2!.type.type_name, Is.EqualTo("i32"));
        Assert.That(variableDeclaration2!.value, Is.InstanceOf<AST.Expressions.IntegerLiteral>());
    }

    [Test]
    public void TestConstantDeclaration()
    {
        bool result = Parser.TryParse(@"
            function main(): void
            {
                const x: i32 = 1;
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
        Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

        var function = (AST.Statements.FunctionDefinition)statement;
        Assert.That(function!.name.name, Is.EqualTo("main"));
        Assert.That(function!.arguments, Has.Length.EqualTo(0));
        Assert.That(function!.return_type.type_name, Is.EqualTo("void"));
        Assert.That(function!.body, Has.Length.EqualTo(1));

        Assert.That(function!.body[0], Is.InstanceOf<AST.Statements.ConstantDeclaration>());
        var variableDeclaration2 = (AST.Statements.ConstantDeclaration)function!.body[0];
        Assert.That(variableDeclaration2!.name.name, Is.EqualTo("x"));
        Assert.That(variableDeclaration2!.type.type_name, Is.EqualTo("i32"));
        Assert.That(variableDeclaration2!.value, Is.InstanceOf<AST.Expressions.IntegerLiteral>());
    }
}