#include <stdio.h>
#include "..\CommonHeaders\socket_operations.h"

#define SIMPLE_REQUEST "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

int main(void)
{
	WSADATA wsaData;
	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
	{
		perror("WSAStartup() failed...");
		return 1;
	}

	SOCKET mainSocket = initSocket();
	if (mainSocket == SOCKET_ERROR || !establishConnection(mainSocket))
	{
		return 1;
	}

	char *msg = (char*)malloc(2000);
	for (int i = 0; i < 2000; i++)
	{
		msg[i] = rand() % 250 + 1;
	}
	msg[1999] = '\0';

	sendMsg(mainSocket, msg);
	char *response = recvMsg(mainSocket);
	closeSocket(mainSocket);
	WSACleanup();

	printf("Request: %s\n", msg);
	printf("Response: %s\n", response);
	free(msg);
	free(response);
	getchar();

	return 0;
}