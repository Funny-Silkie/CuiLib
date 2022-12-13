# CuiLib

.NETのCUIアプリケーション制作用ライブラリです。
コマンドライン引数を解析してオプションやパラメータなどの情報を取得します。

## Usage

- `Command` クラスをコマンドの単位として使用します
  - `Command.Children` に `Command` クラスのインスタンスを登録することでサブコマンドを実装可能です
  - `Command.OnExecution()` または `Command.OnExecutionAsync()` をオーバーライドすることでコマンドの処理内容を記述できます
  - `Command.WriteHelp(Logger)` で標準的なヘルプコマンドを出力できます
- `Option` クラスをオプションとして使用します
  - `FlagOption` では `--help` のようなフラグとしてのオプションを扱います
  - `SingleValueOption<T>` では `-i hoge.txt` のように値を取るオプションを扱います
  - `MultipleValueOption<T>` では `-i hoge.txt -i fuga.txt` のように複数の値を取るオプションを扱います
  - `ValueConverter<T>` クラスで文字列からの値の変換をカスタマイズできます
  - `ValueChecker<T>` クラスで値のエラーチェックをカスタマイズできます
- `Parameter<T>` ではパラメータ引数を扱います
  - `ValueConverter<T>` クラスで文字列からの値の変換をカスタマイズできます
  - `ValueChecker<T>` クラスで値のエラーチェックをカスタマイズできます

以下の例は， `-i` (`--in`) オプションで1つ以上指定されたテキストファイルを順に結合して `-o`　(`--out`) オプションで指定されたファイルに出力するコマンドの実装です。
`Command.Invoke(string[])` でアプリケーション引数をそのまま引数解析して `ConcatCommand` の処理を実行します。

```cs
using System;
using System.Collections.Generic;
using System.IO;
using CuiLib.Commands;
using CuiLib.Loggers;
using CuiLib.Options;

static void Main(string[] args)
{
    var command = new ConcatCommand();
    // Invoke command with parameter analysis
    command.Invoke(args);
}


// concat command class
class ConcatCommand : Command
{
    private readonly FlagOption optionHelp;
    private readonly MultipleValueOption<FileInfo> optionInput;
    private readonly SingleValueOption<FileInfo> optionOutput;

    public ConcatCommand() : base("concat")
    {
        Description = "concat texts";

        // Define options
        optionHelp = new FlagOption('h', "help")
        {
            Description = "Display help",
        };
        optionInput = new MultipleValueOption<FileInfo>('i', "in")
        {
            Description = "Input files",
            Checker = new SourceFileChecker(),
            Required = true,
        };
        optionOutput = new SingleValueOption<FileInfo>('o', "out")
        {
            Description = "Output file",
            Checker = new DestinationFileChecker()
            {
                AllowMissedDirectory = false,
                AllowOverwrite = true,
            },
            Required = true,
        };

        // Add options
        Options.Add(optionHelp);
        Options.Add(optionInput);
        Options.Add(optionOutput);
    }

    protected override void OnExecution()
    {
        // create logger
        var logger = new Logger()
        {
            ConsoleStdoutLogEnabled = true,
        };

        // show help if "-h" or "--help" option available
        if (optionHelp.ValueAvailable)
        {
            WriteHelp(logger);
            return;
        }

        FileInfo[] input = optionInput.Value; // "-i" or "--in" value
        FileInfo output = optionOutput.Value; // "-o" or "--out" value

        using StreamWriter writer = output.CreateText();

        // output each files
        foreach (FileInfo currentInput in input)
        {
            using StreamReader reader = currentInput.OpenText();
            string? line = reader.ReadLine();
            writer.WriteLine(line);
        }
    }
}
```
