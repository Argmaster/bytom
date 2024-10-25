



namespace Bytom.Language.AST
{
    public class Module
    {
        public Statement[] statements;
        public Module(Statement[] statements)
        {
            this.statements = statements;
        }
    }

    public class Statement
    {
    }

    public class StructDefinition : Statement
    {
        public string name;
        public AliasDeclaration[] fields;

        public StructDefinition(string name, AliasDeclaration[] fields)
        {
            this.name = name;
            this.fields = fields;
        }
    }

    public class BitFieldDefinition : Statement
    {
    }

    public class FunctionDefinition : Statement
    {
        public Name name;
        public FunctionParameter[] arguments;
        public TypeName return_type;
        public Statement[] body;
        public FunctionDefinition(
            Name name,
            FunctionParameter[] arguments,
            TypeName return_type,
            Statement[] body
        )
        {
            this.name = name;
            this.arguments = arguments;
            this.return_type = return_type;
            this.body = body;
        }
    }

    public class FunctionParameter
    {
        public Name name;
        public TypeName type;
        public FunctionParameter(Name name, TypeName type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class AliasDeclaration : Statement
    {
    }

    public class VariableDeclaration : AliasDeclaration
    {
        public Name name;
        public TypeName type;
        public Expression? value;

        public VariableDeclaration(Name name, TypeName type, Expression? value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }
    }

    public class ConstantDeclaration : AliasDeclaration
    {
        public Name name;
        public TypeName type;
        public Expression value;

        public ConstantDeclaration(Name name, TypeName type, Expression value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }
    }

    public class ValueAssignment : Statement
    {
        public LeftIdentifier name;
        public Expression value;
        public ValueAssignment(LeftIdentifier name, Expression value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public class Return : Statement
    {
        public Expression? value;
        public Return(Expression? value)
        {
            this.value = value;
        }
    }

    public class While : Statement
    {
        public Expression condition;
        public Statement[] body;
        public While(Expression condition, Statement[] body)
        {
            this.condition = condition;
            this.body = body;
        }
    }

    public class If : Statement
    {
        public Expression condition;
        public Statement[] body;
        public If(Expression condition, Statement[] body)
        {
            this.condition = condition;
            this.body = body;
        }
    }

    public class Elif : Statement
    {
        public Expression condition;
        public Statement[] body;
        public Elif(Expression condition, Statement[] body)
        {
            this.condition = condition;
            this.body = body;
        }
    }

    public class Else : Statement
    {
        public Statement[] body;
        public Else(Statement[] body)
        {
            this.body = body;
        }
    }

    public class SideEffect : Statement
    {
        public Expression expression;
        public SideEffect(Expression expression)
        {
            this.expression = expression;
        }
    }

    public class Expression
    {
    }

    public class Cast : Expression
    {
        public TypeName type;
        public Expression value;
        public Cast(TypeName type, Expression value)
        {
            this.type = type;
            this.value = value;
        }
    }

    public class FunctionCall : Expression
    {
        public Name name;
        public Expression[] arguments;
        public FunctionCall(Name name, Expression[] arguments)
        {
            this.name = name;
            this.arguments = arguments;
        }
    }

    public class Literal : Expression
    {
    }

    public class StringLiteral : Literal
    {
        public string value;
        public StringLiteral(string value)
        {
            this.value = value;
        }
    }

    public class InlineAssembly : Expression
    {
        public string assembly;
        public InlineAssembly(string assembly)
        {
            this.assembly = assembly;
        }
    }

    public interface LeftIdentifier
    {
    }

    public class Name : Expression, LeftIdentifier
    {
        public string name;
        public Name(string name)
        {
            this.name = name;
        }
    }

    public class DotAccess : Expression, LeftIdentifier
    {
        public Name[] names;
        public DotAccess(Name[] names)
        {
            this.names = names;
        }
    }

    public class TypeName : Expression
    {
        public string type_name;
        public int pointer_level;
        public TypeName(string type_name, int pointer_level)
        {
            this.type_name = type_name;
            this.pointer_level = pointer_level;
        }
    }
}
