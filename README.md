csharp-expression-dump
======================

Reflection trickery to print out the name and value of an expression - useful for easily logging the name/value of a variable



TODO:
- Fix:
	- Constants
- Add:
	- Arithmetic ops
	- Readonly 
- Limitations:
    - Compiler gets in the way at times, by replacing:
        - Constants with their values
        - Expressions like 'true ? 2 : 3' with '2'
	- Can't handle things which expressions can't handle; i.e.
		- Optional parameters
        - Multi-dimensional array initialisers
