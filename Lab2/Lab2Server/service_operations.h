#ifndef SERVICE_OPERATIONS_H
#define SERVICE_OPERATIONS_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int parseRequest(const char *request);
char* createResponse(char *response, const char *ip);

#endif //SERVICE_OPERATIONS_H
