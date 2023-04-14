#include "CORE_PREDEFS.h"
#include "CORE_TYPE_PREDEFS.h"
#include "CORE_PREDEFS.h"
#include "CORE_TYPE_PREDEFS.h"
#include "CORE_PREDEFS.h"
#include "CORE_TYPE_PREDEFS.h"

int main()
{
	int (*add)(int *, int *) = Add;
	add = Sub;
	int _temp_var_6_3_0_0 = 1;
	int _temp_var_7_3_0_0 = 2;
	return add(&_temp_var_6_3_0_0, &_temp_var_7_3_0_0);
}
