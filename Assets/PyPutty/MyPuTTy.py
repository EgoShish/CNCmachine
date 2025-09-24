import serial
import serial.tools.list_ports
import socket

port_name = "COM3"
baudrate = 115200
ser = None
SERVER_ADDRESS = ('localhost', 8080)
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(SERVER_ADDRESS)
server_socket.listen(1)
server_socket.settimeout(20.0)
print(f"Сервер запущен на {SERVER_ADDRESS[0]}:{SERVER_ADDRESS[1]}")

try:
    ports = serial.tools.list_ports.comports()

    com_found = False
    for p in ports:
       if port_name == p.device:
          com_found = True
          break
       
    if not com_found:
        other_ports = [p.device for p in ports]
        raise ValueError(f"Порт {port_name} не найден, доступные порты: {other_ports}")
    
    ser = serial.Serial(port_name, baudrate=baudrate, timeout=1)
    print(f"Подключение к {port_name} установлено на скорости {baudrate}")

    # Perform operations on the COM port
    while True:
       try:  
         client_socket, client_address = server_socket.accept()
         print(f"Подключился клиент: {client_address}")
         data = client_socket.recv(1024)
         print(f"Получено от клиента: {data.decode()}")
         client_socket.sendall("Да-да клиент?".encode('utf-8'))
         try:
            while True:
               line = ser.readline().decode(errors='ignore').strip()
               if line:
                  client_socket.sendall(f"{line}\n".encode('utf-8'))
         except (ConnectionResetError, BrokenPipeError):
            print("Клиент отключился")
         except socket.timeout:
            pass
         finally:
            client_socket.close()

       except socket.timeout:
         pass
       
except ValueError as ve:
   print("Ошибка:", str(ve))

except serial.SerialException as se:
   print("Ошибка последовательного порта:", str(se))

except KeyboardInterrupt:
   print("\nВыход по запросу пользователя")

except Exception as e:
   print("Неожиданная ошибка:", str(e))

finally:
      server_socket.close()
      print("Сервер закрыт.")
      if ser is not None and ser.is_open:      
         ser.close()
         print("Соединение с портом закрыто.")