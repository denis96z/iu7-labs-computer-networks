#include "io_operations.h"

#define FILE_NAME "C:\\University\\Labs—N\\Lab2\\Debug\\ip.txt"
#define IP_BUFFER_LEN 16

char* readIP(int line)
{
	if (line <= 0)
	{
		return NULL;
	}

	FILE *f = fopen(FILE_NAME, "r");
	if (!f)
	{
		return NULL;
	}

	char *ipBuffer = (char*)malloc(IP_BUFFER_LEN);
	if (!ipBuffer)
	{
		fclose(f);
		return NULL;
	}

	int curLine = 1;
	ipBuffer[0] = '\0';
	while (!feof(f))
	{
		fgets(ipBuffer, IP_BUFFER_LEN, f);
		if ((curLine++) == line)
		{
			ipBuffer[strlen(ipBuffer) - 1] = '\0';
			break;
		}
	}

	fclose(f);

	if (line >= curLine)
	{
		free(ipBuffer);
		ipBuffer = NULL;
	}

	return ipBuffer;
}