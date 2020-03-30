#include "Endianness.h"

const int ENDIANNESS_TEST_NUM = 1;

int endianness()
{
	if( isBigEndian() )
	{
		return BIG_ENDIAN;
	}
	
	return LITTLE_ENDIAN;
}

int isBigEndian()
{
	char* temp = (char*)&ENDIANNESS_TEST_NUM;
	return temp[0] == 0;
}

int isLittleEndian()
{
	return !isBigEndian();
}
