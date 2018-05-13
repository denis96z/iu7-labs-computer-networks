#include "service_operations.h"

int parseRequest(const char *request)
{
	return atoi(request);
}

char* createResponse(char *response, const char *ip)
{
	char ipParts[4][4] = { '\0' };

	int ipPartIndex = 0;
	for (int ipIndex = 0, partBufferIndex = 0; ip[ipIndex]; ipIndex++)
	{
		if (ip[ipIndex] == '.')
		{
			partBufferIndex = 0;
			ipPartIndex++;
		}
		else
		{
			ipParts[ipPartIndex][partBufferIndex] = ip[ipIndex];
			partBufferIndex++;
		}
	}

	sprintf(response, "BIN:");
	for (int i = 0; i < 4; i++)
	{
		int ipPart = atoi(ipParts[i]);
		for (int j = 7; j >= 0; j--)
		{
			sprintf(response + strlen(response), "%d", (ipPart >> j) & 1);
		}
		sprintf(response + strlen(response), ".");
	}

	sprintf(response + strlen(response) - 1, ";OCT:");
	for (int i = 0; i < 4; i++)
	{
		int ipPart = atoi(ipParts[i]);
		sprintf(response + strlen(response), "%o.", ipPart);
	}

	sprintf(response + strlen(response) - 1, ";HEX:");
	for (int i = 0; i < 4; i++)
	{
		int ipPart = atoi(ipParts[i]);
		sprintf(response + strlen(response), "%X.", ipPart);
	}
	response[strlen(response) - 1] = '\0';
}