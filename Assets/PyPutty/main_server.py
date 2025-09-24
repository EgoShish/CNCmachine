import json
from server_core import CNCServer
from com_manager import COMManager

class CNCMainServer:
    def __init__(self):
        self.server = CNCServer()
        self.com_manager = COMManager()
        self.setup_callback()

    def setup_callback(self):
        self.server.set_data_callback(self.handle_client_command)

    def handle_client_command(self, command: str):
        try:
            if command.startwith('SET_COM:'):
                parts = command.split(':')
                if len(parts) >= 3:
                    port = parts[1]
                    baudrate = int(parts[2])
                    success = self.com_manager.connect(port, baudrate)
                    response = f"COM_STATUS:{'OK' if success else 'ERROR'}."
                    self.server.broadcast(response)

            elif command == 'GET_COM_PORTS':
                ports = self.com_manager.get_avaliable_ports()
                response = f"COM_PORTS:{json.dumps(ports)}."
                self.server.broadcast(response)
            elif command == 'START_READING':
                self.com_manager.start_reading(self.hamdle_com_data)
                self.server.broadcast("READING:STARTED")
            elif command == 'STOP_READING':
                self.com_manager.stop_reading()
                self.server.broadcast("READING:STOPPED")
            elif command == 'DISCONNECT_COM':
                self.com_manager.disconnect()
                self.server.broadcast("COM:DISCONNECTED")
        
        except Exception as e:
            error_msg = f"ERROR:{str(e)}."
            self.server.broadcast(error_msg)
    
    def handle_com_data(self, data: str):
        self.server.broadcast(f"DATA:{data}")

    def start(self):
        print("Запуск CNC сервера...")
        print("Доступные команды:")
        print("  SET_COM:COM3:115200")
        print("  GET_COM_PORTS")
        print("  GET_COM_STATUS")
        print("  START_READING")
        print("  STOP_READING")
        print("  DISCONNECT_COM")
        self.server.start()
    
    def stop(self):
        self.com_manager.disconnect()
        self.server.stop()

if __name__ == "__main__":
    server = CNCMainServer()
    try:
        server.start()
    except KeyboardInterrupt:
        server.stop() 