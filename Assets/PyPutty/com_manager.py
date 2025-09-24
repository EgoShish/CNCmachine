import serial
import serial.tools.list_ports
from typing import Optional, List

class COMManager:
    def __init__(self):
        self.ser = None
        self.current_port = None
        self.current_baudrate = None
        self.is_reading = False
        self.read_thread = None

    def get_avaliable_ports(self) -> List[str]:
        return [p.device for p in serial.tools.list_ports.comports()]
    
    def connect(self, port: str, baudrate: int = 115200) -> bool:
        try:
            if self.ser and self.ser.is_open:
                self.disconnect()

            self.ser = serial.Serial(port, baudrate, timeout=1)
            self.current_port = port
            self.current_baudrate = baudrate
            print(f"Подключено к {port} на скорости {baudrate}.")
            return True
        except serial.SerialExceptin as e:
            print(f"Ошибка подключения: {e}.")
            return False
    
    def disconnect(self):
        self.stop_reading()
        if self.ser and self.ser.is_open:
            self.ser.close()
        self.ser = None
        self.current_port = None
        self.current_baudrate = None
        print("COM-порт отключен")

    def start_reading(self, data_callback: callable):
        if not self.ser or not self.ser.is_open:
            print("COM-порт не подключен")
            return
        self.is_reading = True
        self.read_thread = threading.Thread(
            target=self.read_loop,
            args=(data_callback,),
            daemon=True
        )
        self.read_thread.start()
        print("Начато чтение данных")
    
    def stop_reading(self):
        self.is_reading = False
        if self.read_thread:
            self.read_thread.join(timeout=1.0)
        self.read_thread = None
        print("Остановлено чтение данных")

    def _read_loop(self, data_callback: callable):
        while self.is_reading and self.ser and self.ser.is_open:
            try:
                line = self.ser.readline().decode('utf-8', errors='ignore').strip()
                if line:
                    data_callback(line)
            except serial.SerialException:
                break
            except Exception as e:
                print(f"Ошибка чтения: {e}.")
                time.sleep(0.1)

    def get_status(self) -> dict:
        return {
            'connected': self.ser and self.ser.is_open,
            'port': self.current_port,
            'baudrate': self.current_baudrate,
            'reading': self.is_reading
        }
