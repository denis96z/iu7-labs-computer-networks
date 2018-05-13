import sys
import socket
import ipaddress

from urllib.parse import urlsplit

def parse_host_port(url):
    uri = urlsplit(url)
    return (uri.hostname, uri.port)

def create_request(url):
    uri = urlsplit(url)
    return 'GET ' + uri.path + ' HTTP/1.1\r\n' + \
        'Host: ' + uri.hostname + ':' + str(uri.port) + '\r\n\r\n'

def get_response(host, port, request):
    http_socket = socket.socket()
    http_socket.connect((host, port))

    http_socket.send(request.encode())

    buffer_size = 10
    buffer = http_socket.recv(buffer_size)
    response = buffer.decode('utf-8')
    while len(buffer) == buffer_size:
        buffer = http_socket.recv(buffer_size)
        response += buffer.decode('utf-8')
    http_socket.close()

    return response
    
def main(argv):
    if len(argv) != 2:
        print("URL не указан.")
    else:
        try:
            host, port = parse_host_port(argv[1])
            request = create_request(argv[1])
            response = get_response(host, port, request)

            print(request)
            print(response)
        except:
            print('Не удалось получить ответ...')
    input('Нажмите любую клавишу для продолжения...')

if __name__ == '__main__':
    sys.exit(main(sys.argv))