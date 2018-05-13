#include <stdio.h>
#include <winsock2.h>
#include "constants.h"

int main(void)
{
	WSADATA wsaData;
	if (FAILED(WSAStartup(MAKEWORD(2, 2), &wsaData)))
	{
		perror("WSAStartup failed...");
		return WSAGetLastError();
	}

	SOCKET mainSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (mainSocket == INVALID_SOCKET)
	{
		perror("socket failed...");
		return WSAGetLastError();
	}

	struct sockaddr_in serverAddr;
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_addr.s_addr = inet_addr(SERVER_IP);
	serverAddr.sin_port = htons(SERVER_PORT);

	char queryBuf[BUF_LEN];
	char responseBuf[BUF_LEN];
	int responseLen = 0;

	printf("%s", "Query: ");
	scanf("%s", queryBuf);
	
	sendto(mainSocket, queryBuf, strlen(queryBuf), 0,
		(struct sockaddr*)&serverAddr, sizeof(serverAddr));

	if ((responseLen = recvfrom(mainSocket, responseBuf, BUF_LEN, 0, 0, 0)) > 0)
	{
		responseBuf[responseLen] = '\0';
		printf("Response: %s\n", responseBuf);

		getchar();
		getchar();

		closesocket(mainSocket);
		WSACleanup();
		return 0;
	}
	else
	{
		perror("recvfrom failed...");
		return WSAGetLastError();
	}
}