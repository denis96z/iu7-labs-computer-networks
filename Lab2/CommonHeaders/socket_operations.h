#ifndef SOCKET_OPERATIONS_H
#define SOCKET_OPERATIONS_H

#include <stdio.h>
#include <stdlib.h>
#include <winsock2.h>

SOCKET initSocket(void);

int bindSocket(SOCKET s);
int listenSocket(SOCKET s);

int establishConnection(SOCKET s);
SOCKET acceptConnection(SOCKET s);

void sendMsg(SOCKET sock, const char *msgBuffer);
char* recvMsg(SOCKET sock);

void closeSocket(SOCKET s);

#endif //SOCKET_OPERATIONS_H