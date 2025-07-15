# ZD# Programming Language
![ZD# Logo](https://zddeis.github.io/portfolio/img/work/zdsharp.png)

## Installation

### Step 1: Download Required Files
1. Go to [https://github.com/zddeis/zdSharp/releases](https://github.com/zddeis/zdSharp/releases)
2. Download the following files:
   - `zds.exe` (the ZD# interpreter)
   - `zds.dll` (required library for the interpreter)
   - `zds-extension.vsix` (Visual Studio Code syntax highlighting extension)

### Step 2: Install Visual Studio Code Extension
1. Open Visual Studio Code
2. Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac) to open the Command Palette
3. Type "`Extensions: Install from VSIX...`" and select it
4. Navigate to where you downloaded `zds-extension.vsix` and select it
5. Once the message "Completed installing extension." is prompted. You are ready for the next step

### Step 3: Set Up Your ZD# Development Folder
1. Create a new folder on your computer (e.g., "ZDSharp-Projects" on your Desktop)
2. Drag both the downloaded `zds.exe` and `zds.dll` files into this folder
3. This folder will be your workspace where you'll write all your ZD# programs

### Step 4: Write Your First Program
1. In your ZD# development folder, create a new file called `hello.zds`
2. Open the file in Visual Studio Code
3. Write your first ZD# program:
```
print("Hello World")
```
4. Save the file

### Step 5: Run Your Program
1. Open a terminal/command prompt
2. Navigate to your ZD# development folder using `cd` command
3. Run your program with:
```
zds.exe hello.zds
```

**Note:** Each ZD# program should be contained in a single `.zds` file.

*A beginner friendly programming language*

- 🔹 **Easy-to-Learn Syntax** – A clean and readable syntax for beginners and experts alike.

## Features

### Basic Syntax

- **Variables** - Simple assignment with automatic type inference
```
name = "John"
age = 25
is_active = true
```

- **Comments** - Single line comments
```
// This is a comment
```

### Control Flow

- **If Statements** - Conditional execution with else branches
```
if age >= 18 then
    print("You are an adult")
else
    print("You are a minor")
end
```

- **While Loops** - Repeat code while a condition is true
```
counter = 1
while counter <= 10 then
    print(counter)
    counter = counter + 1
end
```

- **For Loops** - Iterate over a range of values with optional step
```
for i = 1 to 10 then
    print(i)
end

// With custom step
for i = 0 to 100 step 10 then
    print(i)
end
```

### Functions

- **Function Declarations** - Define reusable blocks of code
```
function greet(name)
    return "Hello, " + name + "!"
end

// Function with multiple parameters
function add(a, b)
    return a + b
end
```

- **Function Calls** - Invoke functions with arguments
```
message = greet("World")
print(message)  // Outputs: Hello, World!

sum = add(5, 3)
print(sum)      // Outputs: 8
```

### Data Structures

- **Arrays** - Create and manipulate lists of values
```
// Array creation
numbers = [1, 2, 3, 4, 5]

// Access elements by index
first = numbers[0]  // 1

// Modify elements
numbers[2] = 10     // [1, 2, 10, 4, 5]
```

- **Array Operations** - Built-in functions for working with arrays
```
// Add element to array
insert(numbers, 6)  // [1, 2, 10, 4, 5, 6]

// Sort array
sorted = sort(numbers)  // [1, 2, 4, 5, 6, 10]

// Map function to each element
doubled = map(numbers, function(n)
    return n * 2
end)

// Filter array elements
evens = filter(numbers, function(n)
    return n % 2 == 0
end)

// Find an element
found = find(numbers, function(n)
    return n > 5
end)
```

### String Manipulation

- **String Operations** - Functions for working with text
```
// Substring extraction
name = "John Doe"
first_name = substring(name, 0, 4)  // "John"

// String replacement
new_name = replace(name, "John", "Jane")  // "Jane Doe"

// String splitting
parts = split("apple,banana,orange", ",")  // ["apple", "banana", "orange"]

// String joining
fruits = ["apple", "banana", "orange"]
csv = join(fruits, ",")  // "apple,banana,orange"
```

### Math Functions

- **Basic Math** - Common mathematical operations
```
max_value = max(10, 5)        // 10
min_value = min(10, 5)        // 5
clamped = clamp(0, 15, 10)    // 10 (clamps 15 between 0 and 10)
random_num = random(1, 100)   // Random number between 1 and 100
rounded = round(3.7)          // 4
floored = floor(3.7)          // 3
absolute = abs(-5)            // 5
```

- **Trigonometry** - Trigonometric functions
```
sine = sin(pi / 2)      // 1
cosine = cos(pi)        // -1
tangent = tan(pi / 4)   // 1
```

- **Advanced Math** - Additional mathematical functions
```
square_root = sqrt(16)  // 4
power = pow(2, 3)       // 8 (2^3)
```

### Input/Output

- **Console Output** - Print to the console
```
print("Hello, World!")
print("The answer is", 42)
```

- **Console Input** - Get user input
```
name = input("Enter your name: ")
print("Hello, " + name)
```

- **Console Control** - Manipulate the console
```
clear()  // Clears the console screen
key = waitKey()  // Waits for a key press and returns the key
```

### Type System

- **Dynamic Typing** - Variables can hold different types
```
// Type checking
type = typeOf(42)        // "number"
type = typeOf("hello")   // "string"
type = typeOf([1, 2, 3]) // "array"
```

- **Type Conversion** - Automatic conversion in operations
```
// String concatenation with numbers
message = "Age: " + 25   // "Age: 25"
```

## Constants and Utilities

- **Built-in Constants**
```
pi  // 3.14159...
```

- **Time Functions**
```
timestamp = epoch()  // Current Unix timestamp in seconds
```

- Visit the documentation to find more resources like this


zd# © 2025 by David Gouveia is licensed under CC BY 4.0
