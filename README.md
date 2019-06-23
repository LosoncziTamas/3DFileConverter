# 3DFileConverter
OBJ to STL converter

## Building the project
Open the solution in VS or Rider, select the project 'Converter' and build it.

## Usage
Open a terminal and navigate to the appropriate bin folder (ex: bin/Release). Then run
```bash
Converter -i ..\..\..\SampleFiles\cow.obj
```
This converts cow.obj and creates a corresponding cow.stl file in the folder you are in.
You can use the --help command to display all the available options.
```bash>
  -i, --input        Required. Input filename

  -o, --output       Name of the output file. Input filename is used if not specified.

  -s, --srcFormat    Format of the source file. Set to .obj by default.

  -d, --dstFormat    Destination file format. Set to .stl by default.

  --help             Display this help screen.

  --version          Display version information.
```
