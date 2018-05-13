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

	if (bind(mainSocket, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR)
	{
		perror("bind failed...");
		closesocket(mainSocket);
		return WSAGetLastError();
	}

	struct sockaddr_in clientAddr;
	int clientAddrSize = sizeof(clientAddr);

	char recvbuf[BUF_LEN];
	char sendbuf[BUF_LEN];
	int recvLen = 0;

	while (1)
	{
		if ((recvLen = recvfrom(mainSocket, recvbuf, BUF_LEN, 0,
			(struct sockaddr*)&clientAddr, &clientAddrSize)) > 0)
		{
			recvbuf[recvLen] = 0;
			printf("Query: %s\n", recvbuf);

			sprintf_s(sendbuf, BUF_LEN, "%s", "OK");
			sendto(mainSocket, sendbuf, strlen(sendbuf), 0,
				(struct sockaddr*)&clientAddr, clientAddrSize);
			printf("Response: %s\n", "OK");

			if (strcmp("SHUTDOWN", recvbuf) == 0)
			{
				closesocket(mainSocket);
				WSACleanup();
				return 0;
			}
		}
		else
		{
			perror("recvfrom failed...");
			closesocket(mainSocket);
			return WSAGetLastError();
		}
	}
}