# Glacier

[English](README.md) | [Русский](README.ru.md)

Простой интерпретируемый язык, реализованный на C# с использованием ANTLR4.

## Текущее состояние

На данный момент реализованы:

- арифметические выражения/сравнение
- переменные и области видимости
- функции первого класса с лексическим захватом
- условное исполнение
- рекурсия

## Быстрый старт

Требования:

- .NET SDK 10.0+

Сборка:

```bash
dotnet build Glacier/Glacier.csproj -c Release -f net10.0
```

Чтение/интерпретация файла:

```bash
dotnet run --project Glacier/Glacier.csproj -f net10.0 Glacier/test.glacier
```

Пример вывода:

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


## Лицензия

MIT. См. `LICENSE`.
