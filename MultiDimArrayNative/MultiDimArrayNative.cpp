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
