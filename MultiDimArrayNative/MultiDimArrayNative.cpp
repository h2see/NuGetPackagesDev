// MultiDimArrayNative.cpp : Defines the exported functions for the DLL.
//

#include "pch.h"
#include "framework.h"
#include "MultiDimArrayNative.h"


// This is an example of an exported variable
MULTIDIMARRAYNATIVE_API int nMultiDimArrayNative=0;

// This is an example of an exported function.
MULTIDIMARRAYNATIVE_API int fnMultiDimArrayNative(void)
{
    return 0;
}

// This is the constructor of a class that has been exported.
CMultiDimArrayNative::CMultiDimArrayNative()
{
    return;
}

// ===============================================================================================

MULTIDIMARRAYNATIVE_API void Add(
    const double* a,
    const double* b,
    double* result,
    size_t n
)
{
    for (size_t i = 0; i < n; i++) {
        result[i] = a[i] * b[i];
    }
}
