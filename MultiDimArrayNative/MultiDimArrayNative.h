// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the MULTIDIMARRAYNATIVE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// MULTIDIMARRAYNATIVE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef MULTIDIMARRAYNATIVE_EXPORTS
#define MULTIDIMARRAYNATIVE_API __declspec(dllexport)
#else
#define MULTIDIMARRAYNATIVE_API __declspec(dllimport)
#endif

// This class is exported from the dll
class MULTIDIMARRAYNATIVE_API CMultiDimArrayNative {
public:
	CMultiDimArrayNative(void);
	// TODO: add your methods here.
};

extern MULTIDIMARRAYNATIVE_API int nMultiDimArrayNative;

MULTIDIMARRAYNATIVE_API int fnMultiDimArrayNative(void);

// ===============================================================================================

MULTIDIMARRAYNATIVE_API void Add
(
    const double* a,
    const double* b,
    double* result,
    size_t n
);