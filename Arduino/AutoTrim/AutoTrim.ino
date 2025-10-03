#define ENA 8
#define DIR 9
#define PUL 10

int stepDelay = 150;

void setup() {
  pinMode(ENA, OUTPUT);
  pinMode(DIR, OUTPUT);
  pinMode(PUL, OUTPUT);
  
  digitalWrite(ENA, LOW);  // Motor habilitado siempre
  digitalWrite(DIR, LOW);
  digitalWrite(PUL, LOW);
  
  Serial.begin(9600);
  Serial.println("Sistema listo");
}

void loop() {
  // Leer comandos
  if (Serial.available() > 0) {
    String comando = Serial.readStringUntil('\n');
    comando.trim();
    
    procesarComando(comando);
  }
}

void procesarComando(String cmd) {
  if (cmd == "H") {
    // Movimiento continuo horario
    moverContinuo(LOW);
    Serial.println("Horario continuo");
    
  } else if (cmd == "A") {
    // Movimiento continuo antihorario
    moverContinuo(HIGH);
    Serial.println("Antihorario continuo");
    
  } else if (cmd.startsWith("MH:")) {
    // Mover N pasos horario: MH:200
    int pasos = cmd.substring(3).toInt();
    moverPasos(LOW, pasos);
    Serial.print("Movidos ");
    Serial.print(pasos);
    Serial.println(" pasos horario");
    
  } else if (cmd.startsWith("MA:")) {
    // Mover N pasos antihorario: MA:200
    int pasos = cmd.substring(3).toInt();
    moverPasos(HIGH, pasos);
    Serial.print("Movidos ");
    Serial.print(pasos);
    Serial.println(" pasos antihorario");
    
  } else if (cmd.startsWith("SPEED:")) {
    // Cambiar velocidad: SPEED:200
    int newSpeed = cmd.substring(6).toInt();
    if (newSpeed >= 50 && newSpeed <= 5000) {
      stepDelay = newSpeed;
      Serial.print("Velocidad: ");
      Serial.println(stepDelay);
    }
    
  } else {
    Serial.println("Comando desconocido");
  }
}

// Mover un número específico de pasos
void moverPasos(int direccion, int pasos) {
  digitalWrite(DIR, direccion);
  
  for (int i = 0; i < pasos; i++) {
    digitalWrite(PUL, HIGH);
    delayMicroseconds(stepDelay);
    digitalWrite(PUL, LOW);
    delayMicroseconds(stepDelay);
    
    // Permitir interrumpir con comando 'S'
    if (Serial.available() > 0) {
      char c = Serial.read();
      if (c == 'S') {
        Serial.println("Movimiento interrumpido");
        return;
      }
    }
  }
}

// Mover continuamente hasta recibir 'S'
void moverContinuo(int direccion) {
  digitalWrite(DIR, direccion);
  
  while (true) {
    digitalWrite(PUL, HIGH);
    delayMicroseconds(stepDelay);
    digitalWrite(PUL, LOW);
    delayMicroseconds(stepDelay);
    
    // Verificar si hay comando de parar
    if (Serial.available() > 0) {
      char c = Serial.read();
      if (c == 'S') {
        Serial.println("Stop");
        return;
      }
    }
  }
}