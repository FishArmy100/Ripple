#include "CORE_PREDEFS.h"
#include "CORE_TYPE_PREDEFS.h"
#include "CORE_PREDEFS.h"
#include "CORE_TYPE_PREDEFS.h"

int Add(int *a, int *b)
{
	return *a + *b;
}
int Sub(int *a, int *b)
{
	return *a - *b;
}
int main()
{
	int (*add)(int *, int *) = Add;
	add = Sub;
	int _temp_var_0_3_0_0 = 1;
	int _temp_var_1_3_0_0 = 2;
	return add(&_temp_var_0_3_0_0, &_temp_var_1_3_0_0);
}
