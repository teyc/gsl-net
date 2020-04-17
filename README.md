# gsl-net
Code generation based on .NET Core

Code generated by a computer can be good-looking,
and can work for large scale engineering because
it results in highly uniform applications.

The templating language GSL is originally
created by Peter Hintjens of iMatix. gsl-net
is powered by Javascript and its syntax 
reads like this:

    . /* this is a comment */
    . for (var i = 0; i < 10; i ++) {
    .    setOutput('SKU' + i.toString() + '.cs');
    /**
     * Generated by gsl-net
     * do not modify by hand.
     */
    namespace Demo
    {
        public class SKU${i.toString()}
        {
            
        }
    }
    .}

## Quick Start

1. Grab these two files

    - https://raw.githubusercontent.com/teyc/gsl-net/master/Gsl.Tests/data/demo.json

    - https://raw.githubusercontent.com/teyc/gsl-net/master/Gsl.Tests/data/demo.gsl

2. Download the latest release (TBD)

3. Run 

      `Gsl.exe` demo.gsl demo.json

   and you should see a file called `demo.ts` drop into your current directory.

## Gsl.Template nuget package

Here's some sample code to host your own templating engine.

    dotnet add package Gsl.Template
    dotnet add package System.IO.Abstractions
    dotnet add package Microsoft.Extensions.Logging

    var logger = loggerFactory.CreateLogger<Program>();
    var fileSystem = new FileSystem();
    var engine = new Engine(new Gsl.VM(fileSystem, logger), logger);
    try
    {
        engine.Execute(new FileInfoWrapper(fileSystem, new FileInfo(pathToTemplate)),
                        new FileInfoWrapper(fileSystem, new FileInfo(pathToData)));
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine(exception.Message);
        Console.Error.WriteLine(exception.StackTrace);
    }

## Stop code-generation

If you decide that there are specific files that should not be subject
to further code generation, you can apply the following command in your template:

    . doNotOverwriteIf("DO NOT OVERWRITE", ".bak")

If `gsl-net` finds the phrase "DO NOT OVERWITE" in the output file, it will write
its output to a `.bak` file instead.

Then you can just diff the output and decide what part you may wish to copy
into your source manually. Hint: `code.exe --diff FileA FileB`

## Protected sections

`gsl-net` supports protected sections where the code generation engine
will not overwrite specific named sections of generated code.

For instance,

    . setOutput('Customer.cs');
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsValid() 
        {
    .      protect('customer-validation', '//')
        }
    }

This creates two markers with CUSTOM-CODE-BEGIN and CUSTOM-CODE-END. You can add your
own code in these sections and they will be protected against being overwritten.

The `protect` command is sufficiently clever to indent the comment markers at the
same level as your own code. I'd like `gsl-net` to pay attention to how good-looking the
generated code can be.

    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsValid() 
        {
           // CUSTOM-CODE-BEGIN customer-validation
           return ! string.IsNullOrEmpty(FirstName) && ! string.IsNullOrEmpty(LastName);
           // CUSTOM-CODE-END customer-validation
        }
    }

## Aligning blocks of text

(Warning - this will likely be revised)

    . setOutput('Customer.cs')
    . for (var i = 0; i < fields.length; i++) {
        ${fields[i].type ${fields[i].name}; /* ${fields[i].comment} */
    . }

Results in a rather untidy-looking generated code:

        string FirstName; /* the first name of the contact */
        string SurName; /* the family name or the surname of the contact, it may include hyphens */
        Date dob; /* date of birth */

`gsl-net` solves this problem with alignment markers

    . setOutput('Customer.cs')
    . for (var i = 0; i < fields.length; i++) {
    .   |                |                  |                       |
        ${fields[i].type ${fields[i].name}; /* ${fields[i].comment} */
    . }

And the results look so much better:

        string FirstName; /* the first name of the contact                                         */
        string SurName;   /* the family name or the surname of the contact, it may include hyphens */
        Date   dob;       /* date of birth                                                         */

## Skipping the last comma in a loop

Let's say you need to generate some SQL statements that look like

    CREATE TABLE foo (
        bar INTEGER NOT NULL,
        baz INTEGER NOT NULL,
        qux INTEGER NOT NULL
    )

Getting rid of the last comma is a common problem.

To solve this, we introduce an optional directive `?`. If it occurs on a line, then the final optional character is left out.

    CREATE TABLE foo (
    .for (var i = 0; i < fields.length; i++) {
    .                                ?
        ${fields[i]} INTEGER NOT NULL,
    .}
    )

## Include File directive

GSL files can be refactored and `include`d 

    . // we are in file 1
    . // files are included relative to the current file
    . include('../file2.gsl')

## Replace Text directive

It is convenient to start with a working file and perform search and replace
to arrive at a template. Suppose you have a source like this:

       class Foo
       {
           findFoo(id): Foo;
           deleteFoo(id): void;
           createFoo(): Foo;
       }

You can use a `replaceText()` directive on your template.

       . var className = data.className;
       . replaceText("Foo", className);
       class Foo
       {
           findFoo(id): Foo;
           deleteFoo(id): void;
           createFoo(): Foo;
       }

instead of the uglier option:

       . var className = data.className;
       class Foo
       {
           find${className}(id): Foo;
           delete${className}(id): void;
           create${className}(): Foo;
       }

## Functions

`gsl-net` provides these helper functions

    kebabCase(properCase: string): string

    camelCase(properCase: string): string
    
## Contributing

There are a few shortcuts for building this project

1. run `. _ok.ps1`, and this should show you a list of options when you type `ok`

2. You can build a self-contained .exe with `dotnet warp`

# TODO

1. `Done` protected sections

2. import modules

3. .NET modules

4. load local files (eval JSON)