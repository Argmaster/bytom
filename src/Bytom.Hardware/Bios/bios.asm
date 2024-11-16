start:
    mov RD0, 0x0
    cpuid RD0
    cmp RD0, 0x0
    jne idle
    cmp RD1, 0x0
    jne idle
    cmp RD2, 0x0
    jne idle
    jmp core0
idle:
    jmp idle
core0:
    jmp core0
