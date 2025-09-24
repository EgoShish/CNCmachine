import socket
import threading
import time
from typing import Optional, Callable

class CNCServer:
    def __init__(self, host = 'localhost', port = 8080):
        self.host = host
        self.port = port
        self.clients = []
        self.is_running = False
        self.server_socket = None
        self.on_data_callback = None

    def set_data_callback(self, callback: Callable[[str], None]):
        self.on_data_callback = callback

    def broadcast(self, message: str):
        if not message.endswith('\n'):
            message += '\n'

        disconnected_clients = []
        for client in self.clients:
            try:
                client.sendall(message.encode('utf-8'))
            except (ConnectionError, OSError):
                disconnected_clients.append(client)

        for client in disconnected_clients:
            self.clients.remove(client)
            client.close()

    def handle_client(self, client_socket, address):
        print(f"Клиент подключен: {address}.")
        self.clients.append(client_socket)

        try:
            while self.is_running:
                try:
                    data = client_socket.recv(1024).decode('utf-8').strip()
                    if not data:
                        break
                    print(f"Команда от {address}: {data}.")

                    if self.on_data_callback:
                        self.on_data_callback(data)
                    
                except socket.timeout:
                    continue
                except ConnectionResetError:
                    break
        finally:
            if client_socket in self.clients:
                self.clients.remove(client_socket)
            client_socket.close()
            print(f"Клиент отключен: {address}.")

    def start(self):
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.server_socket.bind((self.host, self.port))
        self.server_socket.listen(2)
        self.server_socket.settimeout(1.0)
        self.is_running = True

        print(f"Сервер запущен на {self.host}:{self.port}")

        try:
            while self.is_running:
                try:
                    client_socket, address = self.server_socket.accept()
                    client_socket.settimeout(1.0)
                    client_thread = threading.Thread(
                        target=self.handle_client,
                        args=(client_socket, address),
                        daemon=True
                    )
                    client_thread.start()
                except socket.timeout:
                    continue
        except KeyboardInterrupt:
            print("Остановка по запросу пользователя.")
        finally:
            self.stop()

    def stop(self):
        self.is_running = False
        for client in self.clients:
            client.close()
        if self.server_socket:
            self.server_socket.close()
        print("Сервер остановлен.")

