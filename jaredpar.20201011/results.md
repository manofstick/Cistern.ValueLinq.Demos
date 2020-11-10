|                                           Method | Length |        Mean |      Error |     StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------------------- |------- |------------:|-----------:|-----------:|------:|-------:|------:|------:|----------:|
|                               Example0_handcoded |     10 |    18.90 ns |   0.111 ns |   0.098 ns |  0.08 |      - |     - |     - |         - |
|                              Example1_all_in_one |     10 |    51.00 ns |   0.056 ns |   0.043 ns |  0.20 |      - |     - |     - |         - |
|                      Example2_calling_a_function |     10 |    51.31 ns |   0.053 ns |   0.047 ns |  0.21 |      - |     - |     - |         - |
|  Example3_Using_standard_delegate_expected_usage |     10 |    98.59 ns |   0.844 ns |   0.748 ns |  0.39 | 0.0153 |     - |     - |      64 B |
|                 Example3_Using_standard_delegate |     10 |   104.59 ns |   0.523 ns |   0.464 ns |  0.42 | 0.0153 |     - |     - |      64 B |
| Example3_Using_standard_delegate_via_IEnumerable |     10 |   153.23 ns |   1.680 ns |   1.403 ns |  0.61 | 0.0248 |     - |     - |     104 B |
|                                    Standard_Linq |     10 |   250.39 ns |   3.013 ns |   2.818 ns |  1.00 | 0.0420 |     - |     - |     176 B |
|                                                  |        |             |            |            |       |        |       |       |           |
|                               Example0_handcoded |   1000 | 1,704.79 ns |   3.897 ns |   3.254 ns |  0.18 |      - |     - |     - |         - |
|                              Example1_all_in_one |   1000 | 1,063.58 ns |   1.157 ns |   1.083 ns |  0.11 |      - |     - |     - |         - |
|                      Example2_calling_a_function |   1000 | 1,065.23 ns |   1.650 ns |   1.463 ns |  0.11 |      - |     - |     - |         - |
|  Example3_Using_standard_delegate_expected_usage |   1000 | 3,798.27 ns |   6.746 ns |   5.633 ns |  0.40 | 0.0153 |     - |     - |      64 B |
|                 Example3_Using_standard_delegate |   1000 | 4,158.12 ns |  19.009 ns |  17.781 ns |  0.44 | 0.0153 |     - |     - |      64 B |
| Example3_Using_standard_delegate_via_IEnumerable |   1000 | 3,940.96 ns |  27.728 ns |  21.648 ns |  0.42 | 0.0229 |     - |     - |     104 B |
|                                    Standard_Linq |   1000 | 9,393.64 ns | 187.649 ns | 313.520 ns |  1.00 | 0.0305 |     - |     - |     176 B |