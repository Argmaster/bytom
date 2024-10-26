# Bytom Language

Bytom language is a C/Rust like language that is designed to be used in the Bytom
infrastructure. It was not created to be better than any other language, but rather to
be simpler to implement. It is statically and strongly typed.

Bytom language source files use `.by` as the file extension.

Language recognizes two base types of code constructs - expressions and statements. The
main difference is that expressions have some value while statements are purely
declarative. Statements must end with semicolon, while expressions do not. For example,
a variable or constant assignment is a statement, as it does not have a value, even
though it assign value to a variable. On the other hand, a function call is an
expression, as it returns a value which can be then for example to assign value to
variable in assignment statement.

List of statements:

- variable declaration
- constant declaration
- value assignment
- function declaration
- struct declaration
- if statement
- while statement
- for statement
- return statement
- break statement
- continue statement

List of expressions:

- function call
- integer literal
- float literal
- string literal
- attribute access

List of build in types:

- i32
- u32
- f32

## Modules

Every bytom source file is a module. A module is a collection of statements. At runtime
all executable statements (basically anything except struct and function definitions)
are executed top-down. Therefore is no `main` function. As a result, every module is a
program and can execute code upon loading.

## Variables

To create a variable, use the `var` keyword followed by the variable name, type and
optionally a value:

```
var x: i32 = 5;
```

Initial value **is not** mandatory, but if it is not provided, the variable must be
initialized before it is used.

## Constants

To create a constant, use the `const` keyword followed by the constant name, type and
value:

```
const x: i32 = 5;
```

Initial value **is** mandatory.

## Conditional statements

To create a conditional statement, use the `if` keyword followed by the condition and
the body:

```
if (gt(x, 5))
{
    return 1;
}
```

Optionally you can include any amount of `elif` blocks:

```
if (gt(x, 5))
{
    return 1;
}
elif (lt(x, 5))
{
    return -1;
}
```

And an optional `else` block:

```
if (gt(x, 5))
{
    return 1;
}
elif (lt(x, 5))
{
    return -1;
}
else
{
    return 0;
}
```

## While loop

To create a while loop, use the `while` keyword followed by the condition and the body:

```
while (lt(x, 5))
{
    x = add(x, 1);
}
```

## For loop

To create a for loop, use the `for` keyword followed by the initialization, condition,
increment and the body:

```
for (var i: i32 = 0; lt(i, 5); i = add(i, 1);)
{
    x = add(x, 1);
}
```

Please note that all three parts of the for loop condition is mandatory and must end
with semicolon.


## Functions

To create a function, use the `functionn` keyword followed by the function name, list of
arguments, return type, and the function body:

```
function foo(var a: i32; var b: i32;): i32 {
    return add(mul(a, a), b);
}
```

Please note that declaration of function parameters uses the same syntax as variable
declaration, so you can also use `const` keyword to declare constant parameters. The
difference is that `var` parameters are mutable references, while `const` are immutable
references.