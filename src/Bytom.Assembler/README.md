# Bytom architecture

Big endian

## Instruction encoding

Instructions are either 32 bit or 64 bit, if they involve constant

In 32 bit instructions:

- bit 0 is set to 0
- bits 5, 6, 7, 8, 9, 10 (6 bits) indicate first register ID (if present)
- bits 11, 12, 13, 14, 15, 16 (6 bits) indicate second register ID (if present)

in 64 bit instructions

- bit 0 is set to 1
- bits 5, 6, 7, 8, 9, 10 (6 bits) indicate first register ID (if present)
- bits 11, 12, 13, 14, 15, 16 (6 bits) indicate second register ID (if present)

## Registers

Only 32-bit registers can be used in arithmetic operations, jumps etc. 16-bit and 8-bit
registers can be only used to read and write to RAM and other registers. 16-bit and
8-bit registers are mapped to values in 32-bit registers. When writing 16-bit register
to different 32 bit register, it is always written to lower 16 bits. When writing 8-bit
register to different 32 bit register, it is always written to lower 8 bits. When
writing 32-bit register to 16-bit register, always lower 16 bits are taken. When writing
32-bit register to 8-bit register, always lower 16 bits are taken.

### 32-bit General purpose registers

- byte - 8 bits
- word - 16 bits
- dword - 32 bits
- qword - 64 bits

- `RD0` `0b00_0001`
- `RD1` `0b00_0010`
- `RD2` `0b00_0011`
- `RD3` `0b00_0100`
- `RD4` `0b00_0101`
- `RD5` `0b00_0110`
- `RD6` `0b00_0111`
- `RD7` `0b00_1000`
- `RD8` `0b00_1001`
- `RD9` `0b00_1010`
- `RDA` `0b00_1011`
- `RDB` `0b00_1100`
- `RDC` `0b00_1101`
- `RDD` `0b00_1110`
- `RDE` `0b00_1111`
- `RDF` `0b01_0000`

### 16-bit register access

- `RW0H` `0b10_0000` - High word of `RD0`
- `RW0L` `0b10_0001` - Low word of `RD0`

### 8-bit register access

- `RB0HH` `0b10_1000` - High byte of `RW0H`
- `RB0HL` `0b10_1001` - Low byte of `RW0H`
- `RB0LH` `0b10_1010` - High byte of `RW0L`
- `RB0LL` `0b10_1011` - Low byte of `RW0L`

### Special registers

- `CR0` `0b10_0000` - Configuration Register 0 [kmd-only]
  - bit 0: enable virtual memory
  - bit 31: supervisor bit
- `STP` `0b10_0100` - Call Stack Top Pointer containing virtual address of top of the
  call stack. (direct write disallowed, use push and pop)
- `FBP` `0b10_0101` - Frame Base Pointer containing address of the base of the stack frame. [kmd-only]
- `VATTA` `0b10_0110` - Virtual Address Translation Table Physical Address [kmd-only]
- `IDT` `0b10_0111` - Interrupt Descriptor Table containing virtual address of the
  Interrupt Handlers [kmd-only]
- `IRA` `0b10_1000` - Interrupt Return Address containing virtual address of the next
  instruction after the interrupt [kmd-only]
- `IP` `0b10_1001` - Instruction Pointer containing virtual address of next instruction
  (direct write disallowed, use jump instructions)
- `TRA` `0b10_1010` - Task Descriptor Address (address of currently running task)
- `TDTA` `0b10_1011` - Task Descriptor Table Address
- `KERNEL_STP` `0b11_1010` - Kernel Call Stack Top Pointer containing virtual address of
  top of the kernel call stack.
- `KERNEL_FBP` `0b11_1011` - Kernel Call Stack Base Pointer containing virtual address
  of the bottom of the kernel call stack.
- `KERNEL_IP` `0b11_1100` - Kernel Instruction Pointer containing virtual address of
  next instruction to be executed in kernel mode.

## Declaration syntax

Arithmetic operations in parameters are not allowed

- `<reg>` 32 bit register RD0-RD7 or special register
- `<mem>` A memory address loaded from register
- `<con>` 32-bit constant

Hardware only supports 32-bit operations

## Data Movement Instructions

`nop` - `0b0000_0000_0000_0000_0000_0000_0000_0000` # 32 bit

`halt` - `0b0000_0000_0000_0000_0000_0000_0001_0000` # 32 bit

`xxxx_xx` mark first operand, destination. `yy_yyyy` mark second operand, source.

### mov

The mov instruction copies the data item referred to by its second operand (i.e.
register contents, memory contents, or a constant value) into the location referred to
by its first operand (i.e. a register or memory). While register-to-register moves are
possible, direct memory-to-memory moves are not. In cases where memory transfers are
desired, the source memory contents must first be loaded into a register, then can be
stored to the destination memory address.

- `mov <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0000_0010_0000` # 32 bit
- `mov <reg>,<mem>` - `0b0000_xxxx_xxyy_yyyy_0000_0000_0010_0001` # 32 bit
- `mov <mem>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0000_0010_0100` # 32 bit
- `mov <reg>,<con>` - `0b0000_xxxx_xx00_0000_0000_0000_0010_0010` # 64 bit
- `mov <mem>,<con>` - `0b0000_xxxx_xx00_0000_0000_0000_0010_0110` # 64 bit

### push

The push instruction places its operand onto the top of the hardware supported stack in
memory. Specifically, push first decrements STP by 4, then places its operand into the
contents of the 32-bit location at address [STP]. STP (the stack pointer) is decremented
by push since the x86 stack grows down - i.e. the stack grows from high addresses to
lower addresses.

- `push <reg>` - `0b0000_xxxx_xx00_0000_0000_0000_0011_0000` # 32 bit
- `push <mem>` - `0b0000_xxxx_xx00_0000_0100_0000_0011_0000` # 32 bit
- `push <con>` - `0b0000_0000_0000_0000_1000_0000_0011_0000` # 64 bit

### pop

The pop instruction removes the 4-byte data element from the top of the
hardware-supported stack into the specified operand (i.e. register or memory location).
It first moves the 4 bytes located at memory location [STP] into the specified register
or memory location, and then increments STP by 4.

- `pop <reg>` - `0b0000_xxxx_xx00_0000_0000_0000_0100_0000` # 32 bit
- `pop <mem>` - `0b0000_xxxx_xx00_0000_0000_0000_0100_0100` # 32 bit

### swap

The swap instruction exchanges the values of two registers.

- `swap <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0000_0101_0000` # 32 bit

## 32-bit Integer Arithmetic and Logic Instructions

### add

The add instruction adds together its two operands, storing the result in its first
operand. Note, whereas both operands may be registers, at most one operand may be a
memory location.

- `add <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_0000_0000` # 32 bit

TODO: adc, add with carry

### sub

The sub instruction stores in the value of its first operand the result of subtracting
the value of its second operand from the value of its first operand. As with add

- `sub <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_0001_0000` # 32 bit

TODO: sbb, sub with borrow

### inc

The inc instruction increments the value of its first operand by one and stores it
there.

- `inc <reg>` - `0b0000_xxxx_xx00_0000_0000_0001_0010_0000` # 32 bit

### dec

The dec instruction decrements the value of its first operand by one and stores it
there.

- `dec <reg>` - `0b0000_xxxx_xx00_0000_0000_0001_0011_0000` # 32 bit

### mul

The mul instruction does unsigned integer multiplication of two operands together and
stores the result in the first operand.

- `mul <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_0100_0000` # 32 bit

### imul

The mul instruction does signed integer multiplication of two operands together and
stores the result in the first operand.

- `imul <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_0101_0000` # 32 bit

### div

The div instruction does unsigned integer division of the first operand by the second
operand, storing the quotient in the first operand and the remainder in the second
operand.

- `div <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_0110_0000` # 32 bit

### idiv

The idiv instruction does signed integer division of the first operand by the second
operand, storing the quotient in the first operand and the remainder in the second
operand.

- `idiv <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_0111_0000` # 32 bit

### and

The and instruction does a bitwise AND of its two operands and stores the result in the
first operand.

- `and <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_1000_0000` # 32 bit

### or

The or instruction does a bitwise OR of its two operands and stores the result in the
first operand.

- `or <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_1001_0000` # 32 bit

### xor

The xor instruction does a bitwise XOR of its two operands and stores the result in the
first operand.

- `xor <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_1010_0000` # 32 bit

### not

The not instruction does a bitwise NOT of its operand and stores the result there.

- `not <reg>` - `0b0000_xxxx_xx00_0000_0000_0001_1011_0000` # 32 bit

### shl

The shl instruction does a bitwise shift left of its first operand by the number of bits
specified in the second operand and stores the result in the first operand.

- `shl <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_1100_0000` # 32 bit

### shr

The shr instruction does a bitwise shift right of its first operand by the number of
bits specified in the second operand and stores the result in the first operand.

- `shr <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_1101_0000` # 32 bit

### 32-bit Floating Arithmetic Instructions

### fadd

The fadd instruction adds together its two operands, storing the result in its first
operand.

- `fadd <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0100_0000_0000` # 32 bit

### fsub

The fsub instruction stores in the value of its first operand the result of subtracting
the value of its second operand from the value of its first operand.

- `fsub <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0100_0010_0000` # 32 bit

### fmul

The fmul instruction does unsigned integer multiplication of two operands together and
stores the result in the first operand.

- `fmul <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0100_0100_0000` # 32 bit

### fdiv

The fdiv instruction does unsigned integer division of the first operand by the second
operand, storing the quotient in the first operand and the remainder in the second
operand.

- `fdiv <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0100_0110_0000` # 32 bit

### fcmp

Compare the values of the two specified operands, setting the condition codes in the
machine status word appropriately. This instruction is equivalent to the sub

- `fcmp <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0100_1111_0000` # 32 bit

## Control Flow Instructions

In all jumps can have form `j* <label>`, then it is translated to dynamic address
calculation.

### jmp

The jmp instruction unconditionally transfers control to the instruction at the address
specified by its operand.

- `jmp <mem>` - `0b0000_xxxx_xx00_0000_0000_0010_0000_0000` # 32 bit
- `jmp <con>` - `0b0000_0000_0000_0000_0000_0010_0000_0001` # 64 bit, constant is
  threated as address

### jeq

The jeq instruction transfers control to the instruction at the address specified by its
operand if the two operands are equal.

- `jeq <mem>` - `0b0000_xxxx_xx00_0000_0000_0010_0001_0000` # 32 bit
- `jeq <mem>` - `0b0000_0000_0000_0000_0000_0010_0001_0001` # 64 bit, constant is
  threated as address

### jne

The jne instruction transfers control to the instruction at the address specified by its
operand if the two operands are not equal.

- `jne <mem>` - `0b0000_xxxx_xx00_0000_0000_0010_0010_0000` # 32 bit
- `jne <mem>` - `0b0000_0000_0000_0000_0000_0010_0010_0001` # 64 bit, constant is
  threated as address

### jlt

The jlt instruction transfers control to the instruction at the address specified by its
operand if the first operand is less than the second operand.

- `jlt <mem>` - `0b0000_xxxx_xx00_0000_0000_0010_0011_0000` # 32 bit
- `jlt <mem>` - `0b0000_0000_0000_0000_0000_0010_0011_0001` # 64 bit, constant is
  threated as address

### jle

The jle instruction transfers control to the instruction at the address specified by its
operand if the first operand is less than or equal to the second operand.

- `jle <mem>` - `0b0000_xxxx_xx00_0000_0000_0010_0100_0000` # 32 bit
- `jle <mem>` - `0b0000_0000_0000_0000_0000_0010_0100_0001` # 64 bit, constant is
  threated as address

### jgt

The jgt instruction transfers control to the instruction at the address specified by its
operand if the first operand is greater than the second operand.

- `jgt <mem>` - `0b0000_xxxx_xx00_0000_0000_0010_0101_0000` # 32 bit
- `jgt <mem>` - `0b0000_0000_0000_0000_0000_0010_0101_0001` # 64 bit, constant is
  threated as address

### jge

The jge instruction transfers control to the instruction at the address specified by its
operand if the first operand is greater than or equal to the second operand.

- `jge <mem>` - `0b0000_xxxx_xx00_0000_0000_0010_0110_0000` # 32 bit
- `jge <mem>` - `0b0000_0000_0000_0000_0000_0010_0110_0001` # 64 bit, constant is
  threated as address

### call

The call instruction first pushes the current code location onto the hardware supported
stack in memory (see the push instruction for details), and then performs an
unconditional jump to the code location indicated by the label operand. Unlike the
simple jump instructions, the call instruction saves the location to return to when the
subroutine completes.

- `call <mem>` - `0b0000_xxxx_xx00_0000_0000_0010_1000_0000` # 32 bit
- `call <con>` - `0b0000_0000_0000_0000_0000_0010_1000_0001` # 64 bit, constant is
  threated as address

### ret

The ret instruction pops the top of the hardware-supported stack in memory into the
instruction pointer, effectively returning control to the location saved by the most
recent call instruction.

- `ret` - `0b0000_0000_0000_0000_0000_0010_1001_0000` # 32 bit

### cmp

Compare the values of the two specified operands, setting the condition codes in the
machine status word appropriately. This instruction is equivalent to the sub
instruction, except the result of the subtraction is discarded instead of replacing the
first operand.

- `cmp <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_0001_1111_0000` # 32 bit

## I/O Instructions

### in

The in instruction reads a byte from the I/O port specified by the second operand and
stores it in the first operand.

- `in <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_1000_0001_0000` # 32 bit
- `in <reg>,<con>` - `0b0000_xxxx_xx00_0000_0000_1000_0001_0001` # 64 bit

### out

The out instruction writes a byte from the second operand to the I/O port specified by
the first operand.

- `out <reg>,<reg>` - `0b0000_xxxx_xxyy_yyyy_0000_1000_0010_0000` # 32 bit
- `out <reg>,<con>` - `0b0000_xxxx_xx00_0000_0000_1000_0010_0001` # 64 bit
- `out <con>,<reg>` - `0b0000_xxxx_xx00_0000_0000_1000_0010_0010` # 64 bit
- `out <con>,<con>` - `0b0000_xxxx_xx00_0000_0000_1000_0010_0011` # 64 bit

### Special Instructions

### sysenter

The sysenter instruction is used to enter the kernel mode. It is used to call the
operating system kernel. The operating system kernel is responsible for handling system
calls. The sysenter instruction is used to switch from user mode to kernel mode.

- `sysenter` - `0b0000_0000_0000_0000_1000_1000_0010_0000` # 32 bit

### Address translation

32-bit virtual address space

- bits 0-11: 12 bits for page offset
- bits 12-21: 10 bits for page directory table index
- bits 22-31: 10 bits for page table index

Page size 1KB

Page Directory Table (PDT) - 1024 entries - 4096 bytes total

Lower 12 bits used as flags:

- bit 0: present - indicates if page is present in memory, if not, a page fault is
  raised
- bit 1: readonly - if set, writes to the page will raise a page fault(?)
- bit 2: user - if set, indicates that page is accessible by user mode code
- bit 3: execute disable - if set, indicates that the page is not executable

- bits 12-31: 20 bits for physical address of the page table, first 10 bits are masked,
  page table must be 1KB aligned

Page Table (PT) - 1024 entries - 4096 bytes total

Lower 12 bits used as flags:

- bit 0: present - indicates if page is present in memory, if not, a page fault is
  raised
- bit 1: readonly - if set, writes to the page will raise a page fault(?)
- bit 2: user - if set, indicates that page is accessible by user mode code
- bit 3: execute disable - if set, indicates that the page is not executable

- bits 12-31: 20 bits for physical address of the page, or-ed with 12 bits of page
  offset in the virtual address
