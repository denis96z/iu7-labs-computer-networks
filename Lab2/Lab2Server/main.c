#include <stdio.h>
#include "io_operations.h"
#include "service_operations.h"
#include "..\CommonHeaders\socket_operations.h"

int main(void)
{
	WSADATA wsaData;
	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
	{
		perror("WSAStartup() failed...");
		return 1;
	}

	SOCKET mainSocket = initSocket();
	if (mainSocket == SOCKET_ERROR || !bindSocket(mainSocket) || !listenSocket(mainSocket))
	{
		return 1;
	}

	while (1)
	{
		SOCKET clientSocket = acceptConnection(mainSocket);

		char* request = recvMsg(clientSocket);
		if (request)
		{
			printf("Received: %s\n", request);
			free(request);
			sendMsg(clientSocket, "OK");
		}
		else
		{
			puts("Failed to get request...");
		}

		closeSocket(clientSocket);
	}

	getchar();
	return 0;
}
