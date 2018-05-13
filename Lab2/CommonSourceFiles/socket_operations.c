#include "..\CommonHeaders\socket_operations.h"

#define BUFFER_LEN 5

#define SERVER_IP "127.0.0.1"
#define SERVER_PORT 8080

#define MAX_QUEUE_LEN 50

struct sockaddr_in getServerAddress()
{
	struct sockaddr_in serverAddr;
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_addr.s_addr = inet_addr(SERVER_IP);
	serverAddr.sin_port = htons(SERVER_PORT);
	return serverAddr;
}

SOCKET initSocket(void)
{
	SOCKET s = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (s == INVALID_SOCKET)
	{
		perror("socket() failed...");
		WSACleanup();
		return SOCKET_ERROR;
	}
	return s;
}

int bindSocket(SOCKET s)
{
	struct sockaddr_in serverAddr = getServerAddress();

	int result = bind(s, (struct sockaddr*)&serverAddr, sizeof(serverAddr));
	if (result == SOCKET_ERROR)
	{
		perror("bind() failed...");
		closesocket(s);
		WSACleanup();
		return 0;
	}

	return 1;
}

int listenSocket(SOCKET s)
{
	if (listen(s, MAX_QUEUE_LEN) == SOCKET_ERROR)
	{
		perror("listen() failed...");
		closesocket(s);
		WSACleanup();
		return 0;
	}
	return 1;
}

int establishConnection(SOCKET s)
{
	struct sockaddr_in serverAddr = getServerAddress();
	if (connect(s, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR)
	{
		perror("connect() failed...");
		closesocket(s);
		WSACleanup();
		return 0;
	}
	return 1;
}

SOCKET acceptConnection(SOCKET s)
{
	return accept(s, NULL, NULL);
}

void sendMsg(SOCKET sock, const char *msgBuffer)
{
	char sendBuffer[BUFFER_LEN];

	int msgLen = strlen(msgBuffer);
	int nFullBuffers = msgLen / BUFFER_LEN;
	for (int i = 0; i < nFullBuffers; i++)
	{
		strncpy(sendBuffer, &(msgBuffer[BUFFER_LEN * i]), BUFFER_LEN);
		send(sock, sendBuffer, BUFFER_LEN, 0);
	}

	int msgRestLen = msgLen % BUFFER_LEN;
	if (msgRestLen)
	{
		strncpy(sendBuffer, &(msgBuffer[BUFFER_LEN * nFullBuffers]), msgRestLen);
	}

	send(sock, sendBuffer, msgRestLen, 0);
	shutdown(sock, SD_SEND);
}

char* recvMsg(SOCKET sock)
{
	char* msgBuffer = NULL;
	int msgBufferLen = 0;

	char recvBuffer[BUFFER_LEN];
	int recvLen = recv(sock, recvBuffer, BUFFER_LEN, 0);

	while (recvLen > 0)
	{	
		msgBuffer = (char*)realloc(msgBuffer, msgBufferLen + recvLen + 1);
		strncpy(&(msgBuffer[msgBufferLen]), recvBuffer, recvLen);
		msgBufferLen += recvLen;
		recvLen = recv(sock, recvBuffer, BUFFER_LEN, 0);
	}

	if (msgBuffer && (recvLen == 0))
	{
		msgBuffer[msgBufferLen] = '\0';
	}

	return msgBuffer;
}

void closeSocket(SOCKET s)
{
	closesocket(s);
}