#include "CORE_PREDEFS.h"
#include "CORE_TYPE_PREDEFS.h"
#include "CORE_PREDEFS.h"
#include "CORE_TYPE_PREDEFS.h"
#include "CORE_PREDEFS.h"
#include "CORE_TYPE_PREDEFS.h"

void ArrayTesting()
{
	struct int_array_10 array = (struct int_array_10 ){{5, 6, 7}};
	int *ref = &array.data[0];
	int _temp_var_0_3_0_0 = 5;
	struct ptr_int_array_1 _temp_var_1_3_0_0 = (struct ptr_int_array_1 ){{&_temp_var_0_3_0_0}};
	struct ptr_int_array_1 *ref2 = &_temp_var_1_3_0_0;
}
void PointerTesting()
{
	int *ptr1 = NULL;
	float *ptr2 = (float *)ptr1;
	float *f1 = (float *)ptr2;
	float *f2 = (float *)f1;
	struct int_array_2 arr = (struct int_array_2 ){{1, 2}};
	int *first = (int *)&arr.data[0];
	int *second = first + 1;
	int i = arr.data[1];
	int *null = NULL;
}
void ReferenceTesting()
{
	int _temp_var_2_1_0_0 = 0;
	int *_temp_var_3_1_0_0 = &_temp_var_2_1_0_0;
	int **r1 = &_temp_var_3_1_0_0;
	int _temp_var_4_2_0_0 = 5;
	int *_temp_var_5_2_0_0 = &_temp_var_4_2_0_0;
	int **r2 = &_temp_var_5_2_0_0;
	**r2 = 4;
}
void BasicOperatorTests()
{
	int i1 = 1;
	int i2 = 2;
	float f1 = (float )(i1 + i2 - i1 * i2 / i1 % i2);
	int i3 = (int )f1;
	float f2 = 5;
	float f3 = f2 + f1 - f1 / f2 * f2;
	char c1 = 'c';
	char c2 = 'b';
	bool b1 = c1 == c2 || c1 != c2;
	bool b2 = f1 > f2 || f2 < f1 && f3 == f2 || f1 != f2;
	bool b3 = i1 > i2 || i2 < i1 && i3 == i2 || i1 != i2;
}
void CStringTests()
{
	char *string = "Hello World!";
	printf(string);
}
int Add(int *a, int *b)
{
	return *a + *b;
}
int Sub(int *a, int *b)
{
	return *a - *b;
}
