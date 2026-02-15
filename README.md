# Glacier

[English](README.md) | [Русский](README.ru.md)

A simple interpreted language implemented in C# using ANTLR4.

## Current State

Currently implemented:

- arithmetic expressions/comparisons
- variables and scopes
- first-class functions with lexical capture
- conditional execution
- recursion

## Quick Start

Prerequisites:

- .NET SDK 10.0+

Build:

```bash
dotnet build Glacier/Glacier.csproj -c Release -f net10.0
```

Read/interpret a file:

```bash
dotnet run --project Glacier/Glacier.csproj -f net10.0 Glacier/test.glacier
```

Example output:

```text
10 -> 20 -> 30
back : 0, front : 2
10
back : 1, front : 1
20
back : 2, front : 0
30
10 -> None -> 30
abc -> None -> 30
```

## License

MIT. See `LICENSE`.
